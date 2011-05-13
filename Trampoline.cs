﻿using UnityEngine;
using System.Collections;

public class Trampoline : MonoBehaviour
{
    Animation m_BounceAnim;
    Player m_Player;
    Vector3 m_vForce;

    // this establishes a maximum end force (after collision and the rebound is being calculated)
    // where anything below it will result in the player not bouncing back at all
    public float RestingThresholdForce = 400.0f;
    // to keep the player from continuously bouncing to get higher and higher, cap the y force
    public float MaxYForce = 2000.0f;
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

    bool m_bPlayerInTrigger = false;
    bool m_bEnteredTop = false;
    bool m_bEnteredBottom = false;
    public bool TwoSided = false;   // if true, the player will bounce off when contacting either side

    ////////////////////////////////////////////////////////////////////

    void Start()
    {
        m_Player = GameObject.Find("Player").GetComponent<Player>();
        m_BounceAnim = transform.parent.gameObject.animation;
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider info)
    {
        if (m_bEnteredTop || m_bEnteredBottom)
            return; // don't react if a trigger has already been entered

        Debug.Log("On Trigger Enter" + name);

        // depending on the trigger name:
        // 
        // Enter from TOP
        if (name == "TrampTriggerTop")
        {
            m_bEnteredTop = true;

            // disable the bottom_pad_collider if it's two-sided (no bottom collider exists if it's one-sided)
            if (TwoSided)
                transform.parent.Find("bottom_pad_collider").gameObject.collider.isTrigger = true;
        }

        Vector3 vNorm = transform.up.normalized;

        // if two-sided & entered through bottom, bounce direction is opposite
        if (m_bEnteredBottom && TwoSided)
        {
            vNorm = -vNorm;
        }

        Vector3 vVel = m_Player.rigidbody.velocity;
        float velX = Mathf.Abs(vVel.x) * Friction * vNorm.x * TensionConstant;
        float velY = Mathf.Abs(vVel.y) * Friction * vNorm.y * TensionConstant;

        // cap Y low
        if (Mathf.Abs(velY) < RestingThresholdForce * vNorm.y)
            velY = RestingThresholdForce * vNorm.y;
        // cap y high
        if (Mathf.Abs(velY) > MaxYForce)
            velY = MaxYForce;

        // only play anim if bounce was high enough
        if (Mathf.Abs(velX) > 0.0f || Mathf.Abs(velY) > 0.0f)
        {
	        // TODO:: maybe play at a speed corresponding to the player's velocity??
	        m_BounceAnim["Take 001"].speed = Mathf.Lerp(LowAnimMultiplier, HighAnimMultiplier, (velX + velY) / MaxAnimInfluenceOfVelocity);
//             Debug.Log("Anim Speed:" + m_BounceAnim["Take 001"].speed.ToString());
	        m_BounceAnim.Play();
        }

        // TODO:: make sure the player is coming from "in front" of the bounce pad
        // dot determines this
        Force = new Vector3(velX, velY, 0.0f);

        m_bPlayerInTrigger = true;
        m_Player.OnTrampEnter(this);
    }

    void OnTriggerStay(Collider info)
    {
        // only care about staying if this tramp is one-sided and they're bouncing off the top, or
        if (info.gameObject.name == "Player" && (TwoSided || (!TwoSided && m_bEnteredTop)) )
        {
            Debug.Log("Trigger stay " + name);
            m_bPlayerInTrigger = true;
            //Debug.Log("OnTriggerStay: " + name);
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
            Debug.Log("Trigger exit " + name);

            m_bEnteredTop = m_bEnteredBottom = false;
            m_bPlayerInTrigger = m_bEnteredBottom = m_bEnteredTop = false;
            m_Player.OnTrampExit();

            // the top pad gets re-enabled
            transform.parent.Find("top_pad_collider").gameObject.collider.isTrigger = false;

            // the bottom pad gets re-enabled it it's:
            // 1. two-sided & 
            if (TwoSided)
                transform.parent.Find("bottom_pad_collider").gameObject.collider.isTrigger = false;
        }
    }

    public void OnBottomTriggerEnter()
    {
        m_bEnteredBottom = true;

        // disable top_pad_collider - there will always be a top_pad_collider
        //
        // 1. if it's two-sided, they need to bounce off of the bottom_pad_collider
        // 2. if it's one-sided, they need to go through (there is no bottom_pad_collider in this case)
        transform.parent.Find("top_pad_collider").gameObject.collider.isTrigger = true;
        
        // if two-sided & entered through bottom, bounce direction is opposite
        if (m_bEnteredBottom && TwoSided)
        {
            Vector3 vNorm = transform.up.normalized;
            vNorm = -vNorm;
            Vector3 vVel = m_Player.rigidbody.velocity;
            float velX = Mathf.Abs(vVel.x) * Friction * vNorm.x * TensionConstant;
            float velY = Mathf.Abs(vVel.y) * Friction * vNorm.y * TensionConstant;

            // cap Y low
            if (Mathf.Abs(velY) < RestingThresholdForce * vNorm.y)
                velY = RestingThresholdForce * vNorm.y;
            // cap y high
            if (Mathf.Abs(velY) > MaxYForce)
                velY = MaxYForce;

            // only play anim if bounce was high enough
            if (Mathf.Abs(velX) > 0.0f || Mathf.Abs(velY) > 0.0f)
            {
                // TODO:: maybe play at a speed corresponding to the player's velocity??
                m_BounceAnim["Take 001"].speed = Mathf.Lerp(LowAnimMultiplier, HighAnimMultiplier, (velX + velY) / MaxAnimInfluenceOfVelocity);
                //             Debug.Log("Anim Speed:" + m_BounceAnim["Take 001"].speed.ToString());
                m_BounceAnim.Play();
            }

            // TODO:: make sure the player is coming from "in front" of the bounce pad
            // dot determines this
            Force = new Vector3(velX, velY, 0.0f);

            m_bPlayerInTrigger = true;
            m_Player.OnTrampEnter(this);
        }
        // otherwise, we're going through bottom, no bounce or force applied
    }

    public void OnBottomTriggerExit()
    {
        // the top pad gets re-enabled
        transform.parent.Find("top_pad_collider").gameObject.collider.isTrigger = false;

        m_bEnteredTop = m_bEnteredBottom = false;
        m_bPlayerInTrigger = m_bEnteredBottom = m_bEnteredTop = false;
        m_Player.OnTrampExit();
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