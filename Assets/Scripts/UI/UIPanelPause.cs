using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelPause : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnClose, btnRestart;

    private UIMainManager m_mngr;
    public GameManager m_gameManager;
    private void Awake()
    {
        btnClose.onClick.AddListener(OnClickClose);
        btnRestart.onClick.AddListener(OnClickRestart);
    }

    private void OnDestroy()
    {
        if (btnClose) btnClose.onClick.RemoveAllListeners();
        if (btnRestart) btnClose.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }

    private void OnClickClose()
    {
        m_mngr.ShowGameMenu();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    public void OnClickRestart()
    {
        m_gameManager.Reset();
        m_gameManager.ClearLevel();
        if (m_gameManager.levelMode == GameManager.eLevelMode.TIMER) m_mngr.LoadLevelTimer();
        else m_mngr.LoadLevelMoves();
    }
}
