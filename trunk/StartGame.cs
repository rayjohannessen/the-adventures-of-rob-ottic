using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour
{
	Game m_GameInstance;

	// Use this for initialization
	void Start ()
	{
		m_GameInstance = Game.Instance;
	}

	// Update is called once per frame
//	void Update ()
//	{
//		m_GameInstance.Update();
//	}
//	
//	void LateUpdate()
//	{
//		m_GameInstance.LateUpdate();
//	}
}

