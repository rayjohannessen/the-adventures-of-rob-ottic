#if UNITY_IPHONE
using UnityEngine;
using System.Collections;

public class AccelerometerInput : MonoBehaviour
{	
	int m_nNumAccelEvents;
	
	float m_fXmovement;
	public float XMoveMinThreshold = 0.1f;
	public float XMoveMaxThreshold = 0.9f;
	
	// Use this for initialization
	void Start ()
	{
		m_nNumAccelEvents = 0;
		m_fXmovement = 0.0f;
	}
	
	// TODO:: we may not use accelerometer all the time, may want to put
	// left/right movement in if check for OPT_USE_BUTTONS

	// Update is called once per frame
	void Update ()
	{
		m_fXmovement = 0.0f;
		m_nNumAccelEvents = Input.accelerationEventCount;
		
		if (m_nNumAccelEvents > 0)
		{
			// positive y is movement to the left
			m_fXmovement = -Input.acceleration.y;
			
//			Debug.Log("Num Events:" + m_nNumAccelEvents.ToString());
//			Debug.Log("Accel:" + Input.acceleration.ToString());			
//			
//			AccelerationEvent[] evts = Input.accelerationEvents;
//			int i = 0;
//			foreach (AccelerationEvent evt in evts)
//			{
//				Debug.Log("Accel " + i.ToString() + ":" + evt.acceleration.ToString());
//				Debug.Log("DT:" + evt.deltaTime.ToString());
//				m_fXmovement += evt.acceleration.x;
//				++i;
//			}			
//			Debug.Log("Accumulated Accel:" + m_fXmovement.ToString());
		}
	}
	
	public bool HasValidXMovement()
	{
		float x = Mathf.Abs(m_fXmovement);
		return x > XMoveMinThreshold && x < XMoveMaxThreshold;
	}
	
	public float GetClampedXMovement()
	{
		if (m_fXmovement > XMoveMaxThreshold)
			m_fXmovement = XMoveMaxThreshold;
		else if (m_fXmovement < XMoveMinThreshold)
			m_fXmovement = XMoveMinThreshold;
		return m_fXmovement;
	}
	
	public float XMovement
	{
		get { return m_fXmovement; }
	}
	
}
#endif
