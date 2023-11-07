using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalEvents : MonoBehaviour
{
    [SerializeField] private string m_CrystalTag;
    [SerializeField] private string m_PartsTag;
    [SerializeField] private Vector3 m_InitialPosition;
    private bool m_CanGetDestroyed;
    private Outline m_OutlineScript;
    public string m_Biome;
    
    void Start()
    {
        m_CanGetDestroyed = Vector3.Distance(transform.position, m_InitialPosition) > 9.0f;
        m_Biome = LevelManager.instance.currentWorld;
    }

    private void OnEnable()
    {
        m_OutlineScript = GetComponent<Outline>();
        m_OutlineScript.enabled = false;
        m_CanGetDestroyed = Vector3.Distance(transform.position, m_InitialPosition) > 9.0f;
    }

    private void OnDisable()
    {
    }

    public void GetMined()
    {
        var position = transform.position;
        LevelManager.instance.SpawnObj(m_PartsTag, position, Quaternion.identity);
        LevelManager.instance.ToggleInactive(gameObject);
        LevelManager.instance.UpdateCrystalNums(m_CrystalTag);
        AudioManager.instance.PlaySound(SoundClip.CrystalExplosion, 1f, position);
        VfxManager.instance.PlayVfx(VfxClip.CrystalMine, position);
    }

    public bool GetCanDestroy()
    {
        return m_CanGetDestroyed;
    }

    public Outline GetOutlineComponent()
    {
        return m_OutlineScript;
    }
}
