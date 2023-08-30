using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class AiBehaviour : MonoBehaviour
{
    public float HP;

    private Camera m_MainCamera;

    private GameObject player;
    [SerializeField] private Transform m_Bullet;
    [SerializeField] private Transform m_BulletSpawner;
    [SerializeField] private string m_DamageTag;

    [SerializeField] private string m_BulletTag;

    private Rigidbody m_Rigidbody;
    private NavMeshAgent m_NavmeshAgent;
    private Animator m_Animator;

    private float m_PlayerDistance;
    [SerializeField] private float m_TriggerDistance;
    [SerializeField] private float m_attackDistance;

    private bool m_IsStabbing;
    private bool m_OutOfRange;

    [SerializeField] private Canvas m_AiCanvas;
    [SerializeField] private Slider m_HealthBar;

    [SerializeField] private float m_Cooldown;
    private float m_CooldownElapsed;

    private Outline m_OutlineScript;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void Init()
    {
        m_CooldownElapsed = 0;
        HP = 100;
        player = GameObject.Find("Player");
        m_Rigidbody = GetComponent<Rigidbody>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_IsStabbing = false;
        m_OutOfRange = true;
        m_HealthBar.value = HP / 100;
        m_MainCamera = Camera.main;
        m_OutlineScript = GetComponent<Outline>();
        m_OutlineScript.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CooldownElapsed < m_Cooldown)
        {
            m_CooldownElapsed += Time.deltaTime;
        }
        m_PlayerDistance = Vector3.Distance(player.transform.position, transform.position);
        m_AiCanvas.transform.rotation = player.GetComponent<PlayerStateMachine>().m_PlayerCanvas.transform.rotation;
        
        StateToggler();
    }

    private void OnEnable()
    {
        Init();
    }

    private void StateToggler()
    {
        if (!m_IsStabbing)
        {
            if (m_PlayerDistance < m_attackDistance)
            {
                m_NavmeshAgent.destination = transform.position;
                m_NavmeshAgent.velocity = Vector3.zero;
                m_Rigidbody.velocity = Vector3.zero;
                m_Animator.SetBool("Running", false);
                if (m_Cooldown < m_CooldownElapsed)
                {
                    Attack();
                }
            }
            else if (m_PlayerDistance < m_TriggerDistance)
            {
                m_Animator.SetBool("Running", true);
                Move();
            }
            else
            {
                if (!m_OutOfRange)
                {
                    m_OutOfRange = true;
                    m_NavmeshAgent.SetDestination(transform.position);
                    m_NavmeshAgent.velocity = Vector3.zero;
                    m_Rigidbody.velocity = Vector3.zero;
                    m_Animator.SetBool("Running", false);
                }
            }
        }
        else
        {
            m_Animator.SetBool("Running", false);
            m_NavmeshAgent.SetDestination(transform.position);
            transform.LookAt(player.transform.position);
        }
    }


    private void Move()
    {
        if (m_OutOfRange)
        {
            m_OutOfRange = false;
        }

        m_NavmeshAgent.destination = player.transform.position;
    }

    private void Attack()
    {
        if (m_OutOfRange)
        {
            m_OutOfRange = false;
        }

        m_IsStabbing = true;

        m_Animator.SetTrigger("Stab");
        m_CooldownElapsed = 0;
    }

    private void LaunchBasicAttack()
    {
        LevelManager.instance.SpawnObj(m_BulletTag, m_BulletSpawner.position, m_BulletSpawner.rotation);
    }

    private void EndAttack()
    {
        m_IsStabbing = false;
    }

    public void TakeDamage(float _damage)
    {
        HP -= _damage;
        if (HP <= 0 && gameObject.activeSelf)
        {
            LevelManager.instance.ToggleInactive(gameObject);
        }
        m_HealthBar.value = HP / 100;
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
    
    public Outline GetOutlineComponent()
    {
        return m_OutlineScript;
    }
}
