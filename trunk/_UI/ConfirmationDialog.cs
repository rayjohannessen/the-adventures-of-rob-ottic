using UnityEngine;
using System.Collections;

public class ConfirmationDialog : MonoBehaviour
{
	public delegate void OnConfirmation();
	
	OnConfirmation m_ConfFunc;
	
	GameObject m_Yes;
	GameObject m_No;
	
	GameObject m_ObjToInstantiate = null;	
	
	void Start ()
	{
		m_Yes = GameObject.Find("BtnYes");
		m_No = GameObject.Find("BtnNo");
		
		// set the position to be right in front of the camera
		transform.position = Camera.main.transform.position;
		transform.position += Camera.main.transform.forward * 10.0f;
	}

	void Update ()
	{
				
#if UNITY_IPHONE
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			Touch touch = Input.GetTouch(0);
        	Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y));
						
            if (m_Yes.collider.bounds.IntersectRay(ray))
            {
				DestroyImmediate(gameObject);
				m_ConfFunc();
            }
            else if (m_No.collider.bounds.IntersectRay(ray))
            {
				DestroyImmediate(gameObject);
				Instantiate(m_ObjToInstantiate);
			}            
		}
#else
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // TODO:: color text on hover:
            if (m_Yes.collider.bounds.IntersectRay(ray))
            {
				DestroyImmediate(gameObject);
				m_ConfFunc();				
            }
            else if (m_No.collider.bounds.IntersectRay(ray))
            {
				DestroyImmediate(gameObject);
				Instantiate(m_ObjToInstantiate);				
            }
        }
#endif
	}
	
	public void SetConfirmationInfo(OnConfirmation func, ref GameObject instantiateOnNo)
	{
		m_ConfFunc = func;
		m_ObjToInstantiate = instantiateOnNo;
	}
}

