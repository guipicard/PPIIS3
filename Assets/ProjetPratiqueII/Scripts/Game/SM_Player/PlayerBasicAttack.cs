using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttack : PlayerState
{
    private float m_Elapsed;
    private Quaternion m_BulletRotation;
    public PlayerBasicAttack(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        m_BulletRotation = new Quaternion();
        m_RigidBody.velocity = Vector3.zero;
        m_Animator.SetBool(Running, false);
        m_Elapsed = 0.0f;
        BasicAttack();
    }

    public override void UpdateExecute()
    {
        m_Elapsed += Time.deltaTime;
        if (m_Elapsed >= 0.075)
        {
            LaunchBasicAttack();
            _StateMachine.SetState(new PlayerIdle(_StateMachine));
        }
    }

    public override void FixedUpdateExecute()
    {
    }
    
    private void BasicAttack()
    {
        m_Transform.LookAt(m_TargetEnemy.transform.position);
        m_BulletRotation = m_Transform.rotation;
        m_Animator.SetTrigger(Attack);
    }
    
    private void LaunchBasicAttack()
    {
        // LevelManager.instance.SetTarget(m_TargetEnemy);
        GameObject bullet =  LevelManager.instance.SpawnObj("Player_Bullet", m_BulletSpawner.position, m_BulletSpawner.transform.rotation);
        bullet.GetComponent<PlayerBullet>().SetTarget(m_TargetEnemy, m_BulletSpawner.transform.position, m_Transform);
    }
}
