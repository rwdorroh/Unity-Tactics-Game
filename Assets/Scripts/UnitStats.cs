using UnityEngine;

[System.Serializable]
public struct UnitStats
{
	public float speed; // adds movement?
	public float perception; // adds attack range

	public UnitStats(float newSpeed, float newPerception)
	{
		speed = newSpeed;
		perception = newPerception;
	}
}
