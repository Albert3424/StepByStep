using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Tilemaps;
using System.Collections;

public class RecordsManager : MonoBehaviour
{
    public GameObject recordsPanel;
    public TextMeshProUGUI[] levelRecordsText;

    public void OpenRecords()
    {
        LoadAndDisplayRecords();
        recordsPanel.SetActive(true);
    }

    public void CloseRecords()
    {
        recordsPanel.SetActive(false);
    }

    void LoadAndDisplayRecords()
    {
        for (int i = 1; i <= 5; i++)
        {
            int moves = PlayerPrefs.GetInt($"Level{i}_BestMoves", 0);
            float time = PlayerPrefs.GetFloat($"Level{i}_BestTime", 0f);
            string timeStr = FormatTime(time);

            if (levelRecordsText[i-1] != null)
            {
                if (moves == 0 && time == 0f)
                    levelRecordsText[i-1].text = $"Level {i}     Moves: –     Time: --:--";
                else
                    levelRecordsText[i-1].text = $"Level {i}     Moves: {moves}     Time: {timeStr}";
            }
        }
    }

    string FormatTime(float seconds)
    {
        if (seconds <= 0) return "--:--";
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return $"{minutes:00}:{secs:00}";
    }
}