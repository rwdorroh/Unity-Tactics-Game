using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
	[Header("Map Setting")]
	public int width = 10;
	public int height = 10;
	public GameObject tilePrefab;
	public string valueMap;
	public const int MAX_MOVE_COST = 5;

	[Header("Materials")]
	public Material lightMaterial;
	public Material darkMaterial;
	public Gradient terrainColors;

	private Tile[,] map;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Awake()
	{
		map = new Tile[width, height];
		GenerateGrid();

	}

	// Generates the grid on play
	public void GenerateGrid()
	{
		for (int x = 0; x<width; x++) 
		{
			for (int y = 0; y<height; y++)
			{
				Vector3 tilePosition = new Vector3(x - width/2, 0, y - height/2);
				GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
				tile.name = $"Tile {x},{y}";
				tile.transform.SetParent(transform);

				Renderer renderer = tile.GetComponentInChildren<Renderer>();

				renderer.material = new Material((x + y) % 2 == 0 ? lightMaterial : darkMaterial);

				Tile tileScript = tile.GetComponent<Tile>();
				tileScript.gridPosition = new Vector2Int(x, y);
				tileScript.moveCost = valueMap[x * width + y] - '0';
				float normalizedCost = (float)tileScript.moveCost / MAX_MOVE_COST;
				tileScript.originalColor = tileScript.moveCost > MAX_MOVE_COST ? Color.red : terrainColors.Evaluate(normalizedCost);
				map[x, y] = tileScript;
			}
		}
	}

	// Get tile neighbors for movement / highlighting
	private List<Tile> GetTileNeighbors(Vector2Int tilePosition)
	{
		List<Tile> neighbors = new List<Tile>();

		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				int posX = tilePosition.x + x;
				int posY = tilePosition.y + y;

				if(posX >= 0 && posY >= 0 && posX < width && posY < height && new Vector2Int(posX, posY) != tilePosition)
				{
					neighbors.Add(map[posX, posY]);
				}

			}
		}

		return neighbors;
	}

	// Diagonal tile check
	private bool IsDiagonal(Tile a, Tile b)
	{
		int dx = Mathf.Abs(a.gridPosition.x - b.gridPosition.x);
		int dy = Mathf.Abs(a.gridPosition.y - b.gridPosition.y);

		return dx == 1 && dy == 1;
	}

	// Clears grid highlights
	private void ResetGridHighlights()
	{
		foreach(Tile tile in map)
		{
			if (Tile.selectedTile != tile)
			{
				tile.inMoveRange = false;
				tile.ChangeColor(tile.originalColor);
			}
		}
	}
	
	// Calculates tiles to be highlighted using Dijkstra's
	public List<Tile> GetHighlightRange(Vector2Int start, int range)
	{
		ResetGridHighlights();

		List<Tile> reachable = new List<Tile>();
		Dictionary<Tile, int> costSoFar = new Dictionary<Tile, int>();
		Queue<Tile> edge = new Queue<Tile>();

		Tile startTile = map[start.x, start.y];
		edge.Enqueue(startTile);
		costSoFar[startTile] = 0;

		while(edge.Count >0)
		{
			Tile current = edge.Dequeue();
			int currentCost = costSoFar[current];

			foreach (Tile neighbor in GetTileNeighbors(current.gridPosition))
			{
				int stepCost = IsDiagonal(current, neighbor) ? 1 + neighbor.moveCost : neighbor.moveCost;
				int newCost = currentCost + stepCost;

				if (newCost <= range && (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor]) && !neighbor.isOccupied)
				{
					costSoFar[neighbor] = newCost;
					edge.Enqueue(neighbor);

					if(!reachable.Contains(neighbor) && neighbor !=  startTile)
					{
						reachable.Add(neighbor);
						neighbor.inMoveRange = true;
					}
				}
			}
		}

		return reachable;
	}

	// Highlights tiles in move range
	public void HighlightMoveRange(Tile start, int range)
	{
		List<Tile> reachableTiles = GetHighlightRange(start.gridPosition, range);

		foreach(Tile tile in reachableTiles)
		{
			tile.ChangeColor(Color.cyan);
		}
	}

	// Getter for a tile
	public Tile GetTile(Vector2Int position)
	{
		return map[position.x, position.y];
	}

	public Tile GetTile(Vector3 position)
	{
		float offsetX = width / 2f;
		float offsetY = height / 2f;

		int x = Mathf.RoundToInt(position.x + offsetX);
		int y = Mathf.RoundToInt(position.z + offsetY);

		return map[x, y];
	}

	// Manhatten heuristic for A*
	private int GetHeuristic (Tile start, Tile end)
	{
		return Mathf.Abs(start.gridPosition.x - end.gridPosition.x) + Mathf.Abs(start.gridPosition.y - end.gridPosition.y);
	}

	private List<Tile> RetracePath(Tile start, Tile end)
	{
		List<Tile> path = new List<Tile>();

		Tile current = end;

		while (current != start)
		{
			path.Add(current);
			current = current.parent;
		}

		path.Reverse();
		return path;
	}

	public List<Tile> GetPath(Tile start, Tile end)
	{
		List<Tile> open = new List<Tile>();
		HashSet<Tile> closed = new HashSet<Tile>();

		open.Add(start);

		start.gCost = 0;
		start.hCost = GetHeuristic(start, end);
		start.parent = null;

		while (open.Count > 0)
		{
			Tile current = open.OrderBy(t => t.fCost).ThenBy(t => t.hCost).First();

			if (current == end)
			{
				return RetracePath(start, end);
			}

			open.Remove(current);
			closed.Add(current);

			foreach(Tile neighbor in GetTileNeighbors(current.gridPosition))
			{
				if (closed.Contains(neighbor) || neighbor.isOccupied) continue;

				int tempG = current.gCost + neighbor.moveCost;

				if (!open.Contains(neighbor) || tempG < neighbor.gCost)
				{
					neighbor.gCost = tempG;
					neighbor.hCost = GetHeuristic(neighbor, end);
					neighbor.parent = current;
					if(!open.Contains(neighbor))
					{
						open.Add(neighbor);
					}
				}
			}
		}

		return null;
	}

}
