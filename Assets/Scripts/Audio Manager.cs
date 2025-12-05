using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    [Tooltip("The music that should be played in this scene")]
    [SerializeField] MusicType sceneMusic;
    [SerializeField] private AudioSource saloonMusic;
    [SerializeField] private AudioSource hogMusic;

    [Header("Sound Effects")]
    
    private AudioSource[] songs;
    private MusicType currentMusic = MusicType.None;
    
    public static AudioManager Instance;
    
    private bool ignoreNextMusicChange = false;

    public void SetIgnoreNextMusicChange()
    {
        ignoreNextMusicChange = true;
    }

    private void Start()
    {
        PlayMusic(sceneMusic);
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Instance.PlayMusic(sceneMusic);
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
    }

    public void PlayMusic(MusicType musicType)
    {
        if(musicType == MusicType.None)
        {
            StopAllMusic();
            return;
        }
        if (ignoreNextMusicChange)
        {
            ignoreNextMusicChange = false;
            Debug.Log("Ignoring music change due to reset");
            return;
        }
        if(musicType == MusicType.None)
        {
            songs[(int)musicType].Play();
        }
        else if (musicType != currentMusic && !songs[(int)musicType-1].isPlaying)
        {
            StopAllMusic();
            if (songs[(int)musicType-1] != null)
            {
                songs[(int)musicType-1].Play();
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
}