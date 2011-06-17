using UnityEngine;
using System.Collections;

public class PulleyTarget : MonoBehaviour
{
	Vector3 m_vStartPos;
	public Vector3 EndPos;	
	public Vector3 Velocity;
	
	bool m_bActivated;
	bool m_bMoveForward;	// forward is in the direction of the Velocity
	
	void Start ()
	{
		m_vStartPos = transform.position;
	}

	void Update ()
	{
		if (m_bActivated)
		{
			// move the target object
			if (m_bMoveForward)
			{
				transform.position += Velocity * Time.deltaTime;
				
				if ( (transform.position - EndPos).magnitude < 0.5f)
					m_bActivated = false;
			}
			else
			{
				transform.position -= Velocity * Time.deltaTime;	
				
				if ( (transform.position - m_vStartPos).magnitude < 0.5f)
					m_bActivated = false;
			}			
		}
	}
	
	public void SetActivate(bool _hasLetGo)
	{
		m_bActivated = true;
		m_bMoveForward = !_hasLetGo;
	}
}

