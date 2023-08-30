using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalPartsDestroyer : MonoBehaviour
{
    public int m_ChildCollected = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
            transform.GetChild(i).gameObject.GetComponent<CrystalPartsBehaviour>().Explode();
        }

        m_ChildCollected = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_ChildCollected == 8)
        {
            LevelManager.instance.ToggleInactive(gameObject);
        }
    }
}
