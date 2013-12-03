/**
 * TextMessage.cs
 * 
 * Author: 	Milton Plotkin
 * Date:	3/12/2013
 * 
 * Plays a single animation of zooming/fading text when SetText() is called.
 * 
 * Animation time and zoom multiplication are stored to prevent calculation each Update().
 * 
 * previosText is used to store the last string that was sent to the TextMessage,
 * and prevent the same message being played twice. This is used by Game as a means to determine
 * if the TextMessage has finished playing its animation, and then skip requesting the same animation
 * being played as it returns to the position that its loop was halted in.
 */

using UnityEngine;
using System.Collections;

public class TextMessage : MonoBehaviour {

	#region vars
	private const float animTime = 2.5f;		// The amount of time it takes for the text to fade
	private const float animSize = 0.1f;	// How much more it will be zoomed in at the end of its animation.

	private float animTimer = animTime;		// Current time in animation.
	private const float multiScale = animSize/animTime;	// Helps with the animation.
	private Vector3 originalScale;			// The initial size of the text (used for resetting).

	private bool animating = false;			// Is the text currently animating.
	private string previosText = "";		// What was the last thing the text showed?
	#endregion

	// ====================================================================================================================

	/// <summary>
	/// Store the initial scale of the text (defined in Unity editor).
	/// </summary>
	void Start () {
		originalScale = this.transform.localScale;
	}
	
	/// <summary>
	/// Animates the text once.
	/// If no animation is requested/animation has finished playing,
	/// do not render the text.
	/// </summary>
	void Update () {

		// If currently animating
		if (animTimer < animTime)
		{
			// Increase the timer
			animTimer += Time.deltaTime;
			renderer.enabled = true;

			// Zoom the text over time
			transform.localScale = new Vector3(originalScale.x + multiScale*animTimer,
			                                   originalScale.y + multiScale*animTimer,
			                                   originalScale.z + multiScale*animTimer);

			// Fade it over time
			renderer.material.SetColor ("_Color", new Color(1f,1f,1f, (animTime-animTimer)/animTime));
		}
		else
		{
			// If not animating, don't render the object.
			renderer.enabled = false;
			animating = false;
		}
	}

	// ====================================================================================================================

	#region public methods

	/// <summary>
	/// Sets the text to show and starts the animation.
	/// Returns true if the animation will play.
	/// </summary>
	/// <param name="s">The string to display.</param>
	public bool SetText(string s) {

		// This prevents the same animation playing repeatedly.
		if (s == previosText)
			return false;

		// Set the text
		TextMesh tm = GetComponent<TextMesh>();
		tm.text = s;
		previosText = s;

		animTimer = 0f;
		animating = true;

		return true;
	}

	/// <summary>
	/// Clears the previosText value to allow for a duplicate animation to play.
	/// </summary>
	public void ClearText() {
		previosText = "";
	}

	/// <summary>
	/// Returns true if the zoom animation currently playing.
	/// </summary>
	public bool IsAnimating() {
		return animating;
	}

	#endregion
}
