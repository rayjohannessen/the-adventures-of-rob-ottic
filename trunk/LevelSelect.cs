using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour
{
    int world, lvl;

    public float ScrollSpeed = 7.0f;
    public float MouseScrollArea = 50.0f;   // distance from edge of screen
    public float RightXMax, DownYMax;
    public float SwitchSceneDelay = 1.25f;

    float m_fDelayTimer;

    //GameObject m_Camera;


    void Start()
    {
        m_fDelayTimer = 0.0f;
        int numWorlds = Game.Instance.NumWorlds;
        int numLevls = Game.Instance.NumLevelsInWorld;
        //m_Camera = GameObject.Find("Main Camera");

        for (int i = 0; i < numWorlds; ++i)
        {
            for (int j = 0; j < numLevls; ++j)
            {
                GameObject lvl = GameObject.Find("Btn_Level" + (j + 1).ToString());
                lvl.GetComponent<LevelSelect_Level>().World = i + 1;
                lvl.GetComponent<LevelSelect_Level>().Level = j + 1;
                //Debug.Log("Placing lvl " + (j + 1).ToString() + " at " + (m_Worlds[i].transform.position + LevelOffset * (float)j).ToString());
            }
        }
    }

    void Update()
    {
        if (m_fDelayTimer > 0.0f)
        {
            m_fDelayTimer -= Time.deltaTime;
            if (m_fDelayTimer <= 0.0f)
                Game.Instance.StartGame(lvl, world);
        }
#if UNITY_IPHONE
		
#else
        if (Input.GetButtonUp("Escape"))
            Game.Instance.OnGotoMainMenu();
#endif
		
#region SCROLLING
// scroll right
//         if (Input.mousePosition.x > Screen.width - MouseScrollArea && m_Camera.transform.position.x < RightXMax)
//         {
//             m_Camera.transform.position += m_Camera.transform.right * ScrollSpeed * Time.deltaTime;
//         }
//         // scroll left
//         else if (Input.mousePosition.x < MouseScrollArea && m_Camera.transform.position.x > 0.0f)
//         {
//             m_Camera.transform.position += m_Camera.transform.right * -ScrollSpeed * Time.deltaTime;
//         }
//         // scroll down
//         else if (Input.mousePosition.y < MouseScrollArea && m_Camera.transform.position.y > 1.0f)
//         {
//             m_Camera.transform.position += m_Camera.transform.up * -ScrollSpeed * Time.deltaTime;
//         }
//         // scroll up
//         else if (Input.mousePosition.y > Screen.height - MouseScrollArea && m_Camera.transform.position.y < DownYMax)
//         {
//             m_Camera.transform.position += m_Camera.transform.up * ScrollSpeed * Time.deltaTime;
//         }
#endregion
    }

    public void OnLevelClicked(int _world, int _level)
    {
        world = _world;
        lvl = _level;
        m_fDelayTimer = SwitchSceneDelay;
    }
}