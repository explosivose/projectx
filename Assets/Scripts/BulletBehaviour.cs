using UnityEngine;
using System.Collections;

public class BulletBehaviour : MonoBehaviour {
	
	public AudioClip hitFlesh;
	
	public float BulletForce = 3000f;
	public float BulletLifetime = 0.3f;
	private bool BulletHit = false;
	private Collider ObjectHit;
	
	// Use this for initialization
	void Start () {
		rigidbody.AddForce(transform.TransformDirection(Vector3.forward)*BulletForce);
		BulletHit = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (BulletHit == true) {
			BulletLifetime -= Time.deltaTime;
			Debug.DrawLine(ObjectHit.transform.position, transform.position, Color.blue, 1f);
		}
		if (BulletLifetime <= 0f) {
			Debug.Log("Bullet Destroyed");
			Destroy (gameObject);
		}
	}
	
	void OnCollisionEnter (Collision collision)
	{
		ObjectHit = collision.collider;
		if (ObjectHit.gameObject.tag == "Zombie" || ObjectHit.gameObject.tag == "Human")
		{
			AudioSource.PlayClipAtPoint(hitFlesh, transform.position);
			Destroy (gameObject);
		}
		BulletHit = true;
		Debug.DrawLine(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position, Color.yellow, 1f);
	}
}