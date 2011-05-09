using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    bool m_bReached;
    Game m_Game;
    //GameObject m_DummySeeSaw;
    Vector3 m_vSeeSawPos;

    void Start()
    {
        m_bReached = false;
        m_Game = Game.Instance;
        //m_DummySeeSaw = transform.FindChild("SeeSaw").gameObject;
        m_vSeeSawPos = transform.FindChild("SeeSawDummy").gameObject.transform.position;
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider _info)
    {
        if (_info.gameObject.name == "Player" && !m_bReached)
        {
			_info.gameObject.rigidbody.Sleep();
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