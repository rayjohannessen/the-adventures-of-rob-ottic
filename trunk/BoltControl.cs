using UnityEngine;
using System.Collections;

public class BoltControl : MonoBehaviour
{
    public float UnscrewTime = 1.0f;
    public float ScrewTime = 1.0f;

    public void Unscrew()
    {
        iTween.MoveTo(gameObject, new Vector3(transform.position.x, transform.position.y, -0.1f), UnscrewTime);	
    }

    public void Screw()
    {
        iTween.MoveTo(gameObject, new Vector3(transform.position.x, transform.position.y, 0.0f), UnscrewTime);	
    }
}