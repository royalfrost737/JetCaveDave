using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    //public GameObject hazard;
    //public Vector3 spawnValues;
	public Text scoreTextBox;
	public string exitAttemptMessage;
	public string youWinMessage;
	private int score;
	public int enemyValue;
	public bool hasKey;

	public AudioClip EnemyKill;
	public AudioClip Powerup;
	
	void OnEnable()
	{
		DestroyByContact.scoreChanged += UpdateScore;
		DestroyByContact.gotKey += GotTheKey;
		DestroyByContact.exitAttempt += attemptedExit;
		DestroyByContact.gotShield += PlayPowerupSound;
		DestroyByContact.gotGunUpgrade += PlayPowerupSound;
	}
	
	void OnDisable()
	{
		DestroyByContact.scoreChanged -= UpdateScore;
		DestroyByContact.gotKey -= GotTheKey;
		DestroyByContact.exitAttempt -= attemptedExit;
		DestroyByContact.gotShield -= PlayPowerupSound;
		DestroyByContact.gotGunUpgrade -= PlayPowerupSound;
	}
	
	void Start()
	{	
		hasKey = false;
		score = 0;
		scoreTextBox.text = "Score: 0";
		//SpawnWaves();
    }

   /* void SpawnWaves()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
        Quaternion spawnRotation = Quaternion.identity;
        Instantiate(hazard, spawnPosition, spawnRotation);
    }*/

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
