using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    RectTransform m_Instructions;

    [SerializeField]
    RectTransform m_StartMenu;

    [SerializeField]
    RectTransform m_GameOverMenu;

    [SerializeField]
    RectTransform m_ScoreMenu;

    [SerializeField]
    RectTransform m_AISelectMenu;

    [SerializeField]
    TextMeshProUGUI m_ScorePrefab;

    bool m_IsShowingInstructions = false;

    private void Start()
    {
    }

    private void Update()
    {
        if (m_IsShowingInstructions && Input.anyKeyDown)
        {
            HideAll();
            ShowStartMenu();
        }
    }

    public void HideAll()
    {
        m_Instructions.gameObject.SetActive(false);
        m_StartMenu.gameObject.SetActive(false);
        m_GameOverMenu.gameObject.SetActive(false);
        m_AISelectMenu.gameObject.SetActive(false);
        m_ScoreMenu.gameObject.SetActive(false);

        m_IsShowingInstructions = false;
    }

    public void ShowInstructions()
    {
        HideAll();
        m_Instructions.gameObject.SetActive(true);
        m_IsShowingInstructions = true;
    }

    public void ShowStartMenu()
    {
        HideAll();
        m_StartMenu.gameObject.SetActive(true);
    }

    public void ShowGameOverMenu()
    {
        HideAll();
        m_GameOverMenu.gameObject.SetActive(true);
        m_ScoreMenu.gameObject.SetActive(true);
    }

    public void ShowAISelectMenu()
    {
        HideAll();
        m_AISelectMenu.gameObject.SetActive(true);
    }

    public TextMeshProUGUI AddScoreUI()
    {
        return Instantiate(m_ScorePrefab, m_ScoreMenu);
    }
}
