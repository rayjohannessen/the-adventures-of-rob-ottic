using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider _info)
    {
        if (_info.gameObject.name == "Player")
        {
            GameObject.Find("Player").GetComponent<Player>().OnHitWater();
            Game.Instance.OnCharacterDied();
        }
    }
}