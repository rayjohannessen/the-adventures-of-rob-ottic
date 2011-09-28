using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuManager
{		
	public Dictionary<string, MenuOverlay> Overlays = new Dictionary<string, MenuOverlay>(); 
	
	MenuOverlay m_CurrentMenuOverlay;
	MenuOverlay m_NextMenuOverlay;
	MenuOverlay m_PrevMenuOverlay;
		
	Game m_Game;
	
	bool m_bBegun = false;
	
	public MenuManager()
	{
		if (m_Instance != null)
			return;
		m_Instance = this;
		
		Debug.Log("MenuManager CTOR");
		
		m_Game = Game.Instance;
		
		MenuOverlay[] objs = (MenuOverlay[])GameObject.FindSceneObjectsOfType(typeof(MenuOverlay));
		
		for (int i = 0; i < objs.Length; ++i)
		{
			if(objs[i].name.Contains("Overlay"))
			{
				string name = objs[i].name.Substring(8);
				Overlays.Add(name, objs[i]);
			}
		}	
		
		m_NextMenuOverlay = null;
		m_PrevMenuOverlay = null;
		
		m_CurrentMenuOverlay = Overlays["MainMenu"];
	}
	
	/// <summary>
	/// Must be called AFTER the first menu has been setup
	/// </summary>
	public void Begin()
	{
		if (!m_bBegun)
			m_CurrentMenuOverlay.TransitionIn();	
		
		m_bBegun = true;
	}
	
	public void OnTransitionFinished()
	{
		if (m_NextMenuOverlay != null)
		{
			m_NextMenuOverlay.TransitionIn();
			m_PrevMenuOverlay = m_CurrentMenuOverlay;
			m_CurrentMenuOverlay = m_NextMenuOverlay;
			m_NextMenuOverlay = null;
		}
	}

	public void SetStartTransition(string transitionToMenu)
	{
		_SetStartTrans(Overlays[transitionToMenu]);
	}
	
	// special case button that is common to multiple menus
	public void OnBackClicked()
	{
		if (m_PrevMenuOverlay != null)
		{
			_SetStartTrans(m_PrevMenuOverlay);
		}
		else
		{
			// transition out, but don't transition in anything
			SetStartTransition(null);
		}
	}
	
	/// <summary>
	/// PRIVATE FUNCS
	/// </summary>

	private void _SetStartTrans(MenuOverlay transToMenu)
	{
		m_NextMenuOverlay = transToMenu;
		m_CurrentMenuOverlay.TransitionOut();		
	}
	
	/// <summary>
	/// FIELDS
	/// </summary>
	public MenuOverlay CurrMenu
	{
		get { return m_CurrentMenuOverlay; }
		set { m_CurrentMenuOverlay = value; }
	}
	
	public MenuOverlay NextMenu
	{
		get { return m_NextMenuOverlay; }
		set { m_NextMenuOverlay = value; }
	}
	
	
	/// <summary>
	// SINGLETON-related info
	/// </summary>
	static MenuManager m_Instance = null;
		
	public static MenuManager Instance
	{
		get
		{
			if (m_Instance == null)
				new MenuManager();			
			return m_Instance;
		}
	}
}

