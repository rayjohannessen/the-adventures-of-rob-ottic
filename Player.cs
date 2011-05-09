using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    bool m_bHitSpikes, m_bHitWater;
    bool m_bDied;
    bool m_bBoostValid;
    bool m_bWallJumpStarted;

    public float AdditionalForceTime = 1.0f;
    public float ClimbForce = 50.0f;
    public float BoostForce = 300.0f;
    public float BoostDuration = 0.5f;
    public float HandOffset = 3.0f;

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

    public Vector2 DefaultForceCharControl = new Vector2(400, 200);

    // when the player's velocity becomes lower than this
    // the seesaw is moved to that location if they have enough health
    public Vector2 ResetVelocityThreshold = new Vector2(15.0f, 5.0f);

    public Vector3 AdditionalForceOnLaunch = new Vector2(0, 0);

    Vector3 m_vOrigPos;

    Quaternion m_vOrigRot;

    GUIText m_DiedTxt;

    Trampoline_OneWay m_HitTramp;

    WallJump m_WallJump;

    void Start()
    {
        m_fApplyJumpTimer = 0.0f;
        m_fBoostTimer = m_fResetableTimer = m_fAddForceTimer = 0.0f;
        m_bBoostValid = m_bWallJumpStarted = false;
    }

    void Update()
    {
        if (Game.Instance.LaunchStarted)
        {
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
#if UNITY_IPHONE
			// use accelerometer
			if (!Game.Instance.Options.IsOptionActive(Options.eOptions.OPT_USE_ARROWS))
			{
				if (Game.Instance.AccelInput.HasValidXMovement())
				{					
					// move left/right depending on accelerometer x movement,
					// we want lower values to increase velocity more quickly
					// than would higher values
					float x = Mathf.Sqrt(Mathf.Abs(Game.Instance.AccelInput.XMovement * 0.75f));
					      
					Quaternion rot;
					// turn to facing right    
					if (Game.Instance.AccelInput.XMovement > 0.0f)
					{
						rot = Quaternion.LookRotation(Vector3.right);
					}
					// turn to facing left
					else
					{
						rot = Quaternion.LookRotation(-Vector3.right);	
						x = -x;
					}
					
					transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);	
		            rigidbody.AddForce(DefaultForceCharControl.x * x, 0.0f, 0.0f);
				}
			}
			// use buttons	 
			else
			{			
				if (Game.Instance.MI.BtnDown(MobileInput.BTN_RIGHT))
#else
            if (Input.GetButton("Character Control Right"))
#endif
		        {
		            rigidbody.AddForce(DefaultForceCharControl.x, 0.0f, 0.0f);
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
		            rigidbody.AddForce(-DefaultForceCharControl.x, 0.0f, 0.0f);
					// turn to facing left
					Quaternion rot = Quaternion.LookRotation(-Vector3.right);
					transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime);
	            }
			
//             if (m_fResetableTimer < 0.0f && !m_bHealthDecremented && !m_bDied &&
//                 Mathf.Abs(rigidbody.velocity.x) < ResetVelocityThreshold.x && 
//                 Mathf.Abs(rigidbody.velocity.y) < ResetVelocityThreshold.y)
//             {
//                 m_bHealthDecremented = true;
//                 rigidbody.Sleep();
//                 // TODO:: check if the see-saw can be moved to new location, or if it's a complete restart
//                 Game.Instance.PlayerInfo.Health--;
//                 if (Game.Instance.PlayerInfo.Health > 0)
//                 {
//                     Game.Instance.CurrLevel.ResetLevel(false, true);
//                 }
//                 else
//                 {
//                     Game.Instance.OnCharacterDied();
//                 }
//             }
#if UNITY_IPHONE
			}
#endif
            m_fResetableTimer -= Time.deltaTime;
            m_fAddForceTimer -= Time.deltaTime;
            m_fBoostTimer -= Time.deltaTime;

            if (m_bWallJumpStarted)
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
        if (m_HitTramp != null && m_HitTramp.ContinueToApplyForce())
        {
            //Debug.Log("Tramp Force:" + m_HitTramp.Force.ToString());
            rigidbody.AddForce(m_HitTramp.Force);
        }
        if (m_bWallJumpStarted)
        {
            // don't begin the jump off of the wall until the timer is up
            if (m_fApplyJumpTimer > 0.0f)
            {
	            if (m_fWallJumpTimer < 0.0f)
	            {
                    //Debug.Log("Applying wall jump force");
		            Vector3 dir = new Vector3(m_WallJump.JumpDirection * WallJumpXDirection, WallJumpYDirection, 0.0f);
                    rigidbody.AddForce(dir * WallJumpForce);
	            }
            } 
            else
            {
                OnWallJumpEnded();
            }
        }
        if (m_bDied/*m_bHitSpikes*/)
        {
            // TODO:: develop to work with ragdoll, etc
            rigidbody.Sleep();
        }


        if (m_fAddForceTimer > 0.0f)
            rigidbody.AddForce(AdditionalForceOnLaunch);
#if UNITY_IPHONE
		if (m_bBoostValid && Game.Instance.MI.BtnPressed(MobileInput.BTN_A))
#else
        if (m_bBoostValid && Input.GetButtonDown("Action Btn 1"))
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
    public void OnTrampEnter(Trampoline_OneWay _tramp)
    {
        m_HitTramp = _tramp;
        m_bBoostValid = true;
    }
    public void OnTrampExit()
    {
        //Debug.Log("Exiting tramp");
        m_HitTramp = null;
        m_bBoostValid = false;
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

        m_bWallJumpStarted = true;

        m_fApplyJumpTimer = WallJumpForceDuration;
        m_fWallJumpTimer = WallJumpDelay;

        rigidbody.Sleep();
        rigidbody.velocity = Vector3.zero;
    }

    public void OnWallJumpEnded()
    {
        //Debug.Log("Wall Jump Ended");

        m_bWallJumpStarted = false;
        m_fWallJumpTimer = m_fApplyJumpTimer = 0.0f;
        m_WallJump = null;
    }

    /// <summary>
    /// DEATH
    /// </summary>
    public void OnHitSpikes()
    {
        m_bHitSpikes = m_bDied = true;
        m_DiedTxt.enabled = true;
    }
    public void OnHitWater()
    {
        m_bHitWater = m_bDied = true;
        m_DiedTxt.enabled = true;
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

        m_DiedTxt = GameObject.Find("OnDeath").guiText;
        m_DiedTxt.enabled = m_bHitSpikes = m_bHitWater = m_bDied = false;

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
        m_bHitSpikes = m_bHitWater = m_bDied = m_bBoostValid = m_bWallJumpStarted = false;
        m_DiedTxt.enabled = false;
        rigidbody.transform.rotation = m_vOrigRot;
        rigidbody.transform.position = m_vOrigPos;
        if (m_WallJump != null)
            m_WallJump.OnReset();
        m_HitTramp = null;
        m_WallJump = null;
    }

    /// <summary>
    /// ACCESSORS/MUTATORS
    /// </summary>
    /// 
    public bool BoostValid
    {
        get { return m_bBoostValid; }
        set { m_bBoostValid = value; }
    }
    public bool WallJumpStarted
    {
        get { return m_bWallJumpStarted; }
        set { m_bWallJumpStarted = value; }
    }
}