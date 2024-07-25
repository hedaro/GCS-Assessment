using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

[Serializable]
public class PlayerInfo
{
    public PlayerController Player;
    public Transform PlayerStart;
    public float Score;
    public TextMeshProUGUI ScoreUI;
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    MenuManager m_MenuManager;

    [SerializeField]
    CameraEffectsManager m_CameraEffectsManager;

    [SerializeField]
    WindConditionManager m_WindConditionManager;

    [SerializeField]
    List<PlayerInfo> m_Players;

    [SerializeField]
    AIController m_AIPlayer;

    [SerializeField]
    bool m_TestMode;

    [SerializeField]
    bool m_TestAI;

    [SerializeField]
    EAIBehavior m_TestAIBehavior;

    public CameraEffectsManager CameraFXManager { get { return m_CameraEffectsManager; } }

    public List<PlayerInfo> PlayersInfo { get { return m_Players; } }

    bool m_VsAI = false;

    static public GameManager Instance 
    {
        get { return m_Instance; }
    }

    static GameManager m_Instance;

    public Bounds CameraBounds {  get { return m_CameraBounds; } }

    float m_CameraHeight = 0f;
    float m_ScreenAspect = 0f;
    Bounds m_CameraBounds;

    private void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        m_Instance = this;
    }

    void Start()
    {
        m_CameraHeight = Camera.main.orthographicSize * 2;
        m_ScreenAspect = (float)Screen.width / Screen.height;
        m_CameraBounds = new Bounds(Camera.main.transform.position, new Vector3(m_CameraHeight * m_ScreenAspect, m_CameraHeight, 0));

        Time.timeScale = 0f;

        m_MenuManager.gameObject.SetActive(true);
        m_MenuManager.HideAll();
        m_MenuManager.ShowInstructions();

        int id = 0;
        foreach (var p in m_Players)
        {
            p.ScoreUI = m_MenuManager.AddScoreUI();
            p.Player.SetPlayerID(id++);
        }

        if (m_TestMode)
        {
            if (m_TestAI)
            {
                PlayVsAI(m_TestAIBehavior);
            }
            else
            {
                PlayVsPlayer();
            }
        }
    }

    public PlayerController GetPlayer(int id)
    {
        if (id >= m_Players.Count)
        {
            return null;
        }

        return m_Players[id].Player;
    }

    public PlayerController GetOtherPlayer(int id)
    {
        PlayerInfo Info = m_Players.Find(p => p.Player.PlayerID != id);
        if (Info != null)
        {
            return Info.Player;
        }

        return null;
    }

    public void AddScore(PlayerController player)
    {
        PlayerInfo playerInfo = m_Players.Find(p => p.Player == player);
        playerInfo.Score++;

        playerInfo.ScoreUI.text = playerInfo.Score.ToString();
    }

    public void PlayVsPlayer()
    {
        m_AIPlayer.enabled = false;
        for (int p = 0; p < m_Players.Count; ++p)
        {
            m_Players[p].Score = 0;
            m_Players[p].ScoreUI.text = m_Players[p].Score.ToString();
            if (p > 0)
            {
                m_Players[p].Player.InputManager.enabled = true;
            }
        }

        m_VsAI = false;
        StartGame();
    }

    public void PlayBalancedAI()
    {
        PlayVsAI(EAIBehavior.BALANCED);
    }

    public void PlayAggressiveAI()
    {
        PlayVsAI(EAIBehavior.AGGRESSIVE);
    }

    public void PlayDefensiveAI()
    {
        PlayVsAI(EAIBehavior.DEFENSIVE);
    }

    public void PlayVsAI(EAIBehavior Behavior)
    {
        for (int p = 0; p < m_Players.Count; ++p)
        {
            m_Players[p].Score = 0;
            m_Players[p].ScoreUI.text = m_Players[p].Score.ToString();
            if (p > 0)
            {
                m_Players[p].Player.InputManager.enabled = false;
            }
        }

        m_AIPlayer.enabled = true;
        m_AIPlayer.SetBehavior(Behavior);

        m_VsAI = true;
        StartGame();
    }

    public void StartGame()
    {
        foreach (var p in m_Players)
        {
            p.Player.InitializePlayer();
            p.Player.transform.position = p.PlayerStart.position;
            p.Player.transform.rotation = p.PlayerStart.rotation;
        }

        if (m_VsAI)
        {
            m_AIPlayer.InitializeAI();
        }

        Time.timeScale = 1f;
        m_MenuManager.HideAll();

        m_WindConditionManager.StartWind();
    }

    public void GameOver()
    {
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(2f);

        foreach (var p in m_Players)
        {
            p.Player.DeactivatePlayer();
        }

        m_WindConditionManager.StopWind();
        Time.timeScale = 0f;
        m_MenuManager.ShowGameOverMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
