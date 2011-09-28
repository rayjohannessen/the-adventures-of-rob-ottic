using UnityEngine;
using System.Collections;

public class RopeAlt : MonoBehaviour
{
    int m_nConnectedLinkIndex;

    public float MaxGrabDistance;
    public float MinLetGoXVel = 5.0f;
    public float MaxLetGoXVel = 15.0f;

    float m_fMoveTimer;

    Rigidbody[] m_lLinks;

    GameObject m_Player;
	
    Player m_PlayerScript;	
	
	Pulley m_PulleyScript;

    ////////////////////////////////////////////////////////////////////////

    void Start()
    {
        m_lLinks = GetComponentsInChildren<Rigidbody>();

        //m_Hand = GameObject.Find("TestHand");
        m_Player = GameObject.Find("Player");
        m_PlayerScript = m_Player.GetComponent<Player>();
		m_PulleyScript = GetComponent<Pulley>();
        //Debug.Log("Num Links:" + m_lLinks.Length.ToString());
    }

    void Update()
    {
        if (!m_PlayerScript.GetRopeGrabbed())
        {
#if UNITY_IPHONE
			if (Game.Instance.MI.BtnPressed(MobileInput.BTN_A))
#else
        	if (Input.GetButtonDown("Action Btn 1"))
#endif
			{
                m_nConnectedLinkIndex = -1;
				
				GameObject hand = GameObject.Find("TestHand");
				
                // Determine which link the player's arm is nearest and "grab" onto that one:
                for (int i = 0; i < m_lLinks.Length; ++i)
                    if ((m_lLinks[i].transform.position - hand.transform.position).magnitude < MaxGrabDistance)
                        m_nConnectedLinkIndex = i;

                if (m_nConnectedLinkIndex > -1)
                {
                    //Debug.Log("Link found:" + m_nConnectedLinkIndex.ToString());

					m_PlayerScript.OnRopeGrabbed();
					if (m_PulleyScript != null)
						m_PulleyScript.SetActivate(false);
					
                    m_Player.AddComponent<HingeJoint>();
                    //m_Player.rigidbody.constraints = RigidbodyConstraints.None;
                    m_Player.hingeJoint.connectedBody = m_lLinks[m_nConnectedLinkIndex];
					m_Player.hingeJoint.axis = new Vector3(0.0f, 0.0f, 1.0f);
                }
            }
        }
        else if (m_PlayerScript.GetRopeGrabbed())
        {
            // let go of the rope???
#if UNITY_IPHONE
			if (Game.Instance.MI.BtnReleased(MobileInput.BTN_A))
#else
            if (Input.GetButtonUp("Action Btn 1"))
#endif
			{				
				Debug.Log("Rope released");
				
				m_PlayerScript.OnRopeReleased();
				if (m_PulleyScript != null)
					m_PulleyScript.SetActivate(true);
				
				//GameObject hand = GameObject.Find("TestHand");
				Destroy(m_Player.hingeJoint);
            }
        }
    }
}