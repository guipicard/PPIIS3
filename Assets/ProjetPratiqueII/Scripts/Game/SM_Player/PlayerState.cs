using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerState
{
    protected PlayerStateMachine _StateMachine;

    // SERIALIZABLE
    protected float m_Speed;
    protected float m_RotationSpeed;
    protected float m_AttackRange;

    protected Transform m_BulletSpawner;
    private string m_DamageTag;
    private GameObject m_BossDoor;
    private Slider m_HealthBar;
    private Vector3 m_HealthBarOffset;
    private GameObject m_AimSphere;
    private GameObject m_Shield;
    private float m_MinRegenerateAmount;
    private float m_MaxRegenerateAmount;
    private float m_MinHealth;
    private float m_MaxHealth;
    private float m_MaxDamage;
    private float m_HealAmount;
    private float m_RedSpellDamage;
    private float m_RegenerateTimer;
    private float m_YellowSpellTimer;
    private float m_ShieldTimer;
    private UnityEngine.Vector3 m_AimOffset;
    private Texture2D m_MineCursor;
    private Texture2D m_AttackCursor;
    
    // COMPONENTS
    protected UnityEngine.Rigidbody m_RigidBody;
    protected UnityEngine.Animator m_Animator;
    protected UnityEngine.Transform m_Transform;
    
    
    // PRIVATES
    protected UnityEngine.Vector3 m_Direction;
    protected UnityEngine.Vector3 m_CurrentVelocity;
    protected UnityEngine.Quaternion m_TargetRotation;
    protected UnityEngine.Vector3 m_Destination;
    protected float m_StoppingDistance;
    private float m_RegenerateAmount;
    private float m_HealthCapacity;
    private float m_RegenerateElapsed;

    private float m_YellowSpellElapsed;
    private float m_ShieldElapsed;
    private float m_Hp;
    
    private Camera m_MainCamera;
    private Ray m_MouseRay;
    private Ray m_TargetRay;
    private RaycastHit m_TargetHit;
    protected RaycastHit m_HitInfo;
    protected GameObject m_TargetCrystal;
    protected GameObject m_TargetEnemy;
    protected bool m_Mining;
    
    private bool m_BlueSpell;
    private bool m_YellowSpell;
    private bool m_GreenSpell;
    private bool m_RedSpell;
    private bool m_Aiming;
    
    private int spellsCost;
    private int unlockPrice;
    protected Quaternion m_BulletRotation;

    // PUBLIC
    public Canvas m_PlayerCanvas;
    
    
    protected UnityEngine.Ray m_GroundRay;
    protected UnityEngine.RaycastHit m_GroundHit;
    
    protected static readonly int Running = Animator.StringToHash("Running");
    protected static readonly int Attack = Animator.StringToHash("Attack");
    protected static readonly int MineAnim = Animator.StringToHash("MineAnim");


    public PlayerState(PlayerStateMachine stateMachine)
    {
        Init(stateMachine);
    }
    
    private void Init(PlayerStateMachine _stateMachine)
    {
        _StateMachine = _stateMachine;

        LoadMembers(_stateMachine);
        LoadSerializable(_stateMachine);
        LoadComponents(_stateMachine);
    }

    public abstract void UpdateExecute();
    public abstract void FixedUpdateExecute();

    private void LoadSerializable(PlayerStateMachine _stateMachine)
    {
        m_Speed = _stateMachine.m_Speed;
        m_RotationSpeed = _stateMachine.m_RotationSpeed;
        m_AttackRange = _stateMachine.m_AttackRange;
        m_BulletSpawner = _stateMachine.m_BulletSpawner;
        m_DamageTag = _stateMachine.m_DamageTag;
        m_BossDoor = _stateMachine.m_BossDoor;
        m_HealthBar = _stateMachine.m_HealthBar;
        m_HealthBarOffset = _stateMachine.m_HealthBarOffset;
        m_AimSphere = _stateMachine.m_AimSphere;
        m_Shield = _stateMachine.m_Shield;
        m_MinRegenerateAmount = _stateMachine.m_MinRegenerateAmount;
        m_MaxRegenerateAmount = _stateMachine.m_MaxRegenerateAmount;
        m_MinHealth = _stateMachine.m_MinHealth;
        m_MaxHealth = _stateMachine.m_MaxHealth;
        m_MaxDamage = _stateMachine.m_MaxDamage;
        m_HealAmount = _stateMachine.m_HealAmount;
        m_RedSpellDamage = _stateMachine.m_RedSpellDamage;
        m_RegenerateTimer = _stateMachine.m_RegenerateTimer;
        m_YellowSpellTimer = _stateMachine.m_YellowSpellTimer;
        m_ShieldTimer = _stateMachine.m_ShieldTimer;
        m_AimOffset = _stateMachine.m_AimOffset;
        m_MineCursor = _stateMachine.m_MineCursor;
        m_AttackCursor = _stateMachine.m_AttackCursor;
        
        
    }

    private void LoadComponents(PlayerStateMachine _stateMachine)
    {
        m_RigidBody = _stateMachine.m_RigidBody;
        m_Animator = _stateMachine.m_Animator;
        m_Transform = _stateMachine.m_Transform;
    }

    private void LoadMembers(PlayerStateMachine _stateMachine)
    {
    m_Direction = _stateMachine.m_Direction;
    m_CurrentVelocity = _stateMachine.m_CurrentVelocity;
    m_TargetRotation = _stateMachine.m_TargetRotation;
    m_Destination = _stateMachine.m_Destination;
    m_StoppingDistance = _stateMachine.m_StoppingDistance;
    m_RegenerateAmount = _stateMachine.m_RegenerateAmount;
    m_HealthCapacity = _stateMachine.m_HealthCapacity;
    m_RegenerateElapsed = _stateMachine.m_RegenerateElapsed;

    m_YellowSpellElapsed = _stateMachine.m_YellowSpellElapsed;
    m_ShieldElapsed = _stateMachine.m_ShieldElapsed;
    m_Hp = _stateMachine.m_Hp;
    
    m_MainCamera = _stateMachine.m_MainCamera;
    m_MouseRay = _stateMachine.m_MouseRay;
    m_TargetRay = _stateMachine.m_TargetRay;
    m_TargetHit = _stateMachine.m_TargetHit;
    m_HitInfo = _stateMachine.m_HitInfo;
    m_TargetCrystal = _stateMachine.m_TargetCrystal;
    m_TargetEnemy = _stateMachine.m_TargetEnemy;
    m_Mining = _stateMachine.m_Mining;
    
    m_BlueSpell = _stateMachine.m_BlueSpell;
    m_YellowSpell = _stateMachine.m_YellowSpell;
    m_GreenSpell = _stateMachine.m_GreenSpell;
    m_RedSpell = _stateMachine.m_RedSpell;
    m_Aiming = _stateMachine.m_Aiming;
    
    spellsCost = _stateMachine.spellsCost;
    unlockPrice = _stateMachine.unlockPrice;
    m_BulletRotation = _stateMachine.m_BulletRotation;
    }
}
