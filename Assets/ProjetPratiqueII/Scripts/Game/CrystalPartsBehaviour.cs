using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Object = System.Object;
using Random = UnityEngine.Random;

public class CrystalPartsBehaviour : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_ExplosionForce;
    [SerializeField] private float m_RedirectionTime;
    public string m_Color;
    private GameObject m_Player;
    private Vector3 heightOffset;
    private Rigidbody m_Rigidbody;
    private BoxCollider m_BoxCollider;
    private Vector3 m_InitialDirection;
    private float m_DirectionLerpTime;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_BoxCollider = GetComponent<BoxCollider>();
        m_Player = GameObject.FindWithTag("Player");
        heightOffset = new Vector3(0, 1, 0);
        m_InitialDirection = new Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f)) * m_ExplosionForce;
        m_DirectionLerpTime = 0;
    }

    private void OnEnable()
    {
        m_InitialDirection = new Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f)) * m_ExplosionForce;
        m_DirectionLerpTime = 0;
        if (m_BoxCollider) m_BoxCollider.isTrigger = false;
    }

    void Update()
    {
        if (!m_BoxCollider.isTrigger) m_BoxCollider.isTrigger = true;
        Vector3 playerPosHeight = m_Player.transform.position + heightOffset;
        Vector3 playerPosDifference = (playerPosHeight - transform.position).normalized * m_Speed;
        if (m_DirectionLerpTime <= m_RedirectionTime)
        {
            m_DirectionLerpTime += Time.deltaTime;
        }

        m_Rigidbody.velocity =
            Vector3.Lerp(m_InitialDirection, playerPosDifference, m_DirectionLerpTime / m_RedirectionTime);
    }

    public void Explode()
    {
        if (m_Rigidbody != null)
        {
            transform.position = transform.parent.position;
            m_InitialDirection =
                new Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f)).normalized * m_ExplosionForce;
            m_Rigidbody.AddForce(m_InitialDirection, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            transform.parent.GetComponent<CrystalPartsDestroyer>().m_ChildCollected++;
           
            LevelManager.instance.CollectAction?.Invoke(1, m_Color);
            gameObject.SetActive(false);
        }
    }
}