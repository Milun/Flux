using UnityEngine;
using System.Collections;

public class ColorObject : MonoBehaviour {

	#region vars
	private float glowTimer = 0f;	
	private Color normColor;		// The hue of this object
	private Color glowColor;		// The color to highlight it
	private Color maxColor;			// The color to dehighlight it

	private Color curColor = new Color();
	#endregion

	// Use this for initialization
	void Start () {
		curColor.a = 1f;
	}
	
	// Update is called once per frame
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

	#region color changing methods

	/// <summary>
	/// Makes the object light up slightly (become brighter as if highlighted).
	/// </summary>
	public void Glow() {
		glowTimer = 0.01f;
		curColor.r = glowColor.r;
		curColor.g = glowColor.g;
		curColor.b = glowColor.b;
	}

	/// <summary>
	/// Makes the object light up a lot (used for selection)
	/// </summary>
	public void MaxGlow() {
		glowTimer = 0.01f;
		curColor.r = maxColor.r;
		curColor.g = maxColor.g;
		curColor.b = maxColor.b;
	}

	/// <summary>
	/// Make this object semi transparent.
	/// </summary>
	public void Fade() {
		glowTimer = 0.01f;
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
