using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAttack : AiState
{
    private float m_Elapsed;
    private bool m_Shot;
    private bool m_Launched;
    private int m_AttacksNum;
    private int m_CurrentAttack;
    private float m_ArrowRelease;

    public AiAttack(AIStateMachine stateMachine) : base(stateMachine)
    {
        switch (m_BulletTag)
        {
            case "Green_Bullet":
                m_AttacksNum = 1;
                break;
            case "Blue_Spear":
                m_AttacksNum = 3;
                break;
            case "Yellow_Hammer":
                m_AttacksNum = 1;
                m_ArrowRelease = 0.08f;
                break;
            case "Red_Arrow":
                m_AttacksNum = 2;
                m_ArrowRelease = 0.07f;
                break;
        }

        if (m_AttacksNum > 1) m_CurrentAttack = Random.Range(0, m_AttacksNum);
        m_Elapsed = 0.0f;
        m_Shot = false;
        m_Launched = false;
    }

    public override void UpdateExecute()
    {
        m_PlayerDistance = Vector3.Distance(player.transform.position, m_Transform.position);
        if (m_PlayerDistance > m_TriggerDistance)
        {
            _AiStateMachine.TargetLost();
        }
        switch (m_BulletTag)
        {
            case "Green_Bullet":
                GreenAttack();
                break;
            case "Blue_Spear":
                BlueAttack();
                break;
            case "Yellow_Hammer":
                YellowAttack();
                break;
            case "Red_Arrow":
                RedAttack();
                break;
        }
    }

    public override void FixedUpdateExecute()
    {
        Vector3 playerPosition = player.transform.position;
        m_Transform.LookAt(playerPosition);
    }

    private void LaunchBasicAttack()
    {
        GameObject bullet =
            LevelManager.instance.SpawnObj(m_BulletTag, m_BulletSpawner.position, m_Transform.rotation);
        switch (m_BulletTag)
        {
            case "Green_Bullet":
                bullet.GetComponent<GreenBullet>().SetTarget(player.transform.position, m_PlayerDistance);
                break;
            case "Red_Arrow":
                bullet.GetComponent<BulletBehaviour>().SetTarget(player.transform.position);
                break;
        }
        
    }

    private void GreenAttack()
    {
        m_Elapsed += Time.deltaTime;
        if (!m_Launched)
        {
            m_Animator.SetTrigger(attack1);
            m_Launched = true;
        }
        
        if (m_Elapsed > 0.2f && !m_Shot)
        {
            LaunchBasicAttack();
            m_Shot = true;
        }

        if (m_Elapsed > 0.32f)
        {
            _AiStateMachine.SetState(new AiDenfending(_AiStateMachine));
        }
    }

    private void BlueAttack()
    {
        m_NavmeshAgent.destination = player.transform.position;
        
        if (m_Launched)
        {
            m_Elapsed += Time.deltaTime;
        }
        
        switch (m_CurrentAttack)
        {
            case 0:
                if (m_PlayerDistance <= 1.5f && !m_Launched)
                {
                    m_Animator.SetInteger(moveState, 0);
                    m_Animator.SetTrigger(attack1);
                    m_NavmeshAgent.destination = m_Transform.position;
                    m_Launched = true;
                }
                else
                {
                    m_Animator.SetInteger(moveState, 1);
                }
                
                if (m_Elapsed > 0.15f)
                {
                    _AiStateMachine.SetState(new AiDenfending(_AiStateMachine));
                }
                else if (m_Elapsed > 0.08f && !m_Shot)
                {
                    m_Shot = true;
                    player.GetComponent<PlayerStateMachine>().TakeDmg(15.0f);
                }
                break;
            case 1:
                if (m_PlayerDistance <= 1.5f && !m_Launched)
                {
                    m_Animator.SetInteger(moveState, 0);
                    m_Animator.SetTrigger(attack2);
                    m_NavmeshAgent.destination = m_Transform.position;
                    m_Launched = true;
                }
                else
                {
                    m_Animator.SetInteger(moveState, 1);
                }
                
                if (m_Elapsed > 0.2f)
                {
                    _AiStateMachine.SetState(new AiDenfending(_AiStateMachine));
                }
                else if (m_Elapsed > 0.09f && !m_Shot)
                {
                    m_Shot = true;
                    player.GetComponent<PlayerStateMachine>().TakeDmg(20.0f);
                }
                break;
            case 2:
                if (m_PlayerDistance <= 1.5f && !m_Launched)
                {
                    m_Animator.SetInteger(moveState, 0);
                    m_Animator.SetTrigger(attack3);
                    m_NavmeshAgent.destination = m_Transform.position;
                    m_Launched = true;
                }
                else
                {
                    m_Animator.SetInteger(moveState, 1);
                }
                
                if (m_Elapsed > 0.19f)
                {
                    _AiStateMachine.SetState(new AiDenfending(_AiStateMachine));
                }
                else if (m_Elapsed > 0.07f && !m_Shot)
                {
                    m_Shot = true;
                    player.GetComponent<PlayerStateMachine>().TakeDmg(25.0f);
                }
                break;
        }
        
    }
    
    private void YellowAttack()
    {
        m_NavmeshAgent.destination = player.transform.position;
        if (m_Launched)
        {
            m_Elapsed += Time.deltaTime;
        }
        else if (m_PlayerDistance <= 1.3f && !m_Launched)
        {
            m_Animator.SetInteger(moveState, 0);
            m_Animator.SetTrigger(attack1);
            m_NavmeshAgent.destination = m_Transform.position;
            m_Launched = true;
        }
        else
        {
            m_Animator.SetInteger(moveState, 1);
        }

        if (m_Elapsed > m_ArrowRelease && !m_Shot)
        {
            player.GetComponent<PlayerStateMachine>().TakeDmg(10.0f);
            m_ArrowRelease += 0.08f;
            if (m_Elapsed > 0.16f) m_Shot = true;
        }

        if (m_Elapsed > 0.22f)
        {
            _AiStateMachine.SetState(new AiDenfending(_AiStateMachine));
        }
    }
    
    private void RedAttack()
    {
        m_Elapsed += Time.deltaTime;
        
        switch (m_CurrentAttack)
        {
            case 0:
                if (!m_Launched)
                {
                    m_Animator.SetTrigger(attack1);
                    m_Launched = true;
                }

                if (m_Elapsed > 0.2f && !m_Shot)
                {
                    m_Shot = true;
                    LaunchBasicAttack();
                }
                if (m_Elapsed > 0.3f)
                {
                    _AiStateMachine.SetState(new AiDenfending(_AiStateMachine));
                }
                break;
            case 1:
                if (!m_Launched)
                {
                    m_Animator.SetTrigger(attack2);
                    m_Launched = true;
                }

                if (m_Elapsed > m_ArrowRelease && !m_Shot)
                {
                    if (m_Elapsed > 0.07f + (0.08f * 2)) m_Shot = true;
                    LaunchBasicAttack();
                    m_ArrowRelease += 0.08f;
                }
                if (m_Elapsed > 0.3f)
                {
                    _AiStateMachine.SetState(new AiDenfending(_AiStateMachine));
                }
                break;
        }
    }
}
