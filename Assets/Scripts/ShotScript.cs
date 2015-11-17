using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShotScript : MonoBehaviour {

	public delegate void ScoreChanged();
	public static event ScoreChanged scoreChanged;
	
	void OnCollisionEnter(Collision other)
	{
		// If a bullet hits an enemy, destory the bullet, destroy the enemy
		// and let the game controller know to increase the player's score
		if (other.gameObject.tag == "Enemy") 
		{
			scoreChanged();
			Destroy(other.gameObject);
			Destroy(gameObject);
		}

		if (other.gameObject.tag == "Shot") 
		{
			Destroy (gameObject);
		}
	}
}
