using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxPool : MonoBehaviour
{
    public static VfxManager instance;
    
    private GameObject newObj;
    
    [System.Serializable]
    public class Pool
    {
        public VfxClip tag;
        public GameObject prefab;
        public int size;
        public Transform parent;
    }
    
    public List<Pool> Pools;
    public Dictionary<VfxClip, List<GameObject>> VfxDictionary;
    private List<AudioSource> activeSources;

    private AudioPool audioPool;

    private void Awake()
    {
        VfxDictionary = new Dictionary<VfxClip, List<GameObject>>();
        foreach (var pool in Pools)
        {
            List<GameObject> newList = new List<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.parent = transform;
                newList.Add(obj);
                LevelManager.instance.ToggleInactive(obj);
            }
            VfxDictionary.Add(pool.tag, newList);
            
        }
    }
    
    public GameObject GetObj(VfxClip listName)
    {
        foreach (var obj in VfxDictionary[listName])
        {
            if (!obj.activeSelf)
            {
                return obj;
            }
        }
        
        foreach (var pool in Pools)
        {
            if (pool.tag == listName)
            {
                newObj = Instantiate(pool.prefab, Vector3.zero, Quaternion.identity, pool.parent);
                VfxDictionary[listName].Add(newObj);
                LevelManager.instance.ToggleInactive(newObj);
                return newObj;
            }
        }

        return null;
    }

    public Transform GetParent(VfxClip _clip)
    {
        foreach (var pool in Pools)
        {
            if (pool.tag == _clip)
            {
                return pool.parent;
            }
        }

        return null;
    }
}
