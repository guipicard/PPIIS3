using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundClip
{
    BlueSpellLaunch,
    BlueSpellHit,
    CrystalExplosion,
    Buff,
    Heal,
}

public enum MusicClip
{
    Ice,
    Earth,
    Lava,
    Desert
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private Transform m_PoolParent;
    [SerializeField] private Transform m_MusicPoolParent;

    public AudioSource audioSourcePrefab;
    public AudioSource musicSourcePrefab;
    public int poolSize = 10;
    public List<AudioClip> SoundList;
    public List<AudioClip> SnowStepsList;
    public List<AudioClip> MusicList;
    private Dictionary<SoundClip, AudioClip> SoundDictionary;
    private Dictionary<MusicClip, AudioClip> MusicDictionary;
    private List<AudioSource> activeSources;
    private List<AudioSource> MusicActiveSources;


    private AudioPool audioPool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioPool = new AudioPool(audioSourcePrefab, musicSourcePrefab, poolSize, m_PoolParent, m_MusicPoolParent);
            SoundDictionary = new Dictionary<SoundClip, AudioClip>();
            MusicDictionary = new Dictionary<MusicClip, AudioClip>();
            activeSources = new List<AudioSource>();
            MusicActiveSources = new List<AudioSource>();
            for (int i = 0; i < SoundList.Count; i++)
            {
                SoundDictionary.Add((SoundClip)i, SoundList[i]);
            }

            for (int i = 0; i < MusicList.Count; i++)
            {
                MusicDictionary.Add((MusicClip)i, MusicList[i]);
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySnowStep(Vector3 _position)
    {
        var source = audioPool.GetPooledObject();
        if (source != null)
        {
            source.transform.position = _position;
            source.clip = SnowStepsList[Random.Range(0, SnowStepsList.Count)];
            source.volume = 1.0f;
            activeSources.Add(source);
            source.Play();
        }
    }
    
    public void PlaySound(SoundClip _clip, float _volume, Vector3 _position)
    {
        var source = audioPool.GetPooledObject();
        if (source != null)
        {
            source.transform.position = _position;
            source.clip = SoundDictionary[_clip];
            source.volume = _volume;
            activeSources.Add(source);
            source.Play();
        }
    }

    public void PlayMusic(MusicClip _clip, float _volume)
    {
        var source = audioPool.GetMusicSource();
        if (source != null)
        {
            source.loop = true;
            source.clip = MusicDictionary[_clip];
            source.volume = _volume;
            source.time = 0.0f;
            MusicActiveSources.Add(source);
            source.Play();
        }
    }

    public void StopMusic()
    {
        for (int i = MusicActiveSources.Count - 1; i >= 0; i--)
        {
            MusicActiveSources[i].Stop();
        }
    }

    private void Update()
    {
        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            if (!activeSources[i].isPlaying)
            {
                audioPool.ReturnPooledObject(activeSources[i]);
                activeSources.RemoveAt(i);
            }
        }
        for (int i = MusicActiveSources.Count - 1; i >= 0; i--)
        {
            if (!MusicActiveSources[i].isPlaying)
            {
                audioPool.ReturnPooledObject(MusicActiveSources[i]);
                MusicActiveSources.RemoveAt(i);
            }
        }
    }
}