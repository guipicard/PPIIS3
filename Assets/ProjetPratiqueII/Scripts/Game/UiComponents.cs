using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiComponents : MonoBehaviour
{
    [SerializeField] private TextMeshPro m_GreenCrystalsText;
    [SerializeField] private TextMeshPro m_RedCrystalsText;
    [SerializeField] private TextMeshPro m_YellowCrystalsText;
    [SerializeField] private TextMeshPro m_BlueCrystalsText;

    [SerializeField] private TextMeshProUGUI m_BlueCooldownText;
    [SerializeField] private TextMeshProUGUI m_YellowCooldownText;
    [SerializeField] private TextMeshProUGUI m_GreenCooldownText;
    [SerializeField] private TextMeshProUGUI m_RedCooldownText;

    [SerializeField] private float m_BlueSpellTimer;
    [SerializeField] private float m_YellowSpellTimer;
    [SerializeField] private float m_GreenSpellTimer;
    [SerializeField] private float m_RedSpellTimer;

    private float m_BlueSpellElapsed;
    private float m_YellowSpellElapsed;
    private float m_GreenSpellElapsed;
    private float m_RedSpellElapsed;

    [SerializeField] private Image m_GreenCrystalImage;
    [SerializeField] private Image m_RedCrystalImage;
    [SerializeField] private Image m_YellowCrystalImage;
    [SerializeField] private Image m_BlueCrystalImage;

    [SerializeField] private TextMeshProUGUI m_ErrorText;
    private float m_ErrorElapsed;

    [SerializeField] private GameObject m_PauseScreen;
    private bool paused;

    private int BluePrice;
    private int YellowPrice;
    private int GreenPrice;
    private int RedPrice;

    private int spellsCost;
    private int unlockPrice;

    void Start()
    {
        m_ErrorElapsed = 0.0f;

        spellsCost = LevelManager.instance.m_SpellsCost;
        unlockPrice = LevelManager.instance.m_UnlockPrice;

        LevelManager.instance.ErrorAction += ShowErrorMessage;
        LevelManager.instance.CollectAction += UpdateUi;
        LevelManager.instance.SpellCastAction += TriggerSpell;

        ChangeAlpha(m_BlueCrystalImage, 0.2f);
        ChangeAlpha(m_GreenCrystalImage, 0.2f);
        ChangeAlpha(m_YellowCrystalImage, 0.2f);
        ChangeAlpha(m_RedCrystalImage, 0.2f);

        BluePrice = unlockPrice;
        YellowPrice = unlockPrice;
        GreenPrice = unlockPrice;
        RedPrice = unlockPrice;

        paused = false;
        m_PauseScreen.SetActive(false);
        Time.timeScale = 1;

        m_BlueSpellElapsed = 0.0f;
        m_YellowSpellElapsed = 0.0f;
        m_GreenSpellElapsed = 0.0f;
        m_RedSpellElapsed = -1.0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseToggle();
        }

        if (m_BlueSpellElapsed > 0)
        {
            m_BlueSpellElapsed -= Time.deltaTime;
            m_BlueCooldownText.text = $"{Mathf.Ceil(m_BlueSpellElapsed)}";
            ChangeAlpha(m_BlueCrystalImage, 0.2f);
        }
        else if (m_BlueCooldownText.alpha != 0.0f && BluePrice == spellsCost)
        {
            m_BlueCooldownText.alpha = 0.0f;
            UpdateUi(0, "Blue");
        }

        if (m_YellowSpellElapsed > 0)
        {
            m_YellowSpellElapsed -= Time.deltaTime;
            m_YellowCooldownText.text = $"{Mathf.Ceil(m_YellowSpellElapsed)}";
            ChangeAlpha(m_YellowCrystalImage, 0.2f);
        }
        else if (m_YellowCooldownText.alpha != 0.0f && YellowPrice == spellsCost)
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
        else if (m_GreenCooldownText.alpha != 0.0f && GreenPrice == spellsCost)
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
        else if (m_RedCooldownText.alpha != 0.0f && RedPrice == spellsCost)
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
                if (crystalAmount >= BluePrice)
                {
                    ChangeAlpha(m_BlueCrystalImage, 1.0f);
                    if (BluePrice == unlockPrice) BluePrice = spellsCost;
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
                if (crystalAmount >= YellowPrice)
                {
                    ChangeAlpha(m_YellowCrystalImage, 1.0f);
                    if (YellowPrice == unlockPrice) YellowPrice = spellsCost;
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
                if (crystalAmount >= GreenPrice)
                {
                    ChangeAlpha(m_GreenCrystalImage, 1.0f);
                    if (GreenPrice == unlockPrice) GreenPrice = spellsCost;
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
                if (crystalAmount >= RedPrice)
                {
                    ChangeAlpha(m_RedCrystalImage, 1.0f);
                     if (RedPrice == unlockPrice) RedPrice = spellsCost;
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
                m_BlueCooldownText.alpha = 1.0f;
                m_BlueSpellElapsed = m_BlueSpellTimer;
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