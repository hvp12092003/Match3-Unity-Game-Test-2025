using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NormalItem;
public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };
    public enum eLevelMode
    {
        TIMER,
        MOVES
    }
    public eLevelMode levelMode;
    public enum eStateGame
    {
        SETUP,
        MAIN_MENU,
        GAME_STARTED,
        PAUSE,
        GAME_OVER,
    }
    private eStateGame m_state;
    public eStateGame State
    {
        get { return m_state; }
        private set
        {
            m_state = value;

            StateChangedAction(m_state);
        }
    }

    public DataItem[] dataItems;

    public float minValueItem;
    /*
        public enum iDItem
        {
            TYPE_ONE, TYPE_TWO, TYPE_THREE, TYPE_FOUR, TYPE_FIVE, TYPE_SIX, TYPE_SEVEN
        }*/
    public void Reset()
    {
        minValueItem = 0;
        dataItems = new DataItem[7];
        for (int i = 0; i < dataItems.Length; i++)
        {
            dataItems[i] = new DataItem();
        }
        dataItems[0].type = eNormalType.TYPE_ONE;
        dataItems[1].type = eNormalType.TYPE_TWO;
        dataItems[2].type = eNormalType.TYPE_THREE;
        dataItems[3].type = eNormalType.TYPE_FOUR;
        dataItems[4].type = eNormalType.TYPE_FIVE;
        dataItems[5].type = eNormalType.TYPE_SIX;
        dataItems[6].type = eNormalType.TYPE_SEVEN;
    }


    public void handleAmountItem(bool isPlus, string objName)
    {
        Debug.Log(objName);
        switch (objName)
        {
            case NewItem.PREFAB_NORMAL_TYPE_ONE:
                if (isPlus) dataItems[0].amount++;
                else dataItems[0].amount--; ;
                break;
            case NewItem.PREFAB_NORMAL_TYPE_TWO:
                if (isPlus) dataItems[1].amount++;
                else dataItems[1].amount--;
                break;
            case NewItem.PREFAB_NORMAL_TYPE_THREE:
                if (isPlus) dataItems[2].amount++;
                else dataItems[2].amount--;
                break;
            case NewItem.PREFAB_NORMAL_TYPE_FOUR:
                if (isPlus) dataItems[3].amount++;
                else dataItems[3].amount--;
                break;
            case NewItem.PREFAB_NORMAL_TYPE_FIVE:
                if (isPlus) dataItems[4].amount++;
                else dataItems[4].amount--;
                break;
            case NewItem.PREFAB_NORMAL_TYPE_SIX:
                if (isPlus) dataItems[5].amount++;
                else dataItems[5].amount--;
                break;
            case NewItem.PREFAB_NORMAL_TYPE_SEVEN:
                if (isPlus) dataItems[6].amount++;
                else dataItems[6].amount--;
                break;
        }
        handleOderItem();

    }
    public void handleOderItem()
    {
        DataItem temp = new DataItem();
        bool done = false;

        while (!done)
        {
            done = true;
            for (int i = 0; i < dataItems.Length - 1; i++)
            {
                Debug.Log(dataItems.Length + "/" + i);
                if (dataItems[i].amount > dataItems[i + 1].amount)
                {
                    temp = dataItems[i];
                    dataItems[i] = dataItems[i + 1];
                    dataItems[i + 1] = temp;
                    done = false;
                }
            }
        }
    }
    private GameSettings m_gameSettings;


    private BoardController m_boardController;

    private UIMainManager m_uiMenu;

    private LevelCondition m_levelCondition;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;

        State = eStateGame.SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiMenu = FindObjectOfType<UIMainManager>();
        m_uiMenu.Setup(this);
    }

    void Start()
    {
        Reset();

        State = eStateGame.MAIN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
        //  if (m_boardController != null) m_boardController.Update();
        if (m_boardController != null) m_boardController.UpdateBoard();
    }


    internal void SetState(eStateGame state)
    {
        State = state;

        if (State == eStateGame.PAUSE)
        {
            DOTween.PauseAll();
        }
        else
        {
            DOTween.PlayAll();
        }
    }

    public void LoadLevel(eLevelMode mode)
    {
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.StartGame(this, m_gameSettings);

        if (mode == eLevelMode.MOVES)
        {
            levelMode = eLevelMode.MOVES;
            if (m_levelCondition == null) m_levelCondition = this.gameObject.AddComponent<LevelMoves>();
            m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), m_boardController);
        }
        else if (mode == eLevelMode.TIMER)
        {
            levelMode = eLevelMode.TIMER;
            if (m_levelCondition == null) m_levelCondition = this.gameObject.AddComponent<LevelTime>();
            m_levelCondition.Setup(m_gameSettings.LevelMoves, m_uiMenu.GetLevelConditionView(), this);
        }

        m_levelCondition.ConditionCompleteEvent += GameOver;

        State = eStateGame.GAME_STARTED;
    }

    public void GameOver()
    {
        StartCoroutine(WaitBoardController());
    }

    internal void ClearLevel()
    {
        if (m_boardController)
        {
            m_boardController.Clear();
            Destroy(m_boardController.gameObject);
            m_boardController = null;
        }
    }

    private IEnumerator WaitBoardController()
    {
        while (m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        State = eStateGame.GAME_OVER;

        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= GameOver;

            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
    }
}
