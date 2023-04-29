using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _i;

    public static SoundManager i
    {
        get
        {
            return _i;
        }
    }

    private void Awake()
    {
        if (_i == null)
        {
            _i = this;
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float SoundVolume = 1f; //TODO: Use volume
    public float MusicVolume = 1f;

    [SerializeField] GameObject sourcePrefab;
    [SerializeField] List<AudioClip> audios;
    Dictionary<string, AudioClip> audioClips;

    [SerializeField] List<AudioClip> musics;
    Dictionary<string, AudioClip> musicsClips;

    AudioSource aSource;

    [SerializeField] AudioSource loopScore;
    [SerializeField] AudioSource moveLoop;
    [SerializeField] AudioSource music;

    string musicPlaying = "";

    // Start is called before the first frame update
    void Init()
    {
        audioClips = new Dictionary<string, AudioClip>();

        foreach (var audio in audios)
        {
            audioClips[audio.name] = audio;
        }

        musicsClips = new Dictionary<string, AudioClip>();

        foreach (var audio in musics)
        {
            musicsClips[audio.name] = audio;
        }

        aSource = GetComponent<AudioSource>();
    }

    public void Play(string name, float pitchRange = 0f, float volume = 1f)
    {
        //if name is in the list of audio clips
        if (!audioClips.ContainsKey(name)) return;
        
        var source = Instantiate(sourcePrefab, Vector3.zero, Quaternion.identity).GetComponent<AudioSource>();
        source.transform.parent = transform;

        source.clip = audioClips[name];
        source.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        source.volume = volume;
        source.Play();

        Destroy(source.gameObject, audioClips[name].length + .5f);
    }

    public void PlayPitch(string name, float fixedPitch = 1f, float volume = 1f)
    {
        //if name is in the list of audio clips
        if (!audioClips.ContainsKey(name)) return;

        var source = Instantiate(sourcePrefab, Vector3.zero, Quaternion.identity).GetComponent<AudioSource>();
        source.transform.parent = transform;

        source.clip = audioClips[name];
        source.pitch = fixedPitch;
        source.volume = volume;
        source.Play();

        Destroy(source.gameObject, audioClips[name].length + .5f);
    }

    public void PlayOneShot(string name, float pitchRange = 0f, float volume = 1f)
    {
        aSource.pitch = 1f + Random.Range(-pitchRange, pitchRange);
        aSource.volume = volume;
        aSource.PlayOneShot(audioClips[name]);
    }

    public void LoopScore(bool on = false)
    {
        if (on)
        {
            loopScore.Play();
        } else
        {
            loopScore.Stop();
        }
    }

    public void MusicIn()
    {
        music.DOComplete();
        music.DOFade(.8f, 1).SetEase(Ease.Linear).SetUpdate(true);
    }
    
    public void MusicOut()
    {
        music.DOKill();
        music.DOFade(0, 2).SetEase(Ease.Linear).SetUpdate(true);
    }

    public void PlayMusic(string musicName)
    {
        Debug.Log("gusic " + musicName);
        if (music.volume <= .05f)
        {
            if (musicPlaying != musicName)
            {
                music.clip = musicsClips[musicName];
                music.Play();
                musicPlaying = musicName;
            }
            MusicIn();
        }
        else if (musicPlaying != musicName)
        {
            Debug.Log("gusic " + musicName);
            music.DOKill();
            music.DOFade(0, 1).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
            {
                music.clip = musicsClips[musicName];
                music.Play();
                musicPlaying = musicName;
                MusicIn();
            });
            
        }
    }

}
