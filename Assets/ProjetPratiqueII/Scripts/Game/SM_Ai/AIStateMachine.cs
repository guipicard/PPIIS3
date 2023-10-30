using System;
using System.Collections;
using System.Collections.Generic;
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
    public Transform m_PlayerCanvas;
    [SerializeField] public Slider m_HealthBar;
    [SerializeField] public float m_Cooldown;
    public float m_Hp;
    [SerializeField] public float m_MaxHp;
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
    public Vector3 m_SpawnPos;

    private bool targetLost;
    private float lostElapsed;
    [SerializeField] private float lostTime;

    public string m_Biome;
    
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
        m_Biome = LevelManager.instance.currentWorld;
        lostElapsed = 0.0f;
        targetLost = false;
        m_CooldownElapsed = 0.0f;
        m_Hp = m_MaxHp;
        player = GameObject.Find("Player");
        m_PlayerCanvas = player.GetComponent<PlayerStateMachine>().m_PlayerCanvas.transform;
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
        m_SpawnPos = m_Transform.position;
        
        Image sprite1 = m_HealthBar.transform.GetChild(0).GetComponent<Image>();
        Image sprite2 = m_HealthBar.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        Color color1 = sprite1.color;
        Color color2 = sprite2.color;
        color1.a = 0.0f;
        color2.a = 0.0f;
        sprite1.color = color1;
        sprite2.color = color2;
        
        SetState(new AiIdle(this));
    }
    
    void Update()
    {
        if (targetLost)
        {
            lostElapsed += Time.deltaTime;
            if (lostElapsed > lostTime)
            {
                Retreat();
            }
        }
        else if (m_CooldownElapsed > m_Cooldown)
        {
            _currentState = new AiAttack(this);
        }
        
        
        _currentState.UpdateExecute();
    }

    private void FixedUpdate()
    {
        _currentState.FixedUpdateExecute();
        m_AiCanvas.transform.rotation = m_PlayerCanvas.rotation;
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
        Image sprite1 = m_HealthBar.transform.GetChild(0).GetComponent<Image>();
        Image sprite2 = m_HealthBar.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        Color color1 = sprite1.color;
        Color color2 = sprite2.color;
        color1.a = 1.0f;
        color2.a = 1.0f;
        sprite1.color = color1;
        sprite2.color = color2;
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

    public void IncrementCD()
    {
        if (targetLost) return;
        m_CooldownElapsed += Time.deltaTime;
        if (m_CooldownElapsed > m_Cooldown)
        {
            _currentState = new AiAttack(this);
            m_CooldownElapsed = 0.0f;
        }
    }

    public void TargetLost()
    {
        lostElapsed = 0.0f;
        targetLost = true;
        SetState(new AiIdle(this));
    }

    public void Retreat()
    {
        targetLost = false;
        m_NavmeshAgent.destination = m_SpawnPos;
        SetState(new AiMoving(this));
    }
}
