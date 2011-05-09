using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{

    void Start()
    {
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider body)
    {
        if (body.name == "Player")
        {
            // they have reached the goal
            Game.Instance.CurrLevel.OnGoalReached();
        }
    }
}