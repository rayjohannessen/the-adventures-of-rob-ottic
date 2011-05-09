#if UNITY_IPHONE
using UnityEngine;
using System.Collections;

// this class itself handles the controller input and
// also contains the class that handles the rest of the touch input

public class MobileInput : MonoBehaviour
{		
    bool    m_bButtonTouched;
	bool[]  m_arrPrevTouches;

	public const int BTN_A	= 0;
	public const int BTN_B	= 1;
	public const int BTN_LEFT = 2;
	public const int BTN_RIGHT= 3;
	public const int BTN_UP 	= 4;
	public const int BTN_DOWN	= 5;
	
	public int NUM_BTNS  = 6;
	
	
	int m_nDownFlags;
	int m_nEnterFlags;
	int m_nReleasedFlags;
		
	GUITexture[] m_arrBtns;
	Color m_clrArrowOrig;
	public Color m_clrHitClr;
	
    TouchInput m_TI;
	
	void Start ()
	{		
		SetUsedButtons();
		
		m_clrArrowOrig = m_arrBtns[0].color;
		m_nDownFlags = m_nEnterFlags = m_nReleasedFlags = 0;

        m_TI = GameObject.Find("MobileInputControls").GetComponent<TouchInput>();
	}

	
	void Update ()
	{			
		// turn off any previously ended flags
		m_nReleasedFlags = 0;
		m_bButtonTouched = false;
		int i = 0;
		for (; i < NUM_BTNS; ++i)
			m_arrPrevTouches[i] = false;
		
		// go through all the buttons
		// if any touches hit a button on began, stationary, or moved, color it & set its flag
		// otherwise, if the previous touch did not hit the current button, the current button gets reset
		i = 0;
		foreach (GUITexture btn in m_arrBtns)
		{
			foreach (Touch touch in Input.touches)
			{
				Vector3 pos = touch.position;
				if (btn.HitTest(pos))
				{
					m_arrPrevTouches[i] = m_bButtonTouched = true;
					btn.color = m_clrHitClr;
					
					if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
					{	
						//Debug.Log("Touch Down");
						
						Utilities.Instance.BitOn(ref m_nDownFlags, i);
						Utilities.Instance.BitOff(ref m_nReleasedFlags, i);
						Utilities.Instance.BitOff(ref m_nEnterFlags, i);
					}
					else if (touch.phase == TouchPhase.Began)
					{
						//Debug.Log("Touch Began");
						
						Utilities.Instance.BitOn(ref m_nEnterFlags, i);
					}
					else if (touch.phase == TouchPhase.Ended)
					{
						//Debug.Log("Touch Ended");
						
						btn.color = m_clrArrowOrig;
						Utilities.Instance.BitOn(ref m_nReleasedFlags, i);
						Utilities.Instance.BitOff(ref m_nDownFlags, i);
						Utilities.Instance.BitOff(ref m_nEnterFlags, i);
					}
				}
				else if (!m_arrPrevTouches[i])
				{
					btn.color = m_clrArrowOrig;
					Utilities.Instance.BitOff(ref m_nDownFlags, i);
					Utilities.Instance.BitOff(ref m_nReleasedFlags, i);
					Utilities.Instance.BitOff(ref m_nEnterFlags, i);
				}
			}			
			++i;
		}
	}
	
	public void SetUsedButtons()
	{
		// use all buttons
		if (Game.Instance.Options.IsOptionActive(Options.eOptions.OPT_USE_ARROWS))
		{
			Debug.Log("Using arrow buttons");
			
			NUM_BTNS = 6;
			
			m_arrBtns = new GUITexture[NUM_BTNS];
			m_arrPrevTouches = new bool[NUM_BTNS];
			
			m_arrBtns[BTN_LEFT] = transform.Find("LeftArrow").guiTexture;
			m_arrBtns[BTN_RIGHT]= transform.Find("RightArrow").guiTexture;
			m_arrBtns[BTN_UP] 	= transform.Find("UpArrow").guiTexture;
			m_arrBtns[BTN_DOWN] = transform.Find("DownArrow").guiTexture;			
			m_arrBtns[BTN_A]	= transform.Find("A_Btn").guiTexture;
			m_arrBtns[BTN_B]	= transform.Find("B_Btn").guiTexture;
		}
		// just use A & B buttons
		else
		{	
			Debug.Log("Using only A & B buttons");
			
			// remove the arrows, since they're not going to be used
			Destroy(transform.Find("LeftArrow").guiTexture);
			Destroy(transform.Find("RightArrow").guiTexture);
			Destroy(transform.Find("UpArrow").guiTexture);
			Destroy(transform.Find("DownArrow").guiTexture);		
			
			NUM_BTNS = 2;
			
			m_arrBtns = new GUITexture[NUM_BTNS];
			m_arrPrevTouches = new bool[NUM_BTNS];
			
			m_arrBtns[0]	= transform.Find("A_Btn").guiTexture;
			m_arrBtns[1]	= transform.Find("B_Btn").guiTexture;
		}	
	}
	
	public bool BtnDown(int _btn)
	{
		return Utilities.Instance.BitTest(m_nDownFlags, _btn);	
	}
	public bool BtnPressed(int _btn)
	{
		return Utilities.Instance.BitTest(m_nEnterFlags, _btn);	
	}
	public bool BtnReleased(int _btn)
	{
		return Utilities.Instance.BitTest(m_nReleasedFlags, _btn);	
	}	

	public bool ButtonTouched
	{
		get { return m_bButtonTouched; }
	}
	public TouchInput TI
	{
		get { return m_TI; }
	}
}

#else
using UnityEngine;
using System.Collections;

public class ControllerInput : MonoBehaviour
{	
	void Start ()
	{		
		DestroyImmediate(transform.Find("LeftArrow"));
		DestroyImmediate(transform.Find("RightArrow"));
		DestroyImmediate(transform.Find("UpArrow"));
		DestroyImmediate(transform.Find("DownArrow"));
		DestroyImmediate(transform.Find("A_Btn"));
		DestroyImmediate(transform.Find("B_Btn"));
	}
}

#endif

