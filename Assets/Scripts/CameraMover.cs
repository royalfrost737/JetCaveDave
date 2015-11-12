using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public float speed, rotationSpeed;
	public GameObject player;

   void FixedUpdate()
	{
		Vector3 movement = new Vector3 (0.0f, 0.0f, speed);
		GetComponent<Rigidbody>().velocity = movement;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,player.transform.rotation.y,0), Time.time * rotationSpeed);

	}
}
