using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerController : MonoBehaviour
{
    [Header("Controls")] [SerializeField] private float m_Speed;
    [SerializeField] private float m_RotationSpeed;
    [SerializeField] private float m_AttackRange;
    private Vector3 m_Direction;
    private Vector3 m_CurrentVelocity;
    private Rigidbody m_RigidBody;
    private Animator m_Animator;
    private Quaternion targetRotation;
    private Vector3 m_Destination;
    private float m_StoppingDistance;

    [Header("Scene Objects References")] [SerializeField]
    private Transform m_BulletSpawner;

    [SerializeField] private string m_DamageTag;
    [SerializeField] private GameObject m_BossDoor;
    [SerializeField] private Slider m_HealthBar;
    [SerializeField] private Vector3 m_HealthBarOffset;
    public Canvas m_PlayerCanvas;
    [SerializeField] private GameObject m_AimSphere;
    [SerializeField] private GameObject m_Shield;

    [Space(10)] [Header("Attributes")] [Space(10)]
    private float m_RegenerateAmount;

    [SerializeField] private float m_MinRegenerateAmount;
    [SerializeField] private float m_MaxRegenerateAmount;
    private float m_HealthCapacity;
    [SerializeField] private float m_MinHealth;
    [SerializeField] private float m_MaxHealth;
    [SerializeField] private float m_MaxDamage;
    [SerializeField] private float m_HealAmount;
    [SerializeField] private float m_RedSpellDamage;

    [Header("Timers")] // Timers 
    private float m_RegenerateElapsed;
    private float m_YellowSpellElapsed;
    private float m_ShieldElapsed;
    private float Hp;

    [SerializeField] private float m_RegenerateTimer;
    [SerializeField] private float m_YellowSpellTimer;
    [SerializeField] private float m_ShieldTimer;

    // HASED TAGS!!!! (get it? hastag -> # (#yoloswag))
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int MineAnim = Animator.StringToHash("MineAnim");

    // Camera / Rays / Interactions
    private Camera m_MainCamera;
    private Ray m_MouseRay;
    private Ray m_TargetRay;
    private RaycastHit m_TargetHit;
    private RaycastHit m_HitInfo;
    private GameObject m_TargetCrystal;
    private GameObject m_TargetEnemy;
    private bool m_Mining;

    [Space] [Header("Spells")] [Space] // unlocks
    private bool m_BlueSpell;

    private bool m_YellowSpell;
    private bool m_GreenSpell;
    private bool m_RedSpell;
    private bool m_Aiming;
    [SerializeField] private Vector3 m_AimOffset;
    private int spellsCost;
    private int unlockPrice;
    private Quaternion m_BulletRotation;

    [Space] [Header("Cursor")] [Space]
    private GameObject m_OutlinedGameObject;
    [SerializeField] private Texture2D m_MineCursor;
    [SerializeField] private Texture2D m_AttackCursor;


    void Start()
    {
        m_BulletRotation = Quaternion.identity;
        spellsCost = LevelManager.instance.m_SpellsCost;
        unlockPrice = LevelManager.instance.m_UnlockPrice;
        m_AimSphere.SetActive(false);
        m_Shield.SetActive(false);
        m_ShieldElapsed = m_ShieldTimer;
        m_Aiming = false;
        m_Mining = false;

        m_BlueSpell = false;
        m_YellowSpell = false;
        m_GreenSpell = false;
        m_RedSpell = false;

        m_YellowSpellElapsed = m_YellowSpellTimer;
        m_RigidBody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();

        m_Direction = Vector3.zero;

        targetRotation = Quaternion.identity;

        m_MainCamera = Camera.main;

        m_HealthCapacity = m_MinHealth;
        Hp = m_MinHealth;
        m_RegenerateAmount = m_MinRegenerateAmount;
        m_RegenerateElapsed = 0;
        UpdateHealthBar();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Blue
        {
            if (m_BlueSpell)
            {
                if (LevelManager.instance.GetSpellAvailable("Blue"))
                {
                    m_Shield.SetActive(true);
                    BlueSpell();
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke("Spell not available.");
                }
            }
            else
            {
                if (LevelManager.instance.GetCollected("Blue") >= unlockPrice)
                {
                    unlockSpell("Blue");
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke(
                        $"Collect {unlockPrice} blue crystals to unlock this spell.");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // Yellow
        {
            if (m_YellowSpell)
            {
                if (LevelManager.instance.GetSpellAvailable("Yellow"))
                {
                    YellowSpell();
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke("Spell not available.");
                }
            }
            else
            {
                if (LevelManager.instance.GetCollected("Yellow") >= unlockPrice)
                {
                    unlockSpell("Yellow");
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke(
                        $"Collect {unlockPrice} yellow crystals to unlock this spell.");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) // Green
        {
            if (m_GreenSpell)
            {
                if (LevelManager.instance.GetSpellAvailable("Green") && Hp < m_HealthCapacity)
                {
                    GreenSpell();
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke("Spell not available.");
                }
            }
            else
            {
                if (LevelManager.instance.GetCollected("Green") >= unlockPrice)
                {
                    unlockSpell("Green");
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke(
                        $"Collect {unlockPrice} green crystals to unlock this spell.");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) // Red
        {
            if (m_RedSpell)
            {
                if (LevelManager.instance.GetSpellAvailable("Red"))
                {
                    m_Aiming = true;
                    m_AimSphere.SetActive(true);
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke("Spell not available.");
                }
            }
            else
            {
                if (LevelManager.instance.GetCollected("Red") >= unlockPrice)
                {
                    unlockSpell("Red");
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke(
                        $"Collect {unlockPrice} red crystals to unlock this spell.");
                }
            }
        }


        if (m_OutlinedGameObject != null)
        {
            ToggleEnableOutline(true);
        }


        if (Input.GetMouseButtonDown(1))
        {
            m_TargetRay = m_MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(m_TargetRay, out m_HitInfo))
            {
                if (m_HitInfo.collider.gameObject.layer == 7)
                {
                    m_StoppingDistance = 10.0f;
                    m_Destination = m_HitInfo.point;
                    m_TargetEnemy = m_HitInfo.collider.gameObject;
                }
                else
                {
                    m_TargetEnemy = null;
                    m_StoppingDistance = 0;
                }

                if (m_HitInfo.collider.gameObject.layer == 8)
                {
                    m_Destination = m_HitInfo.point;
                }

                if (m_HitInfo.collider.gameObject.layer == 6)
                {
                    m_Destination = m_HitInfo.point;
                    m_TargetCrystal = m_HitInfo.collider.gameObject;
                    m_StoppingDistance = 3.0f;
                }
                else
                {
                    m_Mining = false;
                    m_TargetCrystal = null;
                    m_StoppingDistance = 0;
                }
            }
        }

        GetDirection();
        
        if (m_TargetCrystal != null)
        {
            if (m_Direction == Vector3.zero)
            {
                transform.LookAt(m_HitInfo.collider.transform.position);
                if (!m_Mining) m_Animator.SetTrigger(MineAnim);
                m_Mining = true;
            }
        }
        else if (m_TargetEnemy != null)
        {
            if (Vector3.Distance(transform.position, m_HitInfo.transform.position) <= m_AttackRange)
            {
                BasicAttack();
            }
        }


        if (m_ShieldElapsed < m_ShieldTimer)
        {
            m_Shield.transform.position = transform.position + new Vector3(0, 0.5f, 0);
            m_ShieldElapsed += Time.deltaTime;
        }
        else if (m_Shield.activeSelf)
        {
            m_Shield.SetActive(false);
            LevelManager.instance.SpellCastAction?.Invoke("Blue");
        }

        if (m_YellowSpellElapsed < m_YellowSpellTimer)
        {
            LevelManager.instance.SetPlayerDamage(100);
            m_YellowSpellElapsed += Time.deltaTime;
        }
        else if (LevelManager.instance.playerDamage == 100)
        {
            LevelManager.instance.SetPlayerDamage(m_MaxDamage);
            LevelManager.instance.SpellCastAction?.Invoke("Yellow");
        }

        if (m_Aiming)
        {
            LayerMask groundLayer = 1 << 8;
            m_MouseRay = m_MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(m_MouseRay, out m_TargetHit, Mathf.Infinity, groundLayer))
            {
                if (Time.timeScale == 1.0f)
                {
                    Vector3 pos = m_TargetHit.point;
                    pos.y = 1.0f;
                    pos -= m_AimOffset;
                    m_AimSphere.transform.position = pos;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                m_Aiming = false;
                RedSpell();
            }
            else if (Input.GetMouseButton(1))
            {
                m_Aiming = false;
                LevelManager.instance.RedSpellAction = null;
            }
        }
        else
        {
            if (m_AimSphere.activeSelf) m_AimSphere.SetActive(false);

            if (Physics.Raycast(m_MouseRay, out m_TargetHit, Mathf.Infinity))
            {
                Outline outlineComponent = m_TargetHit.collider.gameObject.GetComponent<Outline>();
                if (outlineComponent && Time.timeScale == 1.0f)
                {
                    m_OutlinedGameObject = m_TargetHit.collider.gameObject;
                }
                else
                {
                    if (m_OutlinedGameObject != null) ToggleEnableOutline(false);
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    m_OutlinedGameObject = null;
                }
            }
        }


        if (m_BlueSpell && m_YellowSpell && m_GreenSpell && m_RedSpell && m_BossDoor.activeSelf)
        {
            m_BossDoor.SetActive(false);
            LevelManager.instance.ErrorAction?.Invoke(
                "Boss room has been opened but cannot be accessed for the moment.");
        }

        if (m_RegenerateElapsed >= 0.0f)
        {
            m_RegenerateElapsed -= Time.deltaTime;
        }
        else if (Hp < m_MaxHealth)
        {
            Heal(m_MinRegenerateAmount);
        }

        m_PlayerCanvas.transform.position = transform.position + m_HealthBarOffset;
        m_PlayerCanvas.transform.LookAt(m_MainCamera.transform.position);
    }

    private void FixedUpdate()
    {
        m_CurrentVelocity = m_RigidBody.velocity;
        if (m_Direction != Vector3.zero)
        {
            Move();
            targetRotation = Quaternion.LookRotation(m_Direction, Vector3.up);
            Rotate();
        }
        else
        {
            m_CurrentVelocity.x = 0;
            m_CurrentVelocity.z = 0;
        }

        m_RigidBody.velocity = m_CurrentVelocity;
        Animate();
        targetRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(m_DamageTag))
        {
            if (m_ShieldElapsed >= m_ShieldTimer) TakeDmg(20.0f);
            LevelManager.instance.ToggleInactive(other.gameObject);
        }
        else if (other.gameObject.layer == 9)
        {
            LevelManager.instance.CollectAction?.Invoke(1,
                other.gameObject.GetComponent<CrystalPartsBehaviour>().m_Color);
        }
    }

    private void GetDirection()
    {
        if (Vector3.Distance(transform.position, m_Destination) >= m_StoppingDistance + 0.25f)
        {
            m_Direction = (m_Destination - transform.position).normalized;
            m_Direction.y = 0;
        }
        else
        {
            m_Direction = Vector3.zero;
        }
    }

    private void Move()
    {
        m_CurrentVelocity.x = m_Direction.x * m_Speed;
        m_CurrentVelocity.z = m_Direction.z * m_Speed;
    }

    private void Rotate()
    {
        if (transform.rotation != targetRotation)
        {
            // Slerp looks smoother than Lerp
            transform.rotation =
                Quaternion.Slerp(m_RigidBody.rotation, targetRotation, Time.fixedDeltaTime * m_RotationSpeed);
        }
    }

    private void Animate()
    {
        // run
        m_Animator.SetBool(Running, m_Direction != Vector3.zero);
    }

    private void UpdateHealthBar()
    {
        m_HealthBar.value = Hp / m_HealthCapacity;
    }

    private void TakeDmg(float damage)
    {
        Hp -= damage;
        if (Hp <= 0)
        {
            Death();
        }

        UpdateHealthBar();
        m_RegenerateElapsed = m_RegenerateTimer;
    }

    private void Death()
    {
        Hp = 0;
    }

    private void BasicAttack()
    {
        m_Destination = transform.position;
        transform.LookAt(m_HitInfo.collider.transform.position);
        m_BulletRotation = transform.rotation;
        m_Animator.SetTrigger(Attack);
        m_TargetEnemy = null;
    }

    private void LaunchBasicAttack()
    {
        LevelManager.instance.SpawnObj("Player_Bullet", m_BulletSpawner.position, m_BulletRotation);
    }

    private void MineCrystal()
    {
        if (m_TargetCrystal != null && m_HitInfo.collider.gameObject.layer == 6)
        {
            m_TargetCrystal.GetComponent<CrystalEvents>().GetMined();
            m_TargetCrystal = null;
            m_Mining = false;
        }
    }

    private void BlueSpell()
    {
        m_ShieldElapsed = 0;
        LevelManager.instance.CollectAction?.Invoke(-spellsCost, "Blue");
        LevelManager.instance.SetSpellAvailable("Blue", false);
    }

    private void GreenSpell()
    {
        Heal(m_HealAmount);
        LevelManager.instance.CollectAction?.Invoke(-spellsCost, "Green");
        LevelManager.instance.SpellCastAction?.Invoke("Green");
        LevelManager.instance.SetSpellAvailable("Green", false);
    }

    private void YellowSpell()
    {
        m_YellowSpellElapsed = 0.0f;
        LevelManager.instance.CollectAction?.Invoke(-spellsCost, "Yellow");
        LevelManager.instance.SetSpellAvailable("Yellow", false);
    }

    private void RedSpell()
    {
        LevelManager.instance.CollectAction?.Invoke(-spellsCost, "Red");
        LevelManager.instance.RedSpellAction?.Invoke(m_RedSpellDamage);
        LevelManager.instance.SpellCastAction?.Invoke("Red");
        LevelManager.instance.SetSpellAvailable("Red", false);
    }

    public void Heal(float amount)
    {
        Hp += amount;
        if (Hp > m_HealthCapacity) Hp = m_HealthCapacity;
        UpdateHealthBar();
    }

    private void unlockSpell(string _color)
    {
        switch (_color)
        {
            case "Blue":
                m_BlueSpell = true;
                LevelManager.instance.CollectAction?.Invoke(-unlockPrice, "Blue");
                m_HealthCapacity = m_MaxHealth;
                Hp += 50.0f;
                break;
            case "Green":
                m_GreenSpell = true;
                LevelManager.instance.CollectAction?.Invoke(-unlockPrice, "Green");
                m_RegenerateAmount = m_MaxRegenerateAmount;
                break;
            case "Yellow":
                m_YellowSpell = true;
                LevelManager.instance.CollectAction?.Invoke(-unlockPrice, "Yellow");
                break;
            case "Red":
                m_RedSpell = true;
                LevelManager.instance.CollectAction?.Invoke(-unlockPrice, "Red");
                break;
        }
    }

    private void ToggleEnableOutline(bool _state)
    {
        if (m_OutlinedGameObject.layer == 6)
        {
            m_OutlinedGameObject.GetComponent<CrystalEvents>().GetOutlineComponent().enabled = _state;
            Cursor.SetCursor(m_MineCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (m_OutlinedGameObject.layer == 7)
        {
            m_OutlinedGameObject.GetComponent<AiBehaviour>().GetOutlineComponent().enabled = _state;
            Cursor.SetCursor(m_AttackCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}
