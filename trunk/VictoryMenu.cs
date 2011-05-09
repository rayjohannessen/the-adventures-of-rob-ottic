using UnityEngine;
using System.Collections;

public class VictoryMenu : MonoBehaviour
{
    GameObject m_NextLvl;

    void Start()
    {
        m_NextLvl = GameObject.Find("BtnNextLevel");
    }

    void Update()
    {
#if UNITY_IPHONE
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			Touch touch = Input.GetTouch(0);
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y));		
	        if (m_NextLvl.collider.bounds.IntersectRay(ray))
	        {
	            // TODO:: color text on hover:	
                Game.Instance.NextLevel();
	        }			
		}
#else
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);		
        if (m_NextLvl.collider.bounds.IntersectRay(ray))
        {
            // TODO:: color text on hover:

            if (Input.GetMouseButtonUp(0))
                Game.Instance.NextLevel();
        }
#endif
    }
}