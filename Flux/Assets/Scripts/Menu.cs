/**
 * Menu.cs
 * 
 * Author: 	Milton Plotkin
 * Date:	3/12/2013
 * 
 * Defines the first screen (GUI) of the game. Allows custom definition for board size,
 * player ammount and player starting counter amount for the game. These values are
 * passed to the Game object through Unities inbuild global "PlayerPrefs".
 */

using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	#region vars
	private float boardSize = 4f;
	private float playerSize = 2f;
	private float startCounters = 4f;
	#endregion

	// ====================================================================================================================

	#region gui methods

	/// <summary>
	/// Handles rendering and events involving the GUI over the camera.
	/// </summary>
	void OnGUI () {

		// Start button
		if (GUI.Button (new Rect (30, 30, 100, 30), "Start")) {
			PlayerPrefs.SetInt ("BoardSize",(int)boardSize);
			PlayerPrefs.SetInt ("PlayerSize",(int)playerSize);
			PlayerPrefs.SetInt ("StartCounters",(int)startCounters);

			Application.LoadLevel ("flux");
		}

		// Sliders to set variou game preferences.
		_CreateSlider(ref boardSize, 80, "Board Size: ", 2f, 6f);
		_CreateSlider(ref playerSize, 120, "Players: ", 2f, 4f);
		_CreateSlider(ref startCounters, 160, "Start Counters: ", 1f, 6f);

	}

	/// <summary>
	/// Automates the creation of sliders
	/// </summary>
	/// <param name="val">Reference to the value this slider will affect.</param>
	/// <param name="y">The sliders y position on the sceen</param>
	/// <param name="label">Text above the slider describing it.</param>
	/// <param name="min">Minimum value of the slider</param>
	/// <param name="max">Maximum value of the slider</param>
	private void _CreateSlider(ref float val, float y, string label, float min, float max)
	{
		string tempString = ((int)val).ToString();
		GUI.Label (new Rect(30f, y, 100f, 30f), label + tempString);
		val = GUI.HorizontalSlider(new Rect(30, y+20, 100, 30), val, min, max);
	}

	#endregion
}