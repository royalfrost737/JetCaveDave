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
			Debug.Log (player.GetComponent<PlayerController> ().rotating + " rotating");
			Debug.Log (player.GetComponent<PlayerController> ().inThreshold + " in threshold");
			if (player.GetComponent<PlayerController> ().rotating && player.GetComponent<PlayerController> ().inThreshold) 
			{
				Debug.Log ("Got this far");
				StartCoroutine(RotateCamera(lerpSpeed, target));

				//transform.position = Vector3.MoveTowards (transform.position, target, lerpSpeed * Time.deltaTime);
			}
		}
	}	

	IEnumerator RotateCamera(float inTime, Vector3 target)
	{
		for(float t = 0f ; t < 1f ; t += Time.deltaTime/inTime)
		{
			transform.position = Vector3.MoveTowards (transform.position, target, t);
			//transform.position = Vector3.Lerp(transform.position, target, t);
			Debug.Log (target + " target");
			Debug.Log (transform.position + " camera position");
			yield return null;
		}
	}
}
