using UnityEngine;
using System.Collections;

public class DefeatMenu : MonoBehaviour
{
    GameObject m_Level;
    GameObject m_MainMenu;

    void Start()
    {
        m_Level = GameObject.Find("BtnReset");
        m_MainMenu = GameObject.Find("BtnMainMenu");
    }

    void Update()
    {
#if UNITY_IPHONE
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			Touch touch = Input.GetTouch(0);
        	Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y));
			
            // TODO:: color text on hover:
            if (m_Level.collider.bounds.IntersectRay(ray))
            {
                Game.Instance.RedoCurrLevel();
            }
            else if (m_MainMenu.collider.bounds.IntersectRay(ray))
            {
                // TODO:: perform any resetting necessary here:
                Game.Instance.OnGotoMainMenu();
            }
		}
#else
        if (Input.GetMouseButtonUp(0))
        {			
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
            // TODO:: color text on hover:
            if (m_Level.collider.bounds.IntersectRay(ray))
            {
                Game.Instance.RedoCurrLevel();
            }
            else if (m_MainMenu.collider.bounds.IntersectRay(ray))
            {
                // TODO:: perform any resetting necessary here:
                Game.Instance.OnGotoMainMenu();
            }
        }
#endif
    }
}
			