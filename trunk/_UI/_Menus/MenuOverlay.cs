using UnityEngine;
using System.Collections;

public class MenuOverlay : MonoBehaviour
{	
	public string Title;
		
	public Vector3 ActivePosition; 
	public Vector3 OffscreenPosition;
		
	public float TransTime = 1.0f;
	float m_fOrigTransTime;
	
	public float TransSpeed = 1.0f;
	
	Vector3 m_vCurrTargetPos;
	
	//ArrayList m_lButtons = new ArrayList();
	void Awake()
	{
		m_fOrigTransTime = TransTime;
		TransTime = 0f;	
	}
	
	void Start ()
	{
		transform.position = OffscreenPosition;	
		MenuManager.Instance.Begin();
	}

	void Update ()
	{
		// perform transition in/out
		if (TransTime > 0f)
		{
			iTween.MoveUpdate(gameObject, m_vCurrTargetPos, TransTime);
			
			TransTime -= Time.deltaTime * TransSpeed;
			
			if (TransTime <= 0f)
			{				
				MenuManager.Instance.OnTransitionFinished();
			}
		}
		
	}
	
	
	
	// use itween to do transitions
	public void TransitionIn()
	{
		Debug.Log("Transitioning " + Title + " in");
		m_vCurrTargetPos = ActivePosition;
		TransTime = m_fOrigTransTime;
	}

	public void TransitionOut()
	{
		Debug.Log("Transitioning " + Title + " out");
		m_vCurrTargetPos = OffscreenPosition;
		TransTime = m_fOrigTransTime;
	}
	
	public void AddBtn(GameObject btn)
	{
		// set the button's parent transform to the menu overlay
		btn.transform.parent = gameObject.transform;
				
		// hold on to the button scripts
		//m_lButtons.Add(btn.GetComponent<Button>());
	}
}