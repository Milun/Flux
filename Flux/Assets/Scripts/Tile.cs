/**
 * Tile.cs
 * 
 * Author: 	Milton Plotkin
 * Date:	3/12/2013
 * 
 * A single square tile on the board.
 * 
 * Contains references of up to 8 neighbouring tiles (the maximum that should be possible
 * on a checkered board). Needs to be linked to neighbouring nodes externally.
 * 
 * Can return a reference using GetNeighbours to all neighbours (not including this).
 * 
 * Stores a reference to the Counter currently on top of it (if any). Does not determine if the
 * Counter is on it, it must be manually parented/unparented.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : ColorObject {
	
	private List<Tile> nodeLinks = new List<Tile>(); // The (up to 8) neighbor nodes for this tile.
	private Counter counter;						// A reference to the counter on this tile (if any).

	/// <summary>
	/// Overload the inherited Glow() to make any counters on this tile
	/// light up as well.
	/// </summary>
	new public void Glow() {
		base.Glow();

		if (counter) counter.Glow ();
	}

	/// <summary>
	/// Gives this tile a reference to the counter on top of it.
	/// </summary>
	/// <param name="counter">The counter on it</param>
	public void SetCounter(Counter counter) {
		this.counter = counter;
	}

	/// <summary>
	/// Tells this tile that it's empty;
	/// removes the reference to Counter
	/// </summary>
	public void RemoveCounter() {
		this.counter = null;
	}

	/// <summary>
	/// Returns a reference to the Counter on this Tile (if any)
	/// </summary>
	public Counter GetCounter() {
		return counter;
	}

	/// <summary>
	/// Returns a List of (up to 8) Tiles that surround this one.
	/// </summary>
	public List<Tile> GetNeighbours() {
		return nodeLinks;
	}

	/// <summary>
	/// Links this tile to a neighbouring node.
	/// Will throw an error if more than 8 neighbours allocated.
	/// </summary>
	/// <param name="link">The Tile to provide a link to for this one.</param>
	public void AddLink(Tile link) {

		if (nodeLinks.Count <= 8)
			nodeLinks.Add (link);
		else
			throw new System.Exception("Maximum Tile nodeLinks exceeded!");
	}
}
