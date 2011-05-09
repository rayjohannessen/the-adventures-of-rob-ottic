using UnityEngine;
using System.Collections;

public class BasicObjAnimation : MonoBehaviour
{
    /// <summary>
    /// Forward means in the direction specified
    /// </summary>
    public float MoveAmountForward = 1.0f;
    /// <summary>
    /// Backwards means the opposite of the direction specified
    /// </summary>
    public float MoveAmountBackward = -1.0f;
    public float ForwardSpeed = 1.0f;
    public float BackwardSpeed = 1.0f;

    bool m_bCurrDirection;  // true == forward
    float m_fCurrMoveAmt;

    public Vector3 MoveDirection;
    Vector3 m_vStartPos;

    void Start()
    {
        m_fCurrMoveAmt = 0.0f;
        m_bCurrDirection = true;    // true == forward
        m_vStartPos = transform.position;
    }

    void Update()
    {
        if (m_bCurrDirection)
        {
            m_fCurrMoveAmt += ForwardSpeed * Time.deltaTime;

            if (m_fCurrMoveAmt > MoveAmountForward)
            {
                m_fCurrMoveAmt = MoveAmountForward;
                m_bCurrDirection = false;
            }
        }
        else
        {
            m_fCurrMoveAmt -= BackwardSpeed * Time.deltaTime;

            if (m_fCurrMoveAmt < MoveAmountBackward)
            {
                m_fCurrMoveAmt = MoveAmountBackward;
                m_bCurrDirection = true;
            }
        }

        // set the position based on how much it has moved from the start
        transform.position = m_vStartPos + MoveDirection * m_fCurrMoveAmt;
    }
}