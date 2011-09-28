using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour
{
    float m_fHalfBoardLength;

    Vector3 m_vOrigPos;
    Quaternion m_vOrigRot;

    void Start() 
    {
        rigidbody.isKinematic = true;

        HalfBoardLength = this.collider.bounds.size.x * 0.5f;

        m_vOrigRot = rigidbody.rotation;
        m_vOrigPos = rigidbody.position;
    }
	
	void Update () 
	{
	}

    public void OnReset()
    {
        rigidbody.isKinematic = true;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;
    }
    public void OnResetToNewCheckpoint(Vector3 _pos)
    {
        m_vOrigPos.x = _pos.x;

        rigidbody.isKinematic = true;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;
    }
    public void OnLaunchStarted()
    {
        rigidbody.isKinematic = false;
        rigidbody.constraints = 0;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
    }



    /// <summary>
    /// PROPERTIES
    /// </summary>
    public float HalfBoardLength
    {
        get { return m_fHalfBoardLength; }
        set { m_fHalfBoardLength = value; }
    }
}
