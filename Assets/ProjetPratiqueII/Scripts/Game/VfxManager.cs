using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum VfxClip
{
    RedAOE,
    RedSpell,
    ToonPunch,
    Buff,
    Heal,
    CrystalMine,
}

public class VfxManager : MonoBehaviour
{
    public static VfxManager instance;

    [SerializeField] private Transform m_PoolParent;

    private VfxPool vfxPool;

    private List<GameObject> activeSources;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            vfxPool = GameObject.FindObjectOfType<VfxPool>();
            activeSources = new List<GameObject>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayVfx(VfxClip _clip, Vector3 _position)
    {
        var source = vfxPool.GetObj(_clip);
        if (source == null) return;

        
        source.transform.position = _position;
        source.transform.parent = vfxPool.GetParent(_clip);
        activeSources.Add(source);
        source.SetActive(true);
        source.GetComponent<ParticleSystem>().Play();
    }

    private void Update()
    {
        for (int i = activeSources.Count - 1; i >= 0; i--)
        {
            if (!activeSources[i].GetComponent<ParticleSystem>().isPlaying)
            {
                activeSources[i].transform.parent = m_PoolParent;
                activeSources[i].SetActive(false);
                activeSources.RemoveAt(i);
            }
        }
    }
}