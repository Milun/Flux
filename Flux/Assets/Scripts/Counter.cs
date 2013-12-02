using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Counter : ColorObject {

	#region vars

	private int health = 1;

	protected Tile tile;				// Reference to the tile this counter is on
	protected Player player;			// This counters player
	protected TextMesh text;

	#endregion

	// ====================================================================================================================
	
	// Update is called once per frame
	new void Update () {

		base.Update();
	}

	// ====================================================================================================================

	#region public accessor methods

	public Player GetPlayer() {
		return player;
	}
	
	public int GetHealth() {
		return health;
	}
	
	public void SetHealth(int val) {
		health = val;
		text.text = health.ToString();
		
		// Check for death.
		if (health <= 0)
			Die();
	}
	
	public Tile GetTile() {
		return tile;
	}

	public void SetPlayer(Player player, Color color, TextMesh text) {
		SetColor(color);
		this.player = player;
		
		this.text = text;
		this.text.transform.position = this.transform.position;
		this.text.transform.Translate(new Vector3(0f, 0f, -0.5f));
		this.text.text = health.ToString();
	}

	// Sets the location of the counter (and performs any other effects).
	public bool SetTile(Tile tile) {
		
		// Don't do anything if the counter is already on this tile
		if (this.tile == tile) return false;
		
		// Otherwise, remove the current tiles reference to you.
		if (this.tile) this.tile.RemoveCounter(); 
		
		// Set this counters tile and move its position onto it
		this.tile = tile;
		tile.SetCounter(this);
		
		
		// Move the position of the counters text.
		_SetPos(tile.transform.position);
		
		return true;
	}

	#endregion

	// ====================================================================================================================

	#region private methods

	protected void _SetPos(Vector3 pos) {
		transform.position = pos;
		this.text.transform.position = pos;
		this.text.transform.Translate(new Vector3(0f, 0f, -0.5f));
	}

	#endregion

	// ====================================================================================================================

	#region destructor

	public void Die() {
		// Remove references.
		if (player) player.RemoveCounter((GameCounter)this);
		if (tile) tile.SetCounter(null);
		Destroy(text.gameObject);
		Destroy(this.gameObject);
	}

	#endregion
	
}
