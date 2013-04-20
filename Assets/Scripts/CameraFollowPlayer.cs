using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour {
	
	private string TargetName = "Player";
	private float cameraMaxSpeed = 0.1F;
	public float cameraOffsetx = 10;
	public float cameraOffsety = 15;
	public float cameraOffsetz = 10;
	public bool panning;
	// Use this for initialization
	void Start () {
		panning = true;
		transform.LookAt(transform.position - Vector3.up);
	}
	
	// Update is called once per frame
	void Update () {
		
		Vector3 chaseTarget = GameObject.FindGameObjectWithTag(TargetName).transform.position;
		Vector3 target;
		target.x = chaseTarget.x + cameraOffsetx - 1;
		target.y = chaseTarget.y + cameraOffsety;
		target.z = chaseTarget.z + cameraOffsetz - 1;
		transform.position = Vector3.MoveTowards(transform.position, target, cameraMaxSpeed);
		if (panning) {
			if ( transform.position == target ) panning = false;
			transform.LookAt(chaseTarget);
		}
	}
}
