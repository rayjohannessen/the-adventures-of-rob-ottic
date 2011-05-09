using UnityEngine;
using System.Collections;

public class ResetTransforms : MonoBehaviour
{
    Quaternion vOriginalRotation;
    Vector3 vOriginalPosition;

    // Initialization
    void Start()
    {
        vOriginalRotation = transform.rotation;
        vOriginalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Reset"))
        {
            // reset position & rotation
            transform.position = vOriginalPosition;
            transform.rotation = vOriginalRotation;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetButtonDown("Reset"))
        {
            if (rigidbody != null)
            {
                rigidbody.Sleep();
            }
        }
    }
}