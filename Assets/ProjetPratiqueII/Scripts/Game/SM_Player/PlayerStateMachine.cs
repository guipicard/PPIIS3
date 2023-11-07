using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] public Slider m_HealthBar;
    [SerializeField] public Canvas m_PlayerCanvas;
    [SerializeField] public GameObject m_AimSphere;

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
    private float m_FullHpElapsed;

    [Header("Timers")] // Timers 
    [HideInInspector]
    public float m_RegenerateElapsed;

    [HideInInspector] public float m_YellowSpellElapsed;
    [HideInInspector] public float m_Hp;

    [SerializeField] public float m_RegenerateTimer;
    [SerializeField] public float m_YellowSpellTimer;

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
    [HideInInspector] public bool m_AimingYellow;
    [SerializeField] public Vector3 m_AimOffset;
    [SerializeField] public GameObject m_Lightning;
    private int m_BlueSpellCost;
    private int m_GreenSpellCost;
    private int m_RedSpellCost;
    private int m_YellowSpellCost;
    [HideInInspector] public int unlockPrice;

    [Space]
    [Header("Blue Spell")]
    [Space] //
    [SerializeField]
    private GameObject m_BlueBall;

    [Space]
    [Header("Cursor")]
    [Space] //
    [HideInInspector]
    private GameObject m_OutlinedGameObject;

    [SerializeField] public Texture2D m_MineCursor;
    [SerializeField] public Texture2D m_AttackCursor;

    private bool m_YellowSpellActive;

    [SerializeField] private GameObject HealingVFX;
    private float m_HealingVFXElapsed;

    private string m_CurrentSun;
    [SerializeField] private List<string> m_Colors;
    [SerializeField] private List<GameObject> m_SunsObj;
    private Dictionary<string, GameObject> m_Suns;


    struct CrystalWave
    {
        public int wave;
        public GameObject crystal;
        public HashSet<GameObject> next;

        public CrystalWave(int wave, GameObject crystal)
        {
            this.wave = wave;
            this.crystal = crystal;
            this.next = new HashSet<GameObject>();
        }
    }

    void Start()
    {
        Init();
    }

    private void Init()
    {
        m_MainCamera = Camera.main;
        m_FullHpElapsed = 0.0f;
        m_Suns = new Dictionary<string, GameObject>();
        for (int i = 0; i < m_SunsObj.Count; i++)
        {
            m_Suns.Add(m_Colors[i], m_SunsObj[i]);
        }

        m_CurrentSun = "";
        m_HealingVFXElapsed = 0.0f;
        m_BlueSpellCost = LevelManager.instance.m_BlueSpellCost;
        m_GreenSpellCost = LevelManager.instance.m_GreenSpellCost;
        m_RedSpellCost = LevelManager.instance.m_RedSpellCost;
        m_YellowSpellCost = LevelManager.instance.m_YellowSpellCost;
        unlockPrice = LevelManager.instance.m_UnlockPrice;
        m_AimSphere.SetActive(false);
        m_AimingYellow = false;
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
        m_TargetRotation = m_Transform.rotation;
        m_HealthCapacity = m_MinHealth;
        m_Hp = m_MinHealth;
        m_RegenerateAmount = m_MinRegenerateAmount;
        m_RegenerateElapsed = 0;
        m_OutlinedGameObject = null;
        UpdateHealthBar();
        m_YellowSpellActive = false;

        m_Transform.position = LevelManager.instance.Worlds[0].entrancePosition;
        m_Transform.eulerAngles = LevelManager.instance.Worlds[0].entranceRotation;

        LevelManager.instance.NextBiomeAction += TeleportNext;
        LevelManager.instance.LastBiomeAction += TeleportLast;
        LevelManager.instance.NextAnimAction += TeleportNextAnim;
        LevelManager.instance.LastAnimAction += TeleportLastAnim;
        LevelManager.instance.SpellUnlockAction += unlockSpell;

        SetState(new PlayerIdle(this));
    }

    public void SetState(PlayerState state)
    {
        _currentState = state;
    }

    void Update()
    {
        if (LevelManager.instance.takeInput)
        {
            SpellTimers();
            SpellsInput();
            SetInteraction();
        }

        if (m_Hp == m_HealthCapacity)
        {
            m_FullHpElapsed += Time.deltaTime;
            if (m_FullHpElapsed > 2.0f)
            {
                Image sprite1 = m_HealthBar.transform.GetChild(0).GetComponent<Image>();
                Image sprite2 = m_HealthBar.transform.GetChild(1).GetChild(0).GetComponent<Image>();
                Color color1 = sprite1.color;
                Color color2 = sprite2.color;
                float alpha = Mathf.Lerp(1.0f, 0.0f, m_FullHpElapsed - 2.0f);
                color1.a = alpha;
                color2.a = alpha;
                sprite1.color = color1;
                sprite2.color = color2;
            }
        }

        _currentState.UpdateExecute();
    }

    private void FixedUpdate()
    {
        m_Transform = transform;
        _currentState.FixedUpdateExecute();
    }

    private void SetInteraction()
    {
        m_TargetRay = m_MainCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButton(1)) // MOVE
        {
            if (Physics.Raycast(m_TargetRay, out m_TargetHit, Mathf.Infinity, 1 << 8))
            {
                m_TargetCrystal = null;
                m_Destination = m_TargetHit.point;
                m_StoppingDistance = 0;
                SetState(new PlayerMoving(this));
            }
        }

        if (Input.GetMouseButtonDown(0)) // MINING
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
                else if (m_TargetHit.collider.gameObject.CompareTag("CaveMan"))
                {
                    if (!m_BlueSpell) unlockSpell("Blue");
                    LevelManager.instance.LevelUp();
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
        bullet.GetComponent<PlayerBullet>().SetTarget(m_TargetEnemy, bsPos, m_Transform);
        AudioManager.instance.PlaySound(SoundClip.BlueSpellLaunch, 1.0f, bsPos);
    }

    private void UpdateHealthBar()
    {
        m_HealthBar.value = m_Hp / m_HealthCapacity;
    }

    public void TakeDmg(float damage)
    {
        if (!LevelManager.instance.playerGodmode)
        {
            m_Hp -= damage;
            if (m_Hp <= 0)
            {
                Death();
            }

            UpdateHealthBar();
            m_RegenerateElapsed = m_RegenerateTimer;

            Image sprite1 = m_HealthBar.transform.GetChild(0).GetComponent<Image>();
            Image sprite2 = m_HealthBar.transform.GetChild(1).GetChild(0).GetComponent<Image>();
            Color color1 = sprite1.color;
            Color color2 = sprite2.color;
            color1.a = 1.0f;
            color2.a = 1.0f;
            // float alpha = Mathf.Lerp(1.0f, 0.0f, m_FullHpElapsed - 2.0f);
            sprite1.color = color1;
            sprite2.color = color2;
            m_FullHpElapsed = 0.0f;
        }
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
            if (LevelManager.instance.GetSpellAvailable("Blue"))
            {
                if (Physics.Raycast(m_TargetRay, out m_TargetHit))
                {
                    if (m_TargetHit.collider.gameObject.layer == 7)
                    {
                        if (Vector3.Distance(transform.position, m_TargetHit.collider.transform.position) <
                            m_AttackRange)
                        {
                            m_TargetCrystal = null;
                            m_TargetEnemy = m_TargetHit.collider.gameObject;
                            BlueSpell();
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
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // Green
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
        }

        if (Input.GetKeyDown(KeyCode.Alpha3)) // Red
        {
            if (m_RedSpell)
            {
                if (LevelManager.instance.GetSpellAvailable("Red"))
                {
                    LevelManager.instance.RedSpellAction = null;
                    m_AimingRed = !m_AimingRed;
                    if (m_AimingRed)
                    {
                        ChangeSun("Red");
                    }
                    else
                    {
                        ChangeSun("Blue");
                    }

                    m_AimSphere.SetActive(m_AimingRed);
                    LevelManager.instance.ActiveAction("Red", m_AimingRed);
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke("Spell not available.");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha4)) // Yellow
        {
            if (m_YellowSpell)
            {
                if (LevelManager.instance.GetSpellAvailable("Yellow"))
                {
                    m_AimingYellow = !m_AimingYellow;
                    if (m_AimingYellow)
                    {
                        ChangeSun("Yellow");
                    }
                    else
                    {
                        ChangeSun("Blue");
                    }

                    LevelManager.instance.ActiveAction("Yellow", m_AimingYellow);
                }
                else
                {
                    LevelManager.instance.ErrorAction?.Invoke("Spell not available.");
                }
            }
        }
    }

    private void unlockSpell(string _color)
    {
        var position = transform.position;
        VfxManager.instance.PlayVfx(VfxClip.Buff, position);
        AudioManager.instance.PlaySound(SoundClip.Buff, 1f, position);
        switch (_color)
        {
            case "Blue":
                m_BlueSpell = true;
                ChangeSun(_color);
                break;
            case "Green":
                m_GreenSpell = true;
                m_RegenerateAmount = m_MaxRegenerateAmount;
                break;
            case "Red":
                m_RedSpell = true;
                m_HealthCapacity = m_MaxHealth;
                break;
            case "Yellow":
                m_YellowSpell = true;
                LevelManager.instance.SetPlayerDamage(m_MaxDamage);
                break;
        }
    }

    private void BlueSpell()
    {
        ChangeSun("Blue");
        LevelManager.instance.CollectAction?.Invoke(-m_BlueSpellCost, "Blue");
        LaunchBasicAttack();
    }


    private void GreenSpell()
    {
        ChangeSun("Green");
        LevelManager.instance.CollectAction?.Invoke(-m_GreenSpellCost, "Green");
        LevelManager.instance.SpellCastAction?.Invoke("Green");
        LevelManager.instance.SetSpellAvailable("Green", false);
        m_HealingVFXElapsed = 0.0f;
        var position = transform.position;
        VfxManager.instance.PlayVfx(VfxClip.Heal, position);
        AudioManager.instance.PlaySound(SoundClip.Heal, 1f, position);
        Heal(m_HealAmount);
    }

    private void RedSpell()
    {
        ChangeSun("Red");
        LevelManager.instance.CollectAction?.Invoke(-m_RedSpellCost, "Red");
        LevelManager.instance.RedSpellAction?.Invoke(m_RedSpellDamage);
        LevelManager.instance.SpellCastAction?.Invoke("Red");
        LevelManager.instance.SetSpellAvailable("Red", false);
        VfxManager.instance.PlayVfx(VfxClip.RedSpell, m_AimSphere.transform.position);
        m_AimSphere.SetActive(false);
    }

    private void YellowSpell(GameObject _crystal)
    {
        ChangeSun("Yellow");
        m_YellowSpellElapsed = 0.0f;
        LevelManager.instance.CollectAction?.Invoke(-m_YellowSpellCost, "Yellow");
        LevelManager.instance.SpellCastAction?.Invoke("Yellow");
        LevelManager.instance.SetSpellAvailable("Yellow", false);

        HashSet<HashSet<CrystalWave>> wavesAll = new HashSet<HashSet<CrystalWave>>();
        HashSet<CrystalWave> wavesOne = new HashSet<CrystalWave>();
        HashSet<CrystalWave> wavesTwo = new HashSet<CrystalWave>();
        HashSet<CrystalWave> wavesThree = new HashSet<CrystalWave>();

        CrystalWave currentCW = new CrystalWave(1, _crystal);

        float CrystalSpacing = LevelManager.instance.m_CrystalSpaceBetween;
        float crystalHeight = currentCW.crystal.transform.position.y;

        Vector2[] surroundOffsets = new Vector2[4]
        {
            new(CrystalSpacing, CrystalSpacing),
            new(-CrystalSpacing, CrystalSpacing),
            new(CrystalSpacing, -CrystalSpacing),
            new(-CrystalSpacing, -CrystalSpacing)
        };

        Ray blueRay = new Ray();
        RaycastHit blueHit = new RaycastHit();
        foreach (var pos in surroundOffsets)
        {
            Vector2 crystalPos = new Vector2(currentCW.crystal.transform.position.x,
                currentCW.crystal.transform.position.z);
            Vector2 currentPosition = pos + crystalPos;

            blueRay.origin = new Vector3(currentPosition.x, crystalHeight + 2.0f, currentPosition.y);
            blueRay.direction = Vector3.down;
            if (Physics.Raycast(blueRay, out blueHit, Mathf.Infinity))
            {
                if (blueHit.collider.gameObject.layer == 6)
                {
                    GameObject currentCrystal = blueHit.collider.gameObject;
                    currentCW.next.Add(currentCrystal);
                    wavesTwo.Add(new CrystalWave(currentCW.wave + 1, currentCrystal));
                }
            }
        }

        wavesOne.Add(currentCW);


        foreach (var crystal in wavesTwo)
        {
            if (crystal.wave == 2)
            {
                foreach (var pos in surroundOffsets)
                {
                    Vector2 crystalPos = new Vector2(crystal.crystal.transform.position.x,
                        crystal.crystal.transform.position.z);
                    Vector2 currentPosition = pos + crystalPos;

                    blueRay.origin = new Vector3(currentPosition.x, crystalHeight + 2.0f, currentPosition.y);
                    blueRay.direction = Vector3.down;
                    if (Physics.Raycast(blueRay, out blueHit, Mathf.Infinity))
                    {
                        if (blueHit.collider.gameObject.layer == 6)
                        {
                            GameObject currentCrystal = blueHit.collider.gameObject;
                            crystal.next.Add(currentCrystal);
                            wavesThree.Add(new CrystalWave(crystal.wave + 1, currentCrystal));
                        }
                    }
                }
            }
        }

        wavesAll.Add(wavesOne);
        wavesAll.Add(wavesTwo);
        // wavesAll.Add(wavesThree);

        foreach (var wave in wavesAll)
        {
            foreach (var crystal in wave)
            {
                GameObject crystalObj = crystal.crystal;
                foreach (var destination in crystal.next)
                {
                    GameObject lightning = Instantiate(m_Lightning);
                    Transform lightningLeft = lightning.transform.GetChild(0);
                    Transform lightningRight = lightning.transform.GetChild(1);
                    lightningLeft.position = crystalObj.transform.position;
                    lightningRight.position = destination.transform.position;
                    lightningLeft.LookAt(lightningRight.position);

                    HS_FrontAttack lightningScript = lightningLeft.gameObject.GetComponent<HS_FrontAttack>();
                    lightningScript.playMeshEffect = true;

                    GameObject blueBall = Instantiate(m_BlueBall);
                    BlueBallBehaviour ballScript = blueBall.GetComponent<BlueBallBehaviour>();
                    ballScript.SetInitialPos(crystalObj.transform.position);
                    ballScript.SetTarget(destination);
                    ballScript.SetTimer(crystal.wave - 1);
                }
            }
        }

        _crystal.GetComponent<CrystalEvents>().GetMined();
    }

    private void SpellTimers()
    {
        if (m_AimingYellow)
        {
            LayerMask crystalsLayer = 1 << 6;
            m_MouseRay = m_MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(m_MouseRay, out m_TargetHit, Mathf.Infinity, crystalsLayer))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    m_AimingYellow = false;
                    YellowSpell(m_TargetHit.collider.gameObject);
                    m_TargetCrystal = null;
                    LevelManager.instance.ActiveAction("Yellow", false);
                }
            }

            if (Input.GetMouseButton(1))
            {
                ChangeSun("Blue");
                m_AimingYellow = false;
                LevelManager.instance.ActiveAction("Yellow", false);
            }
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
                ChangeSun("Blue");
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

    private void TeleportNextAnim(Biome _biome)
    {
        m_TargetCrystal = null;
        m_StoppingDistance = 0;
        m_Destination = _biome.EndRoad;
        SetState(new PlayerMoving(this));
    }

    private void TeleportLastAnim(Biome _biome)
    {
        m_TargetCrystal = null;
        m_StoppingDistance = 0;
        m_Destination = _biome.StartRoad;
        SetState(new PlayerMoving(this));
    }

    private void TeleportNext(Biome _biome)
    {
        transform.position = _biome.StartRoad;
        transform.eulerAngles = _biome.entranceRotation;
        m_Destination = _biome.entrancePosition;
        SetState(new PlayerMoving(this));
    }

    private void TeleportLast(Biome _biome)
    {
        transform.position = _biome.EndRoad;
        transform.eulerAngles = _biome.exitRotation;
        m_Destination = _biome.exitPosition;
        SetState(new PlayerMoving(this));
    }

    private void ChangeSun(string _color)
    {
        if (m_CurrentSun != "") m_Suns[m_CurrentSun].SetActive(false);
        m_Suns[_color].SetActive(true);
        m_CurrentSun = _color;
    }

    public void Footstep()
    {
        if (LevelManager.instance.currentWorld == "Ice")
        {
            AudioManager.instance.PlaySnowStep(transform.position);
        }
    }
}