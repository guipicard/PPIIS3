 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerBullet : MonoBehaviour
{
    private Transform m_PlayerTransform;
    
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_MaxSpeed;
    [SerializeField] private int m_Angle1;
    [SerializeField] private int m_Angle2;
    [SerializeField] private float m_StepTwoTime;

    private float m_Elapsed;

    private GameObject m_Target;

    private Vector3 m_InitialPosition;

    private Vector3 m_HeightOffset;

    private Rigidbody m_Rigidbody;
    private float m_InitialDistance;

    private Vector3 m_InitialVelocity;
    private Vector3 m_TargetVelocity;
    private Vector3 currentTargetPos;

    private Vector3 m_PlayerInitialPos;

    private bool targetAlive;

    private void Awake()
    {
        m_PlayerInitialPos = new Vector3();
        m_PlayerTransform = null;
        m_Rigidbody = GetComponent<Rigidbody>();
        m_InitialVelocity = new Vector3();
        m_TargetVelocity = new Vector3();
        currentTargetPos = new Vector3();
    }

    private void OnDisable()
    {
        m_Target = null;
    }

    private void OnEnable()
    {
        GetComponent<ParticleSystem>().Play();
        transform.GetChild(0).GetComponent<TrailRenderer>().Clear();
        transform.GetChild(0).GetChild(0).GetComponent<TrailRenderer>().Clear();
        m_Elapsed = 0.0f;
        targetAlive = true;
    }

    void Update()
    {
        m_Elapsed += Time.deltaTime;
        Vector3 currentPos = transform.position;
        Vector3 playerVelocity = m_PlayerTransform.position;
        if (targetAlive) currentTargetPos = m_Target.transform.position + m_HeightOffset;
        
        if (m_Elapsed > m_StepTwoTime)
        {
            m_TargetVelocity = (currentTargetPos - currentPos).normalized;
            m_Rigidbody.velocity = m_TargetVelocity * m_MaxSpeed;
        }
        else
        {
            Vector3 initial = playerVelocity.normalized + m_InitialVelocity * m_Speed;
            Vector3 target = playerVelocity.normalized + m_TargetVelocity * m_Speed;
            m_Rigidbody.velocity = Vector3.Lerp( initial, target, m_Elapsed / m_StepTwoTime);
        }
            

        transform.LookAt(currentTargetPos);

        if (targetAlive)
        {
            AIStateMachine aiSM = m_Target.GetComponent<AIStateMachine>();
            if (aiSM.IsDead())
            {
                m_Target = null;
                targetAlive = false;
            }
        }
        if (Vector3.Distance(currentPos, currentTargetPos) <= 0.2f)
        {
            if (targetAlive)
            {
                AIStateMachine aiSM = m_Target.GetComponent<AIStateMachine>();
                AudioManager.instance.PlaySound(SoundClip.BlueSpellHit,  1f, transform.position);
                VfxManager.instance.PlayVfx(VfxClip.ToonPunch, transform.position);
                aiSM.TakeDamage(LevelManager.instance.playerDamage);
                if (aiSM.IsDead())
                {
                    m_Target = null;
                    targetAlive = false;
                }
            }
            LevelManager.instance.ToggleInactive(gameObject);
            m_Rigidbody.velocity = Vector3.zero;
        }

        Transform sparks = transform.GetChild(1);
        sparks.LookAt(transform.position + m_Rigidbody.velocity);
    }

    public void SetTarget(GameObject _target, Vector3 _pos, Transform _transform)
    {
        if (_target != null)
        {
            m_PlayerTransform = _transform;
            m_PlayerInitialPos = m_PlayerTransform.position;
            m_Target = _target;
            m_HeightOffset = new Vector3(0, 1.0f, 0);
            Vector3 targetPos = m_Target.transform.position + m_HeightOffset;
            m_InitialPosition = _pos;
            int x1 = Random.Range(-m_Angle1, m_Angle1);
            int y1 = m_Angle1 - Mathf.Abs(x1);
            int x2 = m_Angle2 * x1 / m_Angle1;
            int y2 = m_Angle2 * y1 / m_Angle1;
            Vector3 direction = (targetPos - m_InitialPosition).normalized;
            Vector3 offset1 = new Vector3(x1, y1, 0).normalized;
            Vector3 offset2 = new Vector3(x2, y2, 0).normalized;
            m_InitialVelocity = (-direction + offset1).normalized;
            m_TargetVelocity = (direction + offset2).normalized;
        }
    }
}
