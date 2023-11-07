using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool
{
    private Queue<AudioSource> pool;
    private AudioSource musicSource;
    private AudioSource prefab;
    private AudioSource musicPrefab;
    private Transform parent;
    private Transform musicParent;

    public AudioPool(AudioSource _prefab, AudioSource _musicPrefab, int _size, Transform _parent, Transform _musicParent)
    {
        parent = _parent;
        musicParent = _musicParent;
        pool = new Queue<AudioSource>();
        prefab = _prefab;
        musicPrefab = _musicPrefab;
        for (int i = 0; i < _size; i++)
        {
            AudioSource instance = GameObject.Instantiate(prefab, parent);
            instance.spatialize = true;
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
        musicSource = GameObject.Instantiate(_musicPrefab, _musicParent);
        musicSource.gameObject.SetActive(false);
    }

    public AudioSource GetPooledObject()
    {
        int initialPoolCount = pool.Count;

        AudioSource instance = null;
        for (int i = 0; i < initialPoolCount; i++)
        {
            instance = pool.Dequeue();
            if (!instance.isPlaying)
            {
                instance.gameObject.SetActive(true);
                return instance;
            }
            else
            {
                pool.Enqueue(instance);
                instance = null;
            }
        }

        if (instance == null)
        {
            instance = GameObject.Instantiate(prefab, parent);
            pool.Enqueue(instance);
        }

        instance.gameObject.SetActive(true);
        return instance;
    }

    public AudioSource GetMusicSource()
    {
        musicSource.gameObject.SetActive(true);
        return musicSource;
    }


    public void ReturnPooledObject(AudioSource instance)
    {
        instance.clip = null;
        instance.time = 0.0f;
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }
}
