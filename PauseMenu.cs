using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    GameObject m_Continue;
    GameObject m_Restart;
    GameObject m_MainMenu;
    GameObject m_Options;
	GameObject m_Quit;
	
	float m_fSimulatedDelay;

    void Start()
    {
        m_Restart = GameObject.Find("BtnRestart");
        m_Continue = GameObject.Find("BtnContinue");
        m_MainMenu = GameObject.Find("BtnMainMenu");
        m_Options = GameObject.Find("BtnOptions");
		m_Quit = GameObject.Find("BtnQuit");
		
		if (m_Restart == null)
			Debug.Log("restart==null");
		if (m_Continue == null)
			Debug.Log("continue==null");
		if (m_MainMenu == null)
			Debug.Log("m_MainMenu==null");
		if (m_Options == null)
			Debug.Log("m_Options==null");
		if (m_Quit == null)
			Debug.Log("m_Quit==null");
		
		// set the position to be right in front of the camera
		transform.position = Camera.main.transform.position;
		transform.position += Camera.main.transform.forward * 15.0f;
		
		Debug.Log("Pause Menu Created");
		m_fSimulatedDelay = 2.5f;
    }

    void Update()
    {
		// wait a bit to simulate the animation before any buttons can be pressed
		if (m_fSimulatedDelay > 0.0f)
		{
			m_fSimulatedDelay -= Time.deltaTime;
			return;
		}
		
#if UNITY_IPHONE
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			Touch touch = Input.GetTouch(0);
        	Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y));
			
            // TODO:: color text on hover:
            if (m_Restart.collider.bounds.IntersectRay(ray))
            {
				Game.Instance.RedoCurrLevel();
            }
            else if (m_Continue.collider.bounds.IntersectRay(ray))
            {
				Game.Instance.CurrLevel.PauseMenuActive = false;
				DestroyImmediate(gameObject);
            }
            else if (m_MainMenu.collider.bounds.IntersectRay(ray))
            {
                Game.Instance.OnGotoMainMenu();
            }
            else if (m_Options.collider.bounds.IntersectRay(ray))
            {                
    			Application.LoadLevel("OptionsMenu");
            }
			else if (m_Quit.collider.bounds.IntersectRay(ray))
			{
				Application.Quit();	
			}
		}
#else
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // TODO:: color text on hover:
            if (m_Restart.collider.bounds.IntersectRay(ray))
            {
				Game.Instance.RedoCurrLevel();
            }
            else if (m_Continue.collider.bounds.IntersectRay(ray))
            {
				Game.Instance.CurrLevel.PauseMenuActive = false;
				DestroyImmediate(GameObject.Find("PauseMenu"), true);
            }
            else if (m_MainMenu.collider.bounds.IntersectRay(ray))
            {
                Game.Instance.OnGotoMainMenu();
            }
            else if (m_Options.collider.bounds.IntersectRay(ray))
            {
                Application.LoadLevel("OptionsMenu");
            }
			else if (m_Quit.collider.bounds.IntersectRay(ray))
			{
				Application.Quit();	
			}
        }
#endif
    }
	
	public float PauseMenuAnimating
	{
		get { return m_fSimulatedDelay; }
	}
}
