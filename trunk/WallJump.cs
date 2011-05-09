using UnityEngine;
using System.Collections;

public class WallJump : MonoBehaviour
{
    bool m_bCanWallJump;

    /// <summary>
    /// the x axis (which way is away from the wall) from -1 to 1
    /// the y is stored in the player
    /// </summary>
    public float JumpDirection;

    //Player m_Player;
    GameObject parent;

    void Start()
    {
        m_bCanWallJump = false;
        //m_Player = GameObject.Find("Player").GetComponent<Player>();
        //parent = transform.parent.gameObject;
    }

    void Update()
    {
#if UNITY_IPHONE
		if (m_bCanWallJump && Game.Instance.MI.BtnPressed(MobileInput.BTN_A))
#else
        if (m_bCanWallJump && Input.GetButtonDown("Action Btn 1"))
#endif
		{
            GameObject.Find("Player").GetComponent<Player>().OnWallJumpStart(this);
            m_bCanWallJump = false; // can't wall jump off this wall again until you exit trigger
        }
    }

    void OnTriggerEnter(Collider _info)
    {
        //Debug.Log("Can wall jump:" + name);
        m_bCanWallJump = true;
        //parent.renderer.material.color = Color.red;
    }

    void OnTriggerExit(Collider _info)
    {
        //Debug.Log("Can NOT wall jump:" + name);
        m_bCanWallJump = false;
        //parent.renderer.material.color = Color.green;
    }

    public void OnReset()
    {
        //Debug.Log("Reset Wall Jump");
        m_bCanWallJump = false;
    }

    public bool CanWallJump
    {
        get { return m_bCanWallJump; }
        set { m_bCanWallJump = value; }
    }
}