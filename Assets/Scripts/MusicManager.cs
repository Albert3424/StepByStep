using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeMusic(AudioClip newTrack)
    {
        if(audioSource != null && newTrack != null)
        {
            audioSource.Stop();
            audioSource.clip = newTrack;
            audioSource.Play();
        }
    }
}
