using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour {

	Transform target;

	NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		StartCoroutine(CheckForTarget());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator CheckForTarget()
	{

		//Debug.Log("Checking" + Time.time);
		while (target)
		{
			agent.SetDestination(target.position);
			FaceTarget();
			yield return new WaitForSeconds(0.03f);
		}
		yield return new WaitForSeconds(0.2f);
		StartCoroutine(CheckForTarget());
	}

	public void MoveToPoint (Vector3 point)
	{
		agent.SetDestination(point);
	}

	public void FollowTarget(Interactable newTarget)
	{
		agent.stoppingDistance = newTarget.radius * 0.8f;
		agent.updateRotation = false;

		target = newTarget.interactionTransform;
	}

	public void StopFollowingTarget()
	{
		agent.stoppingDistance = 0f;
		agent.updateRotation = true;

		target = null;
	}

	void FaceTarget()
	{
		
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));

		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
	}
}
