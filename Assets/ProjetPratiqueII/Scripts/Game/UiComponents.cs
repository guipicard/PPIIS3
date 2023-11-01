using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiComponents : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_BlueCrystalsText;
    [SerializeField] private TextMeshProUGUI m_GreenCrystalsText;
    [SerializeField] private TextMeshProUGUI m_RedCrystalsText;
    [SerializeField] private TextMeshProUGUI m_YellowCrystalsText;

    [SerializeField] private TextMeshProUGUI m_BlueCooldownText;
    [SerializeField] private TextMeshProUGUI m_GreenCooldownText;
    [SerializeField] private TextMeshProUGUI m_RedCooldownText;
    [SerializeField] private TextMeshProUGUI m_YellowCooldownText;

    [SerializeField] private TextMeshProUGUI m_BlueInputText;
    [SerializeField] private TextMeshProUGUI m_GreenInputText;
    [SerializeField] private TextMeshProUGUI m_RedInputText;
    [SerializeField] private TextMeshProUGUI m_YellowInputText;
    
    [SerializeField] private float m_YellowSpellTimer;
    [SerializeField] private float m_GreenSpellTimer;
    [SerializeField] private float m_RedSpellTimer;

    private float m_YellowSpellElapsed;
    private float m_GreenSpellElapsed;
    private float m_RedSpellElapsed;

    [SerializeField] private Image m_BlueCrystalImage;
    [SerializeField] private Image m_GreenCrystalImage;
    [SerializeField] private Image m_RedCrystalImage;
    [SerializeField] private Image m_YellowCrystalImage;

    [SerializeField] private float m_ActiveHeight;

    [SerializeField] private TextMeshProUGUI m_ErrorText;
    private float m_ErrorElapsed;

    [SerializeField] private GameObject m_PauseScreen;
    private bool paused;

    private int BluePrice;
    private int YellowPrice;
    private int GreenPrice;
    private int RedPrice;

    private int m_BlueSpellCost;
    private int m_GreenSpellCost;
    private int m_RedSpellCost;
    private int m_YellowSpellCost;

    private int unlockPrice;

    void Start()
    {
        m_ErrorElapsed = 0.0f;

        unlockPrice = LevelManager.instance.m_UnlockPrice;

        LevelManager.instance.ErrorAction += ShowErrorMessage;
        LevelManager.instance.CollectAction += UpdateUi;
        LevelManager.instance.SpellCastAction += TriggerSpell;
        LevelManager.instance.ActiveAction += ActivateSpell;
        LevelManager.instance.SpellUnlockAction += UnlockSpell;

        ChangeAlpha(m_BlueCrystalImage, 0.0f);
        ChangeAlpha(m_GreenCrystalImage, 0.0f);
        ChangeAlpha(m_YellowCrystalImage, 0.0f);
        ChangeAlpha(m_RedCrystalImage, 0.0f);

        BluePrice = unlockPrice;
        YellowPrice = unlockPrice;
        GreenPrice = unlockPrice;
        RedPrice = unlockPrice;

        m_BlueSpellCost = LevelManager.instance.m_BlueSpellCost;
        m_GreenSpellCost = LevelManager.instance.m_GreenSpellCost;
        m_RedSpellCost = LevelManager.instance.m_RedSpellCost;
        m_YellowSpellCost = LevelManager.instance.m_YellowSpellCost;

        paused = false;
        m_PauseScreen.SetActive(false);
        Time.timeScale = 1;

        m_YellowSpellElapsed = 0.0f;
        m_GreenSpellElapsed = 0.0f;
        m_RedSpellElapsed = -1.0f;

        m_BlueInputText.alpha = 0.0f;
        m_GreenInputText.alpha = 0.0f;
        m_RedInputText.alpha = 0.0f;
        m_YellowInputText.alpha = 0.0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseToggle();
        }

        if (m_YellowSpellElapsed > 0)
        {
            m_YellowSpellElapsed -= Time.deltaTime;
            m_YellowCooldownText.text = $"{Mathf.Ceil(m_YellowSpellElapsed)}";
            ChangeAlpha(m_YellowCrystalImage, 0.2f);
        }
        else if (m_YellowCooldownText.alpha != 0.0f && YellowPrice == m_YellowSpellCost)
        {
            m_YellowCooldownText.alpha = 0.0f;
            UpdateUi(0, "Yellow");
        }

        if (m_GreenSpellElapsed > 0)
        {
            m_GreenSpellElapsed -= Time.deltaTime;
            m_GreenCooldownText.text = $"{Mathf.Ceil(m_GreenSpellElapsed)}";
            ChangeAlpha(m_GreenCrystalImage, 0.2f);
        }
        else if (m_GreenCooldownText.alpha != 0.0f && GreenPrice == m_GreenSpellCost)
        {
            m_GreenCooldownText.alpha = 0.0f;
            UpdateUi(0, "Green");
        }

        if (m_RedSpellElapsed > 0)
        {
            m_RedSpellElapsed -= Time.deltaTime;
            m_RedCooldownText.text = $"{Mathf.Ceil(m_RedSpellElapsed)}";
            ChangeAlpha(m_RedCrystalImage, 0.2f);
        }
        else if (m_RedCooldownText.alpha != 0.0f && RedPrice == m_RedSpellCost)
        {
            m_RedCooldownText.alpha = 0.0f;
            UpdateUi(0, "Red");
        }

        if (m_ErrorText.alpha > 0.0f)
        {
            m_ErrorText.alpha = Mathf.Lerp(3.0f, 0.0f, m_ErrorElapsed / 3.0f);
            m_ErrorElapsed += Time.deltaTime;
        }
    }

    private void UpdateUi(int _cost, string _color)
    {
        int crystalAmount = LevelManager.instance.GetCollected(_color);
        switch (_color)
        {
            case "Blue":
                m_BlueCrystalsText.text = crystalAmount.ToString();
                if (!LevelManager.instance.GetSpellUnlocked("Blue")) break;
                if (crystalAmount >= m_BlueSpellCost)
                {
                    ChangeAlpha(m_BlueCrystalImage, 1.0f);
                    m_BlueInputText.alpha = 1.0f;
                    LevelManager.instance.SetSpellAvailable("Blue", true);
                }
                else
                {
                    ChangeAlpha(m_BlueCrystalImage, 0.2f);
                    LevelManager.instance.SetSpellAvailable("Blue", false);
                }

                break;
            case "Yellow":
                m_YellowCrystalsText.text = crystalAmount.ToString();
                if (!LevelManager.instance.GetSpellUnlocked("Yellow")) break;
                if (crystalAmount >= YellowPrice)
                {
                    ChangeAlpha(m_YellowCrystalImage, 1.0f);
                    m_YellowInputText.alpha = 1.0f;
                    LevelManager.instance.SetSpellAvailable("Yellow", true);
                }
                else
                {
                    ChangeAlpha(m_YellowCrystalImage, 0.2f);
                    LevelManager.instance.SetSpellAvailable("Yellow", false);
                }

                break;
            case "Green":
                m_GreenCrystalsText.text = crystalAmount.ToString();
                if (!LevelManager.instance.GetSpellUnlocked("Green")) break;
                if (crystalAmount >= GreenPrice)
                {
                    ChangeAlpha(m_GreenCrystalImage, 1.0f);
                    m_GreenInputText.alpha = 1.0f;
                    LevelManager.instance.SetSpellAvailable("Green", true);
                }
                else
                {
                    ChangeAlpha(m_GreenCrystalImage, 0.2f);
                    LevelManager.instance.SetSpellAvailable("Green", false);
                }

                break;
            case "Red":
                m_RedCrystalsText.text = crystalAmount.ToString();
                if (!LevelManager.instance.GetSpellUnlocked("Red")) break;
                if (crystalAmount >= RedPrice)
                {
                    ChangeAlpha(m_RedCrystalImage, 1.0f);
                    m_RedInputText.alpha = 1.0f;
                    LevelManager.instance.SetSpellAvailable("Red", true);
                }
                else
                {
                    ChangeAlpha(m_RedCrystalImage, 0.2f);
                    LevelManager.instance.SetSpellAvailable("Red", false);
                }

                break;
        }
    }

    private void ChangeAlpha(Image _img, float _alpha)
    {
        Color currentColor = _img.color;
        currentColor.a = _alpha;
        _img.color = currentColor;
    }

    private void TriggerSpell(string _color)
    {
        switch (_color)
        {
            case "Blue":
                break;
            case "Green":
                m_GreenCooldownText.alpha = 1.0f;
                m_GreenSpellElapsed = m_GreenSpellTimer;
                break;
            case "Yellow":
                m_YellowCooldownText.alpha = 1.0f;
                m_YellowSpellElapsed = m_YellowSpellTimer;
                break;
            case "Red":
                m_RedCooldownText.alpha = 1.0f;
                m_RedSpellElapsed = m_RedSpellTimer;
                break;
        }
    }

    private void ActivateSpell(string _color, bool _state)
    {
        Image img = m_BlueCrystalImage;
        switch (_color)
        {
            case "Blue":
                img = m_BlueCrystalImage;
                break;
            case "Green":
                img = m_GreenCrystalImage;
                break;
            case "Yellow":
                img = m_YellowCrystalImage;
                break;
            case "Red":
                img = m_RedCrystalImage;
                break;
        }

        Vector2 m_rect = img.rectTransform.anchoredPosition;
        m_rect.y += _state ? m_ActiveHeight : -m_ActiveHeight;
        img.rectTransform.anchoredPosition = m_rect;
    }

    private void UnlockSpell(string _color)
    {
        Image img = m_BlueCrystalImage;
        switch (_color)
        {
            case "Blue":
                img = m_BlueCrystalImage;
                break;
            case "Green":
                GreenPrice = m_GreenSpellCost;
                break;
            case "Yellow":
                YellowPrice = m_YellowSpellCost;
                break;
            case "Red":
                RedPrice = m_RedSpellCost;
                break;
        }
    }

    public void PauseToggle()
    {
        paused = !paused;
        m_PauseScreen.SetActive(paused);
        Time.timeScale = paused ? 0 : 1;
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitToDesktop()
    {
        Application.Quit();
    }

    private void ShowErrorMessage(string _message)
    {
        m_ErrorText.text = _message;
        m_ErrorText.alpha = 1.0f;
        m_ErrorElapsed = 0.0f;
    }
}