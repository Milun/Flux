/**
 * Game.cs
 * 
 * Author: 	Milton Plotkin
 * Date:	3/12/2013
 * 
 * Executes the main game loop and checks Player states for victory/loss and removes them accordingly.
 * 
 * This class initialised the game board using Tiles, and is passed information on Players and Camera
 * through the Unity GUI. Specifications on board size and player amount are provided through PlayerPrefs,
 * which are defined in the previous scene: "menu".
 * 
 * The game loop waits for a player to make input before executing. When a player does make input,
 * the loop will execute, but will halt to play a text animation if required. After the animation
 * finishes playing, the loop resumes execution. This is accomplished using the "dontBlock" variable. 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour {

	#region inspector values
	// These values are defined in the Unity GUI
	public List<Player> Players;	// All possible players that this game might have (predefined).
	public GameObject FabTile;		// Reference to the tile prefab which will be used to create the board.
	public TextMessage Message;		// Reference to the text animation which plays on action.
	public Camera Cam;				// Reference to the games camera.

	public Color TileColor1;		// The color for odd tiles.
	public Color TileColor2;		// The color for even tiles.
	#endregion

	#region vars
	private List<Tile> board;		// Reference to all tiles in the board.
	private int currentPlayer = 0;	// The iterator for the current player in "Players".

	private bool dontBlock = false;	// Used with "Message" to check player states while skipping waiting for input.
	#endregion

	// ====================================================================================================================

	/// <summary>
	/// Initializes game entities.
	/// </summary>
	void Start () {

		// Initialize board to a custom size.
		board = new List<Tile>();

		int boardSize = 4;
		if (PlayerPrefs.HasKey ("BoardSize"))
			boardSize = PlayerPrefs.GetInt ("BoardSize");

		// Give each player a custom number of counters to start with.
		int startCounters = 4;
		if (PlayerPrefs.HasKey ("StartCounters"))
			startCounters = PlayerPrefs.GetInt ("StartCounters");

		// Have this many players in the game.
		int maxPlayers = 2;
		if (PlayerPrefs.HasKey ("PlayerSize"))
			maxPlayers = PlayerPrefs.GetInt ("PlayerSize");

		_InitBoard (boardSize,boardSize);
		_InitPlayers(boardSize, maxPlayers, startCounters);
		_InitCamera(boardSize);
		_InitMessage(boardSize);
	}

	// ====================================================================================================================

	#region game loop

	/// <summary>
	/// The main game loop.
	/// </summary>
	void Update () {

		///////////////////////////
		/// PERFORMED EACH STEP ///
		///////////////////////////

		// Make inactive players have counters appear faded.
		foreach (Player e in Players)
		{
			if (currentPlayer < Players.Count)
			{
				if (e != Players[currentPlayer])
					e.Deactivate(); // Executing this makes player counters appear faded.
			}
			else
				e.Deactivate();
		}
		

		// Text animation //
		// Don't allow any input/processing during the text animation.
		if (Message.IsAnimating()) return;


		/////////////////////////////////////////////////////////////////////
		/// GAME LOOP (Waits for player input unless "dontBlock" is true) ///
		/////////////////////////////////////////////////////////////////////

		// The following executes when the current player makes a valid move.
		// It will skip waiting for input if a message has just finished playing.
		// This makes the game loop both blocking and non-blocking.
		if (dontBlock || Players[currentPlayer].Turn())
		{	
			///////////////////////////
			// Check for a stalemate //
			///////////////////////////
			// Returns true if the player directly after this one was removed.
			// By returning, it will loop again to check if the new next player is also in stalemate.
			// _CheckStalemate sets "dontBlock" to true.
			if (_CheckStalemate())
				return;

			//////////////////////////////////////////
			// If it's the end of last players turn //
			//////////////////////////////////////////
			if (_GetNextPlayer() == 0)
			{
				// Check for any dead players.
				// This is different to stalemate, as it checks whether they have no counters on the board
				// at the end of the last players turn only.
				if (_RemoveDeadPlayers())
					return;	// If a dead player is found, play a text animation before proceeding.
				
				// Check for a winner.
				if (Players.Count == 1) // If there's only one player left that hasn't been deleted.
				{
					if (Players[0])
					{
						if (_SetMessage(Players[0].GetName () + " WINS!"))
							return; // Wait for the victory message to play before proceeding.
						else
							Application.LoadLevel ("menu"); // Go back to the menu (reset the game).
					}
					else
						throw new System.Exception("Winning player has no allocated memory!");
				}
				else if (Players.Count == 0) // The game rules prevent a tie from ever occuring.
				{
					throw new System.Exception("No players left in memory!");
				}
				else
				{
					// Play a text animation before executing the Flux.
					if (_SetMessage("FLUX!"))
						return;
					
					foreach (Player e in Players)
						e.Flux();
					
					// Wait for the next players input before proceeding.
					dontBlock = false;
				}
			}

			// Set the current player to the next player in line.
			currentPlayer = _GetNextPlayer();
			Message.ClearText();
		}
	}
	#endregion

	// ====================================================================================================================

	#region private methods

	/// <summary>
	/// Center the message animation on the screen based on board size.
	/// </summary>
	/// <param name="boardSize">Board size</param>
	private void _InitMessage(int boardSize) {
		Message.transform.position = new Vector3((float)boardSize/2f, 1f, (float)boardSize/2f);
	}

	/// <summary>
	/// Checks if the player directly after the currentPlayer can make any valid moves.
	/// If a player has stalemated, this will remove that player and tell the game to play a 
	/// text animation stating this.
	/// 
	/// Returns true if a player was removed.
	/// </summary>
	private bool _CheckStalemate() {
		///////////////////////////
		// Check for a stalemate //
		///////////////////////////
		
		// If the player after this one cannot make a move, make them lose!
		if (Players[_GetNextPlayer()].CheckStalemate())
		{
			// Make a text animation play stating that this player has lost. Continue
			// functioning only AFTER the animation has finished playing.
			if (_SetMessage(Players[_GetNextPlayer()].GetName() + " CAN'T MAKE A MOVE!"))
				return true;
			
			// After the animation has finished playing, the code will return to this point.
			_DeletePlayer(_GetNextPlayer());
		}

		return false;
	}

	/// <summary>
	/// Initialize a custom number of players and remove the rest.
	/// boardSize is used to determine the positions of their off-board counters.
	/// </summary>
	/// <param name="boardSize">Board size</param>
	/// <param name="maxPlayers">How many players this game will have</param> 
	/// <param name="startCounters">How many counters each player starts with</param>
	private void _InitPlayers(int boardSize, int maxPlayers, int startCounters)
	{

		if (maxPlayers > Players.Count)
			throw new System.Exception("Attempted to use more players than are available!");
		
		// Initiate players on both sides of the board.
		for (int i = 0; i < Players.Count; i++)
		{
			// Remove players we won't be using.
			if (i >= maxPlayers) {
				_DeletePlayer(i);
				i--;

				continue;
			}

			// Odd players at the bottom of the board, even at the top.
			if (i % 2 == 0)
				Players[i].InitPlayer((i+1)*-0.5f-0.5f, startCounters);
			else
				Players[i].InitPlayer((i+1)*0.5f + boardSize - 1f, startCounters);
		}
	}

	/// <summary>
	/// Centers the camera to fit the board size
	/// </summary>
	/// <param name="boardSize">Board size.</param>
	private void _InitCamera(int boardSize)
	{
		// Have the camera zoom to fit the board.
		Cam.transform.position = new Vector3((float)boardSize/2f,
		                                     (float)boardSize + 1f + 4f,
		                                     (float)boardSize/-12f + ((float)boardSize-3)*0.2f - 1f);
	}

	/// <summary>
	/// Initiates a board with "w" x "h" tiles, spaced "dist" appart.
	/// </summary>
	/// <param name="w">Ammount of columns.</param>
	/// <param name="h">Ammount of rows.</param>
	/// <param name="dist">Distance between tile centers</param>
	private void _InitBoard(int w, int h, float dist = 1f)
	{
		// Create unlinked tiles.
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				GameObject temp = (GameObject)GameObject.Instantiate(FabTile);
				temp.transform.position = new Vector3(i*dist, 0f, j*dist);
				Tile tile = temp.GetComponent<Tile>();

				// Store their references in case we need access to them directly.
				board.Add (tile);

				// Set their colors to create the checkered effect
				if ((i+j) % 2 == 0)
					tile.SetColor(TileColor1);
				else
					tile.SetColor(TileColor2);
			}
		}

		// Link the Tile nodes to their neighbours.
		_LinkTiles(w,h);
	}

	/// <summary>
	/// Removes the player of iterator i in "Players" from the game.
	/// </summary>
	/// <param name="i">The index.</param>
	private void _DeletePlayer(int i) {
		if (i >= Players.Count)
			throw new System.Exception("Attempted to delete non-existant player!");
		
		// Have the player call its destructor functions
		Players[i].Destruct ();
		
		// Remove the player instance from the game, and remove all references to it.
		Destroy(Players[i].gameObject);
		Players.RemoveAt(i);
	}

	/// <summary>
	/// Links each tile as a node with each of up to 8 surrounding tiles.
	/// </summary>
	/// <param name="w">Ammount of columns.</param>
	/// <param name="h">Ammount of rows.</param>
	private void _LinkTiles(int w, int h) {
		for (int i = 0; i < board.Count; i++)
		{
			// Get position
			int x = i%w;
			int y = (int)i/h;
			
			// Link all surrounding.
			if (x > 0) {
				board[i].AddLink(board[_PosToInt(x-1, y, w)]);
				if (y > 0)
					board[i].AddLink(board[_PosToInt(x-1, y-1, w)]);
			}
			
			if (x < w-1) {
				board[i].AddLink(board[_PosToInt(x+1, y, w)]);
				if (y < h-1)
					board[i].AddLink(board[_PosToInt(x+1, y+1, w)]);
			}
			
			if (y > 0) {
				board[i].AddLink(board[_PosToInt(x, y-1, w)]);
				if (x < w-1)
					board[i].AddLink(board[_PosToInt(x+1, y-1, w)]);
			}
			
			if (y < h-1) {
				board[i].AddLink(board[_PosToInt(x, y+1, w)]);
				if (x > 0)
					board[i].AddLink(board[_PosToInt(x-1, y+1, w)]);
			}
		}
	}

	/// <summary>
	/// Converts a series of x, y coordinates into an iterator for "board".
	/// </summary>
	/// <returns>The position to int.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="w">The amount of columns the board has</param>
	private int _PosToInt(int x, int y, int w) {
		return (w*y)+x;
	}

	/// <summary>
	/// Returns the iterator of the next player (who needs to have a turn).
	/// If the current player is the last one (before a Flux), returns 0.
	/// </summary>
	private int _GetNextPlayer() {
		if (currentPlayer >= Players.Count-1)
			return 0;

		return currentPlayer+1;
	}

	/// <summary>
	/// Called at the end of the last players turn.
	/// Removes the first dead player and plays a text animation.
	/// Returns false if no dead players.
	/// </summary>
	private bool _RemoveDeadPlayers() {
		for (int i = 0; i < Players.Count; i++)
		{
			if (Players[i].IsDead())
			{
				if (_SetMessage(Players[i].GetName() + " HAS BEEN DEFEATED!"))
				{
					_DeletePlayer(i);
					currentPlayer = _GetNextPlayer();

					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Sets the text to show with the text animation and tells it to play.
	/// 
	/// Returns true if a message was successfully set to play.
	/// Returns false if the message being set has just played.
	/// </summary>
	/// <param name="s">The string to display.</param>
	private bool _SetMessage(string s) {

		if (Message.SetText(s))
		{
			dontBlock = true;
			return true;
		}

		return false;
	}

	#endregion

}
