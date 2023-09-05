using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStateMachine : MonoBehaviour
{
    PlayerState _currentState;

    [Space(10)]
    [Header("Controls")]
    [Space(10)] //
    [SerializeField]
    public float m_Speed;

    [SerializeField] public float m_RotationSpeed;
    [SerializeField] public float m_AttackRange;
    [SerializeField] public float m_MiningRange;
    [HideInInspector] public Vector3 m_Direction;
    [HideInInspector] public Vector3 m_CurrentVelocity;
    [HideInInspector] public Rigidbody m_RigidBody;
    [HideInInspector] public Animator m_Animator;
    [HideInInspector] public Transform m_Transform;
    [HideInInspector] public Quaternion m_TargetRotation;
    [HideInInspector] public Vector3 m_Destination;
    [HideInInspector] public float m_StoppingDistance;

    [Header("Scene Objects References")] //
    [SerializeField]
    public Transform m_BulletSpawner;

    [SerializeField] public string m_DamageTag;
    [SerializeField] public GameObject m_BossDoor;
    [SerializeField] public Slider m_HealthBar;
    [SerializeField] public Vector3 m_HealthBarOffset;
    [SerializeField] public Canvas m_PlayerCanvas;
    [SerializeField] public GameObject m_AimSphere;
    [SerializeField] public GameObject m_Shield;

    [Space(10)] [Header("Attributes")] [Space(10)] [HideInInspector]
    public float m_RegenerateAmount;

    [SerializeField] public float m_MinRegenerateAmount;
    [SerializeField] public float m_MaxRegenerateAmount;
    [HideInInspector] public float m_HealthCapacity;
    [SerializeField] public float m_MinHealth;
    [SerializeField] public float m_MaxHealth;
    [SerializeField] public float m_MaxDamage;
    [SerializeField] public float m_HealAmount;
    [SerializeField] public float m_RedSpellDamage;

    [Header("Timers")] // Timers 
    [HideInInspector]
    public float m_RegenerateElapsed;

    [HideInInspector] public float m_YellowSpellElapsed;
    [HideInInspector] public float m_ShieldElapsed;
    [HideInInspector] public float m_Hp;

    [SerializeField] public float m_RegenerateTimer;
    [SerializeField] public float m_YellowSpellTimer;
    [SerializeField] public float m_ShieldTimer;

    // HASED TAGS!!!! (get it? hastag -> # (#yoloswag))
    public static readonly int Running = Animator.StringToHash("Running");
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int MineAnim = Animator.StringToHash("MineAnim");

    // Camera / Rays / Interactions
    [HideInInspector] public Camera m_MainCamera;
    public Ray m_MouseRay;
    public Ray m_TargetRay;
    public RaycastHit m_TargetHit;
    public RaycastHit m_HitInfo;
    [HideInInspector] public GameObject m_TargetCrystal;
    [HideInInspector] public GameObject m_TargetEnemy;
    [HideInInspector] public bool m_Mining;

    [Space]
    [Header("Spells")]
    [Space] // unlocks
    [HideInInspector]
    public bool m_BlueSpell;

    [HideInInspector] public bool m_YellowSpell;
    [HideInInspector] public bool m_GreenSpell;
    [HideInInspector] public bool m_RedSpell;
    [HideInInspector] public bool m_AimingRed;
    [HideInInspector] public bool m_AimingBlue;
    [SerializeField] public Vector3 m_AimOffset;
    [HideInInspector] public int spellsCost;
    [HideInInspector] public int unlockPrice;
    [HideInInspector] public Quaternion m_BulletRotation;

    [Space]
    [Header("Cursor")]
    [Space] //
    [HideInInspector]
    private GameObject m_OutlinedGameObject;

    [SerializeField] public Texture2D m_MineCursor;
    [SerializeField] public Texture2D m_AttackCursor;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        m_BulletRotation = Quaternion.identity;
        spellsCost = LevelManager.instance.m_SpellsCost;
        unlockPrice = LevelManager.instance.m_UnlockPrice;
        m_AimSphere.SetActive(false);
        m_Shield.SetActive(false);
        m_ShieldElapsed = m_ShieldTimer;
        m_AimingBlue = false;
        m_AimingRed = false;
        m_Mining = false;
        m_BlueSpell = false;
        m_YellowSpell = false;
        m_GreenSpell = false;
        m_RedSpell = false;
        m_YellowSpellElapsed = m_YellowSpellTimer;
        m_RigidBody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        m_Transform = transform;
        m_Direction = Vector3.zero;
        m_TargetRotation = transform.rotation;
        m_MainCamera = Camera.main;
        m_HealthCapacity = m_MinHealth;
        m_Hp = m_MinHealth;
        m_RegenerateAmount = m_MinRegenerateAmount;
        m_RegenerateElapsed = 0;
        m_OutlinedGameObject = null;
        UpdateHealthBar();

        SetState(new PlayerIdle(this));
    }

    public void SetState(PlayerState state)
    {
        _currentState = state;
    }

    void Update()
    {
        SpellsInput();
        SetInteraction();


        _currentState.UpdateExecute();

        CheckBoosRoom();
        SpellTimers();
    }

    private void FixedUpdate()
    {
        m_Transform = transform;
        _currentState.FixedUpdateExecute();
    }

    private void SetInteraction()
    {
        m_TargetRay = m_MainCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(1)) // MOVE
        {
            if (Physics.Raycast(m_TargetRay, out m_TargetHit, Mathf.Infinity, 1 << 8))
            {
                m_TargetCrystal = null;
                m_Destination = m_TargetHit.point;
                m_StoppingDistance = 0;
                SetState(new PlayerMoving(this));
            }
        }

        if (Input.GetKeyDown(KeyCode.Q)) // AUTO ATTACK
        {
            if (Physics.Raycast(m_TargetRay, out m_TargetHit))
            {
                if (m_TargetHit.collider.gameObject.layer == 7)
                {
                    if (Vector3.Distance(transform.position, m_TargetHit.collider.transform.position) < m_AttackRange)
                    {
                        m_TargetCrystal = null;
                        m_TargetEnemy = m_TargetHit.collider.gameObject;
                        LaunchBasicAttack();
                    }
                    else
                    {
                        LevelManager.instance.ErrorAction?.Invoke("Target out of range.");
                    }
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke("Invalid target.");
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // MINING
        {
            if (Physics.Raycast(m_TargetRay, out m_TargetHit))
            {
                if (m_TargetHit.collider.gameObject.layer == 6)
                {
                    m_TargetCrystal = m_TargetHit.collider.gameObject;
                    m_StoppingDistance = m_MiningRange;
                    m_Destination = m_TargetCrystal.transform.position;
                    SetState(new PlayerMoving(this));
                }
                else
                {
                    m_Mining = false;
                    m_TargetCrystal = null;
                }
            }
        }
        else
        {
            if (Physics.Raycast(m_TargetRay, out m_TargetHit))
            {
                if (m_OutlinedGameObject != null)
                {
                    if (m_OutlinedGameObject != m_TargetHit.collider.gameObject || m_AimingRed)
                    {
                        m_OutlinedGameObject.GetComponent<Outline>().enabled = false;
                        m_OutlinedGameObject = null;
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    }
                }

                if (m_TargetHit.collider.gameObject.layer == 6 || m_TargetHit.collider.gameObject.layer == 7)
                {
                    m_OutlinedGameObject = m_TargetHit.collider.gameObject;
                    ToggleEnableOutline(true);
                }
            }
        }
    }

    private void LaunchBasicAttack()
    {
        Vector3 bsPos = m_BulletSpawner.position;
        Quaternion bsRotation = m_BulletSpawner.rotation;
        GameObject bullet = LevelManager.instance.SpawnObj("Player_Bullet", bsPos, bsRotation);
        bullet.GetComponent<PlayerBullet>().SetTarget(m_TargetEnemy, bsPos);
    }

    private void StartTime()
    {
        Time.timeScale = 1;
    }

    private void StopTime()
    {
        Time.timeScale = 0;
    }

    private void UpdateHealthBar()
    {
        m_HealthBar.value = m_Hp / m_HealthCapacity;
    }

    public void TakeDmg(float damage)
    {
        m_Hp -= damage;
        if (m_Hp <= 0)
        {
            Death();
        }

        UpdateHealthBar();
        m_RegenerateElapsed = m_RegenerateTimer;
    }

    private void Death()
    {
        m_Hp = 0;
    }

    public void Heal(float amount)
    {
        m_Hp += amount;
        if (m_Hp > m_HealthCapacity) m_Hp = m_HealthCapacity;
        UpdateHealthBar();
    }

    private void SpellsInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Blue
        {
            if (m_BlueSpell)
            {
                if (LevelManager.instance.GetSpellAvailable("Blue"))
                {
                    m_AimingBlue = !m_AimingBlue;
                    LevelManager.instance.ActiveAction("Blue", m_AimingBlue);
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
                if (LevelManager.instance.GetSpellAvailable("Green") && m_Hp < m_HealthCapacity)
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
                    LevelManager.instance.RedSpellAction = null;
                    m_AimingRed = !m_AimingRed;
                    m_AimSphere.SetActive(m_AimingRed);
                    LevelManager.instance.ActiveAction("Red", m_AimingRed);
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
    }

    private void unlockSpell(string _color)
    {
        switch (_color)
        {
            case "Blue":
                m_BlueSpell = true;
                LevelManager.instance.CollectAction?.Invoke(-unlockPrice, "Blue");
                m_HealthCapacity = m_MaxHealth;
                m_Hp += 50.0f;
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

    private void BlueSpell(GameObject _crystal)
    {
        LevelManager.instance.CollectAction?.Invoke(-spellsCost, "Blue");
        LevelManager.instance.SpellCastAction?.Invoke("Blue");
        LevelManager.instance.SetSpellAvailable("Blue", false);

        HashSet<GameObject> wavesAll = new HashSet<GameObject>();
        HashSet<GameObject> waveOne = new HashSet<GameObject>();
        HashSet<GameObject> waveTwo = new HashSet<GameObject>();
        float CrystalSpacing = LevelManager.instance.m_CrystalSpaceBetween;
        float crystalHeight = _crystal.transform.position.y;

        Vector2[] surroundOffsets = new Vector2[4]
        {
            new Vector2(CrystalSpacing, CrystalSpacing),
            new Vector2(-CrystalSpacing, CrystalSpacing),
            new Vector2(CrystalSpacing, -CrystalSpacing),
            new Vector2(-CrystalSpacing, -CrystalSpacing)
        };

        wavesAll.Add(_crystal);
        Ray blueRay = new Ray();
        RaycastHit blueHit = new RaycastHit();
        foreach (var pos in surroundOffsets)
        {
            Vector2 crystalPos = new Vector2(_crystal.transform.position.x, _crystal.transform.position.z);
            Vector2 currentPosition = pos + crystalPos;

            blueRay.origin = new Vector3(currentPosition.x, crystalHeight + 2.0f, currentPosition.y);
            blueRay.direction = Vector3.down;
            if (Physics.Raycast(blueRay, out blueHit, Mathf.Infinity))
            {
                if (blueHit.collider.gameObject.layer == 6)
                {
                    waveOne.Add(blueHit.collider.gameObject);
                    wavesAll.Add(blueHit.collider.gameObject);
                }
            }
        }

        

        foreach (var crystal in waveOne)
        {
            foreach (var pos in surroundOffsets)
            {
                Vector2 crystalPos = new Vector2(crystal.transform.position.x, crystal.transform.position.z);
                Vector2 currentPosition = pos + crystalPos;

                blueRay.origin = new Vector3(currentPosition.x, crystalHeight + 2.0f, currentPosition.y);
                blueRay.direction = Vector3.down;
                if (Physics.Raycast(blueRay, out blueHit, Mathf.Infinity))
                {
                    if (blueHit.collider.gameObject.layer == 6)
                    {
                        waveTwo.Add(blueHit.collider.gameObject);
                        wavesAll.Add(blueHit.collider.gameObject);
                    }
                }
            }
        }


        foreach (var crystal in wavesAll)
        {
            crystal.GetComponent<CrystalEvents>().GetMined();
        }
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
        LevelManager.instance.ActiveAction("Yellow", true);
    }

    private void RedSpell()
    {
        LevelManager.instance.CollectAction?.Invoke(-spellsCost, "Red");
        LevelManager.instance.RedSpellAction?.Invoke(m_RedSpellDamage);
        LevelManager.instance.SpellCastAction?.Invoke("Red");
        LevelManager.instance.SetSpellAvailable("Red", false);
        m_AimSphere.SetActive(false);
    }

    private void SpellTimers()
    {
        if (m_AimingBlue)
        {
            LayerMask crystalsLayer = 1 << 6;
            m_MouseRay = m_MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(m_MouseRay, out m_TargetHit, Mathf.Infinity, crystalsLayer))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    m_AimingBlue = false;
                    BlueSpell(m_TargetHit.collider.gameObject);
                    m_TargetCrystal = null;
                    LevelManager.instance.ActiveAction("Blue", false);
                }
            }

            if (Input.GetMouseButton(1))
            {
                m_AimingBlue = false;
                LevelManager.instance.ActiveAction("Blue", false);
            }
        }

        // ============
        // if (m_ShieldElapsed < m_ShieldTimer)
        // {
        //     m_Shield.transform.position = transform.position + new Vector3(0, 0.5f, 0);
        //     m_ShieldElapsed += Time.deltaTime;
        // }
        // else if (m_Shield.activeSelf)
        // {
        //     m_Shield.SetActive(false);
        //     LevelManager.instance.SpellCastAction?.Invoke("Blue");
        // }
        // ============
        if (m_YellowSpellElapsed < m_YellowSpellTimer)
        {
            LevelManager.instance.SetPlayerDamage(100);
            m_YellowSpellElapsed += Time.deltaTime;
        }
        else if (LevelManager.instance.playerDamage == 100)
        {
            LevelManager.instance.SetPlayerDamage(m_MaxDamage);
            LevelManager.instance.SpellCastAction?.Invoke("Yellow");
            LevelManager.instance.ActiveAction?.Invoke("Yellow", false);
        }

        if (m_AimingRed)
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
                m_AimingRed = false;
                RedSpell();
                m_AimSphere.SetActive(false);
                LevelManager.instance.ActiveAction("Red", false);
                LevelManager.instance.RedSpellAction = null;
            }
            else if (Input.GetMouseButton(1))
            {
                m_AimingRed = false;
                m_AimSphere.SetActive(false);
                LevelManager.instance.ActiveAction("Red", false);
                LevelManager.instance.RedSpellAction = null;
            }
        }

        if (m_RegenerateElapsed >= 0.0f)
        {
            m_RegenerateElapsed -= Time.deltaTime;
        }
        else if (m_Hp < m_MaxHealth)
        {
            Heal(m_MinRegenerateAmount);
        }
    }

    private void CheckBoosRoom()
    {
        if (m_BlueSpell && m_YellowSpell && m_GreenSpell && m_RedSpell && m_BossDoor.activeSelf)
        {
            m_BossDoor.SetActive(false);
            LevelManager.instance.ErrorAction?.Invoke(
                "Boss room has been opened but cannot be accessed for the moment.");
        }
    }

    private void ToggleEnableOutline(bool _state)
    {
        if (m_OutlinedGameObject == null) return;

        m_OutlinedGameObject.GetComponent<Outline>().enabled = _state;
        if (m_OutlinedGameObject.layer == 6)
        {
            Cursor.SetCursor(m_MineCursor, Vector2.zero, CursorMode.Auto);
        }
        else if (m_OutlinedGameObject.layer == 7)
        {
            Cursor.SetCursor(m_AttackCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}