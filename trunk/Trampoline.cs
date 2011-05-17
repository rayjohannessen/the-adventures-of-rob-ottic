using UnityEngine;
using System.Collections;

public class Trampoline : MonoBehaviour
{
    Player m_Player;
 	
	Vector3 m_vForce;

    // this establishes a maximum end force (after collision and the rebound is being calculated)
    // where anything below it will result in the player not bouncing back at all
    public float RestingThresholdForce = 400.0f;
    // to keep the player from continuously bouncing to get higher and higher, cap the y force
    // public float MaxYForce = 2000.0f;
    // each collision with the pad reduces speed slightly, lower values reduce more
    public float Friction = 0.85f;
    // how "strong" the bounce pad is, directly influencing how fast the player
    // is bounced away from the pad
    public float TensionConstant = 90.0f;
    // to keep the bounce animation in sync with the velocity of the player
    // a cap is set for the amount of player velocity that can increase the anim speed
    public float MaxAnimInfluenceOfVelocity = 1350.0f;   
    // low/high are the values to lerp between, the delta being the sum of velocities / MaxAnimInfluenceOfVelocity
    public float LowAnimMultiplier = 0.75f, HighAnimMultiplier = 1.75f;
	public float ColliderOffset = 0.055f;

    bool m_bPlayerInTrigger = false;
    bool m_bGoThrough = false;
    public bool TwoSided = false;   // if true, the player will bounce off when contacting either side

    ////////////////////////////////////////////////////////////////////

    void Start()
    {
        m_Player = GameObject.Find("Player").GetComponent<Player>();
    }

    void OnTriggerEnter(Collider info)
    {
        m_bPlayerInTrigger = true;
		
        Vector3 vDir = transform.up;
		Debug.Log("vDir=" + vDir.ToString());
		
		// determine the direction of entrance
		Vector3 vPadToTarget = info.transform.position - transform.position;
		float dot = Vector3.Dot(vPadToTarget, transform.up);

        // if two-sided & entered through bottom, bounce direction is opposite
        if (dot < 0.0f && TwoSided)
        {
            vDir = -vDir;
			// need to place the collider above the pad
            transform.Find("PadCollider").position = transform.position + transform.up * ColliderOffset;			
        }
		// one-sided & coming from back side, so the character passes through
		else if (dot < 0.0f)
		{
			m_bGoThrough = true;
            transform.Find("PadCollider").collider.isTrigger = true;	
			// we're done
			return;
		}
		// if none of the above checks are hit, character is approaching from above pad
		else
		{
			// need to place the collider below the pad			
            transform.Find("PadCollider").position = transform.position + transform.up * -ColliderOffset;
		}

        Vector3 vVel = m_Player.rigidbody.velocity;
        float velX = Mathf.Abs(vVel.x) * Friction * vDir.x * TensionConstant;
        float velY = Mathf.Abs(vVel.y) * Friction * vDir.y * TensionConstant;

        // clamp Y low
        if (Mathf.Abs(velY) < RestingThresholdForce * vDir.y)
            velY = RestingThresholdForce * vDir.y;

        // only play anim if bounce was high enough
        if (Mathf.Abs(velX) > 0.0f || Mathf.Abs(velY) > 0.0f)
        {
	        // TODO:: maybe play at a speed corresponding to the player's velocity??
	        animation["Take 001"].speed = Mathf.Lerp(LowAnimMultiplier, HighAnimMultiplier, (velX + velY) / MaxAnimInfluenceOfVelocity);
//             Debug.Log("Anim Speed:" + m_BounceAnim["Take 001"].speed.ToString());
	        animation.Play();
        }

        Force = new Vector3(velX, velY, 0.0f);

        m_Player.OnTrampEnter(this);
    }

    void OnTriggerStay(Collider info)
    {
        // only care about staying if this tramp is one-sided and they're bouncing off the top, or
        if (info.gameObject.name == "Player" && !m_bGoThrough )
        {
            m_bPlayerInTrigger = true;
			
            if (m_Player.rigidbody.velocity.y == 0.0f)
            {
                //Debug.Log("Vel == 0.0");
                m_Player.OnTrampEnter(this);
            }
        }
    }
	
    void OnTriggerExit(Collider info)
    {
        if (info.gameObject.name == "Player")
        {
            //Debug.Log("On Trigger exit TOP");

            m_bPlayerInTrigger = m_bGoThrough = false;
            m_Player.OnTrampExit();

            // the collider always gets re-enabled on exit
            transform.Find("PadCollider").collider.isTrigger = false;
        }
    }

    public bool ContinueToApplyForce()
    {
        return m_bPlayerInTrigger;
    }

    public bool PlayerInTrigger
    {
        get { return m_bPlayerInTrigger; }
    }
    public Vector3 Force
    {
        get { return m_vForce; }
        set { m_vForce = value; }
    }
}