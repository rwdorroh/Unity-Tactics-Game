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

	public void ChangeColor(Color newColor)
	{
		tileRenderer.material.color = newColor;

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if(selectedTile != this)
		{
			ChangeColor(highlightColor);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (selectedTile != this)
		{
			ChangeColor(inMoveRange? Color.cyan : originalColor);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (selectedTile)
		{
			selectedTile.ChangeColor(selectedTile.originalColor);
		}

		selectedTile = this;
		ChangeColor(selectedColor);

		FindAnyObjectByType<GridManager>().HighlightMoveRange(this, FindAnyObjectByType<Unit>().movementRange);
	}

}
