using UnityEngine;
using System.Collections;

public class SmoothLookAtFollow : MonoBehaviour
{
	GameObject m_Player;
		
	Vector3 m_vCurrLA;
	Vector3 m_vTargetLA;
	
    Vector3 m_vZoomedOutPos;
	
	public Vector3 ZoomedInPosOS = new Vector3(10.0f, 10.0f, -45.0f);
	public Vector3 ZoomedInLookAtOS = new Vector3(0.0f, 10.0f, 0.0f);
	
    public float MinZoomedZ = -15.0f;
    public float MaxZoomedZ = -40.0f;
	public float ZoomedOutYOS = 5.0f;
    public float ZoomedOutZDistMultiplier = -0.7759663f;	
    public float FollowDamping = 6.0f;
    public float MoveCamPosSpeed = 5.0f;
	public float MoveCamLASpeed = 2.0f;
	public float MaxTotalXDist = 20.0f;
	public float VelocityInfluenceDamper = 0.5f;
	
	float m_fLeftLevelXExtent = -331.0f;
	float m_fRightLevelXExtent = -50.0f;
	float m_fPanAmountX = 0.0f;
	
	bool m_bZoomedIn = true;

    void LateUpdate()
    {
		if (Game.Instance.CurrLevel.PauseMenuActive)
			return;
		
#if UNITY_IPHONE
        // do we have two touches? zoom in/out if we haven't started the launch yet
        if (!Game.Instance.LaunchStarted)
        {
            if (Game.Instance.MI.TI.CurrTwoTouchOffset != 0.0f)
            {
                float newZ = ZoomedInPosOS.z - Game.Instance.MI.TI.CurrTwoTouchOffset * 0.1f;
    			
                if (newZ > MinZoomedZ)
                    ZoomedInPosOS = new Vector3(ZoomedInPosOS.x, ZoomedInPosOS.y, MinZoomedZ);
                else if (newZ < MaxZoomedZ)
                    ZoomedInPosOS = new Vector3(ZoomedInPosOS.x, ZoomedInPosOS.y, MaxZoomedZ);
                else
                    ZoomedInPosOS = new Vector3(ZoomedInPosOS.x, ZoomedInPosOS.y, newZ);
            
			    Game.Instance.ZoomZ = ZoomedInPosOS.z;
            } 
            else if (Game.Instance.MI.TI.SingleTouchMoveAmt.x != 0.0f)
            {
				float moveAmt = Game.Instance.MI.TI.SingleTouchMoveAmt.x;
				if (transform.position.x + moveAmt < m_fRightLevelXExtent &&
				    transform.position.x + moveAmt > m_fLeftLevelXExtent)
						m_fPanAmountX += moveAmt * 0.5f;
            }
		}
#endif

        if (m_bZoomedIn)    // zoomed in or currently zooming in
        {
			// determine the influence from the character's velocity			
			float velInfluence = m_Player.rigidbody.velocity.x * VelocityInfluenceDamper;
						
			// clamp												
			if (velInfluence < -MaxTotalXDist)
				velInfluence = -MaxTotalXDist;
			else if (velInfluence > MaxTotalXDist)
				velInfluence = MaxTotalXDist;
			
			// get the new target look at and position
			Vector3 newLookAt = m_Player.transform.position + new Vector3(velInfluence + m_fPanAmountX, ZoomedInLookAtOS.y, 0.0f);
			Vector3 newPos = m_Player.transform.position + new Vector3(velInfluence + m_fPanAmountX, ZoomedInPosOS.y, ZoomedInPosOS.z);
			
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * MoveCamPosSpeed);
			m_vCurrLA = Vector3.Lerp(m_vCurrLA, newLookAt, Time.deltaTime * MoveCamLASpeed);
        } 
        else   // zoomed out or currently zooming out
        {
            transform.position = Vector3.Lerp(transform.position, m_vZoomedOutPos, Time.deltaTime * MoveCamPosSpeed);	
			m_vCurrLA = Vector3.Lerp(m_vCurrLA, m_vTargetLA, Time.deltaTime * MoveCamLASpeed);		
        }
		transform.LookAt(m_vCurrLA);
    }

    // need to do this later to make sure all objects are initialized first
    public void LateStart()
    {
		m_Player = GameObject.Find("Player");
		
        // Make the rigid body not change rotation
        if (rigidbody)
            rigidbody.freezeRotation = true;

        // setup the 2 modes' transforms:
        SetNewZoomedOutVars();
				
        // start zoomed in?
        transform.position = m_Player.transform.position + new Vector3(m_Player.transform.position.x, ZoomedInPosOS.y, ZoomedInPosOS.z);
		m_fPanAmountX = 0.0f;
		
		// TODO:: get extents for panning:
    }
	
    public void SetNewZoomedOutVars()
    {
        // calculate how far back we need to be to have the see-saw
        // and the goal in view
        Level lvl = GameObject.Find("Level").GetComponent<Level>();
        float zOffset = lvl.GetDistBoardToTrigger() * ZoomedOutZDistMultiplier;

        m_vZoomedOutPos = lvl.GetLevelCenterPt() + Vector3.forward * zOffset;
        m_vZoomedOutPos = new Vector3(m_vZoomedOutPos.x, ZoomedInPosOS.y + ZoomedOutYOS, m_vZoomedOutPos.z);
		m_vTargetLA = m_vCurrLA = lvl.GetLevelCenterPt();
    }

    public void ToggleZoom()
    {
        m_bZoomedIn = !m_bZoomedIn;
		
		if (!m_bZoomedIn)
			m_vTargetLA = GameObject.Find("Level").GetComponent<Level>().GetLevelCenterPt();
    }
    
    public void OnReset()
    {
		m_fPanAmountX = 0.0f;
		if (!m_bZoomedIn)
			m_vTargetLA = GameObject.Find("Level").GetComponent<Level>().GetLevelCenterPt();
    }
	
	public float PanAmountX 
	{
		get { return this.m_fPanAmountX; }
		set { m_fPanAmountX = value; }
	}
}