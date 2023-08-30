using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjPool : MonoBehaviour
{
    private GameObject newObj;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public Transform parent;
    }

    public List<Pool> Pools;
    public Dictionary<string, List<GameObject>> PoolsDictionary;

    void Awake()
    {
        // Init();
        SceneManager.sceneLoaded += Init;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= Init;
    }


    public void Init(Scene scene, LoadSceneMode mode)
    {
        
        PoolsDictionary = new Dictionary<string, List<GameObject>>();
        foreach (var pool in Pools)
        {
            List<GameObject> newList = new List<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, Vector3.zero, Quaternion.identity, pool.parent);
                newList.Add(obj);
                LevelManager.instance.ToggleInactive(obj);
            }
            PoolsDictionary.Add(pool.tag, newList);
            
        }
        LevelManager.instance.LoadLevel();
    }

    public GameObject GetObj(string listName)
    {
        foreach (var obj in PoolsDictionary[listName])
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
                PoolsDictionary[listName].Add(newObj);
                LevelManager.instance.ToggleInactive(newObj);
                return newObj;
            }
        }

        return null;
    }

    public List<GameObject> GetActive(string _tag)
    {
        List<GameObject> objList = new List<GameObject>();
        foreach (var obj in PoolsDictionary[_tag])
        {
            if (obj.activeSelf) objList.Add(obj);
        }

        return objList;
    }
}
