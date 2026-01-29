using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	
	public Vector2Int gridPosition;

	[Header("Colors")]
	public Color originalColor;
	public Color highlightColor = Color.yellow;
	public Color selectedColor = Color.blue;

	private Renderer tileRenderer;
	public static Tile selectedTile;
	public bool inMoveRange = false;
	public bool inAttackRange = false;

	public int moveCost = 0;

	public bool isOccupied = false;

	public int gCost = int.MaxValue;
	public int hCost = 0;
	public int fCost => gCost = hCost;

	public Tile parent;

	private void Start()
	{
		tileRenderer = GetComponentInChildren<Renderer>();
		ChangeColor(originalColor);
	}

	// Set color of a tile
	public void ChangeColor(Color newColor)
	{
		tileRenderer.material.color = newColor;

	}

	// Color change when mouse enters tile
	public void OnPointerEnter(PointerEventData eventData)
	{
		if(selectedTile != this)
		{
			ChangeColor(highlightColor);
		}
	}

	// Color change when mouse leaves tile
	public void OnPointerExit(PointerEventData eventData)
	{
		if (selectedTile != this)
		{
			ChangeColor(inMoveRange? Color.cyan : inAttackRange ? Color.red : originalColor);
		}
	}

	// When mouse clicks tile
	public void OnPointerDown(PointerEventData eventData)
	{

		GridManager gridManager = GetComponentInParent<GridManager>();

		if(Player.selectedUnit)
		{
			if (Player.selectedUnit.isMoving) return;

			if(inMoveRange)
			{
				gridManager.GetTile(Player.selectedUnit.gridPosition).isOccupied = false;
				Player.selectedUnit.path = gridManager.GetPath(gridManager.GetTile(Player.selectedUnit.gridPosition), this);
				Player.selectedUnit.MoveTo(Player.selectedUnit.path[0].transform.position, Player.selectedUnit.path[0].gridPosition);
				isOccupied = true;
			} 
			else
			{
				Player.selectedUnit = null;
			}
		}

		SelectTile();
		gridManager.ResetGridHighlights();
	}

	private void SelectTile()
	{
		if (selectedTile)
		{
			selectedTile.ChangeColor(selectedTile.originalColor);
		}

		selectedTile = this;
		ChangeColor(selectedColor);
	}
}
