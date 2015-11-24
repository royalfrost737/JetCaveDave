using UnityEngine;
using System.Collections;

public class EnemyMover : MonoBehaviour
{
	public float speed;
	public int radius = 10;
	private Transform player;

	void Start()
	{
		player = GameObject.Find ("player").transform;

		Vector3 distVec = player.transform.position - transform.position;

		if (player.GetComponent<PlayerController> ().direction == "vertical") 
		{
			if (player.transform.TransformDirection(player.transform.forward).z < 0) 
			{
				speed *= -1;
			}
		}
		if (player.GetComponent<PlayerController> ().direction == "horizontal") 
		{
			if (player.transform.TransformDirection(player.transform.forward).x < 0) 
			{
				speed *= -1;
			}
		}
	}
	
	void Update()
	{

		if (player.GetComponent<PlayerController> ().direction == "vertical") 
		{
			transform.Translate (Vector3.forward * speed);
		}
		
		if (player.GetComponent<PlayerController> ().direction == "horizontal") 
		{
			transform.Translate (Vector3.right * speed);
		}
	}
}
