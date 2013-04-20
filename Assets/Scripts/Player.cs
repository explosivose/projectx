using UnityEngine;
using System.Collections;



public class Player : MonoBehaviour {
	
	public GameObject Bullet;
	public GameObject WayPoint;
	
	public float moveSpeed;
	public int zombieTouchCount;
	
	public float attackSpeedFtr = 2f;
	public Vector3 wayPointOrder;
	public int ammo;
	
	public AudioClip[] orderMove;
	public AudioClip[] orderTOME;
	public AudioClip shoot;
	
	private float maxMoveSpeed;
	private float zombieSlowRecover;
	private float deathLimit;
	private float attackTime;
	private bool canIShootYet;
	private bool isDead;
	private bool wayPointSet;
	private Object curWayPoint;
	private bool TOME;
	private CameraFollowPlayer playerCamera;
	private GameManager myManager;
    private bool firstPass;
	
	// Use this for initialization
	void Start () {
		maxMoveSpeed = 250;
		moveSpeed = maxMoveSpeed;
		zombieTouchCount = 0;
		zombieSlowRecover = 20;
		deathLimit = 20;
		attackTime = 1;
		canIShootYet = false;
		isDead = true;
		playerCamera = (CameraFollowPlayer)Camera.main.GetComponent("CameraFollowPlayer");
		myManager = (GameManager)GameObject.Find ("GameManager").GetComponent("GameManager");
		firstPass = true;
		wayPointOrder = transform.position;
		wayPointOrder.y = 1;
		ammo = 6;
		wayPointSet = false;
		Time.timeScale = 0;
		TOME=false;
	}
	
	//	Handle Inputs every frame
	void Update () {
		if (!isDead)
		{
			
			Look();
			Move();
			Shoot();
			Laser();
			GetWayPointOrder();
			if (TOME) wayPointOrder = transform.position + Vector3.up;
		}
		Menu();
		AmIDeadYet();
		//if (Time.timeScale <= 2.5f) {
		//	Time.timeScale += (Time.deltaTime * 0.001f);
		//}
		
	}
	
	void Menu() {
		if (isDead) {
			if (firstPass) {
				Debug.Log ("Fire to play.");
				
			
				if (Input.GetButtonDown("Fire1") && !playerCamera.panning) {
					firstPass = false;
					isDead = false;
					Time.timeScale = 1f;
					Camera.main.transform.GetChild(0).gameObject.SetActive(false);
				}
			}
		}
	}
	
	//	Make the player character look at the mouse
	void Look() {
		Vector3 lclMouseCoord = Input.mousePosition;		//Mouse screen coordinate (x,y,0)
		lclMouseCoord.z = Camera.main.farClipPlane;			//set z to (something)
		Vector3 lclMouseLocation = Camera.main.ScreenToWorldPoint(lclMouseCoord);	//Convert screen coordinate to world coordinate
		lclMouseLocation.y = transform.position.y;			//Set mouse world 'height' to the same height as the player
		transform.LookAt(lclMouseLocation);					//tell the player to look at the mouse
	}
	
	//	Move the character according to inputs 
	void Move() {
		float lclHorizontal = Input.GetAxis("Horizontal");
		float lclVertical = Input.GetAxis("Vertical");
		Vector3 lclMoveCmd = new Vector3(lclHorizontal, 0, lclVertical).normalized;
		lclMoveCmd *= Mathf.Max(Mathf.Abs(lclHorizontal),Mathf.Abs(lclVertical));
		GetComponent<CharacterController>().SimpleMove(lclMoveCmd * moveSpeed * Time.deltaTime);
		if (moveSpeed != maxMoveSpeed)
		{
			if (zombieTouchCount == 0) 
			{
				moveSpeed += zombieSlowRecover * Time.deltaTime;
				if (moveSpeed > maxMoveSpeed) moveSpeed = maxMoveSpeed;
			}
		}
	}
	
	//	PEWPEW
	void Shoot() {
		if (canIShootYet == false) {
			if (ammo > 0)
				{
				if ((attackTime >= 0))
				{
					attackTime -= attackSpeedFtr * Time.deltaTime;
				}
				else
				{
					canIShootYet = true;
				}
			}
		}
		else 
		{
			if (Input.GetButtonDown("Fire1")) 
			{
				Vector3 lclGunPos = transform.position + transform.TransformDirection(Vector3.forward) + Vector3.up;
				Instantiate(Bullet,lclGunPos,transform.rotation);
				AudioSource.PlayClipAtPoint(shoot, transform.position);
				ammo --;
				attackTime = 1;
				canIShootYet = false;
				Debug.Log ("Ammo count = " + ammo);
			}
		}
	}

	
	// Draw Laser REWORK <---------------------------------------------------------------------------------------------------
	void Laser() {
		RaycastHit lclHitInfo;
		if (Physics.Raycast(transform.position + (Vector3.up * 0.9f), transform.TransformDirection(Vector3.forward), out lclHitInfo)) {
			// Draw laser
			LineRenderer lclLaser = (LineRenderer)GetComponent(typeof(LineRenderer));
			lclLaser.SetPosition(0, transform.position + (Vector3.up * 0.9f));
			lclLaser.SetPosition (1, lclHitInfo.point);
			
			// Set laer dot
			Light lclLaserDot = (Light)transform.FindChild("LaserDot").light;
			lclLaserDot.transform.position = lclHitInfo.point + (lclHitInfo.normal * 0.05f);
		}
	}
	
	void GetWayPointOrder()
	{
		if (Input.GetButtonDown("Fire2") )
		{
			TOME = false;
			AudioSource.PlayClipAtPoint(orderMove[Random.Range(0, orderMove.Length)],transform.position);
			Vector3 lclMouseCoord = Input.mousePosition;		//Mouse screen coordinate (x,y,0)
			lclMouseCoord.z = Vector3.Distance(Camera.main.transform.position, transform.position + Vector3.up);
			wayPointOrder = Camera.main.ScreenToWorldPoint(lclMouseCoord);	//Convert screen coordinate to world coordinate
			wayPointSet = true;
			if (curWayPoint != null ) Destroy(curWayPoint);
			curWayPoint = Instantiate(WayPoint,wayPointOrder + Vector3.down,transform.rotation);
		}
		Debug.DrawLine(transform.position,wayPointOrder);
		if (!wayPointSet)
		{
			wayPointOrder = transform.position;
			wayPointOrder.y = 1f;
		}
		if (Input.GetButtonDown("Fire3")) {
			TOME = true;
			AudioSource.PlayClipAtPoint(orderTOME[Random.Range(0,orderTOME.Length)],transform.position);
		}
	}
	
	//	Check if the player is dead
	void AmIDeadYet() {
		
		if (!isDead) Camera.main.fieldOfView = (moveSpeed/maxMoveSpeed) * 60;
		if (!isDead) transform.FindChild("DeathLight").light.intensity = 1 - (moveSpeed/maxMoveSpeed);
		
		if (moveSpeed <= deathLimit) {
			if (!isDead) {
				Debug.Log ("You are dead.");
				isDead = true;
			}
		}
		if (moveSpeed <= -100){
			gameObject.GetComponent<CharacterController>().enabled = false;
			Application.LoadLevel (0);
			Start ();
		}
	}
	
	void OnGUI() {
		if (moveSpeed <=deathLimit)
		{ 
			GUI.Box(new Rect(Screen.width/2,Screen.height/2,100,100), "Score = " + Mathf.Round(myManager.score));
		}
	}
	
}
