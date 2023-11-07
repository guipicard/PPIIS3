using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform m_Player;
    [SerializeField] private Vector3 m_Offset;
    [SerializeField] private float m_Speed;

    private Vector3 m_Position;
    private float m_PosY;

    void Start()
    {
        LevelManager.instance.LoadLevel();
        m_Player = GameObject.Find("Player").transform;
        m_Position = m_Player.position + new Vector3(0, 1, 0) - m_Offset;
        // m_Position.y = m_PosY - m_Offset.y;
        transform.position = m_Position;
        m_Position += m_Offset;
        transform.LookAt(m_Position);
    }

    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (LevelManager.instance.takeInput)
        {
            m_Position = m_Player.position + new Vector3(0, 1, 0) - m_Offset;
            // m_Position.y = m_PosY - m_Offset.y;
            m_Position.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, m_Position, Time.deltaTime * m_Speed);
        }
    }

    public Vector3 GetOffset()
    {
        return m_Offset;
    }
}