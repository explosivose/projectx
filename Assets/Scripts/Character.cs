using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{
	public float maxHealth = 100f;
	public float maxMoveSpeed = 250f;
	
	private float health;
	private float moveSpeed;
	private CharacterController controller;
	
	// Use this for initialization
	void Start ()
	{
		health = maxHealth;
		moveSpeed = maxMoveSpeed;
		controller = transform.GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
