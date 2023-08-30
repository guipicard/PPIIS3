using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] private float m_Speed;

    private GameObject m_Target;

    private Vector3 m_InitialPosition;

    private Vector3 m_HeightOffset;

    private Vector3 m_InitialTargetVelocity;
    private Rigidbody m_Rigidbody;
    private float m_InitialDistance;

    private Vector3 currentTargetPos;

    private bool targetAlive;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        m_Target = null;
    }

    private void OnEnable()
    {
        targetAlive = true;
        transform.GetChild(0).GetComponent<TrailRenderer>().Clear();
    }

    void Update()
    {
        Vector3 currentPos = transform.position;
        if (targetAlive) currentTargetPos = m_Target.transform.position + m_HeightOffset;
        m_InitialDistance = Vector3.Distance(m_InitialPosition, currentTargetPos);

        Vector3 newVelocity = (currentTargetPos - currentPos).normalized;

        Vector3 IT = currentTargetPos - m_InitialPosition;
        Vector3 IC = currentPos - m_InitialPosition;

        Vector3 IT_norm = IC.normalized;

        float distance = Vector3.Dot(IT_norm, IC);

        distance = Mathf.Abs(distance);

        float t = (m_InitialDistance - distance) / m_InitialDistance;
        t = Mathf.Abs(t - 1);
        if (t > 1.0f) t = 1.0f;
        if (t < 0.0f) t = 0.0f;

        float maxSpeed = t > 0.3f ? m_Speed * 1.5f : m_Speed;
        m_Rigidbody.velocity = Vector3.Lerp(m_InitialTargetVelocity * maxSpeed, newVelocity * maxSpeed, t);

        transform.LookAt(currentTargetPos);

        if (Vector3.Distance(currentPos, currentTargetPos) <= 0.2f)
        {
            if (targetAlive)
            {
                AIStateMachine aiSM = m_Target.GetComponent<AIStateMachine>();
                aiSM.TakeDamage(LevelManager.instance.playerDamage);
                if (aiSM.IsDead())
                {
                    m_Target = null;
                    targetAlive = false;
                }
            }
            LevelManager.instance.ToggleInactive(gameObject);
        }
    }

    public void SetTarget(GameObject _target, Vector3 _pos)
    {
        if (_target != null)
        {
            m_Target = _target;
            m_HeightOffset = new Vector3(0, 1.0f, 0);
            Vector3 targetPos = m_Target.transform.position + m_HeightOffset;
            m_InitialPosition = _pos;
            m_InitialDistance = Mathf.Abs(Vector3.Distance(_pos, targetPos));
            float x = Random.Range(-45, 45);
            float y = 45 - Mathf.Abs(x);
            m_InitialTargetVelocity = (targetPos - _pos).normalized + (new Vector3(x, y, 0).normalized);
        }
    }
}