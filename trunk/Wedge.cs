using UnityEngine;
using System.Collections;

public class Wedge : MonoBehaviour 
{
    public float MoveSpeed = 1.5f;
    public float MaxMoveDist = 0.25f;

    float m_fCurrMoveDist;

    Vector3 m_vParentOffset;
    Vector3 m_vOrigPos;
    Quaternion m_vOrigRot;

    GameObject BoardObj;    // the see-saw board (to move the hinge)

    
    void Start() 
	{
        BoardObj = GameObject.Find("Board");
        
        rigidbody.freezeRotation = true;

        rigidbody.isKinematic = true;

        m_fCurrMoveDist = 0.0f;

        m_vParentOffset = transform.parent.transform.position;
        m_vOrigRot = rigidbody.rotation;
        m_vOrigPos = rigidbody.position;
    }
	
	void Update() 
	{
        if (!Game.Instance.WeightDropped && Game.Instance.PreviewDone)
        {
#if UNITY_IPHONE
			if (Game.Instance.MI.BtnDown(MobileInput.BTN_RIGHT) && m_fCurrMoveDist < MaxMoveDist)
#else
            if (Input.GetButton("Move Wedge Right") && m_fCurrMoveDist < MaxMoveDist)
#endif
			{
                m_fCurrMoveDist += MoveSpeed * Time.deltaTime;
                if (m_fCurrMoveDist > MaxMoveDist)
                    m_fCurrMoveDist = MaxMoveDist;

                transform.position = new Vector3(m_vParentOffset.x + m_fCurrMoveDist, transform.position.y, transform.position.z);
                BoardObj.hingeJoint.anchor = new Vector3(m_fCurrMoveDist / BoardObj.GetComponent<Board>().HalfBoardLength, BoardObj.hingeJoint.anchor.y, BoardObj.hingeJoint.anchor.z);
            }
#if UNITY_IPHONE
			else if (Game.Instance.MI.BtnDown(MobileInput.BTN_LEFT) && m_fCurrMoveDist > -MaxMoveDist)
#else
            else if (Input.GetButton("Move Wedge Left") && m_fCurrMoveDist > -MaxMoveDist)
#endif
			{
                m_fCurrMoveDist -= MoveSpeed * Time.deltaTime;
                if (m_fCurrMoveDist < -MaxMoveDist)
                    m_fCurrMoveDist = -MaxMoveDist;

                transform.position = new Vector3(m_vParentOffset.x + m_fCurrMoveDist, transform.position.y, transform.position.z);
                BoardObj.hingeJoint.anchor = new Vector3(m_fCurrMoveDist / BoardObj.GetComponent<Board>().HalfBoardLength, BoardObj.hingeJoint.anchor.y, BoardObj.hingeJoint.anchor.z);
            }
        }
	}

    public void OnReset()
    {
        m_fCurrMoveDist = 0.0f;
        rigidbody.isKinematic = true;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;
    }
    public void OnResetToNewCheckpoint(Vector3 _pos)
    {
        m_vOrigPos.x = _pos.x;

        m_fCurrMoveDist = 0.0f;
        rigidbody.isKinematic = true;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;

        m_vParentOffset = transform.parent.transform.position;
    }
    public void OnLaunchStarted()
    {
        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
    }

}
