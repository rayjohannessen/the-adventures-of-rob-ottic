using UnityEngine;
using System.Collections;

public class MenuOption : MonoBehaviour
{
    public GameObject Bolt;

    public Vector3 ClickScale = new Vector3(0.1f, 0.05f, 0.025f);
	
#if !UNITY_IPHONE
    Color m_OriginalBtnColor;
#endif
	
    public Color ButtonHoverColor = Color.white;
    public Color TextHoverColor = Color.red;

    public float HoverZ = -0.25f;
    public float InterpTime = 0.5f;

    public bool DoClickScale = true;
    public bool DoMoveOnMouseEnter = false;
	
#if !UNITY_IPHONE
    float m_fBtnHoverZ, m_fBtnNormalZ;
    float m_fBoltHoverZ, m_fBoltNormalZ;
#endif
	
#if UNITY_IPHONE
	void Awake()
	{
		iPhoneKeyboard.autorotateToLandscapeLeft = false;
		iPhoneKeyboard.autorotateToLandscapeRight = false;
		iPhoneKeyboard.autorotateToPortrait = false;
		iPhoneKeyboard.autorotateToPortraitUpsideDown = false;		
	}
#endif
	
    void Start()
    {
#if !UNITY_IPHONE
        m_OriginalBtnColor = renderer.material.color;
        m_fBtnHoverZ = transform.position.z + HoverZ;
        m_fBtnNormalZ= transform.position.z;
        if (Bolt != null)
        {
	        m_fBoltHoverZ = Bolt.transform.position.z + HoverZ;
	        m_fBoltNormalZ= Bolt.transform.position.z;
        }
#endif
		
		transform.Find("Text").gameObject.renderer.material.color = ButtonHoverColor;
    }

    void Update()
    {
#if UNITY_IPHONE

        // see if the ray hits the button
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
            if (collider.bounds.IntersectRay(ray))
            {				
		        if (DoClickScale)
		            renderer.transform.localScale += ClickScale;
		        renderer.material.color = ButtonHoverColor;		
		        transform.Find("Text").gameObject.renderer.material.color = TextHoverColor;
				
                if (name == "Btn_Quit")
                {
                    Application.Quit();
                }
                else if (name == "Btn_Play")
                {
                    Application.LoadLevel("LevelSelect");
                }
                else if (name == "Btn_Practice")
                {
                    Game.Instance.StartPractice();
                }
                else if (name == "Btn_Options")
                {
					Application.LoadLevel("OptionsMenu");
                }
                else if (name == "Btn_Back")
                {
                    Game.Instance.OnGotoMainMenu();
                }
            }
        }
#endif
    }

#if !UNITY_IPHONE
    void OnMouseEnter()
    {
        //change the color of the cube
        renderer.material.color = ButtonHoverColor;

        transform.Find("Text").gameObject.renderer.material.color = TextHoverColor;

        if (DoMoveOnMouseEnter)
            iTween.MoveTo(gameObject, new Vector3(transform.position.x, transform.position.y, m_fBtnHoverZ), InterpTime);

        if (Bolt != null)
        {
            Bolt.animation.Play("BoltTurnCCW");
            iTween.MoveTo(Bolt, new Vector3(Bolt.transform.position.x, Bolt.transform.position.y, m_fBoltHoverZ), InterpTime);
        }
    }

    void OnMouseExit()
    {
        //change the color of the cube
        renderer.material.color = m_OriginalBtnColor;

        transform.Find("Text").gameObject.renderer.material.color = ButtonHoverColor;

        if (DoMoveOnMouseEnter)
            iTween.MoveTo(gameObject, new Vector3(transform.position.x, transform.position.y, m_fBtnNormalZ), InterpTime);


        if (Bolt != null)
        {
            iTween.MoveTo(Bolt, new Vector3(Bolt.transform.position.x, Bolt.transform.position.y, m_fBoltNormalZ), InterpTime);
            Bolt.animation.Play("BoltTurnCW");
        }
    }

    void OnMouseDown()
    {
        //change the scale of the cube
        if (DoClickScale)
            renderer.transform.localScale += ClickScale;

        //change the color of the text
        //transform.Find("Text").gameObject.renderer.material.color = ButtonHoverColor;
    }

    void OnMouseUp()
    {
        //change the scale of the cube
        if (DoClickScale)
            renderer.transform.localScale -= ClickScale;

        //change the color of the text
        //transform.Find("Text").gameObject.renderer.material.color = m_OriginalBtnColor;

        if (name == "Btn_Quit")
        {
            Application.Quit();
        }
        else if (name == "Btn_Play")
        {
            Application.LoadLevel("LevelSelect");
        }
        else if (name == "Btn_Practice")
        {
            Game.Instance.StartPractice();
        }
        else if (name == "Btn_Options")
        {
			Application.LoadLevel("OptionsMenu");
        }
        else if (name == "Btn_Back")
        {
            Game.Instance.OnGotoMainMenu();
        }
    }
#endif
}