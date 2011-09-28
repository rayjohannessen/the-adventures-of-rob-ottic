using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {

    }
	
	public void OnTriggerEnter(Collider _info)
	{
		Debug.Log("collectible trigger enter " + _info.name);
		if (_info.name == "Player")
		{
			// add this collectible to the player's info
			Game.Instance.PlayerInfo.IncrementCurrency();
			
			Destroy(gameObject);
		}
	}
}