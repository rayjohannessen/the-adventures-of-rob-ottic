using UnityEngine;
using System.Collections;

public class PauseMenu_old : MonoBehaviour
{
    GameObject m_Continue;
    GameObject m_Restart;
    GameObject m_MainMenu;
    GameObject m_Options;
	GameObject m_Quit;
	
	public GameObject m_Confirmation = null;
	
	float m_fSimulatedDelay;

    void Start()
    {
        m_Restart = GameObject.Find("BtnRestart");
        m_Continue = GameObject.Find("BtnContinue");
        m_MainMenu = GameObject.Find("BtnMainMenu");
        m_Options = GameObject.Find("BtnOptions");
		m_Quit = GameObject.Find("BtnQuit");
				
		// set the position to be right in front of the camera
		transform.position = Camera.main.transform.position;
		transform.position += Camera.main.transform.forward * 15.0f;
		
		Debug.Log("Pause Menu Created");
		m_fSimulatedDelay = 2.5f;
    }

    void Update()
    {
		// wait a bit to simulate the animation before any buttons can be pressed
//		if (m_fSimulatedDelay > 0.0f)
//		{
//			m_fSimulatedDelay -= Time.deltaTime;
//			return;
//		}
		
#if UNITY_IPHONE
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			Touch touch = Input.GetTouch(0);
        	Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y));
						
            if (m_Restart.collider.bounds.IntersectRay(ray))
            {
				DestroyImmediate(gameObject);
				
				m_Confirmation = (GameObject)Instantiate(m_Confirmation);
				m_Confirmation.GetComponent<ConfirmationDialog>().SetConfirmationInfo(Game.Instance.RedoCurrLevel, ref Game.Instance.CurrLevel.m_PauseMenu);
				
				//Game.Instance.RedoCurrLevel();
            }
            else if (m_Continue.collider.bounds.IntersectRay(ray))
            {
				ClosePauseMenu();
            }
            else if (m_MainMenu.collider.bounds.IntersectRay(ray))
            {
				DestroyImmediate(gameObject);
				
				m_Confirmation = (GameObject)Instantiate(m_Confirmation);
				m_Confirmation.GetComponent<ConfirmationDialog>().SetConfirmationInfo(Game.Instance.OnGotoMainMenu, ref Game.Instance.CurrLevel.m_PauseMenu);
            }
            else if (m_Options.collider.bounds.IntersectRay(ray))
            {     
				// bring in options overlay (prefab)
				
				
    			//Application.LoadLevel("OptionsMenu");
            }
			else if (m_Quit.collider.bounds.IntersectRay(ray))
			{
				DestroyImmediate(gameObject);
				
				m_Confirmation = (GameObject)Instantiate(m_Confirmation);
				m_Confirmation.GetComponent<ConfirmationDialog>().SetConfirmationInfo(Game.Instance.Quit, ref Game.Instance.CurrLevel.m_PauseMenu);
			}
		}
#else
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // TODO:: color text on hover:
            if (m_Restart.collider.bounds.IntersectRay(ray))
            {
				DestroyImmediate(gameObject);
				
				m_Confirmation = (GameObject)Instantiate(m_Confirmation);
				m_Confirmation.GetComponent<ConfirmationDialog>().SetConfirmationInfo(OnRestart, ref Game.Instance.CurrLevel.m_PauseMenu);
            }
            else if (m_Continue.collider.bounds.IntersectRay(ray))
            {
				ClosePauseMenu();
            }
            else if (m_MainMenu.collider.bounds.IntersectRay(ray))
            {
				DestroyImmediate(gameObject);
				
				m_Confirmation = (GameObject)Instantiate(m_Confirmation);
				m_Confirmation.GetComponent<ConfirmationDialog>().SetConfirmationInfo(OnMainMenu, ref Game.Instance.CurrLevel.m_PauseMenu);
            }
            else if (m_Options.collider.bounds.IntersectRay(ray))
            {
                //Application.LoadLevel("OptionsMenu");
            }
			else if (m_Quit.collider.bounds.IntersectRay(ray))
			{
				DestroyImmediate(gameObject);	
				
				m_Confirmation = (GameObject)Instantiate(m_Confirmation);
				m_Confirmation.GetComponent<ConfirmationDialog>().SetConfirmationInfo(OnQuit, ref Game.Instance.CurrLevel.m_PauseMenu);
			}
        }
#endif
    }
	
	public void ClosePauseMenu()
	{
		Game.Instance.CurrLevel.PauseMenuActive = false;
		DestroyImmediate(gameObject);		
	}
	
	public float PauseMenuAnimating
	{
		get { return m_fSimulatedDelay; }
	}
	
	// delegate functions, used for confirmation dialog
	public void OnMainMenu()
	{
		Game.Instance.CurrLevel.PauseMenuActive = false;
		Game.Instance.OnGotoMainMenu();
	}
	
	public void OnQuit()
	{
		Game.Instance.CurrLevel.PauseMenuActive = false;
		Game.Instance.Quit();
	}
	
	public void OnRestart()
	{
		Game.Instance.CurrLevel.PauseMenuActive = false;
		Game.Instance.RedoCurrLevel();
	}
}
