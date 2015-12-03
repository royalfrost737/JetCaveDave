using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class DestroyByContact : MonoBehaviour {
	
	// The score changed event fires an alert to the GameController when the player kills an enemy
	public delegate void ScoreChanged();
	public static event ScoreChanged scoreChanged;
	
	// When the player gets the key, let the GameController know
	public delegate void GotGunUpgrade();
	public static event GotGunUpgrade gotGunUpgrade;

	// When the player gets the shield, let the GameController know
	public delegate void GotShield();
	public static event GotShield gotShield;
	
	// When the player gets the key, let the GameController know
	public delegate void GotKey();
	public static event GotKey gotKey;

	// When a player contacts the exit, let the GameController know
	public delegate void ExitAttempt();
	public static event ExitAttempt exitAttempt;

	void OnTriggerEnter(Collider other)
    {
		// If a bullet hits a wall or a threshold or is shot from the player, don't destroy any of those things
		if (other.tag != "Enemy")
        {
			// If a bullet hits a wall, destory the bullet
			if (other.tag == "Maze")
			{
				Destroy (gameObject);
			}

			return;
        }

		// If a bullet hits an enemy, destory the bullet, destroy the enemy
		// and let the game controller to increase the player's score
		if (other.tag == "Enemy") 
		{
			scoreChanged();
		}

        Destroy(other.gameObject);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
		// If an enemy hits the key, don't destroy the key
		if (this.gameObject.tag == "Key" && other.gameObject.tag == "Enemy") {
			return;
		}

		// If the player gets a gun upgrade, fire the gotGunUpgrade event to the PlayerController, destroy the upgrade, and don't kill the player
		if (this.gameObject.tag == "GunUpgrade" && other.gameObject.tag == "Player") 
		{
			gotGunUpgrade();
			Destroy (gameObject);
			return;
		}
		// If the player gets a shield upgrade, set gotShield to true, destroy the upgrade, and don't kill the player
		if (this.gameObject.tag == "Shield" && other.gameObject.tag == "Player") 
		{
			gotShield();
			GameObject.Find ("player").GetComponent<PlayerController>().haveShield = true;
			Destroy (gameObject);
			return;
		}
		// If the player gets the key, fire the gotKey event to the GameController, destroy the key, and don't kill the player
		if (this.gameObject.tag == "Key" && other.gameObject.tag == "Player") 
		{
			gotKey();
			Destroy (gameObject);
			return;
		}

	
	// If the player gets the key, fire the gotKey event to the GameController, destroy the key, and don't kill the player
	if (other.gameObject.tag == "Key" && this.gameObject.tag == "Player") 
	{
		gotKey();
		Destroy (gameObject);
		return;
	}

		// If the player contacts the exit door, fire the exitAttempt event to the GameController and don't kill the player
		if (this.gameObject.tag == "Exit" && other.gameObject.tag == "Player") 
		{
			exitAttempt();
			return;
		}

		// If the enemy hits the player, the game is over
        if (other.gameObject.name == "player") 
		{
			// If the player has a shield, don't destroy the player or end the game
			if (GameObject.Find ("player").GetComponent<PlayerController>().haveShield == true)
			{
				Destroy (gameObject);
				return;
			}

			Transform target; 
			GameObject Player;

			Player = GameObject.FindWithTag ("Player"); 
			target = Player.transform;

			Destroy (other.gameObject);
			Destroy (gameObject);
			target.parent.gameObject.AddComponent<GameOverScript> ();
		} 

		// If an enemy hits anything else, reverse the enemy's direction
        else
        {
            GetComponent<Rigidbody>().velocity *= -1;
        }
    }
}
