using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider volumeSlider;
    public TextMeshProUGUI volumePercentText;
    public Button arrowsButton;
    public Button wasdButton;
    public Button backButton;

    private float lastVolume = 0.8f;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        if (savedVolume <= 0.01f) savedVolume = 0.8f;

        lastVolume = savedVolume;
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        UpdateVolumePercent(savedVolume);

        int savedControls = PlayerPrefs.GetInt("UseWASD", 0);
        HighlightControlButton(savedControls == 1);

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        arrowsButton.onClick.AddListener(() => SetControls(false));
        wasdButton.onClick.AddListener(() => SetControls(true));
        backButton.onClick.AddListener(CloseSettings);
    }

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        lastVolume = value;
        UpdateVolumePercent(value);

        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    void UpdateVolumePercent(float value)
    {
        int percent = Mathf.RoundToInt(value * 100);
        if (volumePercentText != null)
            volumePercentText.text = $"{percent}%";
    }

    void SetControls(bool useWASD)
    {
        PlayerPrefs.SetInt("UseWASD", useWASD ? 1 : 0);
        PlayerPrefs.Save();

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            player.UpdateControls();
        }

        HighlightControlButton(useWASD);
    }

    void HighlightControlButton(bool isWASD)
    {
        Color activeColor = new Color(0.3f, 0.5f, 0.3f);
        Color inactiveColor = new Color(0.4f, 0.4f, 0.4f);

        if (arrowsButton != null)
            arrowsButton.GetComponent<Image>().color = !isWASD ? activeColor : inactiveColor;
        if (wasdButton != null)
            wasdButton.GetComponent<Image>().color = isWASD ? activeColor : inactiveColor;
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}