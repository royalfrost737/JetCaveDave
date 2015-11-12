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

	// Private variables
	private bool inThreshold, up, down, left, right, gunUpgraded;
	private float xMin, xMax, zMin, zMax, yNorm, nextFire; 

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
		zMax = camera.transform.position.z-zOffset/4;
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
	
		// Get the player's orientation
		float leftRight = Input.GetAxis ("Horizontal");
		float upDown = Input.GetAxis ("Vertical");

		// If the player is in an intersection, he or she can rotate in 90-degree directions
		if (inThreshold) 
		{
			if ((up | down) && !alreadyRotated)
			{
				Debug.Log ("going up && " + alreadyRotated);
				if ((leftRight > 0) && (left == false)) 
				{
					right = true;
					up = false;
					down = false;
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90,90,0), Time.time * rotationSpeed);
					alreadyRotated = true;
				}
				if ((leftRight < 0) && (right == false)) 
				{
					left = true;
					up = false;
					down = false;
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90,-90,0), Time.time * rotationSpeed);
					alreadyRotated = true;
				}

			}
			if ((left | right) && !alreadyRotated)
			{
				if ((upDown > 0) && (down == false)) 
				{
					up = true;
					left = false;
					right = false;
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90,0,0), Time.time * rotationSpeed);
					alreadyRotated = true;
				}
				if ((upDown < 0) && (up == false)) 
				{
					down = true;
					left = false;
					right = false;
					transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(90,180,0), Time.time * rotationSpeed);
					alreadyRotated = true;
				}

				alreadyRotated = true;
			}
		}

		GetComponent<Rigidbody>().transform.position = new Vector3 
			(
				Mathf.Clamp (GetComponent<Rigidbody>().transform.position.x, xMin, xMax),
				yNorm,
				Mathf.Clamp(GetComponent<Rigidbody>().transform.position.z, zMin, zMax)
			);
    }

	void FixedUpdate()
	{
		// Move the player forward
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

		GetComponent<Rigidbody>().velocity = movement * speed;
	}

	// Let the PlayerController know that the player is at an intersection
	void OnTriggerEnter(Collider other)
	{
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
	}

	//  Allow the player to shoot three shots at time if the player picked up a gun upgrade
	void gunUpgradeHandler()
	{
		gunUpgraded = true;
	}
	
}
