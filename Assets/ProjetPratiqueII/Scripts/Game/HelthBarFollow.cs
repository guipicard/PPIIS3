using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelthBarFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private float m_Speed;
    [SerializeField] private Vector3 m_HealthBarOffset;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        Vector3 playerPosition = player.position;
        Vector3 targetPosition = playerPosition + m_HealthBarOffset;
        transform.position = Vector3.Lerp(playerPosition, targetPosition, Time.deltaTime * m_Speed);
        transform.LookAt(mainCamera.transform.position);
    }
}
