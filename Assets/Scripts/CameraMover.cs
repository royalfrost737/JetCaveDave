using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {

    public Transform player;
	public float xdistance;
    public float ydistance;
    public float zdistance;

    void Update()
    {
        if (player != null)
        {
            transform.position = new Vector3(player.position.x + xdistance, player.position.y + ydistance, player.position.z + zdistance);

        }   
    }
}
