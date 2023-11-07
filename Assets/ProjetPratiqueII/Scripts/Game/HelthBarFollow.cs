using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelthBarFollow : MonoBehaviour
{
    [SerializeField] private Transform m_Player;
    [SerializeField] private Camera m_MainCamera;
    [SerializeField] private float m_Speed;
    [SerializeField] private Vector3 m_HealthBarOffset;
    
    void Start()
    {
        m_MainCamera = Camera.main;
    }

    private void FixedUpdate()
    {
        Vector3 playerPosition = m_Player.position;
        Vector3 targetPosition = playerPosition + m_HealthBarOffset;
        transform.position = Vector3.Lerp(playerPosition, targetPosition, Time.deltaTime * m_Speed);
        transform.LookAt(m_MainCamera.transform.position);
    }
}
