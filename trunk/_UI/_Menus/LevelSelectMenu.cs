using UnityEngine;
using System.Collections;

public class LevelSelectMenu : MonoBehaviour
{
	MenuOverlay m_Overlay;
	

	void Awake ()
	{
		m_Overlay = MenuManager.Instance.Overlays[name.Substring(8)];
	}

	void Update ()
	{
		
	}
	
	private void SetupButtons()
	{
		
	}
}

