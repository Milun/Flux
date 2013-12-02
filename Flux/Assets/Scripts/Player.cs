using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	#region inspector values
	// These values are defined in the Unity GUI
	public GameObject FabGameCounter;
	public GameObject FabGhostCounter;
	public GameObject FabText;

	public Camera Cam;				// A reference to the game camera (used for raycasting)
	public string Name;				// The name of the player (used for victory screen)
	public Color PlColor;			// The color all counters of this player will use.
	#endregion

	#region vars
	private List<GameCounter> counters = new List<GameCounter>();
	private GameCounter selectedCounter;
	private GhostCounter ghostCounter;
	#endregion

	// ====================================================================================================================

	void Start () {
		_InitGhostCounter();
	}
	// ====================================================================================================================

	#region accessor methods
	/// <summary>
	/// Returns true if this player has no counters on the board
	/// (they have lost)
	/// </summary>
	public bool IsDead() {
		foreach (GameCounter e in counters)
			if (e.GetTile ()) return false;
		
		return true;
	}

	/// <summary>
	/// Gets the players name (defined in Unity GUI)
	/// </summary>
	public string GetName() {
		return Name;
	}
	#endregion

	// ====================================================================================================================

	#region public methods

	/// <summary>
	/// Removes the player and counters from the game.
	/// </summary>
	public void Die() {
		while (counters.Count > 0)
			counters[0].Die();
	}

	/// <summary>
	/// Returns true if none of the counters the player has can make a valid move.
	/// </summary>
	public bool checkStalemate() {
		
		// Easiest stalemate: If there are no counters left to use.
		if (counters.Count <= 0) return true;
		
		// Check if the counters on the board can make a valid move.
		foreach (GameCounter e in counters)
			if (e.GetValidMoves().Count > 0) return false;
		
		// If there are no counters on the board (like in first turn) don't stalemate yet.
		return (!IsDead ());
	}

	/// <summary>
	/// Add a counter to the list the player of counters available to the player
	/// </summary>
	/// <param name="gc">Gc.</param>
	public void AddCounter(GameCounter gc) {
		
		if (counters.Contains(gc))
			throw new System.Exception("Attempted to add a GameCounter to a player that already has it!");
		
		counters.Add(gc);
	}

	/// <summary>
	/// Removes the reference to the GameCounter from the players list.
	/// </summary>
	/// <param name="gc">The GameCounter to remove</param>
	public void RemoveCounter(GameCounter gc) {
		
		if (counters.Contains(gc)) counters.Remove(gc);
	}
	
	/// <summary>
	/// Makes all this players counters look faded.
	/// </summary>
	public void Deactivate() {
		foreach (GameCounter e in counters)
			e.Fade ();
	}
	
	/// <summary>
	/// Increments all the players counters health by 1.
	/// Health rolls over if greater than the counters MaxHealth.
	/// </summary>
	public void Flux() {
		foreach (GameCounter e in counters)
			e.Flux ();
	}

	#endregion

	// ====================================================================================================================

	#region initializer and turn methods

	/// <summary>
	/// Initiates the player.
	/// Populates their starting counters.
	/// </summary>
	/// <param name="zPos">Z-position of the players counters (when off the board).</param>
	/// <param name="startCounters">The ammount of counters the player starts with.</param>
	/// <param name="dist">X-Distance between counters off the board.</param>
	public void InitPlayer(float zPos, int startCounters, int dist = 1) {
		
		// The position of the first off-board counter is (x,0,zPos).
		float x = 0f;
		float z = zPos;
		
		for (int i = 0; i < startCounters; i++)
		{
			// Instantiate startCounters amount of GameCounters off the board for this player.
			GameObject go = (GameObject)GameObject.Instantiate(FabGameCounter);
			go.transform.position = new Vector3(x+i*dist, 0f, z);
			counters.Add (go.GetComponent<GameCounter>());
			
			// Instantiate a text object (used to display the GameCounters health).  
			GameObject goText = (GameObject)GameObject.Instantiate(FabText);
			
			// Pass the information to the GameCounter.
			counters[i].SetPlayer(this, PlColor, goText.GetComponent<TextMesh>());
		}
	}

	/// <summary>
	/// Returns true when the player makes a valid move.
	/// </summary>
	public bool Turn() {
		
		// Highlight the currently selected counter
		if (selectedCounter)
			selectedCounter.MaxGlow();
		
		
		// Raycast events //
		Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		// If the raycast isn't hitting anything, then a valid move cannot be made.
		if (!Physics.Raycast(ray, out hit, 100f, 1)) return false;
		
		// Select the GameCounter to use.
		GameCounter rayCounter = hit.collider.GetComponent<GameCounter>();
		
		Tile rayTile = _GetRayTile(hit, rayCounter);
		
		// Move the GhostCounter to show where the player will place their counter.
		if (selectedCounter && rayTile)
			_GhostOverlay (rayTile);
		
		
		// Mouse click events //
		
		// Right-Click:
		// Increment the GhostCounter value (this is how much health the moved tile will have).
		if (Input.GetMouseButtonUp(1))
		{
			if (selectedCounter)
				ghostCounter.IncHealth (selectedCounter.GetHealth ());
			return false;
		}
		
		// Left-Click:
		if (!Input.GetMouseButtonUp(0)) return false;
		
		// Only select if owned by this player.
		if (rayCounter && rayCounter.GetPlayer() == this)
		{
			selectedCounter = rayCounter;
			ghostCounter.SetHealth(selectedCounter.GetHealth());
			
			// Prevent the raycast affecting anything else.
			return false;
		}
		
		// Make a move.
		if (rayTile && selectedCounter)
		{
			// If a succesful move was played
			if (selectedCounter.SetTile(rayTile, ghostCounter.GetHealth()))
			{	
				// Deselect your counter
				selectedCounter = null;
				
				return true;
			}
		}
		
		return false;
	}
	
	#endregion

	// ====================================================================================================================

	#region private methods	

	/// <summary>
	/// Initialises a GhostCounter to show on raycast.
	/// </summary>
	private void _InitGhostCounter() {

		GameObject go = (GameObject)GameObject.Instantiate(FabGhostCounter);
		ghostCounter = go.GetComponent<GhostCounter>();

		if (ghostCounter == null)
			throw new System.Exception("Incorrect prefab passed for GhostCounter!");

		// Instantiate a text object (used to display the GhostCounters health).  
		GameObject tempText = (GameObject)GameObject.Instantiate(FabText);
		
		// Pass the information to the GhostCounter.
		ghostCounter.SetPlayer(this, PlColor, tempText.GetComponent<TextMesh>());
	}

	/// <summary>
	/// Updates the position of the GhostCounter to the position of the mouse.
	/// </summary>
	/// <param name="tile">Tile.</param>
	private void _GhostOverlay(Tile tile) {
		if (ghostCounter)
			ghostCounter.SetTile(tile);
	}
	
	/// <summary>
	/// Returns a reference to a Tile based on Raycast
	/// </summary>
	/// <returns>Reference to tile, or null if no tile at that position</returns>
	/// <param name="hit">Reference to raycast.</param>
	/// <param name="rayCounter">Reference to counter to check if no tile is hit</param>
	private Tile _GetRayTile(RaycastHit hit, GameCounter rayCounter) {

		// Select the tile to move to
		Tile rayTile = hit.collider.GetComponent<Tile>();
		
		// If the raycast hit a GameCounter instead, get its tile.
		if (!rayTile && rayCounter) rayTile = rayCounter.GetTile();

		return rayTile;
	}

	#endregion	
}
