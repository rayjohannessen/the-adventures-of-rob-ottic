using UnityEngine;
using System.Collections;

public class Weight : MonoBehaviour 
{
#if UNITY_IPHONE
    bool m_bMoveWithTouch = false;
    float m_fOrigTouchY;
#endif

    public float MoveSpeed = 3.0f;
    public float MaxMoveDist = 1.35f;
    public float StartXOffset;  // offset from center of board on the x axis

    float m_fCurrMoveDist;

    Vector3 m_vOrigPos;
    Quaternion m_vOrigRot;

    public GameObject BoardObject;
    public GameObject WedgeObject;
    public GameObject PlayerObject;


	void Start ()
    {
        rigidbody.isKinematic = true;

        m_fCurrMoveDist = 0.0f;

        m_vOrigRot = rigidbody.rotation;
        m_vOrigPos = rigidbody.position;

	}
	
	void Update ()
	{
		if (Game.Instance.CurrLevel.PauseMenuActive)
			return;
		
        if (!Game.Instance.WeightDropped && Game.Instance.PreviewDone)
        {
#if UNITY_IPHONE
            // use arrows
            if (Game.Instance.Options.IsOptionActive(Options.eOptions.OPT_USE_ARROWS))
            {
			    if (Game.Instance.MI.BtnDown(MobileInput.BTN_UP) && m_fCurrMoveDist < MaxMoveDist)
#else
            if (Input.GetButton("Move Weight Up") && m_fCurrMoveDist < MaxMoveDist)
#endif
			    {
                    m_fCurrMoveDist += MoveSpeed * Time.deltaTime;
                    transform.position += Vector3.up * MoveSpeed * Time.deltaTime;
                }
#if UNITY_IPHONE
			    else if (Game.Instance.MI.BtnDown(MobileInput.BTN_DOWN) && m_fCurrMoveDist > -MaxMoveDist)
#else
            else if (Input.GetButton("Move Weight Down") && m_fCurrMoveDist > -MaxMoveDist)
#endif
			    {
                    m_fCurrMoveDist -= MoveSpeed * Time.deltaTime;
                    transform.position -= Vector3.up * MoveSpeed * Time.deltaTime;
                }
#if UNITY_IPHONE
            }
            // use touch input to move weight
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
                            m_fOrigTouchY = touch.position.y;
                        }
                    }
                    
                    if (m_bMoveWithTouch && touch.phase == TouchPhase.Moved)
                    {
                        float moveAmt = (touch.position.y - m_fOrigTouchY) * Time.deltaTime * MoveSpeed * 0.1f;
                        _MoveWeightBy(moveAmt);
                        m_fOrigTouchY = touch.position.y;
                    }
                    else if (m_bMoveWithTouch && touch.phase == TouchPhase.Ended)
                    {
                        m_bMoveWithTouch = false;
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
        m_vOrigPos.x = _pos.x + StartXOffset;

        m_fCurrMoveDist = 0.0f;

        rigidbody.isKinematic = true;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;
    }
    public void OnWeightDropped()
    {
#if UNITY_IPHONE
        m_bMoveWithTouch = false;
#endif
        rigidbody.isKinematic = false;
        rigidbody.constraints = 0;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
        rigidbody.useGravity = true;
    }

    void OnTriggerEnter(Collider body)
    {
        if (body.name == "LaunchTrigger")
        {
            BoardObject.GetComponent<Board>().OnLaunchStarted();
            WedgeObject.GetComponent<Wedge>().OnLaunchStarted();
            PlayerObject.GetComponent<Player>().OnLaunchStarted();
            Game.Instance.LaunchStarted = true;
        }
    }


#if UNITY_IPHONE
	public bool MoveWithTouch
	{
		get { return m_bMoveWithTouch; }
	}

    void _MoveWeightBy(float _moveAmtY)
    {
		float prevMoveDist = m_fCurrMoveDist;
        m_fCurrMoveDist += _moveAmtY;

        if (m_fCurrMoveDist < -MaxMoveDist)
            m_fCurrMoveDist = -MaxMoveDist;  
        else if (m_fCurrMoveDist > MaxMoveDist)
            m_fCurrMoveDist = MaxMoveDist;
		
		_moveAmtY = m_fCurrMoveDist - prevMoveDist;
        
        transform.position = new Vector3(transform.position.x, transform.position.y + _moveAmtY, transform.position.z);
    }
#endif
}
