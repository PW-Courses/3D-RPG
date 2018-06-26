using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {

	public LayerMask movementMask;

	Camera cam;
	PlayerMotor motor;

	public Interactable focus;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		motor = GetComponent<PlayerMotor>();

	}
	
	// Update is called once per frame
	void Update () {

		if (EventSystem.current.IsPointerOverGameObject())
			return;

		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 100, movementMask))
			{
				//Debug.Log("We hit " + hit.collider.name + " " + hit.point);
				motor.MoveToPoint(hit.point);
				// Move our player to what we hit

				// Stop focusing any objects
				RemoveFocus();
			}
		}

		if (Input.GetMouseButtonDown(1))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 100))
			{
				//Check if we hit a interactable object

				Interactable interactable = hit.collider.GetComponent<Interactable>();
				if (interactable)
				{
					SetFocus(interactable);
				}
				//If we did set it as our focus
			}
		}
	}

	void SetFocus (Interactable newFocus)
	{
		if (focus != newFocus)
		{
			if (focus != null)
				focus.OnDefocused();

			focus = newFocus;
			motor.FollowTarget(focus);
		}

		focus.OnFocused(transform);
	}

	void RemoveFocus ()
	{
		if (focus != null)
			focus.OnDefocused();

		focus = null;
		motor.StopFollowingTarget();
	}
}

