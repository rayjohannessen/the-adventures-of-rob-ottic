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
		
	// these positions represent a percentage of the screen width (values from 0 -> 1)
	public Vector2 LeftArrowPos;
	public Vector2 RightArrowPos;
	public Vector2 UpArrowPos;
	public Vector2 DownArrowPos;
	public Vector2 ABtnPosWithArrows;
	public Vector2 ABtnPosWithoutArrows;
	public Vector2 BBtnPosWithArrows;
	public Vector2 BBtnPosWithoutArrows;
	
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
		if (Game.Instance.CurrLevel.PauseMenuActive)
			return;
		
		// turn off any previously ended flags
		m_nReleasedFlags = 0;
		m_bButtonTouched = false;
		int i = 0;
		for (; i < NUM_BTNS; ++i)
			m_arrPrevTouches[i] = false;
		
        // buttons cannot be pressed when either the weight or wedge is being moved
        bool bTouchInput =  TI.CurrTwoTouchOffset != 0.0f || 
							m_TI.SingleTouchMoveAmt.x != 0.0f || 
							m_TI.SingleTouchMoveAmt.y != 0.0f ||
							GameObject.Find("Weight").GetComponent<Weight>().MoveWithTouch || 
							GameObject.Find("Wedge").GetComponent<Wedge>().MoveWithTouch;

		// go through all the buttons
		// if any touches hit a button on began, stationary, or moved, color it & set its flag
		// otherwise, if the previous touch did not hit the current button, the current button gets reset
		i = 0;
		foreach (GUITexture btn in m_arrBtns)
		{
			foreach (Touch touch in Input.touches)
			{
				Vector3 pos = touch.position;
				if (!bTouchInput && btn.HitTest(pos))
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
		Debug.Log("SW:" + Screen.width.ToString());
		Debug.Log("SH:" + Screen.height.ToString());
		
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
			
			float btnWidth = m_arrBtns[BTN_A].pixelInset.width;
			float btnHeight = m_arrBtns[BTN_A].pixelInset.height;
			float arrowWidth = m_arrBtns[BTN_LEFT].pixelInset.width;
			float arrowHeight = m_arrBtns[BTN_LEFT].pixelInset.height;
			
			// set the positions
			transform.Find("LeftArrow").guiTexture.pixelInset = new Rect(Screen.width * LeftArrowPos.x, Screen.height * LeftArrowPos.y, arrowWidth, arrowHeight);
			transform.Find("RightArrow").guiTexture.pixelInset = new Rect(Screen.width * RightArrowPos.x, Screen.height * RightArrowPos.y, arrowWidth, arrowHeight);
			transform.Find("UpArrow").guiTexture.pixelInset = new Rect(Screen.width * UpArrowPos.x, Screen.height * UpArrowPos.y, arrowWidth, arrowHeight);
			transform.Find("DownArrow").guiTexture.pixelInset = new Rect(Screen.width * DownArrowPos.x, Screen.height * DownArrowPos.y, arrowWidth, arrowHeight);
			transform.Find("A_Btn").guiTexture.pixelInset = new Rect(Screen.width * ABtnPosWithArrows.x, Screen.height * ABtnPosWithArrows.y, btnWidth, btnHeight);
			transform.Find("B_Btn").guiTexture.pixelInset = new Rect(Screen.width * BBtnPosWithArrows.x, Screen.height * BBtnPosWithArrows.y, btnWidth, btnHeight);
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
			
			m_arrBtns[BTN_A]	= transform.Find("A_Btn").guiTexture;
			m_arrBtns[BTN_B]	= transform.Find("B_Btn").guiTexture;
			
			float btnWidth = m_arrBtns[BTN_A].pixelInset.width;
			float btnHeight = m_arrBtns[BTN_A].pixelInset.height;
			
			transform.Find("A_Btn").guiTexture.pixelInset = new Rect(Screen.width * ABtnPosWithoutArrows.x, Screen.height * ABtnPosWithoutArrows.y, btnWidth, btnHeight);
			transform.Find("B_Btn").guiTexture.pixelInset = new Rect(Screen.width * BBtnPosWithoutArrows.x, Screen.height * BBtnPosWithoutArrows.y, btnWidth, btnHeight);
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

public class MobileInput : MonoBehaviour
{	
	void Start ()
	{
        DestroyImmediate(transform.Find("LeftArrow").gameObject);
        DestroyImmediate(transform.Find("RightArrow").gameObject);
        DestroyImmediate(transform.Find("UpArrow").gameObject);
        DestroyImmediate(transform.Find("DownArrow").gameObject);
        DestroyImmediate(transform.Find("A_Btn").gameObject);
        DestroyImmediate(transform.Find("B_Btn").gameObject);
	}
}

#endif

