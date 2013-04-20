using UnityEngine;
using System.Collections;

public class HumanBehaviour : MonoBehaviour {
	
	public GameObject Zombie;
	public float moveSpeed;
	public int zombieTouchCount;
	public float waypointDistanceThd;
	
	public AudioClip[] help;
	public AudioClip[] thanks;
	public AudioClip[] death;
	public AudioClip transformToZombie;
	
	private Player playerscript;
	private float maxMoveSpeed;
	private float zombieSlowRecover;
	private float deathLimit;
	private bool transforming;
	private float transformTime;
	private float idleCounter;
	private float idleCounterMax;
	private float idleMoveSpeed;
	private Vector3 waypoint;
	private bool justSpawned;
	private bool isIdle;
	public bool isScared;
	private GameManager gameManager;
	private float helpTimer;
	private float maxHelpTimer;

	// Use this for initialization
	void Start () {
		
		maxMoveSpeed = 250f;
		moveSpeed = maxMoveSpeed;
		idleCounter = 0;
		idleCounterMax = 3f;
		idleMoveSpeed = 50f;
		transforming = false;
		transformTime = 5f;
		deathLimit = 20f;
		zombieSlowRecover = 40f;
		waypointDistanceThd = 1.5f;
		playerscript = (Player)FindObjectOfType(typeof(Player));
		waypoint = GameObject.FindGameObjectWithTag("Player").transform.position + Vector3.up;
		justSpawned = true;
		isIdle = false;
		gameManager = (GameManager)GameObject.Find ("GameManager").GetComponent("GameManager");
		gameManager.humanCount++;
		maxHelpTimer = 5f;
		helpTimer = 0.1f;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (transforming)
		{
			BecomeZombie ();
		}
		else
		{
			setWaypoint ();
			if (Vector3.Distance(waypoint, transform.position) <= waypointDistanceThd || !(transform.FindChild("Head").renderer.isVisible))
			{
				isIdle = true;
				IdleMove ();
			}
			else
			{
				isIdle = false;
				MoveToWaypoint ();	
			}
		
			if (moveSpeed != maxMoveSpeed)
			{
				if (zombieTouchCount == 0) 
				{
					moveSpeed += zombieSlowRecover * Time.deltaTime;
					if (moveSpeed > maxMoveSpeed) moveSpeed = maxMoveSpeed;
					isScared = false;
				}
				else
				{
					isScared = true;
					helpTimer -= Time.deltaTime;
					if (helpTimer <= 0) 
					{
						AudioSource.PlayClipAtPoint(help[Random.Range(0,help.Length)], transform.position);
						helpTimer = maxHelpTimer;
					}
				}
			}
			AmIDeadYet();
		}
	}
	
	void KillHuman()
	{
		Debug.Log ("Kill Human called");
		transforming = true;
		transform.Rotate(new Vector3(90,0,0));		//Fall on floor
		transform.position += new Vector3(0,-0.5f,0);
		gameObject.GetComponent<CharacterController>().enabled = false; //Disable physics
		gameObject.tag = "DeadHuman";
		gameManager.humanCount--;
		AudioSource.PlayClipAtPoint(death[Random.Range(0,death.Length)],transform.position);
	}
	
	void BecomeZombie()
	{
		if (transformTime <= 0)
		{
			AudioSource.PlayClipAtPoint(transformToZombie,transform.position);
			transforming = false;
			transform.Rotate(new Vector3(-90,0,0));		//Stand up
			transform.position += new Vector3(0,1.1f,0);
			Instantiate(Zombie, transform.position, transform.rotation);
			gameManager.zombieCount ++;
			Destroy (gameObject);
			return;
		}
		transformTime -= Time.deltaTime;
	}
	
	void setWaypoint ()
	{
		if (justSpawned)
		{
			if (Input.GetButtonDown ("Fire2"))
			{
				waypoint = playerscript.wayPointOrder;
				justSpawned = false;
			}
			else
			{
				waypoint = playerscript.gameObject.transform.position + Vector3.up;	
				if (isIdle)
				{
					AudioSource.PlayClipAtPoint(thanks[Random.Range(0,thanks.Length)],transform.position);
					justSpawned = false;
				}
			}
		}
		else
		{
			if (isScared)
			{
				waypoint = playerscript.gameObject.transform.position + Vector3.up;
			}
			else
			{
				waypoint = playerscript.wayPointOrder;	
			}
		}
	}
	
	void IdleMove() 
	{
		if (idleCounter > 0) 
		{
			Vector3 lclMoveCmd = idleMoveSpeed * Time.deltaTime * transform.TransformDirection(Vector3.forward);
			GetComponent<CharacterController>().SimpleMove(lclMoveCmd);
			idleCounter -= Time.deltaTime;
		}
		else
		{
			idleCounter = idleCounterMax;
			transform.Rotate(0, Random.Range(0,360), 0);
		}
	}	
	
	void MoveToWaypoint ()
	{
		transform.LookAt(waypoint);
		Vector3 lclMoveCmd = moveSpeed * Time.deltaTime * transform.TransformDirection(Vector3.forward);
		GetComponent<CharacterController>().SimpleMove(lclMoveCmd);
	}
	
	void AmIDeadYet()
	{
		if (moveSpeed <= deathLimit)
		{
			KillHuman();
		}
	}
}
