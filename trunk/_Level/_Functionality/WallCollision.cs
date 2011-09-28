using UnityEngine;
using System.Collections;

public class WallCollision : MonoBehaviour
{
//     Player m_Player;
// 
//     void Start()
//     {
//         m_Player = GameObject.Find("Player").GetComponent<Player>();
//     }
// 
//     void OnCollisionEnter(Collision _info)
//     {
//         if (m_Player.WallJumpStarted && _info.gameObject.name == "Player")
//         {
//             Debug.Log("Wall Collision enter");
//             m_Player.OnWallJumpSleep();
//         }
//     }
// 
//     // need to check for stay collision, in case they're pressing the wall jump btn after 
//     // the initial collision with the actual wall
//     void OnCollisionStay(Collision _info)
//     {
//         if (m_Player.WallJumpStarted && _info.gameObject.name == "Player")
//         {
//             Debug.Log("Wall Collision stay");
//             m_Player.OnWallJumpSleep();
//         }
//     }
}