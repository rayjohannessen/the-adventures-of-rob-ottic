using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{
	MenuOverlay m_Overlay;
	
	public Vector2 PlayPos, PracticePos, OptionsPos, QuitPos;
	
	public GameObject BtnPrefab;
	
	void Awake ()
	{		
		Debug.Log("MainMenu::Start()");
		
		m_Overlay = MenuManager.Instance.Overlays[name.Substring(8)];
		
		SetupButtons();
	}

	void Update ()
	{
		
	}
	
	private void SetupButtons()
	{		
		// Play btn
		GameObject btn = (GameObject)Instantiate(BtnPrefab);
		btn.GetComponent<Button>().Init("PLAY", PlayPos, OnPlayBtnHit, null, null);
		m_Overlay.AddBtn(btn);
		
		// Practice btn
		btn = (GameObject)Instantiate(BtnPrefab);
		btn.GetComponent<Button>().Init("PRACTICE", PracticePos, OnPracticeBtnHit, null, null);
		m_Overlay.AddBtn(btn);
		
		// Options btn
		btn = (GameObject)Instantiate(BtnPrefab);
		btn.GetComponent<Button>().Init("OPTIONS", OptionsPos, OnOptionsBtnHit, null, null);
		m_Overlay.AddBtn(btn);
		
		// Quit btn
		btn = (GameObject)Instantiate(BtnPrefab);
		btn.GetComponent<Button>().Init("QUIT", QuitPos, OnQuitBtnHit, null, null);
		m_Overlay.AddBtn(btn);		
	}
	
	
	/// <summary>
	/// Button Delegate funcs
	/// </summary>
	private void OnPlayBtnHit()
	{
		MenuManager.Instance.SetStartTransition("LevelSelectMenu");
	}
	
	private void OnPracticeBtnHit()
	{
		// TODO::animate out??
        Game.Instance.StartPractice();
	}
	
	private void OnOptionsBtnHit()
	{
		MenuManager.Instance.SetStartTransition("OptionsMenu");
	}
	
	private void OnQuitBtnHit()
	{
		Application.Quit();
	}
}

