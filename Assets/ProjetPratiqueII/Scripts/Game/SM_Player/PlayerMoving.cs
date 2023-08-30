using System.Data;
using UnityEngine;

public class PlayerMoving : PlayerState
{
    public PlayerMoving(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        m_Animator.SetBool(Running, true);
        m_Animator.SetBool(MineAnim, false);
    }

    public override void UpdateExecute()
    {
    }

    public override void FixedUpdateExecute()
    {
        GetDirection();
        Move();
        Rotate();
    }

    private void Move()
    {
        m_CurrentVelocity = m_RigidBody.velocity;
        m_CurrentVelocity.x = m_Direction.x * m_Speed;
        m_CurrentVelocity.z = m_Direction.z * m_Speed;
        m_RigidBody.velocity = m_CurrentVelocity;
    }

    private void Rotate()
    {
        m_TargetRotation = Quaternion.LookRotation(m_Direction, Vector3.up);

        if (m_Transform.rotation != m_TargetRotation)
        {
            // Slerp looks smoother than Lerp
            float t = Time.fixedDeltaTime * m_RotationSpeed;
            m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, m_TargetRotation, t);
        }
    }

    private void GetDirection()
    {
        if (Vector3.Distance(m_Transform.position, m_Destination) >= m_StoppingDistance + 1.0f)
        {
            m_Direction = (m_Destination - m_Transform.position).normalized;
            m_Direction.y = 0;
        }
        else if (m_TargetCrystal != null)
        {
            // MINE
            _StateMachine.SetState(new PlayerMining(_StateMachine));
        }
        else
        {
            _StateMachine.SetState(new PlayerIdle(_StateMachine));
        }
    }

    
}
