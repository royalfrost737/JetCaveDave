using UnityEngine;
using System.Collections;

public class PlayerController2 : MonoBehaviour 
{
	public float speed, xOffset, yOffset, zOffset;
	private float xMin, xMax, zMin, zMax, yNorm; 
	private bool inThreshold, up, down, left, right;
	public GameObject camera;

	void Start()
	{
		inThreshold = false;
		up = true;
		down = false;
		left = false;
		right = false;
	}

	void Update()
	{
		// Get the player's orientation
		float leftRight = Input.GetAxis ("Horizontal");
		float upDown = Input.GetAxis ("Vertical");
		
		// If the player is in an intersection, he or she can rotate in 90-degree directions
		if (inThreshold == true) {
			if ((leftRight > 0) && (left == false)) {
				right = true;
				up = false;
				down = false;
				transform.up = new Vector3 (90, 0, 0);
				transform.Rotate (0, 90, 0);
			}
			if ((leftRight < 0) && (right == false)) {
				left = true;
				up = false;
				down = false;
				transform.up = new Vector3 (-90, 0, 0);
				transform.Rotate (0, -90, 0);
			}
			if ((upDown > 0) && (down == false)) {
				up = true;
				left = false;
				right = false;
				transform.up = new Vector3 (0, 0, 90);
			}
			if ((upDown < 0) && (up == false)) {
				down = true;
				left = false;
				right = false;
				transform.up = new Vector3 (0, 0, -90);
				transform.Rotate (0, 180, 0);
			}
		}

	
	}

	void FixedUpdate()
	{
		xMin = camera.transform.position.x-xOffset;
		xMax = camera.transform.position.x+xOffset;
		zMin = camera.transform.position.z-zOffset;
		zMax = camera.transform.position.z+zOffset;
		yNorm = camera.transform.position.y+yOffset;

		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");


		GetComponent<Rigidbody>().transform.position = new Vector3 
			(
				Mathf.Clamp (GetComponent<Rigidbody>().transform.position.x, xMin, xMax),
				yNorm,
				Mathf.Clamp(GetComponent<Rigidbody>().transform.position.z, zMin, zMax)
				);


		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
		if (((movement.x > 0) && (left == true)) || ((movement.x < 0) && (right == true)) ||
		    ((movement.z > 0) && (down == true)) || ((movement.z < 0) && (up == true)))
		{
			return;
		}



	}

	// Let the PlayerController know that the player is at an intersection
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Threshold") 
		{
			inThreshold = true;
		}
	}

	// Let the PlayerController know that the player has exited an intersection
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Threshold") 
		{
			inThreshold = false;
		}
	}
}
