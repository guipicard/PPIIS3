using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] [Range(5.0f, 30.0f)] public float m_CrystalSpawnTimer;
    public float m_CrystalSpaceBetween;
    public int m_AiByCrystals;

    [Header("Collectables")] public int m_GreenCollected;
    public int m_RedCollected;
    public int m_YellowCollected;
    public int m_BlueCollected;

    [Header("Active Crystals")] public int m_GreenCrystals;
    public int m_RedCrystals;
    public int m_YellowCrystals;
    public int m_BlueCrystals;

    public float playerDamage;

    public Action<int, string> CollectAction;
    public Action<string> ErrorAction;
    public Action<float> RedSpellAction;
    public Action<string> SpellCastAction;
    public Action<string> SpellUnlockAction;
    public Action<string, bool> ActiveAction;
    public Action<Biome> NextBiomeAction;
    public Action<Biome> LastBiomeAction;
    public Action<Biome> NextAnimAction;
    public Action<Biome> LastAnimAction;

    private bool m_BlueSpellAvailable;
    private bool m_YellowSpellAvailable;
    private bool m_GreenSpellAvailable;
    private bool m_RedSpellAvailable;

    private bool m_BlueSpellUnlocked;
    private bool m_YellowSpellUnlocked;
    private bool m_GreenSpellUnlocked;
    private bool m_RedSpellUnlocked;

    private ObjPool m_Pools;

    public int m_UnlockPrice;

    public bool playerGodmode;

    public List<List<GameObject>> SpawnPoints;

    private static LevelManager levelManager;

    public string currentWorld = "";
    public List<Biome> Worlds;
    public Dictionary<string, GameObject> WorldObjects;
    public List<string> WorldObjectNames;
    [SerializeField] private List<GameObject> Blockades;

    public bool inSequence;
    public bool takeInput;

    public int m_BlueSpellCost;
    public int m_GreenSpellCost;
    public int m_RedSpellCost;
    public int m_YellowSpellCost;

    private int m_Steps;

    private Camera m_MainCamera;

    public static LevelManager instance
    {
        get
        {
            if (!levelManager)
            {
                levelManager = FindObjectOfType(typeof(LevelManager)) as LevelManager;

                if (!levelManager)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
                else
                {
                    DontDestroyOnLoad(levelManager);
                }
            }

            return levelManager;
        }
    }

    private void Awake()
    {
    }

    void Start()
    {
    }

    public void LoadLevel()
    {
        m_Steps = 0;
        m_MainCamera = Camera.main;
        inSequence = false;
        takeInput = true;
        currentWorld = Worlds[0].name;
        m_Pools = GameObject.FindObjectOfType<ObjPool>();
        if (!m_Pools) return;
        playerDamage = 20.0f;
        CollectAction += CollectCrystal;

        m_BlueSpellAvailable = false;
        m_YellowSpellAvailable = false;
        m_GreenSpellAvailable = false;
        m_RedSpellAvailable = false;

        m_BlueSpellUnlocked = false;
        m_YellowSpellUnlocked = false;
        m_GreenSpellUnlocked = false;
        m_RedSpellUnlocked = false;

        m_GreenCollected = 0;
        m_RedCollected = 0;
        m_YellowCollected = 0;
        m_BlueCollected = 0;
        playerGodmode = false;
        WorldObjects = new Dictionary<string, GameObject>();
        WorldObjectNames = new List<string>();
        WorldObjectNames.Add("IceEnv");
        WorldObjectNames.Add("EarthEnv");
        WorldObjectNames.Add("LavaEnv");
        WorldObjectNames.Add("DesertEnv");
        for (int i = 0; i < Worlds.Count; i++)
        {
            GameObject obj = GameObject.Find(WorldObjectNames[i]);
            WorldObjects.Add(Worlds[i].name, obj);
            bool active = i == 0;
            obj.SetActive(active);
        }
        // AudioManager.instance.PlayMusic(MusicClip.Ice, 1.0f); 
    }

    public GameObject SpawnObj(string _tag, Vector3 _position, Quaternion _rotation)
    {
        GameObject obj = m_Pools.GetObj(_tag);
        obj.transform.position = _position;
        obj.transform.rotation = _rotation;
        ToggleActive(obj);
        return obj;
    }

    public List<GameObject> GetActiveInScene(string _tag)
    {
        return m_Pools.GetActive(_tag);
    }

    public void CollectCrystal(int _cost, string _color)
    {
        switch (_color)
        {
            case "Green":
                m_GreenCollected += _cost;
                break;
            case "Red":
                m_RedCollected += _cost;
                break;
            case "Yellow":
                m_YellowCollected += _cost;
                break;
            case "Blue":
                m_BlueCollected += _cost;
                break;
        }
    }

    public int GetCollected(string _color)
    {
        switch (_color)
        {
            case "Green":
                return m_GreenCollected;
            case "Red":
                return m_RedCollected;
            case "Yellow":
                return m_YellowCollected;
            case "Blue":
                return m_BlueCollected;
        }

        return 0;
    }

    public void ToggleActive(GameObject _obj)
    {
        _obj.SetActive(true);
        var parent = _obj.transform.parent;
        _obj.transform.parent = parent.parent;
    }

    public void ToggleInactive(GameObject _obj)
    {
        _obj.SetActive(false);
        _obj.transform.parent = _obj.transform.parent.Find("Inactive");
    }

    public int UpdateCrystalNums(string _color)
    {
        switch (_color)
        {
            case "Green_Crystal_Obj":
                m_GreenCrystals = GetActiveInScene(_color).Count;
                return m_GreenCrystals;
            case "Red_Crystal_Obj":
                m_RedCrystals = GetActiveInScene(_color).Count;
                return m_RedCrystals;
            case "Yellow_Crystal_Obj":
                m_YellowCrystals = GetActiveInScene(_color).Count;
                return m_YellowCrystals;
            case "Blue_Crystal_Obj":
                m_BlueCrystals = GetActiveInScene(_color).Count;
                return m_BlueCrystals;
        }

        return 0;
    }

    public void SetPlayerDamage(float _dmg)
    {
        playerDamage = _dmg;
    }

    public bool GetSpellAvailable(string _color)
    {
        switch (_color)
        {
            case "Blue":
                return m_BlueSpellAvailable;
            case "Yellow":
                return m_YellowSpellAvailable;
            case "Green":
                return m_GreenSpellAvailable;
            case "Red":
                return m_RedSpellAvailable;
        }

        return false;
    }

    public void SetSpellAvailable(string _color, bool _state)
    {
        switch (_color)
        {
            case "Blue":
                m_BlueSpellAvailable = _state;
                break;
            case "Yellow":
                m_YellowSpellAvailable = _state;
                break;
            case "Green":
                m_GreenSpellAvailable = _state;
                break;
            case "Red":
                m_RedSpellAvailable = _state;
                break;
        }
    }

    private void SetBiome(string _name)
    {
        currentWorld = _name;
    }

    private int GetBiomeIndex()
    {
        int index = 0;
        for (int i = 0; i < Worlds.Count; i++)
        {
            if (Worlds[i].name == currentWorld)
            {
                index = i;
                break;
            }
        }

        return index;
    }

    public void AnimNextBiome()
    {
        takeInput = false;
        int biomeIndex = GetBiomeIndex();
        NextAnimAction?.Invoke(Worlds[biomeIndex]);
    }

    public void AnimLastBiome()
    {
        takeInput = false;
        int biomeIndex = GetBiomeIndex();
        LastAnimAction?.Invoke(Worlds[biomeIndex]);
    }

    public void GoNextBiome()
    {
        // AudioManager.instance.StopMusic();
        inSequence = true;
        int biomeIndex = GetBiomeIndex();
        string nextBiomeName = Worlds[biomeIndex + 1].name;
        WorldObjects[nextBiomeName].SetActive(true);
        NextBiomeAction?.Invoke(Worlds[biomeIndex + 1]);

        m_MainCamera.transform.position = Worlds[biomeIndex + 1].entrancePosition + new Vector3(0, 2, 0) -
                                          m_MainCamera.GetComponent<CameraFollow>().GetOffset();
        WorldObjects[currentWorld].SetActive(false);
        currentWorld = Worlds[biomeIndex + 1].name;
        // AudioManager.instance.PlayMusic((MusicClip)biomeIndex + 1, 1.0f);
    }

    public void GoLastBiome()
    {
        // AudioManager.instance.StopMusic();
        inSequence = true;
        int biomeIndex = GetBiomeIndex();
        string nextBiomeName = Worlds[biomeIndex - 1].name;
        WorldObjects[nextBiomeName].SetActive(true);
        LastBiomeAction?.Invoke(Worlds[biomeIndex - 1]);

        m_MainCamera.transform.position = Worlds[biomeIndex - 1].exitPosition + new Vector3(0, 2, 0) -
                                          m_MainCamera.GetComponent<CameraFollow>().GetOffset();
        WorldObjects[currentWorld].SetActive(false);
        currentWorld = Worlds[biomeIndex - 1].name;
        // AudioManager.instance.PlayMusic((MusicClip)biomeIndex - 1, 1.0f);
    }

    public void UnlockBiome()
    {
        foreach (var barrage in Blockades)
        {
            if (barrage.activeSelf)
            {
                barrage.SetActive(false);
                break;
            }
        }
    }

    public bool GetSpellUnlocked(string _color)
    {
        switch (_color)
        {
            case "Blue":
                return m_BlueSpellUnlocked;
            case "Yellow":
                return m_YellowSpellUnlocked;
            case "Green":
                return m_GreenSpellUnlocked;
            case "Red":
                return m_RedSpellUnlocked;
        }

        return false;
    }

    public void LevelUp()
    {
        if (m_Steps == 0)
        {
            m_BlueSpellUnlocked = true;
            CollectAction?.Invoke(20, "Blue");
            m_Steps++;
        }
        else if (m_Steps == 1)
        {
            if (m_BlueCollected >= 300)
            {
                CollectAction?.Invoke(-300, "Blue");
                SpellUnlockAction("Green");
                Blockades[0].SetActive(false);
                m_Steps++;
            }
        }
        else if (m_Steps == 2)
        {
            if (m_GreenCollected >= 300)
            {
                m_GreenSpellUnlocked = true;
                CollectAction?.Invoke(-300, "Green");
                SpellUnlockAction("Red");
                Blockades[1].SetActive(false);
                m_Steps++;
            }
        }
        else if (m_Steps == 3)
        {
            if (m_RedCollected >= 300)
            {
                m_RedSpellUnlocked = true;
                CollectAction?.Invoke(-300, "Red");
                SpellUnlockAction("Yellow");
                Blockades[2].SetActive(false);
                m_Steps++;
            }
        }
        else if (m_Steps == 4)
        {
            if (m_YellowCollected >= 300)
            {
                m_YellowSpellUnlocked = true;
                CollectAction?.Invoke(-300, "Yellow");
                Blockades[3].SetActive(false);
                m_Steps++;
            }
        }
    }
}