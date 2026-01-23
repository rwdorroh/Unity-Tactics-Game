using UnityEngine;

public class Player : MonoBehaviour
{
	public static Unit selectedUnit;
	public GridManager gridManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		gridManager = FindAnyObjectByType<GridManager>();
    }

	public void ChangeSelectedUnit(Unit unit)
	{
		selectedUnit = unit;
		gridManager.HighlightMoveRange(gridManager.getTile(unit.gridPosition), unit.movementRange);
	}
}
