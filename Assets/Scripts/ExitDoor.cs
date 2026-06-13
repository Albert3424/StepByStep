using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class ExitDoor : MonoBehaviour
{
    public GameObject finalWinPanel;
    public AudioClip finalTrack;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SaveLevel5Records();

            FadeToBlack fade = FindObjectOfType<FadeToBlack>();
            if (fade != null)
                fade.StartFade();
            else
            {
                if (finalWinPanel != null)
                    finalWinPanel.SetActive(true);
                Time.timeScale = 0f;
            }

            MusicManager mm = FindObjectOfType<MusicManager>();
            if (mm != null && finalTrack != null)
                mm.ChangeMusic(finalTrack);
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu_Scene");
    }

    public void SaveLevel5Records()
    {
        string levelName = "Level5";

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            int currentMoves = player.GetMoveCount();
            int bestMoves = PlayerPrefs.GetInt(levelName + "_BestMoves", 9999);
            if (currentMoves < bestMoves)
            {
                PlayerPrefs.SetInt(levelName + "_BestMoves", currentMoves);
            }
        }

        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            float currentTime = gm.GetCurrentTime();
            float bestTime = PlayerPrefs.GetFloat(levelName + "_BestTime", 9999f);
            if (currentTime < bestTime && currentTime > 0)
            {
                PlayerPrefs.SetFloat(levelName + "_BestTime", currentTime);
            }
        }

        PlayerPrefs.Save();
    }
}