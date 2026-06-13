using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour
{
    public AudioClip pushSound;
    private AudioSource audioSource;
    
    private bool isOnTarget = false;
    private TargetPlate currentTarget;
    private bool isMoving = false;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            isOnTarget = true;
            currentTarget = other.GetComponent<TargetPlate>();
            if (currentTarget != null)
                currentTarget.OnCrateEnter(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Target"))
        {
            isOnTarget = false;
            if (currentTarget != null)
                currentTarget.OnCrateExit(this);
            currentTarget = null;
        }
    }

    public bool IsOnTarget()
    {
        return isOnTarget;
    }

    public void MoveTo(Vector3 targetPos)
    {
        if (isMoving) return;

        Collider2D trapHit = Physics2D.OverlapCircle(targetPos, 0.2f, LayerMask.GetMask("Trap"));
        if (trapHit != null)
        {
            FindObjectOfType<GameManager>().RestartLevel();
            return;
        }

        StartCoroutine(SmoothMove(targetPos));
    }

    private IEnumerator SmoothMove(Vector3 targetPos)
    {
        if (audioSource != null && pushSound != null)
        {
            audioSource.PlayOneShot(pushSound);
        }

        isMoving = true;
        Vector3 startPos = transform.position;
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}