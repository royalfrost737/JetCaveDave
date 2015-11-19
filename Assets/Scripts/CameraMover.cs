using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public float speed, lerpSpeed, rotationSpeed, turnSpeed;
	public GameObject player;
	private float rotation, x, z;
	private bool isLeftEnabled, isRightEnabled;

	void Start()
	{
		isLeftEnabled = true;
		isRightEnabled = true;
	}

	void OnEnable()
	{
		//PlayerController.playerRotated += rotateCamera;
	}
	
	void OnDisable()
	{
		//PlayerController.playerRotated -= rotateCamera;
	}

	void Update()
	{
		Vector3 CharacterRotation = player.transform.eulerAngles;

		transform.rotation = Quaternion.Euler(90, CharacterRotation.y, 0);
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
}
