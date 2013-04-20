using UnityEngine;
using System.Collections;

public class ZombieMove : MonoBehaviour 
{
	
	public float moveSpeed;
	public GameObject ChaseTarget;
	public GameObject Human;
	
	public AudioClip[] idle;
	public AudioClip[] alert;
	public AudioClip[] zombieDeath;
	public AudioClip transformToHuman;
	
	private bool isIdle;
	private float idleCounter;
	private float idleCounterMax;
	private float idleMoveSpeed;
	private bool targetContact;
	private float zombieSlowBy;
	private GameManager gameManager;
	private BoxCollider BoxOfSight;
	private bool transforming;
	private float maxTransformTime;
	private float transformTime;
	private int idleSoundIndex;
	private int alertSoundIndex;
	private int zombieDeathIndex;
	
	void Awake ()
	{
		Debug.Log ("Zombie instantiated!");
		moveSpeed = 250f;
		isIdle = true;
		ChaseTarget = null;
		transforming = false;
		idleCounter = 0;
		idleCounterMax = 3f;
		idleMoveSpeed = 50f;
		targetContact = false;
		zombieSlowBy = 20;
		transforming = false;
		maxTransformTime = 1.5f;
		transformTime = maxTransformTime;
		idleSoundIndex = Random.Range(0, idle.Length);
		alertSoundIndex = Random.Range(0, alert.Length);
		zombieDeathIndex = Random.Range(0,zombieDeath.Length);
		//Enables idle zombies to 'see' humans and the player
		BoxOfSight = (BoxCollider)transform.GetComponent(typeof(BoxCollider));
		BoxOfSight.enabled = true;
/* BUG
 * Player.laser will collide with BoxOfSight.
 * Attempted to assign BoxOfSight.layer = 2 (ignore raycast)
 * Could not assign layer to component, assigned to gameobject instead
 * Effect: player.laser will not collide with zombies (including BoxOfSight)
 */
		BoxOfSight.gameObject.layer = 2; 
		gameManager = (GameManager)GameObject.Find ("GameManager").GetComponent("GameManager");
	}
	
	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (transforming)
		{
			BecomeHuman();
		}
		else{
			if (isIdle) 
			{
				IdleMove();
			}
			else
			{
				ChaseMove();
			}
		}
	}
	
		//	Wonder around aimlessly
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
			
			if (idleSoundIndex < 0)
			{
				idleSoundIndex = Random.Range(0, idle.Length);
				AudioSource.PlayClipAtPoint(idle[idleSoundIndex], transform.position);
			}			
			idleSoundIndex --;
		}
		
	}
	
	//	Set new ChaseTarget (also set Idle = false)
	void StartChasing(GameObject newChaseTarget)
	{
		AudioSource.PlayClipAtPoint(alert[alertSoundIndex], transform.position);
		isIdle = false;
		ChaseTarget = newChaseTarget;
		BoxOfSight.enabled = false;
	}
	
	void ChaseMove()
	{
		if (ChaseTarget == null || ChaseTarget.tag == "DeadHuman") 
		{
			isIdle = true;
			BoxOfSight.enabled = true;
			return;
		}
		//Look at target
		Vector3 lclChaseTargetLocation = ChaseTarget.transform.position;
		lclChaseTargetLocation.y = transform.position.y;
		transform.LookAt(lclChaseTargetLocation);
		
		//Move forward
		Vector3 lclMoveCmd = moveSpeed * Time.deltaTime * transform.TransformDirection(Vector3.forward);
		GetComponent<CharacterController>().SimpleMove(lclMoveCmd);
		
		if (targetContact) 
		{
			DamageTarget();
		}	
	}	
	
	void DamageTarget()
	{
		if (ChaseTarget.tag == "Player")
		{
			Player playerscript = ChaseTarget.GetComponent<Player>();
			playerscript.moveSpeed -= zombieSlowBy * Time.deltaTime;
		}
		else if (ChaseTarget.tag == "Human")
		{
			HumanBehaviour humanscript = ChaseTarget.GetComponent<HumanBehaviour>();
			humanscript.moveSpeed -= 5 * zombieSlowBy * Time.deltaTime;
		}
	}
	
	void KillZombie()
	{
		Debug.Log ("Zombie killed by Bullet.");
		transforming = true;
		transform.Rotate(new Vector3(90,0,0));		//Fall on floor
		transform.position += new Vector3(0,-1,0);
		gameObject.GetComponent<CharacterController>().enabled = false; //Disable physics
		gameObject.tag = "DeadZombie";
		AudioSource.PlayClipAtPoint(zombieDeath[zombieDeathIndex],transform.position);
		gameManager.zombieCount--;
		if (targetContact && ChaseTarget.tag == "Player") 
		{
			Player playerscript = ChaseTarget.GetComponent<Player>();
			playerscript.zombieTouchCount --;
			targetContact = false;
		}
		else if (targetContact && ChaseTarget.tag == "Human") 
		{
			HumanBehaviour humanscript = ChaseTarget.GetComponent<HumanBehaviour>();
			humanscript.zombieTouchCount --;
			targetContact = false;
		}
	}
	
	void BecomeHuman()
	{
		if (transformTime == maxTransformTime) AudioSource.PlayClipAtPoint(transformToHuman,transform.position);
		if (transformTime <= 0)
		{
			transforming = false;
			transform.Rotate(new Vector3(-90,0,0));		//Stand up
			transform.position += new Vector3(0,1,0);
			Instantiate(Human, transform.position, transform.rotation);
			Destroy (gameObject);
			return;
		}
		transformTime -= Time.deltaTime;
	}
	
	void OnCollisionEnter(Collision collision) 
	{
		if (collision.collider.tag == "Bullet") 
		{
			KillZombie();		
		}			
	}
	
// I think that both the following bugs could be fixed with some use of OnTriggerStay?
/* BUG
 * Going from Idle to Chasing does not disable BoxOfSight correctly...
 * This is what happens in OnTriggerEnter: If (isIdle) and we see a player then StartChasing();
 * StartChasing will set isIdle = false, so next If statement is entered...
 * This next if statement sets targetContact = true, which is used in ChaseMove to slow the player
 * Effect: Zombies will slow you if they can see you.
 */
/* BUG
 * If you are inside BoxOfSight but Physics.Raycast does not hit you 
 * then you stand in obvious line of sight, Physics.Raycast is not called again
 * so the zombie does not chase you.
 * Effect: Zombies might not chase you if they first 'see' you whilst you're behind something
 */
	void OnTriggerEnter(Collider collider) 
	{

		if (collider.tag == "Player")
		{
			if (isIdle)
			{
				Vector3 lclDirection = (collider.transform.position + Vector3.up) - transform.position;
				RaycastHit lclHitInfo;
				Debug.DrawRay(transform.position, lclDirection);
				if (Physics.Raycast (transform.position, lclDirection, out lclHitInfo))
				{
					if (lclHitInfo.collider.gameObject.tag == collider.tag)
					{
						Debug.DrawRay(transform.position, lclDirection,Color.red,1f);
						StartChasing (collider.gameObject);
					}
				}
			}
			else
			{
				targetContact = true;
				Player playerscript = ChaseTarget.GetComponent<Player>();
				playerscript.zombieTouchCount ++;
				DamageTarget();
			}
		}	
		else if (collider.tag == "Human")
		{
			
			if (isIdle)
			{
				Vector3 lclDirection = (collider.transform.position) - transform.position;
				RaycastHit lclHitInfo;
				Debug.DrawRay(transform.position, lclDirection,Color.white,5f);
				if (Physics.Raycast (transform.position, lclDirection, out lclHitInfo))
				{
					if (lclHitInfo.collider.gameObject.tag == collider.tag)
					{
						Debug.DrawRay(transform.position, lclDirection,Color.red,1f);
						StartChasing (collider.gameObject);
					}
				}
			}
			else
			{
				targetContact = true;
				HumanBehaviour humanscript = ChaseTarget.GetComponent<HumanBehaviour>();
				humanscript.zombieTouchCount ++;
				DamageTarget();
				Debug.Log ("Damage zombie target (human)");
			}
			
		}	
	}	
	
	void OnTriggerExit(Collider collider) 
	{
		
		if (targetContact) 
		{
			if (collider.tag == "Player") 
			{
				Player playerscript = ChaseTarget.GetComponent<Player>();
				playerscript.zombieTouchCount --;
				targetContact = false;
			}
			if (collider.tag == "Human") 
			{
				HumanBehaviour humanscript = ChaseTarget.GetComponent<HumanBehaviour>();
				humanscript.zombieTouchCount --;
				targetContact = false;
			}
		}
	}
}
