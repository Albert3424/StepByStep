using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button level1Button;
    public Button level2Button;
    public Button level3Button;
    public Button level4Button;
    public Button level5Button;

    public GameObject levelSelectPanel;

    public AudioClip titleTrack;

    void Start()
    {
        UpdateLevelButtonState();
        MusicManager mm = FindObjectOfType<MusicManager>();
        if (mm != null && titleTrack != null)
            mm.ChangeMusic(titleTrack);
    }

    public void StartGame()
    {
        StartCoroutine(DelayedLoad("Level1"));
    }

    public void CloseLevelSelect()
    {
        if(levelSelectPanel != null)
            levelSelectPanel.SetActive(false);
    }
    public void OpenLevelSelect()
    {
        if(levelSelectPanel != null)
            levelSelectPanel.SetActive(true);

        UpdateLevelButtonState();
    }

    public void LoadLevel1()
    {
        StartCoroutine(DelayedLoad("Level1"));
    }

    public void LoadLevel2()
    {
        StartCoroutine(DelayedLoad("Level2"));
    }

    public void LoadLevel3()
    {
        StartCoroutine(DelayedLoad("Level3"));
    }

    public void LoadLevel4()
    {
        StartCoroutine(DelayedLoad("Level4"));
    }

    public void LoadLevel5()
    {
        StartCoroutine(DelayedLoad("Level5"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void UpdateLevelButtonState()
    {
        int maxUnlocked = PlayerPrefs.GetInt("MaxUnlockedLevel", 1);

        level1Button.interactable = true;
        level2Button.interactable = (maxUnlocked >= 2);
        level3Button.interactable = (maxUnlocked >= 3);
        level4Button.interactable = (maxUnlocked >= 4);
        level5Button.interactable = (maxUnlocked >= 5);
    }

    System.Collections.IEnumerator DelayedLoad(string sceneName)
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
    }
}
