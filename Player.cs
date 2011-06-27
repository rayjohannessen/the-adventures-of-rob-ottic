using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{	
	enum ePlayerFlags 
	{
		PF_HIT_SPIKES, 
		PF_HIT_WATER, 
		PF_DIED, 
		PF_BOOST_VALID, 
		PF_WALL_JUMP_STARTED,
		PF_ROPE_GRABBED
	};
	
	int m_nFlags;

    public float AdditionalForceTime = 1.0f;
    public float ClimbForce = 50.0f;
    public float BoostForce = 300.0f;
    public float BoostDuration = 0.5f;
    public float HandOffset = 3.0f;

    // movement
    public float MaxXVel = 300.0f;
    public float AccelerometerScalar = 25.0f;

    // wall jump
    public float WallJumpForce = 1000.0f;
    public float WallJumpDelay = 1.0f;
    public float WallJumpYDirection = 0.75f;
    public float WallJumpXDirection = 0.35f;
    public float WallJumpForceDuration;

    /// <summary>
    /// only used once a wall jump has been started, this is the time the character will
    /// "freeze" or "stick" to the wall before jumping away from it
    /// </summary>
    float m_fWallJumpTimer;
    float m_fApplyJumpTimer;
    float m_fBoostTimer;
    float m_fAddForceTimer;
    // need some leeway for resetting the player if they're not moving fast
    // this is used so no reset occurs immediately after launch
    float m_fResetableTimer;

	public Vector2 CharControlForce = new Vector2(400, 200);

    // when the player's velocity becomes lower than this
    // the seesaw is moved to that location if they have enough health
    public Vector2 ResetVelocityThreshold = new Vector2(15.0f, 5.0f);

    public Vector3 AdditionalForceOnLaunch = new Vector2(0, 0);

    Vector3 m_vOrigPos;

    Quaternion m_vOrigRot;

    GameObject m_DiedTxt;

    Trampoline m_HitTramp;

    WallJump m_WallJump;

    void Start()
    {
        m_fApplyJumpTimer = 0.0f;
        m_fBoostTimer = m_fResetableTimer = m_fAddForceTimer = 0.0f;
		m_nFlags = 0;
    }

    void Update()
    {
        if (Game.Instance.LaunchStarted)
        {
            m_fResetableTimer -= Time.deltaTime;
            m_fAddForceTimer -= Time.deltaTime;
            m_fBoostTimer -= Time.deltaTime;

            if (Utilities.Instance.BitTest(m_nFlags, (int)ePlayerFlags.PF_WALL_JUMP_STARTED) )
                m_fWallJumpTimer -= Time.deltaTime;

            if (m_fWallJumpTimer < 0.0f && m_fApplyJumpTimer > 0.0f)
                m_fApplyJumpTimer -= Time.deltaTime;
        } 
        else
        {
        }
    }

    void FixedUpdate()
    {
        if (Utilities.Instance.BitTest(m_nFlags, (int)ePlayerFlags.PF_DIED) /*m_bHitSpikes*/ ||
		    Game.Instance.CurrLevel.PauseMenuActive ||
		    Game.Instance.CurrLevel.Resetting)
        {
            // TODO:: develop to work with ragdoll, etc
            rigidbody.Sleep();
        }
        
            //Debug.Log("Player Vel:" + rigidbody.velocity.ToString());
// 	        if (Input.GetButton("Character Control Up"))
// 	        {
// 	            rigidbody.AddForce(m_vDefaultForceCharControl.x * 0.0f, m_vDefaultForceCharControl.y, 0.0f);
// 	        }
// 	        else
//             if (Input.GetButton("Character Control Down"))
// 	        {
// 	            rigidbody.AddForce(m_vDefaultForceCharControl.x * 0.0f, -m_vDefaultForceCharControl.y, 0.0f);
// 	        }
// 	        else 
        if (Game.Instance.LaunchStarted)
        {
			
	        if (Utilities.Instance.BitTest(m_nFlags, (int)ePlayerFlags.PF_WALL_JUMP_STARTED) )
	        {
	            // don't begin the jump off of the wall until the timer is up
	            if (m_fApplyJumpTimer > 0.0f)
	            {
		            if (m_fWallJumpTimer < 0.0f)
		            {
	                    //Debug.Log("Applying wall jump force");
			            Vector3 dir = new Vector3(m_WallJump.WallJumpDir * WallJumpXDirection, WallJumpYDirection, 0.0f);
	                    rigidbody.AddForce(dir * WallJumpForce);
		            }
	            } 
	            else
	            {
	                OnWallJumpEnded();
	            }
	        }
#if UNITY_IPHONE
			// use accelerometer, to avoid conflicts with accelerometer movement and physics-based wall jump,
			// only one or the other can be performed at a time
			else if (!Game.Instance.Options.IsOptionActive(Options.eOptions.OPT_USE_ARROWS))
			{
				AccelerometerInput.AccelInfo ai = Game.Instance.AccelInput.GetAccelInfo();
				
				//if (ai.ValidXMovement)
				{					
					// move left/right depending on accelerometer x movement					
	                float x = ai.CurrXMoveAmt * AccelerometerScalar;
                    if (x > MaxXVel)
					    x = MaxXVel;
                    else if (x < -MaxXVel)
                        x = -MaxXVel;
					
					Quaternion rot;
					// turn to facing right    
					if (ai.CurrXMoveAmt > 0.0f)
					{
						rot = Quaternion.LookRotation(Vector3.right);
					}
					// turn to facing left
					else
					{
						rot = Quaternion.LookRotation(-Vector3.right);
					}
					
					transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);	
					rigidbody.velocity = new Vector3(x, rigidbody.velocity.y, 0.0f);
				}
			}
			// use buttons	 
			if (Game.Instance.Options.IsOptionActive(Options.eOptions.OPT_USE_ARROWS))
			{			
				if (Game.Instance.MI.BtnDown(MobileInput.BTN_RIGHT))
#else
	        if (Input.GetButton("Character Control Right"))
#endif
		        {
		            rigidbody.AddForce(CharControlForce.x, 0.0f, 0.0f);
					// turn to facing right
					Quaternion rot = Quaternion.LookRotation(Vector3.right);
					transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
		        }
#if UNITY_IPHONE
			else if (Game.Instance.MI.BtnDown(MobileInput.BTN_LEFT))
#else
	        	else if (Input.GetButton("Character Control Left"))
#endif
				{
		            rigidbody.AddForce(-CharControlForce.x, 0.0f, 0.0f);
					// turn to facing left
					Quaternion rot = Quaternion.LookRotation(-Vector3.right);
					transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
	            }
#if UNITY_IPHONE
			}
#endif
        }
		
        if (m_HitTramp != null && m_HitTramp.ContinueToApplyForce())
        {
            //Debug.Log("Tramp Force:" + m_HitTramp.Force.ToString());
            rigidbody.AddForce(m_HitTramp.Force);
        }

        if (m_fAddForceTimer > 0.0f)
            rigidbody.AddForce(AdditionalForceOnLaunch);
#if UNITY_IPHONE
		if (Utilities.Instance.BitTest(m_nFlags, (int)ePlayerFlags.PF_BOOST_VALID) && Game.Instance.MI.BtnPressed(MobileInput.BTN_A))
#else
        if (Utilities.Instance.BitTest(m_nFlags, (int)ePlayerFlags.PF_BOOST_VALID) && Input.GetButtonDown("Action Btn 1"))
#endif
			m_fBoostTimer = BoostDuration;
        if (m_fBoostTimer > 0.0f)
            rigidbody.AddForce(Vector3.up * BoostForce);
    }

    void OnGUI()
    {
        if (Application.loadedLevel > 0 && Application.loadedLevel <= Game.Instance.NumLevelsInWorld)
        {
            GUI.Label(new Rect(10, 5, 100, 40), Game.Instance.PlayerInfo.Lives.ToString() + " Lives"/*, m_GUIStyle*/);
        }
    }

    public void OnLaunchStarted()
    {
        m_fAddForceTimer = AdditionalForceTime;
        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX;
        rigidbody.freezeRotation = true;
    }

    /// <summary>
    /// Trampoline
    /// </summary>
    /// <param name="_tramp"></param>
    public void OnTrampEnter(Trampoline _tramp)
    {
        m_HitTramp = _tramp;
		Utilities.Instance.BitOn(ref m_nFlags, (int)ePlayerFlags.PF_BOOST_VALID);
    }
    public void OnTrampExit()
    {
        //Debug.Log("Exiting tramp");
        m_HitTramp = null;
		Utilities.Instance.BitOff(ref m_nFlags, (int)ePlayerFlags.PF_BOOST_VALID);
    }

    /// <summary>
    /// Wall Jump
    /// </summary>
    /// <param name="_wallJump"></param>
    public void OnWallJumpStart(WallJump _wallJump)
    {
        //Debug.Log("Wall Jump Started");

        // the player has pressed the button to wall jump at the correct time,
        // sleep the player for WallJumpDelay seconds, then apply force
        m_WallJump = _wallJump;

		Utilities.Instance.BitOn(ref m_nFlags, (int)ePlayerFlags.PF_WALL_JUMP_STARTED);

        m_fApplyJumpTimer = WallJumpForceDuration;
        m_fWallJumpTimer = WallJumpDelay;

        rigidbody.Sleep();
        rigidbody.velocity = Vector3.zero;
    }

    public void OnWallJumpEnded()
    {
        //Debug.Log("Wall Jump Ended");

		Utilities.Instance.BitOff(ref m_nFlags, (int)ePlayerFlags.PF_WALL_JUMP_STARTED);
        m_fWallJumpTimer = m_fApplyJumpTimer = 0.0f;
        m_WallJump = null;
    }

    /// <summary>
    /// DEATH
    /// </summary>
    public void OnHitSpikes()
    {
		Utilities.Instance.BitOn(ref m_nFlags, (int)ePlayerFlags.PF_HIT_SPIKES);
		Utilities.Instance.BitOn(ref m_nFlags, (int)ePlayerFlags.PF_DIED);
        m_DiedTxt.renderer.enabled = true;
		m_DiedTxt.transform.position = transform.position;
    }
    public void OnHitWater()
    {
		Utilities.Instance.BitOn(ref m_nFlags, (int)ePlayerFlags.PF_HIT_WATER);
		Utilities.Instance.BitOn(ref m_nFlags, (int)ePlayerFlags.PF_DIED);
		
        m_DiedTxt.renderer.enabled = true;
		m_DiedTxt.transform.position = transform.position;
    }
	public void OnRopeGrabbed()
	{
		//Debug.Log("OnRopeGrabbed");
		Utilities.Instance.BitOn(ref m_nFlags, (int)ePlayerFlags.PF_ROPE_GRABBED);			
	}
	public void OnRopeReleased()
	{
		//Debug.Log("OnRopeReleased");
		Utilities.Instance.BitOff(ref m_nFlags, (int)ePlayerFlags.PF_ROPE_GRABBED);			
	}

    /// <summary>
    /// Init
    /// </summary>
    public void Init()
    {
        rigidbody.isKinematic = true;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;

        m_vOrigRot = rigidbody.rotation;
        //m_vOrigPos = rigidbody.position;

        m_DiedTxt = GameObject.Find("OnDeath");
        m_DiedTxt.renderer.enabled = false;

        OnReset();
    }
    public void OnReset()
    {
        if (hingeJoint != null)
            Destroy(hingeJoint);
        m_vOrigPos = Game.Instance.CurrLevel.GetPlayerPlacement();
        m_fAddForceTimer = m_fApplyJumpTimer = 0.0f;
        m_fResetableTimer = 1.0f;
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        rigidbody.isKinematic = true;
        //rigidbody.Sleep();
        //rigidbody.velocity = Vector3.zero;
		
        m_DiedTxt.renderer.enabled = false;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;
        if (m_WallJump != null)
            m_WallJump.OnReset();
        m_HitTramp = null;
        m_WallJump = null;
		
		m_nFlags = 0;
    }

    /// <summary>
    /// ACCESSORS/MUTATORS
    /// </summary>
    /// 
    public bool BoostValid
    {
        get { return Utilities.Instance.BitTest(m_nFlags, (int)ePlayerFlags.PF_BOOST_VALID); }
        //set { Utilities.Instance.	m_bBoostValid = value; }
    }
    public bool WallJumpStarted
    {
        get { return Utilities.Instance.BitTest(m_nFlags, (int)ePlayerFlags.PF_WALL_JUMP_STARTED); }
        //set { m_bWallJumpStarted = value; }
    }
	public bool GetRopeGrabbed()
	{
		return Utilities.Instance.BitTest(m_nFlags, (int)ePlayerFlags.PF_ROPE_GRABBED);
	}
}