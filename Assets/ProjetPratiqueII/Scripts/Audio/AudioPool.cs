using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPool
{
    private Queue<AudioSource> pool;
    private AudioSource prefab;
    private Transform parent;

    public AudioPool(AudioSource _prefab, int _size, Transform _parent)
    {
        parent = _parent;
        pool = new Queue<AudioSource>();
        prefab = _prefab;
        for (int i = 0; i < _size; i++)
        {
            AudioSource instance = GameObject.Instantiate(prefab, parent);
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
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


    public void ReturnPooledObject(AudioSource instance)
    {
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }
}
