/**
 * ColorObject.cs
 * 
 * Author: 	Milton Plotkin
 * Date:	3/12/2013
 * 
 * Attaching this script to a Unity prefab will allow the attached model to vary in hues and alpha.
 * 
 * As long as Glow(), MaxGlow() or Fade() is being called for this object, its material will change
 * hues accordingly.
 * If no glow functions are called, the material will automatically be returned
 * to the default specified by SetColor().
 */

using UnityEngine;
using System.Collections;

public class ColorObject : MonoBehaviour {

	#region vars
	private float glowTimer = 0f;	
	private Color normColor;		// The hue of this object
	private Color glowColor;		// The color to highlight it, stored to reduce recalculation.
	private Color maxColor;			// The color to dehighlight it

	private Color curColor = new Color();
	#endregion

	// ====================================================================================================================
	
	/// <summary>
	/// Initiate the material at full opacity.
	/// </summary>
	void Start () {
		curColor.a = 1f;
	}
	
	/// <summary>
	/// Check if a glow function is being called. If not, revert the material to
	/// its normColor (normal hue).
	/// </summary>
	public void Update () {

		// Handle glowing.
		// If no color changing method is being called, the object will go back
		// to its normal color.
		if (glowTimer > 0f)
			glowTimer -= Time.deltaTime;
		else
			curColor = normColor;

		renderer.material.SetColor("_Color", curColor);
	}

	// ====================================================================================================================

	#region color changing methods

	/// <summary>
	/// Makes the object light up slightly (become brighter as if highlighted).
	/// </summary>
	public void Glow() {
		glowTimer = 0.001f;
		curColor.r = glowColor.r;
		curColor.g = glowColor.g;
		curColor.b = glowColor.b;
	}

	/// <summary>
	/// Makes the object light up a lot (used for selected objects)
	/// </summary>
	public void MaxGlow() {
		glowTimer = 0.001f;
		curColor.r = maxColor.r;
		curColor.g = maxColor.g;
		curColor.b = maxColor.b;
	}

	/// <summary>
	/// Make this object appear semi transparent.
	/// </summary>
	public void Fade() {
		glowTimer = 0.001f;
		curColor.a = 0.3f;
	}

	/// <summary>
	/// Instantly reverts the object back to its normal colors.
	/// Used to prevent flickering.
	/// </summary>
	public void StopGlow() {
		glowTimer = 0f;
	}

	#endregion

	// ====================================================================================================================

	#region accessor methods

	/// <summary>
	/// Returns the default (non-highlighted) color of the object.
	/// </summary>
	public Color GetNormColor() {
		return normColor;
	}

	/// <summary>
	/// Sets the default color of the object, as well as generating
	/// highlight and fade colors for later use.
	/// These are stored to prevent the need to recalculate them each frame.
	/// </summary>
	/// <param name="color">Color.</param>
	public void SetColor(Color color) {
		normColor = color;
		normColor.a = 1f;
		curColor = normColor;

		renderer.material.SetColor("_Color", normColor);
		
		glowColor = new Color();
		glowColor.r = color.r*2f;
		glowColor.g = color.g*2f;
		glowColor.b = color.b*1f;
		glowColor.a = 1f;

		maxColor = new Color();
		maxColor.r = color.r*3f;
		maxColor.g = color.g*3f;
		maxColor.b = color.b*2f;
		maxColor.a = 1f;
	}

	#endregion
}
