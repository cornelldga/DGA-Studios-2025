using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioSource saloonMusic;
    [SerializeField] private AudioSource hogMusic;

    [Header("Sound Effects")]
    
    private AudioSource[] songs;
    private MusicType currentMusic;
    
    public static AudioManager Instance;
    
    private bool ignoreNextMusicChange = false;

    public void SetIgnoreNextMusicChange()
    {
        ignoreNextMusicChange = true;
    }

    public void Start()
    {
        if (saloonMusic != null)
        {
            Debug.Log("Attempting to play saloon music");
            saloonMusic.Play();
        }
        else
        {
            Debug.LogError("Saloon music AudioSource is null!");
        }
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        songs = new AudioSource[] { saloonMusic, hogMusic };
        foreach (AudioSource song in songs)
        {
            if (song != null)
            {
                song.loop = true;
            }
        }
        Debug.Log("Audio Manager Set Up");
    }

    public void PlayMusic(MusicType musicType)
    {
        
        if (ignoreNextMusicChange)
        {
            ignoreNextMusicChange = false;
            Debug.Log("Ignoring music change due to reset");
            return;
        }
        
        if (musicType != currentMusic && !songs[(int)musicType].isPlaying)
        {
            StopAllMusic();
            if (songs[(int)musicType] != null)
            {
                songs[(int)musicType].Play();
                currentMusic = musicType;
            }
        }
    }

    public void StopAllMusic()
    {
        foreach (AudioSource song in songs)
        {
            if (song != null && song.isPlaying)
            {
                song.Stop();
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log($"audio manager destoryed, ID: {GetInstanceID()}");
    }
}