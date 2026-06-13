using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Tilemaps;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private int maxUnlockedLevel = 1;

    public AudioClip winSound;
    private AudioSource audioSource;

    private Crate[] allCrates;
    private int totalCrates;
    public GameObject winPanel;
    public GameObject exitDoor;

    public float timeRemaining = 90f;
    public TextMeshProUGUI timerText;
    public bool timerRunning = true;
    public bool gameWon = false;
    public bool isLevel5 = false;
    private bool isDoorOpened = false;
    public Sprite doorOpenSprite;

    public AudioClip airyTrack;
    public AudioClip sectorTrack;
    public AudioClip pulseTrack;
    public AudioClip urgentTrack;
    public AudioClip finalTrack;

    private float startTime;

    void Start()
    {
        startTime = timeRemaining;
        audioSource = GetComponent<AudioSource>();
        allCrates = FindObjectsOfType<Crate>();
        totalCrates = allCrates.Length;
        
        if(winPanel != null )
            winPanel.SetActive(false);

        string currentLevel = SceneManager.GetActiveScene().name;

        if (currentLevel == "Level1")
            timeRemaining = 45f;
        else if (currentLevel == "Level2")
            timeRemaining = 65f;
        else if (currentLevel == "Level3")
            timeRemaining = 60f;
        else if (currentLevel == "Level4")
            timeRemaining = 60f;
        else if (currentLevel == "Level5")
            timeRemaining = 75f;
        else
            timeRemaining = 45f;

        UpdateTimerDisplay();
        LoadProgress();

        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null )
        {
            if (currentLevel == "Level1" || currentLevel == "Level2")
                musicManager.ChangeMusic(airyTrack);
            else if (currentLevel == "Level3")
                musicManager.ChangeMusic(sectorTrack);
            else if (currentLevel == "Level4")
                musicManager.ChangeMusic(pulseTrack);
            else if (currentLevel == "Level5")
                musicManager.ChangeMusic(urgentTrack);
        }
    }

    void Update()
    {
        CheckWinCondition();

        if (timerRunning && !gameWon)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();
            }
            else
            {
                timerRunning = false;
                RestartLevel();
            }

            if (Input.GetKeyUp(KeyCode.R))
                RestartLevel();
        }
    }

    public void UpdateTimerDisplay()
    {
        int min = Mathf.FloorToInt(timeRemaining / 60);
        int sec = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    public void NextLevelLoad()
    {
        Time.timeScale = 1f;
        string currentLevel = SceneManager.GetActiveScene().name;

        if (currentLevel == "Level1")
            StartCoroutine(DelayedLoad("Level2"));
        else if (currentLevel == "Level2")
            StartCoroutine(DelayedLoad("Level3"));
        else if (currentLevel == "Level3")
            StartCoroutine(DelayedLoad("Level4"));
        else if (currentLevel == "Level4")
            StartCoroutine(DelayedLoad("Level5"));
        else
            StartCoroutine(DelayedLoad("MainMenu_Scene"));
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(DelayedLoad("MainMenu_Scene"));
    }

    public void RestartLevel()
    {
        gameWon = false;
        Time.timeScale = 1f;
        StartCoroutine(DelayedLoad(SceneManager.GetActiveScene().name));
    }

    void CheckWinCondition()
    {
        int cratesOnTargets = 0;
        foreach (Crate crate in allCrates)
        {
            if(crate.IsOnTarget())
                cratesOnTargets++;
        }

        if (cratesOnTargets == totalCrates && totalCrates > 0)
        {
            if(isLevel5)
            {
                if (!isDoorOpened)
                    OpenDoor();
            }
            else
                Win();
        }
    }

    void OpenDoor()
    {
        if (isDoorOpened) return;
        isDoorOpened = true;

        Tilemap walls = FindObjectOfType<Tilemap>();
        if(walls != null)
        {
            Vector3Int cellPos = walls.WorldToCell(exitDoor.transform.position);
            walls.SetTile(cellPos, null);
        }

        if(exitDoor != null)
            exitDoor.SetActive(true);
    }

    void Win()
    {
        if (gameWon) return;
        gameWon = true;

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null) player.FinishGame();

        StartCoroutine(WinSequence());
    }

    void SaveProgress()
    {
        PlayerPrefs.SetInt("MaxUnlockedLevel", maxUnlockedLevel);
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        maxUnlockedLevel = PlayerPrefs.GetInt("MaxUnlockedLevel", 1);
    }

    public float GetCurrentTime()
    {
        return startTime - timeRemaining;
    }

    IEnumerator WinSequence()
    {
        if (isLevel5)
        {
            MusicManager mm = FindObjectOfType<MusicManager>();
            if (mm != null && finalTrack != null)
            {
                mm.ChangeMusic(finalTrack);
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (winSound != null && audioSource != null)
            audioSource.PlayOneShot(winSound);

        yield return new WaitForSeconds(0.2f);

        if (winPanel != null)
            winPanel.SetActive(true);

        timerRunning = false;

        string currentSceneName = SceneManager.GetActiveScene().name;
        int completedLevelNumber = 0;
        if (currentSceneName == "Level1") completedLevelNumber = 1;
        else if (currentSceneName == "Level2") completedLevelNumber = 2;
        else if (currentSceneName == "Level3") completedLevelNumber = 3;
        else if (currentSceneName == "Level4") completedLevelNumber = 4;
        else if (currentSceneName == "Level5") completedLevelNumber = 5;

        if (completedLevelNumber == maxUnlockedLevel && maxUnlockedLevel < 5)
        {
            maxUnlockedLevel++;
            SaveProgress();
        }

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            int currentMoves = player.GetMoveCount();
            int bestMoves = PlayerPrefs.GetInt(currentSceneName + "_BestMoves", 9999);
            if (currentMoves < bestMoves)
            {
                PlayerPrefs.SetInt(currentSceneName + "_BestMoves", currentMoves);
            }
        }

        float currentTime = startTime - timeRemaining;
        float bestTime = PlayerPrefs.GetFloat(currentSceneName + "_BestTime", 9999f);
        if (currentTime < bestTime && currentTime > 0)
        {
            PlayerPrefs.SetFloat(currentSceneName + "_BestTime", currentTime);
        }

        PlayerPrefs.Save();

        Time.timeScale = 0f;
    }

    System.Collections.IEnumerator DelayedLoad(string sceneName)
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
    }
}
