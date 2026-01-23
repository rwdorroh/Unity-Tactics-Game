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

	private void Start()
	{
		tileRenderer = GetComponentInChildren<Renderer>();
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
			ChangeColor(inMoveRange? Color.cyan : originalColor);
		}
	}

	// When mouse clicks tile
	public void OnPointerDown(PointerEventData eventData)
	{
		SelectTile();

		if(Player.selectedUnit)
		{
			if(inMoveRange)
			{
				Player.selectedUnit.MoveTo(transform.position, gridPosition);
			} 
			else
			{
				Player.selectedUnit = null;
			}
		}
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
