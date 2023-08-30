using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearEvents : MonoBehaviour
{
    [SerializeField] private float m_Damage;
    private float m_Elapsed;
    private bool m_Hit;
    
    void Start()
    {
        m_Elapsed = 1.5f;
        m_Hit = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Hit)
        {
            m_Elapsed += Time.deltaTime;
            if (m_Elapsed > 1.5f) m_Hit = false;
        }
    }
}
