using UnityEngine;
using System.Collections;

// This script is placed on the trigger object (the rope)

public class Pulley : MonoBehaviour
{
	public GameObject PulleyTargetObj;	// this is the targetted object of the pulley
	
	Vector3 m_vStartPos;
	public Vector3 EndPos;
	public Vector3 Velocity;	// this is the velocity for when this rope is grabbed
	
	bool m_bActivated;
	bool m_bMoveForward;	// forward == Velocity's direction (e.g. a chain rope being grabbed)

	void Start ()
	{
		m_bActivated = false;
		m_vStartPos = transform.position;
		EndPos = m_vStartPos + EndPos;
	}

	void Update ()
	{
		if (m_bActivated)
		{
			if (m_bMoveForward)
			{
				transform.position += Velocity * Time.deltaTime;
				
				if ( (transform.position - EndPos).magnitude < 1.0f)
				{
					m_bActivated = false;
				}
			}
			else
			{
				transform.position -= Velocity * Time.deltaTime;
								
				if ( (transform.position - m_vStartPos).magnitude < 1.0f)
				{
					m_bActivated = false;
				}
			}			
		}
	}
	
	// will be activated from the rope script
	public void SetActivate(bool _hasLetGo) 
	{		
		m_bActivated = true;
		m_bMoveForward = !_hasLetGo;
		
		PulleyTargetObj.GetComponent<PulleyTarget>().SetActivate(_hasLetGo);
	}	
	
	public bool Activated
	{
		get { return m_bActivated; }
	}
}

