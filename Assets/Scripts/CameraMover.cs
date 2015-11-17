using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public float speed;
	public GameObject player;
	private float rotation;

	void OnEnable()
	{
		PlayerController.playerRotated += rotateCamera;
	}
	
	void OnDisable()
	{
		PlayerController.playerRotated -= rotateCamera;
	}

	void FixedUpdate()
	{
		Vector3 movement = new Vector3(0.0f, 0.0f, speed);

		if (rotation == 90) 
		{
			movement = new Vector3 (speed, 0.0f, 0.0f);
		}
		if (rotation == -90) 
		{
			movement = new Vector3 (-speed, 0.0f, 0.0f);
		}
		if (rotation == 180) 
		{
			movement = new Vector3 (0.0f, 0.0f, -speed);
		}

		GetComponent<Rigidbody>().velocity = movement;
	}

	void rotateCamera(float playerRotation, float rotationSpeed, Collider threshold)
	{
		rotation = playerRotation;
		transform.position = new Vector3 (threshold.bounds.center.x, transform.position.y, threshold.bounds.center.z);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90, rotation, 0), Time.time * rotationSpeed);

	}
}
