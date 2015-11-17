using UnityEngine;
using System.Collections;

public class EnemyMover : MonoBehaviour
{
	public float speed;
	public string directionType;
	public int radius = 10;
	private Transform player;

	void Start()
	{
		player = GameObject.Find ("player").transform;
	}
	
	void Update()
	{
		if (player) {
			Vector3 distVec = player.transform.position - transform.position;
			float distSqr = distVec.sqrMagnitude;
			if (distSqr < radius * radius) {
				if (directionType == "vertical") {
					transform.Translate (Vector3.forward * speed);
					//GetComponent<Rigidbody>().velocity = transform.forward * speed;
				}
				if (directionType == "horizontal") {
					//GetComponent<Rigidbody>().velocity = transform.right * speed;
				
					transform.Translate (Vector3.right * speed);
				}		
			}
		}
	}

	void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag == "Maze") 
		{
			speed *= -1;
		}
	}
}
