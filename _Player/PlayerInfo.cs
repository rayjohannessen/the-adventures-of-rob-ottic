using UnityEngine;
using System.Collections;

public class PlayerInfo : MonoBehaviour
{
	int m_nLives;

	// defaults are used when all lives are exhausted
	// and no more than defaults have been obtained
	public const int g_nDefaultLives = 3;

	int m_nMaxLives;
	
	int m_nCurrency;
	int m_nMaxCurrency;

	void Start ()
	{
		DontDestroyOnLoad (this);
		
		m_nCurrency = 0;
		
		m_nMaxLives = 99;
		m_nMaxCurrency = 99;
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
	
	public void IncrementCurrency()
	{
		++m_nCurrency;
		if (m_nCurrency > m_nMaxCurrency)
		{
			m_nCurrency = 0;
			++m_nLives;
			if (m_nLives > m_nMaxLives)
				m_nLives = m_nMaxLives;
		}
	}

	public int Lives {
		get { return m_nLives; }
		set { m_nLives = value; }
	}
	
	public int Currency {
		get { return m_nCurrency; }
		set { m_nCurrency = value; }
	}
}
