#if UNITY_IPHONE

using UnityEngine;
using System.Collections;

public class TouchInput : MonoBehaviour
{
	Vector2 m_vOrigSingleTouchPos;
	Vector2 m_vSingleTouchMoveAmt;

	// the max amount of time that can pass for a second tap to register as a double tap
	public float DoubleTapDelayMax = 0.95f;

	float m_fDoubleTapTimer;
	float m_fOrigTwoTouchDist;
	float m_fCurrTwoTouchOffset;

	int m_nNumTaps;

	void Start ()
	{
		m_fDoubleTapTimer = m_fOrigTwoTouchDist = m_fCurrTwoTouchOffset = 0.0f;
		m_nNumTaps = 0;
	}

	void Update ()
	{
		if (m_nNumTaps > 0)
			m_fDoubleTapTimer += Time.deltaTime;
		
		if (Game.Instance.CurrLevel.PauseMenuActive)
			return;
		
		// check for other touch input besides buttons
		if (!Game.Instance.MI.ButtonTouched) {
			if (Input.touchCount == 1) {
				// if weight or wedge is being moved, don't do anything here
				if (GameObject.Find ("Weight").GetComponent<Weight> ().MoveWithTouch || GameObject.Find ("Wedge").GetComponent<Wedge> ().MoveWithTouch) {
					return;
				}
				m_fCurrTwoTouchOffset = m_fOrigTwoTouchDist = 0.0f;
				Touch touch = Input.GetTouch (0);
				
				if (touch.phase == TouchPhase.Began && m_fDoubleTapTimer < DoubleTapDelayMax) {
					++m_nNumTaps;
					m_vOrigSingleTouchPos = touch.position;
					
					if (m_nNumTaps == 2)
						Game.Instance.CurrLevel.ActivatePauseMenu ();
				// meant to handle panning of camera
				} else if (touch.phase == TouchPhase.Moved) {
					m_vSingleTouchMoveAmt = touch.position - m_vOrigSingleTouchPos;
					m_vOrigSingleTouchPos = touch.position;
				} else if (touch.phase == TouchPhase.Ended) {
					m_fDoubleTapTimer = 0.0f;
					m_vSingleTouchMoveAmt = Vector2.zero;
				}
			} else if (Input.touchCount == 2) {
				// if weight or wedge is being moved, don't do anything here
				if (GameObject.Find ("Weight").GetComponent<Weight> ().MoveWithTouch || GameObject.Find ("Wedge").GetComponent<Wedge> ().MoveWithTouch) {
					return;
				}
				
				m_nNumTaps = 0;
				
				Touch touch1 = Input.GetTouch (0);
				Touch touch2 = Input.GetTouch (1);
				
				if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
					m_fOrigTwoTouchDist = (touch2.position - touch1.position).magnitude;
				else {
					float dist = (touch2.position - touch1.position).magnitude;
					m_fCurrTwoTouchOffset = dist - m_fOrigTwoTouchDist;
					m_fOrigTwoTouchDist = dist;
				}
				
			} else if (Input.touchCount == 0) {
				m_fCurrTwoTouchOffset = m_fOrigTwoTouchDist = 0.0f;
				m_nNumTaps = 0;
				m_vSingleTouchMoveAmt = Vector2.zero;
			}
		}
	}

	public int NumTaps {
		get { return m_nNumTaps; }
	}
	public float CurrTwoTouchOffset {
		get { return m_fCurrTwoTouchOffset; }
	}
	public Vector2 SingleTouchMoveAmt {
		get { return m_vSingleTouchMoveAmt; }
	}
}
#endif