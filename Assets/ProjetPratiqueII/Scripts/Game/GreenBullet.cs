using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBullet : MonoBehaviour
{
    [SerializeField] private GameObject m_Bullet;
    [SerializeField] private GameObject m_Explosion;
    [SerializeField] private GameObject m_ShockWave;

    [SerializeField] private float m_Speed;
    [SerializeField] private string m_DamageTag;
    [SerializeField] private float m_Damage;

    private SphereCollider m_SphereCollier;

    private float m_SpeedMultiplier;
    private Vector3 m_InitialPosition;
    private Vector3 m_TargetPosition;

    private ParticleSystem m_ShockWavePS;
    private float m_SphereColliderStartSize;
    private float m_MaxDistance;
    private bool m_Hit;
    private bool m_Detonate;
    private float m_Elapsed;
    private float m_ExplosionDuration;

    private void Awake()
    {
        m_ShockWavePS = m_ShockWave.GetComponent<ParticleSystem>();
        m_SphereCollier = GetComponent<SphereCollider>();
        m_ExplosionDuration = m_Explosion.GetComponent<ParticleSystem>().main.duration;
        m_SphereColliderStartSize = m_SphereCollier.radius;
    }

    private void OnEnable()
    {
        m_InitialPosition = transform.position;
        m_Elapsed = 0.0f;
        m_Hit = false;
        m_Detonate = false;
        m_SphereCollier.enabled = true;
        m_Bullet.SetActive(true);
    }

    private void OnDisable()
    {
        m_TargetPosition = Vector3.zero;
        m_SphereCollier.radius = m_SphereColliderStartSize;
    }

    void Update()
    {
        if (m_Detonate)
        {
            if (m_SphereCollier.enabled) m_SphereCollier.radius = Mathf.Lerp(m_SphereColliderStartSize, 5.0f, m_Elapsed * 2.0f);
            m_Elapsed += Time.deltaTime;
            if (m_Elapsed >= 0.5f)
            {
                m_SphereCollier.enabled = false;
            }
            if (m_Elapsed > m_ExplosionDuration)
            {
                LevelManager.instance.ToggleInactive(gameObject);
            }
        }
        else
        {
            if (m_TargetPosition == Vector3.zero)
            {
                transform.LookAt(m_TargetPosition);
            }

            m_SpeedMultiplier = m_Speed * Time.deltaTime;
            transform.Translate(Vector3.forward * m_SpeedMultiplier);
            if (Vector3.Distance(transform.position, m_InitialPosition) > m_MaxDistance || m_Hit)
            {
                m_Bullet.SetActive(false);
                m_Detonate = true;
                m_Explosion.GetComponent<ParticleSystem>().Play(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_DamageTag == "Player" && other.gameObject.layer == 3 && !m_Hit)
        {
            other.gameObject.GetComponent<PlayerStateMachine>().TakeDmg(m_Damage);
            m_Hit = true;
        }
    }


    public void SetTarget(Vector3 _pos, float _distance)
    {
        m_TargetPosition = _pos;
        m_MaxDistance = _distance;
    }
}