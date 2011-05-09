using UnityEngine;
using System.Collections;
using System;

public class LevelSelect_Level : MonoBehaviour
{
    LevelSelect m_LvlSel;

    int m_nWorld = 1;
    int m_nLevel = 1;

    void Start()
    {
        m_LvlSel = GameObject.Find("LevelSelectMenu").GetComponent<LevelSelect>();
    }

    void Update()
    {
#if UNITY_IPHONE
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{			
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
			if (collider.bounds.IntersectRay(ray))
			{
				m_LvlSel.OnLevelClicked(m_nWorld, m_nLevel);
			}
		}
#endif
    }
	
#if !UNITY_IPHONE
    void OnMouseUp()
    {
        m_LvlSel.OnLevelClicked(m_nWorld, m_nLevel);
    }

    void OnMouseExit()
    {

    }
#endif

    public int World
    {
        set { m_nWorld = value; }
    }
    public int Level
    {
        set { m_nLevel = value; }
    }
}