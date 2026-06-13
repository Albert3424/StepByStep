using UnityEngine;
using System.Collections;

public class DelayedTrap : MonoBehaviour
{
    public Sprite hiddenSprite;    
    public Sprite activeSprite;    
    public float activationDelay = 1f;     
    public float activeDuration = 2f;     
    public bool loop = false;

    public AudioClip activationSound;
    private AudioSource audioSource;

    private SpriteRenderer spriteRenderer;
    private bool isActive = false;
    private bool isTriggered = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        if (hiddenSprite != null)
            spriteRenderer.sprite = hiddenSprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player") || other.CompareTag("Crate"))
        {
            isTriggered = true;
            StartCoroutine(ActivateTrap());
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player") || other.CompareTag("Crate"))
        {
            Collider2D col = Physics2D.OverlapCircle(transform.position, 0.4f);
            if (col != null && (col.CompareTag("Player") || col.CompareTag("Crate")))
            {
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null) gm.RestartLevel();
            }
        }
    }

    IEnumerator ActivateTrap()
    {
        if ( audioSource != null && activationSound != null)
            audioSource.PlayOneShot(activationSound);

        yield return new WaitForSeconds(activationDelay);

        isActive = true;
        if (activeSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = activeSprite;
        }

        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.4f);
        if (col != null && (col.CompareTag("Player") || col.CompareTag("Crate")))
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null) gm.RestartLevel();
            yield break;
        }

        yield return new WaitForSeconds(activeDuration);

        isActive = false;
        if (hiddenSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = hiddenSprite;
        }

        if (loop)
        {
            isTriggered = false;
            yield return new WaitForSeconds(1f);
            if (!isTriggered)
                StartCoroutine(ActivateTrap());
        }
        else
        {
            isTriggered = false;
        }
    }
}