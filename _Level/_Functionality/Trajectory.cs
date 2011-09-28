using UnityEngine;
using System.Collections;

public class Trajectory : MonoBehaviour
{
    public int NumSamples;  // how many points we will calculate & draw the trajectory at

    public float TimeInterval;  // how often to take a sample
    public float DestroyDelay = 4.0f;  // after the launch is started, wait this long to destroy the trajectory objects
    float m_fDestroyTimer;

    public GameObject TrajObj;

    /// <summary>
    /// x = vt
    /// y = vt - (0.5 * g * t^2)
    /// </summary>

    float m_fGravity;

    public float m_fVel;
	public float m_fInitialTime;
	public float m_fThetaPlayer;

    GameObject[] m_Trajectory;

    GameObject m_Player;

    void Start()
    {
        m_fDestroyTimer = 0.0f;
        m_fGravity = -Physics.gravity.y;
        m_Player = GameObject.Find("Player");
        m_Trajectory = new GameObject[NumSamples];
        for (int i = 0; i < NumSamples; ++i)
            m_Trajectory[i] = (GameObject)Instantiate(TrajObj);

        //_CalculateFinalVelocity();
    }

    void Update()
    {
        if (!Game.Instance.WeightDropped)
        {
            //_CalculateFinalVelocity();

            float x, y, t; int i = 0;
	        foreach (GameObject traj in m_Trajectory)
	        {
	            t = m_fInitialTime + (((float)++i) * TimeInterval);
                x = m_fVel * Mathf.Cos(m_fThetaPlayer) * t;
                y = m_fVel * Mathf.Sin(m_fThetaPlayer) * t - (0.5f * m_fGravity * t * t);
                traj.transform.position = new Vector3(x, y, 0.0f);
                traj.transform.position += m_Player.transform.position;
	        }
        }
        else if (m_fDestroyTimer > 0.0f)
        {
            m_fDestroyTimer -= Time.deltaTime;
            if (m_fDestroyTimer <= 0.0f)
            {
                m_fDestroyTimer = 0.0f;
                foreach (GameObject traj in m_Trajectory)
                    DestroyImmediate(traj);
                m_Trajectory = null;
            }
        }
    }

    public void OnPlayerLaunched()
    {
        m_fDestroyTimer = DestroyDelay;
    }
    public void OnReset()
    {
        m_Trajectory = new GameObject[NumSamples];
        for (int i = 0; i < NumSamples; ++i)
            m_Trajectory[i] = (GameObject)Instantiate(TrajObj);
    }

    private void _CalculateFinalVelocity()
    {
        GameObject weight = GameObject.Find("Weight");
        GameObject board = GameObject.Find("Board");
        GameObject wedge = GameObject.Find("Wedge");

//        // first calculate the distance between the weight and the board
//        float d = weight.transform.position.y - board.transform.position.y; // assumes weight is always above board
//        d -= (weight.collider.bounds.size.y * 0.5f + board.collider.bounds.size.y * 0.5f);  // account for height of each
//
//        // calculate time at which the collision between weight & board occurs
//        float t = Mathf.Sqrt(d / (0.5f * m_fGravity));
//
//        // calculate velocity of the weight at time t, initial velocity = 0
//        float vW = 0.5f * m_fGravity * t * t;

     	// find force of weight on board
        // Fᵍ = mʷ*aᵍ
		float F = weight.rigidbody.mass * m_fGravity;
		
        // find the torque exerted on board by weight
        // where r is the location of weight from axis
        // Fᵍ = mʷ*r*ɑ, Fᵍ*r = mʷ*r*r*ɑ
        // Τ = Fᵍ*r
		float r = weight.transform.position.x - board.transform.position.x;
		float T = F * r;
		
		Debug.Log("T==" + T.ToString());
		
		// the angular acceleration ɑ
        // where I is the moment of inertia for a board of negligible height
        // for rotation about an axis not through the center of mass
        // where d is the distance from the center of mass
        //  Iͨͫ = 1/12 * M*L*L
        // I = Iͨͫ + mᵇ*d*d
        // Τ = I*ɑ
        // ɑ = Τ/I
		float d = Mathf.Abs(wedge.transform.position.x - board.transform.position.x);
		float I = 0.083333333f * board.rigidbody.mass * board.collider.bounds.size.x * board.collider.bounds.size.x;
		I = I + board.rigidbody.mass * d * d;
		float angularAccel = T / I;
		
		Debug.Log("angularAccel==" + angularAccel.ToString());
          
        // find tangential acceleration at player's location from pivot axis
        // aᵗ = r*ɑ
		float tanAccel = (Mathf.Abs(wedge.transform.position.x - m_Player.transform.position.x)) * angularAccel;
         
		Debug.Log("tanAccel==" + tanAccel.ToString());
		
        // find the Force_Radial pushing against the player
        // Fᵣ = mᵇ*aᵗ		         
        // find when player leaves the board
        // Fᵣ + Fᶰ - Fᵍ = 0         
        // find angle at which he leaves the board, θ:
        // Fᶰ = mᵖ*aᵍ*cosθ, Fᵍ = mᵖ*aᵍ*sinθ, Fᵣ = mᵇ*aᵗ		
        // mᵇ*aᵗ + mᵖ*aᵍ*cosθ - mᵖ*aᵍ*sinθ == 0
        // mᵇ*aᵗ + mᵖ*aᵍ(cosθ - sinθ) = 0
        // (cosθ - sinθ) = -mᵇ*aᵗ / mᵖ*aᵍ
        // (cosθ - sinθ)^2 = (-mᵇ*aᵗ / mᵖ*aᵍ)^2
        // (cosθ)^2 - 2cosθsinθ + (sinθ)^2 = (-mᵇ*aᵗ)^2 / (mᵖ*aᵍ)^2
        // (cosθ)^2 + (sinθ)^2 = 1
        // 1 - 2cosθsinθ = (-mᵇ*aᵗ)^2 / (mᵖ*aᵍ)^2
        // -2cosθsinθ = ((-mᵇ*aᵗ)^2 / (mᵖ*aᵍ)^2) - 1
        // 2cosθsinθ = -(((-mᵇ*aᵗ)^2 / (mᵖ*aᵍ)^2) - 1)
        // sin(2θ) = -(((-mᵇ*aᵗ)^2 / (mᵖ*aᵍ)^2) - 1)
        // 2θ = arcsin(-(((-mᵇ*aᵗ)^2 / (mᵖ*aᵍ)^2) - 1))
        // θ = arcsin(-(((-mᵇ*aᵗ)^2 / (mᵖ*aᵍ)^2) - 1)) / 2
		float b = (-board.rigidbody.mass * tanAccel * -board.rigidbody.mass * tanAccel);
		float p = (m_Player.rigidbody.mass * m_fGravity * m_Player.rigidbody.mass * m_fGravity);
		
		Debug.Log("b==" + b.ToString());
		Debug.Log("p==" + p.ToString());
		Debug.Log("b/p==" + (b/p).ToString());
		
		float thetaBoard = Mathf.Asin(-((b/p) - 1.0f)) * 0.5f;
		
		Debug.Log("thetaBoard==" + thetaBoard.ToString());
		
        // launch angle of player
        m_fThetaPlayer = 90.0f - thetaBoard;
		
		Debug.Log("thetaPlayer==" + m_fThetaPlayer.ToString());
		
        // solve for time t:
        // θ = ωt * 1/2*ɑ*t*t, ω = 0
        // θ = 1/2*ɑ*t*t
        // t = sqrt((2*θ/)/ɑ)
        m_fInitialTime = Mathf.Sqrt( (2.0f * m_fThetaPlayer) / angularAccel);
		
		Debug.Log("initialTime==" + m_fInitialTime.ToString());
		
        // launch velocity
        // v = aᵗ*t
		m_fVel = tanAccel * m_fInitialTime;
		
		Debug.Log("vel==" + m_fVel.ToString());
    }

//     private Vector2 _CalculateWedgeInfluence()
//     {
//         Vector2 ret = new Vector2();
// 
//         GameObject board = GameObject.Find("Board");
//         GameObject wedge = GameObject.Find("Wedge");
//         //GameObject ground= GameObject.Find("Ground");
//         
//         // find the point where the board will contact the ground
// 
//         // 1. need height & radius
//         float h = wedge.collider.bounds.size.y;
//         float Rb = board.collider.bounds.max.x - wedge.collider.bounds.center.x;
//         
//         // 2. find the angle between the current board position and where it will be
//         float theta = Mathf.Asin(h / Rb);
// 
//         // 3. find the distance between the center.x & the point.x on the circle where the board/ground meet
//         //float x = Rb * Mathf.Cos(theta);
// 
//         // calculate distance of player from the center (hinge), this is the radius
//         float Rp = Mathf.Abs(m_Player.transform.position.x - wedge.transform.position.x);
// 
//         // using right triangle to determine angle between board start position & 
//         //  when it contacts the ground:
//         // tan A = a/b
//         // theta = arctan(tan A)
// 
//         // 1. calculate distance the player will travel (before separation from board)
//         // this is the Arc Length
//         float L = theta * Rp;
// 
//         // 2. calculate angular displacement
//         float ad = L / Rp;
// 
//         // 3. calculate the time at which separation will occur
//         float t = Mathf.Sqrt(L / (0.5f * m_fGravity));
//         
//         // 4. calculate the angular velocity at time t
//         float w = ad / t;
// 
//         // 5. convert angular velocity to regular velocity
//         float v = Rp * w;
// 
//         return ret;
//     }

    //     private Vector2 _CalculateWeightInfluence()
    //     {
    //         Vector2 ret = new Vector2();
    // 
    //         GameObject weight = GameObject.Find("Weight");
    //         GameObject board  = GameObject.Find("Board");
    // 
    //         // first calculate the distance between the weight and the board
    //         float d = weight.transform.position.y - board.transform.position.y; // assumes weight is always above board
    //         d -= (weight.collider.bounds.size.y * 0.5f + board.collider.bounds.size.y * 0.5f);
    // 
    //         // calculate time at which the collision between weight & board occurs
    //         float t = Mathf.Sqrt(d / (0.5f * m_fGravity));
    // 
    //         // calculate velocity of the weight at time t
    //         float vW = m_fGravity * t;
    // 
    //         // calculate board's velocity after collision
    //         float vB = ((2.0f * weight.rigidbody.mass) / (weight.rigidbody.mass + board.rigidbody.mass)) * vW;
    // 
    //         // finally, calculate the player's velocity after the board collides with it
    //         float v = ((2.0f * board.rigidbody.mass) / (m_Player.rigidbody.mass + board.rigidbody.mass)) * vB;
    //         
    //         return ret;
    //     }

}