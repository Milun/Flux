using UnityEngine;
using System.Collections;

public class GhostCounter : Counter {

	#region vars
	private float visibleTimer = 0f;	// Used to prevent the GhostCounter being rendered when not in use.
	#endregion

	// ====================================================================================================================

	// Update is called once per frame
	new void Update () {

		if (visibleTimer > 0f)
		{
			visibleTimer -= Time.deltaTime;
			_SetVisible(true);
			Fade ();
		}
		else
			_SetVisible(false);

		base.Update();
	}

	// ====================================================================================================================

	#region public methods

	/// <summary>
	/// Moves the GhostCounter to a tile.
	/// Will not move if the tile already has a counter (prevents overlap).
	/// </summary>
	/// <param name="tile">The tile to move to.</param>
	new public void SetTile(Tile tile) {
		
		_SetVisible(false);
		
		// Show the ghost only on empty tiles (to prevent overlap)
		if (!tile.GetCounter ())
		{
			// Make it visible.
			visibleTimer = 0.01f;
			
			// Move to its position
			_SetPos(tile.transform.position);
		}
		else // If the tile has a counter, don't render the GhostCounter.
			visibleTimer = 0f;
	}

	/// <summary>
	/// Increments health. If it exceeds maxHealth, it rolls over to 1.
	/// </summary>
	/// <param name="maxHealth">Maximum health for the increment to reach</param>
	public void IncHealth(int maxHealth)
	{
		base.SetHealth(GetHealth()+1);
		
		if (GetHealth () > maxHealth)
			base.SetHealth(1);
	}

	#endregion

	// ====================================================================================================================

	#region private methods

	/// <summary>
	/// Used by GhostCounter to automate setting its visibility.
	/// </summary>
	/// <param name="b">True: visible, false: invisible.</param>
	private void _SetVisible(bool b) {
		renderer.enabled = b;
		text.renderer.enabled = b;
	}

	#endregion
}
