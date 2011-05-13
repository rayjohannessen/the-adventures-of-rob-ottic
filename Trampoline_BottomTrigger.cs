using UnityEngine;
using System.Collections;

public class Trampoline_BottomTrigger : MonoBehaviour
{
    Trampoline m_Tramp;

    void Start()
    {
        m_Tramp = transform.parent.Find("TrampTriggerTop").GetComponent<Trampoline>();
    }

    void OnTriggerEnter(Collider _info)
    {
        if (_info.gameObject.name == "Player")
        {
            m_Tramp.OnBottomTriggerEnter();
        }
    }

    void OnTriggerExit(Collider _info)
    {
        if (_info.gameObject.name == "Player")
        {
            m_Tramp.OnBottomTriggerExit();
        }
    }
}