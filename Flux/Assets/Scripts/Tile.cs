using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : ColorObject {
	
	private List<Tile> nodeLinks = new List<Tile>(); // The (up to 8) neighbor nodes for this tile.
	private Counter counter;						// A reference to the counter on this tile (if any).

	private float size = 1.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	new void Update () {
	
		base.Update();
	}

	new public void Glow() {
		base.Glow();

		if (counter) counter.Glow ();
	}

	public void SetCounter(Counter counter) {
		this.counter = counter;
	}

	public void RemoveCounter() {
		this.counter = null;
	}

	public Counter GetCounter() {
		return counter;
	}

	public void TestLinks() {

		foreach(Tile e in nodeLinks)
			e.transform.Translate(new Vector3(0f, 1f, 0f));

		transform.Translate(new Vector3(0f, 1f, 0f));
	}

	public List<Tile> GetNeighbours() {
		return nodeLinks;
	}

	public void AddLink(Tile link) {

		if (nodeLinks.Count <= 8)
			nodeLinks.Add (link);
		else
			throw new System.Exception("Maximum Tile nodeLinks exceeded!");
	}

	public float GetSize() {
		return size;
	}
}
