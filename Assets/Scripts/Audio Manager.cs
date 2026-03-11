using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public enum SFXKey
{
    GIN = 0, BEER = 1, 

    BASESWAP = 2, MIXERSWAP = 3,

    WHIPHIT = 4,  WHIPEMPTY = 5, WHIPCRACK = 6
}

public class AudioManager : MonoBehaviour
{
        
    public static AudioManager Instance;

    [Header("Music")]
    [Tooltip("The music that should be played in this scene")]
    [SerializeField] MusicType sceneMusic;
    [SerializeField] private AudioSource saloonMusic;
    [SerializeField] private AudioSource hogMusic;

    [Header("Sound Effects")]
    [Tooltip("configure list here for all SFX in the game")]
    [SerializeField] private Sound[] sounds;
    [SerializeField] AudioMixerGroup musicGroup;
    [SerializeField] AudioMixerGroup sfxGroup;


    // private fields
    private AudioSource[] songs;
    private MusicType currentMusic = MusicType.None;
    private bool ignoreNextMusicChange = false;


    private static readonly float[] pitches = new float[]
    {
        0.875f,
        0.9375f,
        1.0f,
        1.0625f,
        1.125f,  
    };


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
            song.outputAudioMixerGroup = musicGroup;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.loop = s.isLoop;
            s.source.volume = s.volume;
            s.source.outputAudioMixerGroup = sfxGroup;
        }
    }

    public void Update()
    {
        
    }


    // Audio configuration and playback functions
    public void SetIgnoreNextMusicChange()
    {
        ignoreNextMusicChange = true;
    }

    public void PlaySFX(SFXKey key, bool random = false)
    {
        int index = (int) key;
        if (index < 0 || index >= sounds.Length)
        {
            Debug.Log("Sound index out of bounds, refer to Audio Manager object");
            return;
        }
        if (random)
        {
            sounds[index].source.pitch = pitches[Random.Range(0, pitches.Length)];
        }
        sounds[index].source.PlayOneShot(sounds[index].audioClip);
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

    public void UpdateVolumeLevelTo(float musicVol, float sfxVol)
    {
        musicGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(musicVol) * 20);
        sfxGroup.audioMixer.SetFloat("Music Volume", Mathf.Log10(sfxVol) * 20);
    }
}