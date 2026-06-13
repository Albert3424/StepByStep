using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public AudioClip stepSound;
    private AudioSource audioSource;

    private bool isMoving = false;

    public float gridSize = 1f;
    public LayerMask obstacleLayer;
    public LayerMask crateLayer;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;
    private SpriteRenderer spriteRenderer;

    public TMP_Text moveCounterText;
    private int moveCount = 0;

    private bool gameFinished = false;
    private bool useWASD;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        moveCount = 0;
        UpdateMoveCounterUI();

        useWASD = PlayerPrefs.GetInt("UseWASD", 0) == 1;
    }

    void Update()
    {
        if (gameFinished) return;

        if (useWASD)
        {
            if (Input.GetKeyDown(KeyCode.W)) TryMove(Vector3.up);
            if (Input.GetKeyDown(KeyCode.S)) TryMove(Vector3.down);
            if (Input.GetKeyDown(KeyCode.A)) TryMove(Vector3.left);
            if (Input.GetKeyDown(KeyCode.D)) TryMove(Vector3.right);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) TryMove(Vector3.up);
            if (Input.GetKeyDown(KeyCode.DownArrow)) TryMove(Vector3.down);
            if (Input.GetKeyDown(KeyCode.LeftArrow)) TryMove(Vector3.left);
            if (Input.GetKeyDown(KeyCode.RightArrow)) TryMove(Vector3.right);
        }
    }

    void TryMove(Vector3 direction)
    {
        if (gameFinished || isMoving) return;

        if (direction == Vector3.up)
            spriteRenderer.sprite = upSprite;
        else if (direction == Vector3.down)
            spriteRenderer.sprite = downSprite;
        else if (direction == Vector3.left)
            spriteRenderer.sprite = leftSprite;
        else if (direction == Vector3.right)
            spriteRenderer.sprite = rightSprite;

        Vector3 newPos = transform.position + direction * gridSize;

        if (Physics2D.OverlapCircle(newPos, 0.2f, obstacleLayer))
            return;

        Collider2D trapHit = Physics2D.OverlapCircle(newPos, 0.2f, LayerMask.GetMask("Trap"));
        if(trapHit != null)
        {
            FindObjectOfType<GameManager>().RestartLevel();
            return;
        }

        Collider2D hit = Physics2D.OverlapCircle(newPos, 0.2f, crateLayer);

        if (hit != null)
        {
            Vector3 pushPos = newPos + direction * gridSize;
            if (!Physics2D.OverlapCircle(pushPos, 0.2f, obstacleLayer) &&
                !Physics2D.OverlapCircle(pushPos, 0.2f, crateLayer))
            {
                moveCount++;
                UpdateMoveCounterUI();

                Crate crate = hit.GetComponent<Crate>();
                if (crate != null)
                {
                    crate.MoveTo(pushPos);
                }
                else
                {
                    hit.transform.position = pushPos;
                }

                StartCoroutine(SmoothMove(newPos));
            }
        }
        else
        {
            moveCount++;
            UpdateMoveCounterUI();

            StartCoroutine(SmoothMove(newPos));
        }
    }

    void UpdateMoveCounterUI()
    {
        if (moveCounterText  != null)
            moveCounterText.text = "Moves: " + moveCount;
    }

    public void FinishGame()
    {
        gameFinished = true;
    }

    public int GetMoveCount()
    {
        return moveCount;
    }

    public void UpdateControls()
    {
        useWASD = PlayerPrefs.GetInt("UseWASD", 0) == 1;
    }

    IEnumerator SmoothMove(Vector3 targetPos)
    {
        if( audioSource != null && stepSound != null)
        {
            audioSource.PlayOneShot(stepSound);
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