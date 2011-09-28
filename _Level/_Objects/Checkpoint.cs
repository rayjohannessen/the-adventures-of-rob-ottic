using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    bool m_bReached;
	float m_fTimer;
    Game m_Game;
    //GameObject m_DummySeeSaw;
    Vector3 m_vSeeSawPos;

    void Start()
    {
        m_bReached = false;
		m_fTimer = 0.0f;
        m_Game = Game.Instance;
        //m_DummySeeSaw = transform.FindChild("SeeSaw").gameObject;
        m_vSeeSawPos = transform.FindChild("SeeSawDummy").gameObject.transform.position;
    }

    void Update()
    {

    }
	
	void FixedUpdate()
	{
		if (m_fTimer > 0.0f)
		{
			m_fTimer -= Time.fixedDeltaTime;
			GameObject.Find("Player").rigidbody.Sleep();
		}
	}

    void OnTriggerEnter(Collider _info)
    {
        if (_info.gameObject.name == "Player" && !m_bReached)
        {
			m_fTimer = 0.5f;
            m_bReached = true;
            m_Game.CurrLevel.OnCheckpointReached(gameObject);
        }
    }

    public void DestroyDummySeeSaw()
    {
        Destroy(transform.FindChild("SeeSawDummy").gameObject);
    }

    public Vector3 SeeSawPos
    {
        get { return m_vSeeSawPos; }
    }
}