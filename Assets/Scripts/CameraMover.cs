using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{

	public float speed;


   void FixedUpdate()
	{
		Vector3 movement = new Vector3 (0.0f, 0.0f, speed);
		GetComponent<Rigidbody>().velocity = movement;
	}
}
