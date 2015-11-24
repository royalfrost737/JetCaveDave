using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour 
{
	public float speed, lerpSpeed;
	public GameObject player;

	private Vector3 target, CharacterRotation;

	void Start()
	{
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
		if (player != null) 
		{
			target = new Vector3 (player.GetComponent<PlayerController> ().currentThresholdCenter.x, transform.position.y, player.GetComponent<PlayerController> ().currentThresholdCenter.z);
		
			transform.Translate (Vector3.up * speed);

			CharacterRotation = player.transform.eulerAngles;

			transform.rotation = Quaternion.Euler (90, CharacterRotation.y, 0);
			if (player.GetComponent<PlayerController> ().rotating && player.GetComponent<PlayerController> ().inThreshold) {
				transform.position = Vector3.MoveTowards (transform.position, target, lerpSpeed * Time.deltaTime);
			}
		}
	}	
}
