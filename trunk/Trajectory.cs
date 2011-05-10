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

    public Vector2 m_vVel;

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

        m_vVel = _CalculateFinalVelocity();
    }

    void Update()
    {
        if (!Game.Instance.WeightDropped)
        {
            m_vVel = _CalculateFinalVelocity();

            float x, y, t; int i = 0;
	        foreach (GameObject traj in m_Trajectory)
	        {
	            t = (((float)++i) * TimeInterval);
                x = m_vVel.x * t;
                y = m_vVel.y * t - (0.5f * m_fGravity * t * t);
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

    private Vector2 _CalculateFinalVelocity()
    {
        GameObject weight = GameObject.Find("Weight");
        GameObject board = GameObject.Find("Board");
        GameObject wedge = GameObject.Find("Wedge");

        // first calculate the distance between the weight and the board
        float d = weight.transform.position.y - board.transform.position.y; // assumes weight is always above board
        d -= (weight.collider.bounds.size.y * 0.5f + board.collider.bounds.size.y * 0.5f);  // account for height of each

        // calculate time at which the collision between weight & board occurs
        float t = Mathf.Sqrt(d / (0.5f * m_fGravity));

        // calculate velocity of the weight at time t
        float vW = m_fGravity * t;

        // calculate board's velocity after collision
        float vB = ((2.0f * weight.rigidbody.mass) / (weight.rigidbody.mass + board.rigidbody.mass)) * vW;

        // calculate the player's velocity after the board collides with it
        float v = ((2.0f * board.rigidbody.mass) / (m_Player.rigidbody.mass + board.rigidbody.mass)) * vB;

        // Find collision point of board/ground
        // 1. need height & radius
        float h = wedge.collider.bounds.size.y;
        float Rb = board.collider.bounds.max.x - wedge.collider.bounds.center.x;

        // 2. find the angle between the current board position and where it will be
        float theta = Mathf.Asin(h / Rb) /** 57.2957795f*/;

        // 3. find the distance between the center.x & the point.x on the circle where the board/ground meet
        float x = Rb * Mathf.Cos(theta);

        // 4. get the actual position
        Vector3 collisionPt = new Vector3(wedge.collider.bounds.center.x, wedge.collider.bounds.min.y, wedge.collider.bounds.center.z) + Vector3.right * x;

        // calculate distance of player from the center (hinge), this is the radius
        float Rp = Mathf.Abs(m_Player.transform.position.x - wedge.transform.position.x);

        // TODO: get vector perp to board at the time when the board collides with the ground,
        //       this is the direction of the player's velocity
        Vector3 velDir = (collisionPt - ((board.collider.bounds.max + board.collider.bounds.min) * 0.5f)).normalized;
        velDir = Vector3.Cross(Vector3.forward, velDir);

        // GET ANGULAR VELOCITY
        // 1. calculate distance the player will travel (before separation from board)
        // this is the Arc Length
        float L = theta * Rp;

        // 2. calculate angular displacement
        float ad = L / Rp;

        // 3. calculate the time at which separation will occur
        t = Mathf.Sqrt(L / (0.5f * m_fGravity));

        // 4. calculate the angular velocity at time t
        float w = ad / t;

        // convert angular velocity to regular velocity
        v = Rp * w;

        return velDir * v;
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