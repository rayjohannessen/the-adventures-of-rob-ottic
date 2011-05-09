//using UnityEngine;
using System.Collections;

public class Utilities
{
	private static Utilities instance;
	
	
	public void BitOn(ref int _var, int _bit)
	{
		_var |= (1 << _bit);	
	}
	
	public void BitOff(ref int _var, int _bit)
	{
		_var &= ~(1 << _bit);	
	}
	
	public void BitToggle(ref int _var, int _bit)
	{
		_var ^= (1 << _bit);	
	}
	
	public bool BitTest(int _var, int _bit)
	{
		return (_var & (1 << _bit)) != 0;	
	}
	
	
	/// <summary>
	/// Initialization
	/// </summary>
	public Utilities()
	{
		if (instance != null)
			return;
		instance = this;
	}
	
	public static Utilities Instance
	{
		get 
		{
			if (instance == null)
				new Utilities();
			return instance;
		}
	}
}

