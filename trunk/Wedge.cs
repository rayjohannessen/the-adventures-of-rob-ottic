using UnityEngine;
using System.Collections;

public class Wedge : MonoBehaviour 
{
#if UNITY_IPHONE
    bool m_bMoveWithTouch = false;
	float m_fOrigTouchX;
#endif

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
		//Debug.Log("WedgePos:" + m_vOrigPos.x.ToString());
    }
	
	void Update() 
	{
		if (Game.Instance.CurrLevel.PauseMenuActive)
			return;
		
        if (!Game.Instance.WeightDropped && Game.Instance.PreviewDone)
        {
#if UNITY_IPHONE
            // use arrows
            if (Game.Instance.Options.IsOptionActive(Options.eOptions.OPT_USE_ARROWS))
            {
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
#if UNITY_IPHONE
            }
            // use touch input to move wedge
            else
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    Ray ray = Camera.main.ScreenPointToRay(new Vector3(touch.position.x, touch.position.y, 0.0f));
					
					Bounds tempBounds = collider.bounds;
					tempBounds.extents *= 4.0f;
					
                    if (tempBounds.IntersectRay(ray))
                    {
                        if (touch.phase == TouchPhase.Began)
						{
                           	m_bMoveWithTouch = true;
							m_fOrigTouchX = touch.position.x;
							Debug.Log("Hit Wedge with touch");
						}
					}
                    
                    if (m_bMoveWithTouch && touch.phase == TouchPhase.Moved)
                    {						
						float moveAmt = (touch.position.x - m_fOrigTouchX) * Time.deltaTime * MoveSpeed * 0.35f;
                        _MoveWedgeBy(moveAmt);
						m_fOrigTouchX = touch.position.x;
                    }
					else if (m_bMoveWithTouch && touch.phase == TouchPhase.Ended)
					{
						m_bMoveWithTouch = false;
						Debug.Log("Ended wedge movement");
					}
                }
                else
                    m_bMoveWithTouch = false;
            }
#endif
        }
	}

    public void OnReset()
    {
#if UNITY_IPHONE
        m_bMoveWithTouch = false;
#endif
        m_fCurrMoveDist = 0.0f;
        rigidbody.isKinematic = true;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;
    }
    public void OnResetToNewCheckpoint(Vector3 _pos)
    {
#if UNITY_IPHONE
        m_bMoveWithTouch = false;
#endif
        m_vOrigPos.x = _pos.x;

        m_fCurrMoveDist = 0.0f;
        rigidbody.isKinematic = true;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;

        m_vParentOffset = transform.parent.transform.position;
    }
    public void OnLaunchStarted()
    {
#if UNITY_IPHONE
        m_bMoveWithTouch = false;
#endif
        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
    }

#if UNITY_IPHONE
	public bool MoveWithTouch
	{
		get { return m_bMoveWithTouch; }
	}

    void _MoveWedgeBy(float _moveAmtX)
    {
        m_fCurrMoveDist += _moveAmtX;
		
        if (m_fCurrMoveDist < -MaxMoveDist)
            m_fCurrMoveDist = -MaxMoveDist;  
        else if (m_fCurrMoveDist > MaxMoveDist)
            m_fCurrMoveDist = MaxMoveDist;
        
        transform.position = new Vector3(m_vParentOffset.x + m_fCurrMoveDist, transform.position.y, transform.position.z);
        BoardObj.hingeJoint.anchor = new Vector3(m_fCurrMoveDist / BoardObj.GetComponent<Board>().HalfBoardLength, BoardObj.hingeJoint.anchor.y, BoardObj.hingeJoint.anchor.z);
    }
#endif
}
