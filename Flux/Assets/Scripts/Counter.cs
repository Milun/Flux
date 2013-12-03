/**
 * Counter.cs
 * 
 * Author: 	Milton Plotkin
 * Date:	3/12/2013
 * 
 * A base class for Counters (the circular tokens players use).
 * 
 * A Counter knows of the player that owns it, as well as the tile it is currently located on
 * (parented to).
 * 
 * A TextMesh object is passed to the counter for the number it displays above it.
 * 
 * Counter has definitions for parenting it to a Tile/Player, accessor methods to said information,
 * as well as a Desctructor function if it needs to be removed from the game field.
 * It also contains a definition for its health modifier, which will automatically destroy
 * the Counter if its health reaches 0. All derived classes use this function to change health.
 * 
 * Counter is a ColorObject, which allows it to change hues and alpha.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Counter : ColorObject {

	#region vars

	private int health = 1;

	protected Tile tile;				// Reference to the tile this counter is on
	protected Player player;			// This counters player
	protected TextMesh text;			// The passed TextMesh object for displaying its health.

	#endregion

	// ====================================================================================================================

	#region public accessor methods

	/// <summary>
	/// Returns the Player that this counter belongs to (if any). 
	/// </summary>
	public Player GetPlayer() {
		return player;
	}

	/// <summary>
	/// Returns this counters current health (as displayed by the number above it).
	/// </summary>
	public int GetHealth() {
		return health;
	}

	/// <summary>
	/// Sets this counters health to the desired value. It also updates the text display.
	/// If the value is 0, the Counter will remove itself from the game (as well as all references to it
	/// by the Tile/Player that use it).
	/// </summary>
	/// <param name="val">New health value.</param>
	public void SetHealth(int val) {
		health = val;
		text.text = health.ToString();
		
		// Check for death.
		if (health <= 0)
			Destruct();
	}

	/// <summary>
	/// Returns the tile that this Counter is occupying (if any). 
	/// </summary>
	public Tile GetTile() {
		return tile;
	}

	/// <summary>
	/// Parents this Counter to a Player, as well as setting its color and
	/// providing it with a TextMesh to use for its display.
	/// </summary>
	/// <param name="player">The Player that can use this Counter.</param>
	/// <param name="color">The hue this Counter will have.</param>
	/// <param name="text">Reference to a TextMesh that will display this counters health.</param>
	public void SetPlayer(Player player, Color color, TextMesh text) {
		SetColor(color);
		this.player = player;
		
		this.text = text;
		this.text.text = health.ToString();
		_SetPos (this.transform.position);
	}

	/// <summary>
	/// Parents the Counter to the specified Tile and moves its position on top of it.
	/// Removes this Counters reference from the Tile is used to occupy. 
	/// </summary>
	/// <returns><c>true</c>, if tile was set, <c>false</c> otherwise.</returns>
	/// <param name="tile">The new Tile to parent to.</param>
	public bool SetTile(Tile tile) {
		
		// Don't do anything if the counter is already on this tile
		if (this.tile == tile) return false;
		
		// Otherwise, remove the current tiles reference to you.
		if (this.tile) this.tile.RemoveCounter(); 
		
		// Set this counters tile and move its position onto it
		this.tile = tile;
		tile.SetCounter(this);

		// Move the position of the counter and its text.
		_SetPos(tile.transform.position);
		
		return true;
	}

	#endregion

	// ====================================================================================================================

	#region private methods

	/// <summary>
	/// Automates setting the position of the Counter and its text display.
	/// </summary>
	/// <param name="pos">The position to move the counter too.</param>
	protected void _SetPos(Vector3 pos) {
		transform.position = pos;
		this.text.transform.position = pos;
		this.text.transform.Translate(new Vector3(0f, 0f, -0.5f));
	}

	#endregion

	// ====================================================================================================================

	#region destructor

	/// <summary>
	/// Removes this counter from the game. Prevents null reference exceptions
	/// by removing all references to it from its Player and Tile.
	/// </summary>
	public void Destruct() {
		// Remove references.
		if (player) player.RemoveCounter((GameCounter)this);
		if (tile) tile.SetCounter(null);
		Destroy(text.gameObject);
		Destroy(this.gameObject);
	}

	#endregion
	
}
