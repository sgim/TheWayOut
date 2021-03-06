﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;

public class PlayerHealth : MonoBehaviour
{
	public int startingHealth = 100;                            // The amount of health the player starts the game with.
	public int currentHealth;                                   // The current health the player has.
	public Slider healthSlider;                                 // Reference to the UI's health bar.
	public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
	public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
	public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.

	public Slider staminaSlider;
	public Image sliderFill;
	private float staminaMax = 100f;
	private float staminaInterval = 0.5f;
	private float staminaCurrent;
	private float tempRun;
	private bool outOfStamina = false;
	public float fallKillDistance = 100;

	bool isDead;                                                // Whether the player is dead.
	bool damaged;                                               // True when the player gets damaged.
	public static Vector3 respawnPoint;
	public static Vector3 respawnPointAngle;

	PlayerControls playerControls;

	public pickuppost pickpostscript;

	private State state;

	void Awake ()
	{
		staminaCurrent = staminaMax;

		state = State.instance;
		GameObject player = GameObject.Find ("Player");
		playerControls = player.GetComponent<PlayerControls> ();
	
		respawnPoint = state.GetRespawnPoint();
		respawnPointAngle = state.GetRespawnAngle();
		player.transform.position = respawnPoint;
		
	}

	void Start(){
		currentHealth = startingHealth;
		tempRun = playerControls.runMultipler;
	}
		

	void FixedUpdate ()
	{
		if (!outOfStamina && !playerControls.isJumping && playerControls.isRunning && staminaCurrent > 0) {
			staminaCurrent -= staminaInterval;
			if (staminaCurrent <= 0) {
				StaminaDamage ();
			}
		} else if (staminaCurrent < staminaMax) {
			staminaCurrent += staminaInterval / 2;
			if (staminaCurrent >= staminaMax) {
				outOfStamina = false;
				staminaCurrent = staminaMax;
				playerControls.runMultipler = tempRun;
				sliderFill.color = Color.white;
			}
		}
		staminaSlider.value = staminaCurrent;

		damageImage.color = damaged ? flashColour : Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);

		// Reset the damaged flag.
		damaged = false;

		if (transform.position.y <= -fallKillDistance) {
			toKill ();
		}

	}

	private void StaminaDamage() {
		tempRun = playerControls.runMultipler;
		playerControls.runMultipler = 1.0f;
		sliderFill.color = Color.red;
		outOfStamina = true;
	}

	public void TakeDamage (int amount)
	{
		// Set the damaged flag so the screen will flash.
		damaged = true;

		// Reduce the current health by the damage amount.
		currentHealth -= amount;

		// Set the health bar's value to the current health.
		healthSlider.value = currentHealth;

		// If the player has lost all it's health and the death flag hasn't been set yet...
		if(currentHealth <= 0 && !isDead)
		{
			toKill ();
		}
	}

	public void toKill () {
		isDead = true;
//		toRespawn ();
		transform.position = state.GetRespawnPoint();
		transform.eulerAngles = state.GetRespawnAngle ();
		currentHealth = 100;
		healthSlider.value = currentHealth;
		isDead = false;
		// respawn the pillar in level3
//		pickpostscript.PostRespawn ();
	}

}