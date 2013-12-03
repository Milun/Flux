/**
 * GameCounter.cs
 * 
 * Author: 	Milton Plotkin
 * Date:	3/12/2013
 * 
 * A counter that is used by the player.
 * 
 * Overloads its derived classes SetTile() method to include the "taking" of other GameCounters.
 * This GameCounter can also create a new GameCounter (and parent it to its own Player and a new Tile)
 * when moved to an empty Tile and told to "split" (setting the number on the overlay lower than its health
 * by pressing right-click).
 * 
 * Has a reference to the Tile it is on,
 * as well as the Player that uses it.
 * 
 * Also includes visual implementation of highlighting all valid tiles when selected.
 * When this GameCounters health drops to or below 0, it will automatically remove itself from the
 * game and remove all references to it from its Player and Tile (if applicable).
 * 
 * Has a Flux() method. This should be called once, at the end of the last players turn.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCounter : Counter {

	#region vars
	private const int maxHealth = 5;	// If health exceeds this, it rolls over to 1
	#endregion

	// ====================================================================================================================

	#region public methods

	/// <summary>
	/// Increments this counters health by 1.
	/// If it exceeds MaxHealth, it gets set to 1.
	/// 
	/// Only fluxes if the counter is on the board.
	/// </summary>
	public void Flux() {
		
		// Do not Flux unless you're on the board
		if (!tile) return;
		
		// Reset if the counter reaches "critical Flux"
		if (GetHealth() == maxHealth)
			SetHealth(1);
		else
			SetHealth(GetHealth()+1);
	}

	/// <summary>
	/// Overloads the inherited MaxGlow() method to make
	/// all tiles which the counter can be moved to also light up.
	/// </summary>
	new public void MaxGlow() {
		base.MaxGlow ();
		
		// Light up neighbouring (valid) tiles
		foreach (Tile e in GetValidMoves())
			e.Glow();
	}

	/// <summary>
	/// Sets the location of the counter on the board.
	/// If there is an opposing counter on "tile" with less health, take it.
	/// If "tile" has no counter, subtract "newCounterHealth" from this counter and spawn
	/// a new counter on "tile" with that amount of health.
	/// 
	/// Returns true if the it was a valid move.
	/// </summary>
	/// <param name="tile">The tile to move to.</param>
	/// <param name="newCounterHealth">If the tile moved to is empty, how much health should the split counter lose?</param>
	public bool SetTile(Tile tile, int newCounterHealth = 0) {
		
		// Store the counter on the destination tile (if there is one)
		Counter otherCounter = tile.GetCounter();

		// Prevent landing on other tiles if you're not on the board already
		if (!this.tile && otherCounter)
			return false;

		// Check if the tile it's being placed on is valid (if it's on a tile).
		if (this.tile && !GetValidMoves().Contains(tile))
			return false;

		// Unhighlight the moving tile (prevents a frame of flicker)
		StopGlow();

		// Take the other counter (GetValidMoves() already checked for valid health)
		if (otherCounter)
		{
			SetHealth ( GetHealth () - otherCounter.GetHealth() );
			otherCounter.SetHealth(0);
		}
		else // Otherwise, split the tile into two.
		{
			if (GetHealth() < newCounterHealth)
				throw new System.Exception("Attempted to subtract more health than possible!");

			// If the amount we want to move is smaller than the counters health, split it.
			// Otherwise, the current GameCounter will just move its own position.
			if (newCounterHealth != 0 && GetHealth () != newCounterHealth)
			{
				// Create a new GameCounter on the field
				GameObject go = (GameObject)GameObject.Instantiate(this.gameObject);
				GameCounter gc = go.GetComponent<GameCounter>();

				// Give it a text object to show its health.
				GameObject goText = (GameObject)GameObject.Instantiate(text.gameObject);

				// Set its position, and set its player and color to be = to this GameCounters.
				gc.SetPlayer(player, GetNormColor(), goText.GetComponent<TextMesh>());
				gc.SetTile (tile);
				gc.SetHealth (newCounterHealth);

				// Subtract the amount of health the newly created counter has from this one.
				this.SetHealth (GetHealth ()-newCounterHealth);

				// Give the player control over the newly created counter.
				player.AddCounter (gc);

				return true;
			}
		}

		return base.SetTile (tile);
	}

	/// <summary>
	/// Returns a List of all Tiles which this counter can move to (allowed by game rules).
	/// </summary>
	/// <returns>The valid moves.</returns>
	public List<Tile> GetValidMoves() {
		
		List<Tile> validMoves = new List<Tile>();
		
		if (tile) {
			foreach (Tile e in tile.GetNeighbours()) {

				// If there is no counter on the tile, you can always move there.
				if (e.GetCounter())
				{
					// Don't move/attack counters of the same player
					if (e.GetCounter().GetPlayer() == player) 		continue;

					// Don't take counters with >= health to you.
					if (e.GetCounter().GetHealth() >= GetHealth ())	continue;
				}

				// Otherwise, the move is valid.
				validMoves.Add (e);
			}
		}
		
		return validMoves;
	}

	#endregion
}
