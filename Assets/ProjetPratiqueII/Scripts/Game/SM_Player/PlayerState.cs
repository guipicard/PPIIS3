using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerState
{
    protected PlayerStateMachine _StateMachine;

    // SERIALIZABLE
    protected float m_Speed;
    protected float m_RotationSpeed;

    protected Transform m_BulletSpawner;
    
    // COMPONENTS
    protected Rigidbody m_RigidBody;
    protected Animator m_Animator;
    protected Transform m_Transform;
    
    
    // PRIVATES
    protected Vector3 m_Direction;
    protected Vector3 m_CurrentVelocity;
    protected Quaternion m_TargetRotation;
    protected Vector3 m_Destination;
    protected float m_StoppingDistance;
    protected GameObject m_TargetCrystal;
    protected GameObject m_TargetEnemy;
    protected bool m_Mining;
    
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
        m_BulletSpawner = _stateMachine.m_BulletSpawner;
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
    m_TargetCrystal = _stateMachine.m_TargetCrystal;
    m_TargetEnemy = _stateMachine.m_TargetEnemy;
    m_Mining = _stateMachine.m_Mining;
    }
}
