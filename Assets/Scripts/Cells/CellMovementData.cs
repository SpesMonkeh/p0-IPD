
using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "P0/Programming/Cell Movement Data")]
public class CellMovementData : ScriptableObject
{
	[SerializeField] Vector2 moveDirection;

	public Vector2 MoveDirection => moveDirection;

	
}