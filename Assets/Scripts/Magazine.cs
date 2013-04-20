using UnityEngine;
using System.Collections;

public class Magazine : MonoBehaviour {
	
	public AudioClip pickUp;
	
	private Player playerscript;
	
	// Use this for initialization
	void Start () {
		playerscript = (Player)FindObjectOfType(typeof(Player));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collider) 
	{
		if (collider.tag == "Player")
		{
			AudioSource.PlayClipAtPoint(pickUp,transform.position);
			playerscript.ammo += 6;
			Debug.Log("Ammo count = " + playerscript.ammo);
			Destroy (gameObject);
		}
	}
}
