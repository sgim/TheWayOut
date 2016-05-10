﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
//	public string currentScene;
	public static Vector3 respawnPoint;
	private UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController controller;

	void Awake ()
	{
		// Set the initial health of the player.
		currentHealth = startingHealth;
		staminaCurrent = staminaMax;
	}

	void Start(){
		// set the initial respawnPoint position to level start position;
		respawnPoint = new Vector3(0,1,0);
		controller = GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController> ();
		tempRun = controller.movementSettings.RunMultiplier;

	}
		

	void FixedUpdate ()
	{
		if (!outOfStamina && controller.Running && controller.Grounded && staminaCurrent > 0) {
			staminaCurrent -= staminaInterval;
			if (staminaCurrent <= 0) {
				StaminaDamage ();
			}
		} else if (staminaCurrent < staminaMax) {
			staminaCurrent += staminaInterval / 2;
			if (staminaCurrent >= staminaMax) {
				outOfStamina = false;
				staminaCurrent = staminaMax;
				controller.movementSettings.RunMultiplier = tempRun;
				sliderFill.color = Color.white;
			}
		}
		staminaSlider.value = staminaCurrent;

		// If the player has just been damaged...
		// ... set the colour of the damageImage to the flash colour.
		// ... otherwise transition the colour back to clear.
		damageImage.color = damaged ? flashColour : Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);

		// Reset the damaged flag.
		damaged = false;

		if (transform.position.y <= -fallKillDistance) {
			toKill ();
		}

	}

	private void StaminaDamage() {
		tempRun = controller.movementSettings.RunMultiplier;
		controller.movementSettings.RunMultiplier = 1.0f;
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
		Debug.Log("You died"); 
		//Need to be seperated to another function
		currentHealth = 100;
		transform.position = respawnPoint;
		isDead = false;
		//End of seperated function
	}
}