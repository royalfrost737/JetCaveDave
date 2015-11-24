﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	// Public variables
	public AudioClip regularShot, upgradedShot;
	public bool haveShield, rotating, inThreshold;
	public float xOffset, yOffset, zOffset, fireRate, rotationSpeed;
	public GameObject shot;
	public Transform shotSpawn;
	public Vector3 currentThresholdCenter = new Vector3(0,0,0);
	public string direction;

	// Private variables
	private bool gunUpgraded, isLeftEnabled, isRightEnabled, alreadyRotated, isVertical, isHorizontal;
	private float xMin, xMax, yNorm, zMin, zMax, nextFire; 

	// The player rotated event fires an alert to the CameraMover when the player turns a corner
	public delegate void PlayerRotated(float rotation, float rotationSpeed, Vector3 currentThresholdPosition);
	public static event PlayerRotated playerRotated;

	public delegate void ExitThreshold(string direction, Vector3 navigation);
	public static event ExitThreshold exitThreshold;
	
	public float moveSpeed;
	public float turnSpeed;
	public float lerpSpeed;

	void Start()
	{
		// At the start the player is not in a threshold and is facing forward and has no gun upgrades
		inThreshold = false;
		gunUpgraded = false;
		haveShield = false;
		alreadyRotated = false;
		isLeftEnabled = true;
		isRightEnabled = true;
		rotating = false;
		isVertical = true;
		isHorizontal = false;
		direction = "vertical";
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
		// ***************************** Shooting **********************************************

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
				//Quaternion upAdjust = shotSpawn.rotation * Quaternion.Euler (0, 0, 0);
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


			/*
			var position = Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f);
			position = Camera.main.ScreenToWorldPoint(position);
			var go = Instantiate(shotSpawn, transform.position, Quaternion.identity) as GameObject;
			go.transform.LookAt(position);    
			Debug.Log(position);    
			go.rigidbody.AddForce(go.transform.forward * 1000);*/






		}

		// ************************* Moving *******************************************

		// Move up
		if (Input.GetKey (KeyCode.UpArrow) | Input.GetKey (KeyCode.W))
			transform.Translate (Vector3.up * moveSpeed * Time.deltaTime);
		
		// Move down
		if (Input.GetKey (KeyCode.DownArrow) | Input.GetKey (KeyCode.S))
			transform.Translate (-Vector3.up * moveSpeed * Time.deltaTime);

		// If the player is in a threshold, using the left and right keys will rotate instead of move
		if (inThreshold && !alreadyRotated) 
		{
			// When the player rotates, use the center of the current threshold as the pivot point (move the player
			// to the pivot point, and then rotate. This line gets the target pivot point/move-to location.
			Vector3 target = new Vector3 (currentThresholdCenter.x, transform.position.y, currentThresholdCenter.z);

			// Rotate left (using either the left arrow or the A key)
			if (Input.GetKey (KeyCode.LeftArrow) | Input.GetKey (KeyCode.A)) 
			{
				// This boolean is for the camera, to let it know that the player has started rotating
				// The camera will move toward the center of the current threshold if the player is rotating
				rotating = true;

				// If the player chooses to go left, then right is no longer an option
				isRightEnabled = false;

				// If left is the first direction the player picks, then left will still be enabled
				if (isLeftEnabled) 
				{
					// Make sure the player doesn't move past -90 degrees (90 degrees to the left)
					if (transform.rotation.y > -.5) 
					{
						// Rotate the player
						transform.Rotate (Vector3.forward, turnSpeed * Time.deltaTime);
						// Move the player to the target pivot point at the center of the threshold
						transform.position = Vector3.MoveTowards (transform.position, target, lerpSpeed * Time.deltaTime);
					} 
					else 
					{
						// This boolean is for the camera, to let it know that the player has stopped rotating
						rotating = false;
						// If the player has rotated too far, disable the key for rotation
						isLeftEnabled = false;
						// And rotate the player to the correct position
						transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (90, -90, 0), Time.time * rotationSpeed);
						alreadyRotated = true;

						if (isVertical) 
						{
							isVertical = false;
							isHorizontal = true;
							direction = "horizontal";
						} 
						else if (isHorizontal) 
						{
							isHorizontal = false;
							isVertical = true;
							direction = "vertical";
						}
					}
				}
			}

			// Rotate right (using either the right arrow or the D key)
			if (Input.GetKey (KeyCode.RightArrow) | Input.GetKey (KeyCode.D)) 
			{				
				// This boolean is for the camera, to let it know that the player has started rotating
				// The camera will move toward the center of the current threshold if the player is rotating
				rotating = true;

				// If the player chooses to go right, then left is no longer an option
				isLeftEnabled = false;

				// If right is the first direction the player picks, then right will still be enabled
				if (isRightEnabled) 
				{
					// Make sure the player doesn't move past 90 degrees (90 degrees to the right)
					if (transform.rotation.y < .5) 
					{
						// Rotate the player
						transform.Rotate (Vector3.forward, -turnSpeed * Time.deltaTime);
					} 
					else 
					{
						// This boolean is for the camera, to let it know that the player has stopped rotating
						rotating = false;
						// If the player has rotated too far, disable the key for rotation
						isRightEnabled = false;
						// And rotate the player to the correct position
						transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (90, 90, 0), Time.time * rotationSpeed);
						alreadyRotated = true;

						if (isVertical) 
						{
							isVertical = false;
							isHorizontal = true;
							direction = "horizontal";
						} 
						else if (isHorizontal) 
						{
							isHorizontal = false;
							isVertical = true;
							direction = "vertical";
						}
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

		//************************* Clamping ********************************
		if (!rotating) 
		{
			transform.position = new Vector3 (
			(Mathf.Clamp (transform.position.x, (Camera.main.transform.position.x - 0.95f), (Camera.main.transform.position.x + 0.95f))),
		 	transform.position.y, 
			(Mathf.Clamp (transform.position.z, (Camera.main.transform.position.z - 1.3f), (Camera.main.transform.position.z + 0.9f))));
		}
	}
	
	// If the player collides with a trigger, check to see if it is a threshold
	void OnTriggerEnter(Collider other)
	{
		// If it is a threshold ...
		if (other.tag == "Threshold") 
		{
			// Change the alreadyRotated boolean to allow the player to make a single rotation choice
			alreadyRotated = false;
			// Change the inThreshold status
			inThreshold = true;
			// Establish a pivot point for the player and camera at the threshold's center
			currentThresholdCenter = other.bounds.center;
		}
	}
	
	// If the player exits a collision with a trigger, check to see if it is a threshold
	void OnTriggerExit(Collider other)
	{
		// If it is a threshold ...
		if (other.tag == "Threshold") 
		{
			// Change the alreadyRotated boolean to prevent the player from making any more rotations
			alreadyRotated = true;
			// Change the inThreshold status
			inThreshold = false;
			// Reset the direction booleans so that the player can rotate at the next threshold
			isRightEnabled = true;
			isLeftEnabled = true;

			exitThreshold(direction, transform.TransformDirection(transform.forward));
		}
	}
	
	//  Allow the player to shoot three shots at time if the player picked up a gun upgrade
	void gunUpgradeHandler()
	{
		gunUpgraded = true;
	}
}
