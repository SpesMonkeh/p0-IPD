using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElementBehaviour : MonoBehaviour
{
	[SerializeField] Rigidbody2D elementRigidbody;
	[SerializeField] Vector2 elementMovementVector;
	
	
	void Awake()
	{
		elementRigidbody = GetComponent<Rigidbody2D>();
	}

	void FixedUpdate()
	{
		elementRigidbody.AddForce(elementMovementVector, ForceMode2D.Impulse);
	}

	public void OnMoveUp(InputAction.CallbackContext ctx)
	{
		if(ctx.performed)
			elementMovementVector = Vector2.up;
		else if(ctx.canceled)
			elementMovementVector = Vector2.zero;

	}
}