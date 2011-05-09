using UnityEngine;
using System.Collections;

public class OptionsMenu_Option : MonoBehaviour
{
	public Options.eOptions OptionType;
	
	// Use this for initialization
	void Start ()
	{
		
	}

	// Update is called once per frame
	void Update ()
	{
#if UNITY_IPHONE
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{			
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
			if (collider.bounds.IntersectRay(ray))
			{
				GameObject.Find("OptionsMenu").GetComponent<OptionsMenu>().OnOptionChanged(OptionType);
			}
		}
#else
		
#endif		
	}
}

