using UnityEngine;
using System.Collections;

/// <summary>
/// Main game logic class, a singleton
/// Acts as central access point for all classes
/// </summary>
public class Game
{	
    private static Game instance;

    bool m_bPreviewDone = false;
    
    int m_nCurrLevel;
    int m_nCurrWorld;

    int m_nNumLevelsInWorld;
    int m_nNumWorlds;
    //int m_nNumMenus;

    PlayerInfo m_PlayerInfo;
    Level m_CurrLevel;
	
	Options m_Options;
	
#if UNITY_IPHONE
	MobileInput m_MobileInputController;
	AccelerometerInput m_AccelInput;
	
	public AccelerometerInput AccelInput
	{
		get { return m_AccelInput; }
		set { m_AccelInput = value; }
	}
	
	public MobileInput MI
	{
		get { return m_MobileInputController; }
		set { m_MobileInputController = value; }
	}
#endif
	
	public Game()
    {
        Debug.Log("Game instance created (CTOR)");
        if (instance != null)
            return;
        instance = this;

		m_Options = new Options();
        m_nNumLevelsInWorld = 10;
        m_nNumWorlds = 1;
        m_nCurrWorld = 1;
        m_nCurrLevel = Application.loadedLevel;
    }

    public void Update()
    {
    }

    public void LateUpdate()
    {
    }

    public void NextLevel()
    {
        // TODO:: increment g_nCurrLevel/g_nCurrWorld first:
        if (++m_nCurrLevel <= m_nNumLevelsInWorld)
        {
            Debug.Log("Loading next level:" + m_nCurrLevel.ToString());
            Application.LoadLevel(m_nCurrLevel);
        }
        else
        {
            // TODO:: goto next world / end
            OnGotoMainMenu();
        }
    }
    public void RedoCurrLevel()
    {
        Debug.Log("RedoCurrLevel:" + m_nCurrLevel.ToString());
        m_PlayerInfo.RevertToDefaults();
        Application.LoadLevel(GetLevelIndex(m_nCurrLevel, m_nCurrWorld));
    }

    public void StartGame(int _level, int _world)
    {
        Debug.Log("StartGame called, Level:" + _level.ToString());
        m_nCurrLevel = _level;
        m_nCurrWorld = _world;
        Application.LoadLevel(GetLevelIndex(m_nCurrLevel, m_nCurrWorld));
    }

    public void StartPractice()
    {
        Debug.Log("StartPractice called");
        //m_nCurrLevel = _level;
        //m_nCurrWorld = _world;
        Application.LoadLevel("PracticeLevel");
    }

    // called from level's start
    // returns m_PlayerInfo to the calling level
    public PlayerInfo NewLevel(Level lvl)
    {
        CurrLevel = lvl;
        CurrLevel.WeightDropped = CurrLevel.LaunchStarted = PreviewDone = false;

        return m_PlayerInfo;
    }

    public void OnGoalReached()
    {
        if (Application.loadedLevelName == "PracticeLevel")
        {
            m_CurrLevel.ResetLevel(false, false);
        }
        else if (m_nCurrLevel == m_nNumLevelsInWorld && m_nCurrWorld == m_nNumWorlds)
        {
            // TODO:: celebrate this world complete or something, go to next world
            Application.LoadLevel("MainMenu");
        } 
        else
        {
            Application.LoadLevel("VictoryMenu");
        }
    }
    public void OnCharacterDied()
    {
        if (m_PlayerInfo.OnDeath())
        {
            // more lives left, reset level
            m_CurrLevel.ResetLevel(false, false);
        }
        else
        {
            Application.LoadLevel("DefeatMenu");
        }
    }
    public void OnGotoMainMenu()
    {
        if (m_PlayerInfo != null)
            m_PlayerInfo.RevertToDefaults();
        Application.LoadLevel("MainMenu");
    }

    // returns where the player needs to be in relation to the see-saw
    public Vector3 GetPlayerPlacing()
    {
        return CurrLevel.GetPlayerPlacement();
    }

    private int GetLevelIndex(int _lvl, int _world)
    {
        return _lvl + (_world - 1) * m_nNumLevelsInWorld;
    }

    /// <summary>
    /// ACCESSORS/MUTATORS
    /// </summary>
    public Level CurrLevel
    {
        get { return m_CurrLevel; }
        set { m_CurrLevel = value; }
    }
    public int NumLevelsInWorld
    {
        get { return m_nNumLevelsInWorld; }
        set { m_nNumLevelsInWorld = value; }
    }
    public int CurrWorldNum
    {
        get { return m_nCurrWorld; }
        set { m_nCurrWorld = value; }
    }
    public int CurrLevelNum
    {
        get { return m_nCurrLevel; }
        set { m_nCurrLevel = value; }
    }
    public bool WeightDropped
    {
        get { return CurrLevel.WeightDropped; }
        set { CurrLevel.WeightDropped = value; }
    }
    public bool LaunchStarted
    {
        get { return CurrLevel.LaunchStarted; }
        set { CurrLevel.LaunchStarted = value; }
    }
    public bool PreviewDone
    {
        get { return m_bPreviewDone; }
        set { m_bPreviewDone = value; }
    }
    public PlayerInfo PlayerInfo
    {
        get { return m_PlayerInfo; }
        set { m_PlayerInfo = value; }
    }
    public int NumWorlds
    {
        get { return m_nNumWorlds; }
        set { m_nNumWorlds = value; }
    }
	public Options Options
	{
		get { return m_Options; }	
	}

    public static Game Instance
    {
        get 
        {
            if (instance == null)
                new Game();
            return instance; 
        }
    }
}