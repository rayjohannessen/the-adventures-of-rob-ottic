using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour
{
	public delegate void ButtonAction();
	
	ButtonAction fpClickAction;
	
#if !UNITY_IPHONE
	ButtonAction fpHoverAction = null;
	ButtonAction fpUnHoverAction = null;
#endif
	
	const float Z_OFFSET_FROM_CAM = 6.0f;
	
	public void Init(string text, Vector2 pos, ButtonAction clickAction, ButtonAction hoverAction, ButtonAction UnHoverAction)
	{
		Debug.Log("Initializing btn:  " + text);
		
		gameObject.GetComponentInChildren<TextMesh>().text = text;
		
		transform.position = new Vector3(pos.x, pos.y, Camera.mainCamera.transform.position.z + Z_OFFSET_FROM_CAM);
		
		fpClickAction = clickAction;
		
#if !UNITY_IPHONE
		fpHoverAction = hoverAction;
		fpUnHoverAction = UnHoverAction;
#endif
	}	
	
	void Start ()
	{
		
	}

	void Update ()
	{
		
#if UNITY_IPHONE
        // see if the ray hits the button
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
            if (collider.bounds.IntersectRay(ray))
            {
				fpClickAction();
			}			
		}	
#endif
	}
	
	
#if !UNITY_IPHONE
    void OnMouseEnter()
    {
		if (fpHoverAction != null)
			fpHoverAction();
    }

    void OnMouseExit()
    {
		if (fpUnHoverAction != null)
			fpUnHoverAction();
    }

    void OnMouseDown()
    {
		
    }

    void OnMouseUp()
    {
		if (fpClickAction != null)
			fpClickAction();
    }
#endif
}

