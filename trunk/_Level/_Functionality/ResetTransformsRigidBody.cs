using UnityEngine;
using System.Collections;

public class ResetTransformsRigidBody : MonoBehaviour 
{
	Quaternion vOriginalRotation;
	Vector3 vOriginalPosition;

	void Start () 
	{
		vOriginalRotation = transform.rotation;
		vOriginalPosition = transform.position;
	}
	
	void FixedUpdate () 
	{
		if (Input.GetButtonDown("Reset") && !rigidbody.isKinematic)
        {
            // reset position & rotation
            transform.position = vOriginalPosition;
			transform.rotation = vOriginalRotation;
			
			rigidbody.position = vOriginalPosition;
			rigidbody.rotation = vOriginalRotation;
			rigidbody.velocity += -rigidbody.velocity;
            rigidbody.angularVelocity += -rigidbody.angularVelocity;

            rigidbody.useGravity = false;
		}
	}
}
