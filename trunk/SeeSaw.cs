using UnityEngine;
using System.Collections;

public class SeeSaw : MonoBehaviour
{
    GameObject m_WeightObject;
    GameObject m_WedgeObject;
    GameObject m_BoardObject;
    GameObject m_LaunchTrigger;

    // how the player should be offset from the board's center point
    public Vector3 m_PlayerPlacementOffset;
    // the launch trigger's offset from the center of the board
    public Vector3 m_LaunchTriggerOffset;

    void Start()
    {
    }

    void Update()
    {
    }

    public void OnReset()
    {
        m_WeightObject.GetComponent<Weight>().OnReset();
        m_WedgeObject.GetComponent<Wedge>().OnReset();
        m_BoardObject.GetComponent<Board>().OnReset();
    }
    public void OnResetToNewCheckpoint(Vector3 _pos)
    {
        transform.position = new Vector3(_pos.x, transform.position.y, _pos.z);

        m_WedgeObject.GetComponent<Wedge>().OnResetToNewCheckpoint(_pos);
        m_BoardObject.GetComponent<Board>().OnResetToNewCheckpoint(_pos);
        m_WeightObject.GetComponent<Weight>().OnResetToNewCheckpoint(_pos);
        m_LaunchTrigger.transform.position = _pos + m_LaunchTriggerOffset;
    }
    public void OnWeightDropped()
    {
        if (m_WeightObject)
            m_WeightObject.GetComponent<Weight>().OnWeightDropped();
    }


    public void Init()
    {
        m_WeightObject = GameObject.Find("Weight");
        m_WedgeObject = GameObject.Find("Wedge");
        m_BoardObject = GameObject.Find("Board");
        m_LaunchTrigger = GameObject.Find("LaunchTrigger");
    }

    /// <summary>
    /// ACCESSORS/MUTATORS
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerPosition()
    {
        return (m_BoardObject.transform.position + m_PlayerPlacementOffset);
    }

    public GameObject BoardObject
    {
        get { return m_BoardObject; }
        set { m_BoardObject = value; }
    }
    public GameObject WedgeObject
    {
        get { return m_WedgeObject; }
        set { m_WedgeObject = value; }
    }
    public GameObject WeightObject
    {
        get { return m_WeightObject; }
        set { m_WeightObject = value; }
    }
}