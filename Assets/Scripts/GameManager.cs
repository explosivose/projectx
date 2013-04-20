using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	public GameObject MainCamera;
	
	public enum Instance
	{
		Player,
		Zombie,
		Human,
		Ammo,
	}
	
	public float zombieSpawnTime;
	public float ammoSpawnTime;
	public int maxZombieCount;
	public int zombieCount;
	public int humanCount;
	public float score;
	
	private Spawner[] spawnAreas;
	private float curZombieSpawnTime;
	private float curAmmoSpawnTime;
	private Player playerScript;
	
	// Use this for initialization
	void Start () {
		score = 0f;
		zombieSpawnTime = 1f;
		curZombieSpawnTime = zombieSpawnTime;
		ammoSpawnTime = 10f;
		maxZombieCount = 50;
		zombieCount = 0;
		humanCount = 0;
		
		spawnAreas = FindObjectsOfType(typeof(Spawner)) as Spawner[];
		Debug.Log ("Number of spawners in scene: " + spawnAreas.Length);
		
		int lclSpawnerIndex = Random.Range(0, spawnAreas.Length);
		spawnAreas[lclSpawnerIndex].Spawn(Instance.Player);
		
		lclSpawnerIndex = Random.Range(0, spawnAreas.Length);
		Instantiate(MainCamera, spawnAreas[lclSpawnerIndex].transform.position + (Vector3.up * 10f), spawnAreas[lclSpawnerIndex].transform.rotation);
		
		playerScript = (Player)GameObject.FindGameObjectWithTag ("Player").GetComponent("Player");
		
	}
	
	// Update is called once per frame
	void Update () 
	{		
		int lclSpawnerIndex;
		
		if (zombieCount < maxZombieCount) {
			
			if (curZombieSpawnTime <= 0) {
				lclSpawnerIndex = Random.Range(0, spawnAreas.Length);
				if ( spawnAreas[lclSpawnerIndex].Spawn(Instance.Zombie) ) {
					curZombieSpawnTime = zombieSpawnTime;
					zombieCount++;
				}
			}
			else {
				curZombieSpawnTime -= Time.deltaTime;
			}
		}
		
		if (curAmmoSpawnTime <= 0) {
			lclSpawnerIndex = Random.Range(0,spawnAreas.Length);
			if ( spawnAreas[lclSpawnerIndex].Spawn(Instance.Ammo) ) {
				curAmmoSpawnTime = ammoSpawnTime;	
			}
		}
		else {
			curAmmoSpawnTime -= Time.deltaTime;	
		}
		score += humanCount * Time.deltaTime;
	}

	void OnGUI () {
		// Make a background box
		GUI.Box(new Rect(10,10,150,30), "Score = " + Mathf.Round(score));
		GUI.Box(new Rect(10,40,150,30), "Ammo = " + playerScript.ammo);
	}

}


