using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
	int m_nLives;

	// defaults are used when all lives are exhausted
	// and no more than defaults have been obtained
	public const int g_nDefaultLives = 3;

	int m_nMaxLives;

	void Start ()
	{
		DontDestroyOnLoad (this);
		
		m_nMaxLives = 99;
	}

	void Update ()
	{
		
	}

	public void Init (int _lives)
	{
		m_nLives = _lives;
	}

	// return false if game over
	public bool OnDeath ()
	{
		--m_nLives;
		return m_nLives > 0;
	}

	public void RevertToDefaults ()
	{
		m_nLives = g_nDefaultLives;
	}


	public int Lives {
		get { return m_nLives; }
		set { m_nLives = value; }
	}
}
