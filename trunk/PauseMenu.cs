using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    GameObject m_Continue;
    GameObject m_Restart;
    GameObject m_MainMenu;
    GameObject m_Options;

    void Start()
    {
        m_Restart = GameObject.Find("BtnRestart");
        m_Continue = GameObject.Find("BtnContinue");
        m_MainMenu = GameObject.Find("BtnMainMenu");
        m_Options = GameObject.Find("BtnOptions");
    }

    void Update()
    {
#if UNITY_IPHONE
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			Touch touch = Input.GetTouch(0);
        	Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y));
			
            // TODO:: color text on hover:
            if (m_Restart.collider.bounds.IntersectRay(ray))
            {

            }
            else if (m_Continue.collider.bounds.IntersectRay(ray))
            {

            }
            else if (m_MainMenu.collider.bounds.IntersectRay(ray))
            {
                Game.Instance.OnGotoMainMenu();
            }
            else if (m_Options.collider.bounds.IntersectRay(ray))
            {                
    			Application.LoadLevel("OptionsMenu");
            }
		}
#else
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // TODO:: color text on hover:
            if (m_Restart.collider.bounds.IntersectRay(ray))
            {

            }
            else if (m_Continue.collider.bounds.IntersectRay(ray))
            {

            }
            else if (m_MainMenu.collider.bounds.IntersectRay(ray))
            {
                Game.Instance.OnGotoMainMenu();
            }
            else if (m_Options.collider.bounds.IntersectRay(ray))
            {
                Application.LoadLevel("OptionsMenu");
            }
        }
#endif
    }
}
