using System;
using System.Collections;
using System.Collections.Generic;
using RuntimeInspectorNamespace;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AIStateMachine : MonoBehaviour
{
    [SerializeField] public Transform m_Bullet;
    [SerializeField] public Transform m_BulletSpawner;
    [SerializeField] public string m_DamageTag;
    [SerializeField] public string m_BulletTag;
    [SerializeField] public float m_TriggerDistance;
    [SerializeField] public float m_attackDistance;
    [SerializeField] public Canvas m_AiCanvas;
    [SerializeField] public Slider m_HealthBar;
    [SerializeField] public float m_Cooldown;
    [SerializeField] public float m_Hp;
    [SerializeField] public float m_SafeDistance;
    
    [HideInInspector] public Rigidbody m_Rigidbody;
    [HideInInspector] public NavMeshAgent m_NavmeshAgent;
    [HideInInspector] public Animator m_Animator;
    [HideInInspector] public Transform m_Transform;
    
    [HideInInspector] public Camera m_MainCamera;
    [HideInInspector] public GameObject player;
    [HideInInspector] public float m_PlayerDistance;
    [HideInInspector] public bool m_IsStabbing;
    [HideInInspector] public bool m_OutOfRange;
    [HideInInspector] private bool m_Dead;
    [HideInInspector] public float m_CooldownElapsed;
    [HideInInspector] public Outline m_OutlineScript;
    
    AiState _currentState;
    private static readonly int sense = Animator.StringToHash("Sense");

    public void SetState(AiState state)
    {
        _currentState = state;
    }
    void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        
    }

    private void Init()
    {
        m_CooldownElapsed = 0.0f;
        m_Hp = 100;
        player = GameObject.Find("Player");
        m_Rigidbody = GetComponent<Rigidbody>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_Transform = transform;
        m_IsStabbing = false;
        m_OutOfRange = true;
        m_HealthBar.value = m_Hp / 100;
        m_MainCamera = Camera.main;
        m_OutlineScript = GetComponent<Outline>();
        m_OutlineScript.enabled = false;
        m_Dead = false;
        
        SetState(new AiIdle(this));
    }
    
    void Update()
    {
        Debug.Log(m_CooldownElapsed);
        _currentState.UpdateExecute();
    }

    private void FixedUpdate()
    {
        _currentState.FixedUpdateExecute();
        m_AiCanvas.transform.LookAt(m_MainCamera.transform.position);
    }
    
    public Outline GetOutlineComponent()
    {
        return m_OutlineScript;
    }
    
    public void TakeDamage(float _damage)
    {
        m_Hp -= _damage;
        if (m_Hp <= 0 && gameObject.activeSelf)
        {
            m_Dead = true;
            LevelManager.instance.ToggleInactive(gameObject);
        }
        else
        {
            m_HealthBar.value = m_Hp / 100;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AOE"))
        {
            LevelManager.instance.RedSpellAction += TakeDamage;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("AOE"))
        {
            LevelManager.instance.RedSpellAction -= TakeDamage;
        }
    }

    public bool IsDead()
    {
        return m_Dead;
    }
}
