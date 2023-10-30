using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public abstract class AiState
{
    protected AIStateMachine _AiStateMachine;

    // SERIALIZED
    private Transform m_Bullet;
    protected Transform m_BulletSpawner;
    private string m_DamageTag;
    protected string m_BulletTag;
    protected float m_TriggerDistance;
    protected float m_attackDistance;
    private Canvas m_AiCanvas;
    private Slider m_HealthBar;
    protected float m_Cooldown;
    private float m_Hp;
    protected float m_SafeDistance;

    // MEMBERS
    private Camera m_MainCamera;
    protected GameObject player;
    private Rigidbody m_Rigidbody;
    protected NavMeshAgent m_NavmeshAgent;
    protected Animator m_Animator;
    protected Transform m_Transform;
    protected float m_PlayerDistance;
    private bool m_IsStabbing;
    private bool m_OutOfRange;
    private Outline m_OutlineScript;
    protected Vector3 m_SpawnPos;
    
    protected static readonly int running = Animator.StringToHash("Running");
    protected static readonly int combat = Animator.StringToHash("Combat");
    protected static readonly int moveState = Animator.StringToHash("MoveState");
    protected static readonly int attack1 = Animator.StringToHash("Attack1");
    protected static readonly int attack2 = Animator.StringToHash("Attack2");
    protected static readonly int attack3 = Animator.StringToHash("Attack2");

    public AiState(AIStateMachine stateMachine)
    {
        Init(stateMachine);
    }

    private void Init(AIStateMachine _stateMachine)
    {
        _AiStateMachine = _stateMachine;
        LoadSerializable(_stateMachine);
        LoadComponents(_stateMachine);
        LoadMembers(_stateMachine);
    }

    public abstract void UpdateExecute();
    public abstract void FixedUpdateExecute();

    private void LoadSerializable(AIStateMachine _stateMachine)
    {
        m_Bullet = _stateMachine.m_Bullet;
        m_BulletSpawner = _stateMachine.m_BulletSpawner;
        m_DamageTag = _stateMachine.m_DamageTag;
        m_BulletTag = _stateMachine.m_BulletTag;
        m_TriggerDistance = _stateMachine.m_TriggerDistance;
        m_attackDistance = _stateMachine.m_attackDistance;
        m_AiCanvas = _stateMachine.m_AiCanvas;
        m_HealthBar = _stateMachine.m_HealthBar;
        m_Cooldown = _stateMachine.m_Cooldown;
        m_Hp = _stateMachine.m_Hp;
        m_SafeDistance = _stateMachine.m_SafeDistance;
        m_SpawnPos = _stateMachine.m_SpawnPos;
    }

    private void LoadComponents(AIStateMachine _stateMachine)
    {
        m_Rigidbody = _stateMachine.m_Rigidbody;
        m_NavmeshAgent = _stateMachine.m_NavmeshAgent;
        m_Animator = _stateMachine.m_Animator;
        m_Transform = _stateMachine.m_Transform;
    }

    private void LoadMembers(AIStateMachine _stateMachine)
    {
        m_MainCamera = _stateMachine.m_MainCamera;
        player = _stateMachine.player;
        m_PlayerDistance = _stateMachine.m_PlayerDistance;
        m_IsStabbing = _stateMachine.m_IsStabbing;
        m_OutOfRange = _stateMachine.m_OutOfRange;
        m_OutlineScript = _stateMachine.m_OutlineScript;
    }
}