using UnityEngine;
using System.Collections;

// this class brings together the elements of the option menu
public class OptionsMenu : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		
	}

	// Update is called once per frame
	void Update ()
	{
		
	}
	
	public void OnOptionChanged(Options.eOptions _option)
	{	
		Game.Instance.Options.ToggleOption(_option);
		
		switch (_option)
		{
		case Options.eOptions.OPT_USE_ARROWS:
			{
			// checkk if they're changing the option right away before Level inits ControllerInput
				if (Game.Instance.MI != null)				
					Game.Instance.MI.SetUsedButtons();
			}break;
		}	
	}
}

