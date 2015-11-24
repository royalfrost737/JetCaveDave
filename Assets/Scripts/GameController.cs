using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
	public Text scoreTextBox;
	public string exitAttemptMessage;
	public string youWinMessage;
	private int score;
	public int enemyValue, hazardCount;
	public bool hasKey;
	public GameObject Octo, player;
	public AudioClip EnemyKill;
	public AudioClip Powerup;

	private Vector3 spawnPosition;

	// The game controller subscribes to these events from other classes
	void OnEnable()
	{
		DestroyByContact.scoreChanged += UpdateScore;
		DestroyByContact.gotKey += GotTheKey;
		DestroyByContact.exitAttempt += attemptedExit;
		DestroyByContact.gotShield += PlayPowerupSound;
		DestroyByContact.gotGunUpgrade += PlayPowerupSound;
		PlayerController.exitThreshold += SpawnWaves;
	}
	
	void OnDisable()
	{
		DestroyByContact.scoreChanged -= UpdateScore;
		DestroyByContact.gotKey -= GotTheKey;
		DestroyByContact.exitAttempt -= attemptedExit;
		DestroyByContact.gotShield -= PlayPowerupSound;
		DestroyByContact.gotGunUpgrade -= PlayPowerupSound;
		PlayerController.exitThreshold -= SpawnWaves;
	}
	
	void Start()
	{	
		hasKey = false;
		score = 0;
		scoreTextBox.text = "Score: 0";
    }

   void SpawnWaves(string direction, Vector3 navigation)
    {
		for (int i = 0; i < hazardCount; i++) {
			if (player.GetComponent<PlayerController> ().direction == "vertical") {
				if (navigation.z < 0) {
					spawnPosition = new Vector3 (player.transform.position.x, player.transform.position.y, player.transform.position.z + 3 + i);

				}
				if (navigation.z > 0) {
					spawnPosition = new Vector3 (player.transform.position.x, player.transform.position.y, player.transform.position.z - 3 - i);

				}
			}
			if (player.GetComponent<PlayerController> ().direction == "horizontal") {
				if (navigation.x < 0) {
					spawnPosition = new Vector3 (player.transform.position.x + 3 + i, player.transform.position.y, player.transform.position.z);
				}
				if (navigation.x > 0) {
					spawnPosition = new Vector3 (player.transform.position.x - 3 - i, player.transform.position.y, player.transform.position.z);
				}
			}
			Quaternion spawnRotation = Quaternion.identity;
			Instantiate (Octo, spawnPosition, spawnRotation);
		}
    }

	void UpdateScore()
	{
		score += enemyValue;
		scoreTextBox.text = "Score: " + score;
		AudioSource.PlayClipAtPoint (EnemyKill, transform.position);
	}

	void GotTheKey()
	{
		hasKey = true;
		AudioSource.PlayClipAtPoint (Powerup, transform.position);
	}

	void PlayPowerupSound()
	{
		AudioSource.PlayClipAtPoint (Powerup, transform.position);
	}
		
	void attemptedExit()
	{
		if (hasKey) 
		{
			StartCoroutine(ShowExitAttemptResponse(youWinMessage, 30));
			Destroy (GameObject.FindGameObjectWithTag("Player"));
			this.gameObject.AddComponent<GameOverScript>();

		} 
		else 
		{
			StartCoroutine(ShowExitAttemptResponse(exitAttemptMessage, 5));
		}
	}

	IEnumerator ShowExitAttemptResponse (string message, float delay) {
		GetComponent<GUIText>().enabled = true;
		GetComponent<GUIText> ().text = message;
		GetComponent<GUIText> ().fontSize = 24;
		yield return new WaitForSeconds(delay);
		GetComponent<GUIText>().enabled = false;
	}
}
