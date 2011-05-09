using UnityEngine;
using System.Collections;

public class Trajectory : MonoBehaviour
{
    public int NumSamples;  // how many points we will calculate & draw the trajectory at

    public float TimeInterval;  // how often to take a sample

    public GameObject TrajObj;

    /// <summary>
    /// x = vt
    /// y = vt - (0.5 * g * t^2)
    /// </summary>

    float m_fGravity;

    public float m_fVelX, m_fVelY;

    GameObject[] m_Trajectory;

    GameObject m_Player;

    void Start()
    {
        m_fGravity = -Physics.gravity.y;
        m_Player = GameObject.Find("Player");
        m_Trajectory = new GameObject[NumSamples];
        for (int i = 0; i < NumSamples; ++i)
            m_Trajectory[i] = (GameObject)Instantiate(TrajObj);

        m_fVelY = _CalculateVelY();
        m_fVelX = _CalculateVelX();
    }

    void Update()
    {
        if (!Game.Instance.WeightDropped)
        {
            m_fVelX = _CalculateVelX();
            m_fVelY = _CalculateVelY();

            float x, y, t;
	        for (int i = 0; i < NumSamples; ++i)
	        {
	            t = (((float)i+1) * TimeInterval);
                x = m_fVelX * t;
                y = m_fVelY * t - (0.5f * m_fGravity * t * t);
	            m_Trajectory[i].transform.position = new Vector3(x, y, 0.0f);
	            m_Trajectory[i].transform.position += m_Player.transform.position;
	        }
        }
    }

    private float _CalculateVelY()
    {
        GameObject weight = GameObject.Find("Weight");
        GameObject board  = GameObject.Find("Board");

        // first calculate the distance between the weight and the board
        float d = weight.transform.position.y - board.transform.position.y; // assumes weight is always above board
        d -= (weight.collider.bounds.size.y * 0.5f + board.collider.bounds.size.y * 0.5f);

        // calculate time at which the collision between weight & board occurs
        float t = Mathf.Sqrt(d / (0.5f * m_fGravity));

        // calculate velocity of the weight at time t
        float vW = m_fGravity * t;

        // calculate board's velocity after collision
        float vB = ((2.0f * weight.rigidbody.mass) / (weight.rigidbody.mass + board.rigidbody.mass)) * vW;

        // finally, calculate the player's velocity after the board collides with it
        return ((2.0f * board.rigidbody.mass) / (m_Player.rigidbody.mass + board.rigidbody.mass)) * vB;
    }

    private float _CalculateVelX()
    {
        GameObject board = GameObject.Find("Board");
        GameObject wedge = GameObject.Find("Wedge");
        GameObject ground= GameObject.Find("Ground");

        // calculate distance of player from the center (hinge), this is the radius
        float r = Mathf.Abs(m_Player.transform.position.x - wedge.transform.position.x);

        // using right triangle to determine angle between board start position & 
        //  when it contacts the ground:
        // tan A = a/b
        // theta = arctan(tan A)

        // get the distance from the board to the ground
        float a = Mathf.Abs((board.transform.position.y - board.collider.bounds.size.y * 0.5f) - ground.collider.bounds.max.y);

        // get the distance from far end of board to center (hinge)
        float b = board.collider.bounds.size.x * 0.5f;

        // get the angle
        float theta = Mathf.Atan(a / b);

        // calculate distance the player will travel (before separation from board)
        // this is known as Arc Length
        float L = theta * r;

        // calculate angular displacement
        float ad = L / r;

        // calculate the time at which separation will occur
        float t = Mathf.Sqrt(a / (0.5f * m_fGravity));
        
        // calculate the angular velocity at time t
        float w = ad / t;

        return r * w;
    }
}