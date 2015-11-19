using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	// Public variables
	public AudioClip regularShot, upgradedShot;
	public bool haveShield, alreadyRotated;
	public float speed, xOffset, yOffset, zOffset, fireRate, rotationSpeed;
	public GameObject shot, camera;
	public Transform shotSpawn;
	public bool inThreshold, rotating;
	public Vector3 currentThresholdCenter = new Vector3(0,0,0);

	// Private variables
	private bool up, down, left, right, gunUpgraded, isLeftEnabled, isRightEnabled;
	private float xMin, xMax, zMin, zMax, yNorm, nextFire; 

	// The player rotated event fires an alert to the CamerMover when the player turns a corner
	public delegate void PlayerRotated(float rotation, float rotationSpeed, Vector3 currentThresholdPosition);
	public static event PlayerRotated playerRotated;


	public float moveSpeed;
	public float turnSpeed;
	public float lerpSpeed;



	
	void Start()
	{
		// At the start the player is not in a threshold and is facing forward and has no gun upgrades
		inThreshold = false;
		up = true;
		down = false;
		left = false;
		right = false;
		gunUpgraded = false;
		haveShield = false;
		alreadyRotated = false;
		rotating = false;
		isLeftEnabled = true;
		isRightEnabled = true;
	}
	
	// Subscribe to DestroyByContact's alert when a gun upgrade is picked up
	void OnEnable()
	{
		DestroyByContact.gotGunUpgrade += gunUpgradeHandler;
	}
	
	// Unsubscribe to DestroyByContact's alert when a gun upgrade is picked up
	void OnDisable()
	{
		DestroyByContact.gotGunUpgrade -= gunUpgradeHandler;
	}
	
	void Update()
	{
		// Continually calculate the player's boundary in relation to the camera
		xMin = camera.transform.position.x-xOffset;
		xMax = camera.transform.position.x+xOffset;
		zMin = camera.transform.position.z-zOffset;
		// This limits the player to the lower half of the screen
		zMax = camera.transform.position.z-zOffset/8;
		yNorm = camera.transform.position.y+yOffset;
		
		// Use a plane to make it easy to determine distance from player to mouse cursor
		Plane castedPlane = new Plane(Vector3.up, Vector3.zero); 
		
		// Player shots follow the mouse cursor
		// Shots are on a delay rate to avoid rapid fi
		if (Input.GetButton("Fire1") && Time.time > nextFire)
		{
			// Get the allowed fire rate
			nextFire = Time.time + fireRate;
			
			// A ray is an infinite line starting at the origin and going to a specified point
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			float rayDistance;
			
			// If the ray intersects the plane ...
			if (castedPlane.Raycast(ray, out rayDistance))
			{
				// Calcuate the hit point
				Vector3 hitPoint = ray.GetPoint(rayDistance);
				
				// Calculate the shooting angle
				shotSpawn.rotation = Quaternion.LookRotation((hitPoint-shotSpawn.position).normalized);
				
				// Calculate the shot spawn location
				Vector3 forward = transform.TransformDirection(Vector3.up);
				Vector3 toOther = hitPoint - shotSpawn.position;
				Quaternion leftAdjust = shotSpawn.rotation * Quaternion.Euler(0, -30, 0);
				Quaternion rightAdjust = shotSpawn.rotation * Quaternion.Euler (0, 30, 0);
				
				// Fire the shot in the correct direction
				if (Vector3.Dot(forward, toOther) >= 0)
				{
					Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
					if (gunUpgraded)
					{		
						AudioSource.PlayClipAtPoint (regularShot, transform.position);
						Instantiate(shot, shotSpawn.position, leftAdjust);
						Instantiate(shot, shotSpawn.position, rightAdjust);
					}
					else
					{
						AudioSource.PlayClipAtPoint (upgradedShot, transform.position);
					}
				}
			}
		}

		// Move up
		if (Input.GetKey (KeyCode.UpArrow) | Input.GetKey (KeyCode.W))
			transform.Translate (Vector3.up * moveSpeed * Time.deltaTime);
		
		// Move down
		if (Input.GetKey (KeyCode.DownArrow) | Input.GetKey (KeyCode.S))
			transform.Translate (-Vector3.up * moveSpeed * Time.deltaTime);

		if (inThreshold) 
		{
			Vector3 target = new Vector3(currentThresholdCenter.x, transform.position.y, currentThresholdCenter.z);

			// Rotate left
			if (Input.GetKey (KeyCode.LeftArrow) | Input.GetKey (KeyCode.A)) 
			{
				isRightEnabled = false;

				if (isLeftEnabled) 
				{
					if (transform.rotation.y > -.5) 
					{
						transform.Rotate (Vector3.forward, turnSpeed * Time.deltaTime);
						transform.position = Vector3.MoveTowards(transform.position, target, lerpSpeed * Time.deltaTime);
					} 
					else 
					{
						isLeftEnabled = false;
						transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (90, -90, 0), Time.time * rotationSpeed);
						return;
					}
				}
			}

			// Rotate right
			if (Input.GetKey (KeyCode.RightArrow) | Input.GetKey (KeyCode.D))
			{
				isLeftEnabled = false;
				if (isRightEnabled) 
				{
					if (transform.rotation.y < .5) 
					{
						transform.Rotate (Vector3.forward, -turnSpeed * Time.deltaTime);
					} 
					else 
					{
						isRightEnabled = false;
						transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (90, 90, 0), Time.time * rotationSpeed);
						return;
					}
				}
			}
		} 
		else 
		{
			// Move left
			if (Input.GetKey (KeyCode.LeftArrow) | Input.GetKey (KeyCode.A))
				transform.Translate (-Vector3.right * moveSpeed * Time.deltaTime);
			
			// Move right
			if (Input.GetKey (KeyCode.RightArrow) | Input.GetKey (KeyCode.D))
				transform.Translate (Vector3.right * moveSpeed * Time.deltaTime);
		}


		//transform.forward = Vector3.Normalize(new Vector3(Input.GetAxis("Horizontal"), -90, Input.GetAxis("Vertical")));

		
		/*GetComponent<Rigidbody>().transform.position = new Vector3 
			(
				Mathf.Clamp (GetComponent<Rigidbody>().transform.position.x, xMin, xMax),
				yNorm,
				Mathf.Clamp(GetComponent<Rigidbody>().transform.position.z, zMin, zMax)
				);*/
	}
	
	// Let the PlayerController know that the player is at an intersection
	void OnTriggerEnter(Collider other)
	{
		//currentThreshold = other;
		currentThresholdCenter = other.bounds.center;
		if (!alreadyRotated) 
		{
			if (other.tag == "Threshold") 
			{
				inThreshold = true;
			}
		}
	}
	
	// Let the PlayerController know that the player has exited an intersection
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Threshold") {
			inThreshold = false;
			alreadyRotated = false;
		}
		isLeftEnabled = true;
	}
	
	//  Allow the player to shoot three shots at time if the player picked up a gun upgrade
	void gunUpgradeHandler()
	{
		gunUpgraded = true;
	}
}
