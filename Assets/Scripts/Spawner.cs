using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {


	public GameObject player;
	public GameObject zombie;
	public GameObject human;
	public GameObject ammo;
	
	private bool isCollided;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public bool Spawn( GameManager.Instance newInstance ) {
		
		if (renderer.isVisible || isCollided ) {
			return false;	
		}
		
		switch (newInstance) {
		case GameManager.Instance.Zombie:
			Instantiate(zombie, transform.position, transform.rotation);
			break;
		case GameManager.Instance.Human:
			Instantiate(human, transform.position, transform.rotation);
			break;
		case GameManager.Instance.Player:
			Instantiate(player, transform.position, transform.rotation);
			break;
		case GameManager.Instance.Ammo:
			Instantiate(ammo,transform.position,transform.rotation);
			break;
		default:
			Debug.Log ("Spawner asked to spawn unknown Instance type");
			break;
		}
		return true;
	}
	
	void OnTriggerEnter () {
		isCollided = true;	
	}
	
	void OnTriggerExit() {
		isCollided = false;	
	}
	
	
}
