using System.Collections;

public class Options
{
	public enum eOptions 
	{ 
#if UNITY_IPHONE
		OPT_USE_ARROWS,	// use buttons for controls or use accelerometer
#endif
		NUM_OPTIONS
	};
	
	int m_nFlags;
	
	
	public Options()
	{
		m_nFlags = 0;
		
		//Utilities.Instance.BitOn(ref m_nFlags, (int)eOptions.OPT_USE_BUTTONS);
	}
	
	public void ActivateOption(eOptions _option)
	{
		Utilities.Instance.BitOn(ref m_nFlags, (int)_option);
	}
	public void DeactivateOption(eOptions _option)
	{
		Utilities.Instance.BitOff(ref m_nFlags, (int)_option);
	}
	public void ToggleOption(eOptions _option)
	{
		Utilities.Instance.BitToggle(ref m_nFlags, (int)_option);
	}
	
	public bool IsOptionActive(eOptions _option)
	{
		return Utilities.Instance.BitTest(m_nFlags, (int)_option);	
	}
}

