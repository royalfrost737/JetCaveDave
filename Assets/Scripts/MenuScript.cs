using UnityEngine;

/// <summary>
/// Title screen script
/// </summary>
public class MenuScript : MonoBehaviour
{
	public int xOffset;
	public int yOffset;
	public int buttonWidth;
	public int buttonHeight;

    void OnGUI()
    {
        // Determine the button's place on screen
        // Center in X, 2/3 of the height in Y
        Rect buttonRect = new Rect(
              xOffset,
              yOffset,
              buttonWidth,
              buttonHeight
            );

        // Draw a button to start the game
        if (GUI.Button(buttonRect, "Start!"))
        {
            // On Click, load the first level.
            // "Stage1" is the name of the first scene we created.
            Application.LoadLevel("Main");
        }
    }
}