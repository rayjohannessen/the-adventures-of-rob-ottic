#if UNITY_IPHONE
using UnityEngine;
using System.Collections;

public class AccelerometerInput : MonoBehaviour
{	
	public struct AccelInfo
	{
		public bool ValidXMovement;
		public float CurrXMoveAmt;
		public float PrevXMoveAmt;	// this is only set if there was no change in direction
	};
	
	bool m_bChangingDir;
	
	public float XMoveMinThreshold = 0.05f;
	public float XMoveMaxThreshold = 0.9f;
		
	AccelInfo m_sAccelInfo;
	
	// Use this for initialization
	void Start ()
	{
		m_bChangingDir = false;
		m_sAccelInfo.PrevXMoveAmt = 0.001f;
	}
	
	// TODO:: we may not use accelerometer all the time, may want to put
	// left/right movement in if check for OPT_USE_BUTTONS

	// Update is called once per frame
	void Update ()
	{
		m_sAccelInfo.ValidXMovement = false;
		
		if (Game.Instance.LaunchStarted && Input.accelerationEventCount > 0)
		{
			// positive y is movement for the accelerometer means screen movement to the left (when in landscape mode)
			m_sAccelInfo.CurrXMoveAmt = -Input.acceleration.y;
					
			// clamp the x move amount if it's too high:
			if (m_sAccelInfo.CurrXMoveAmt > XMoveMaxThreshold)
				m_sAccelInfo.CurrXMoveAmt = XMoveMaxThreshold;
			else if (m_sAccelInfo.CurrXMoveAmt < -XMoveMaxThreshold)
				m_sAccelInfo.CurrXMoveAmt = -XMoveMinThreshold;
			// if we ever have too small an x movement, we are no longer changing directions
			else if ( Mathf.Abs(m_sAccelInfo.CurrXMoveAmt) < XMoveMinThreshold )
			{
				m_bChangingDir = false;
				m_sAccelInfo.CurrXMoveAmt = 0.0f;
			}
						
			// see if there's a change in direction
			if (m_sAccelInfo.CurrXMoveAmt > 0.0f && m_sAccelInfo.PrevXMoveAmt < 0.0f || m_sAccelInfo.CurrXMoveAmt < 0.0f && m_sAccelInfo.PrevXMoveAmt > 0.0f)
			{
				// need to start smoothly interpolating between previous direction and new
				m_bChangingDir = true;
				//Debug.Log("Switched dir, now moving:" + m_sAccelInfo.CurrXMoveAmt.ToString());
			}
			else
				m_sAccelInfo.PrevXMoveAmt = m_sAccelInfo.CurrXMoveAmt;
							
			// if we still are interpolating
			if ( m_bChangingDir )
			{
				m_sAccelInfo.CurrXMoveAmt = Mathf.Lerp(m_sAccelInfo.PrevXMoveAmt, 0.0f, Time.deltaTime);
				
				//Debug.Log("Curr:" + m_sAccelInfo.CurrXMoveAmt.ToString());
				
				// TODO:: don't need this in final version
				// determine if the character's current x vel is close enough to 0
				if (Mathf.Abs(m_sAccelInfo.CurrXMoveAmt) < XMoveMinThreshold)
				{
					m_bChangingDir = false;
					Debug.Log("vel matches accelerometer");
				}
			}			
		}
	}
	
	public AccelInfo GetAccelInfo()
	{		
		return m_sAccelInfo;
	}	
}
#endif
