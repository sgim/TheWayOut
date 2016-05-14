﻿using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class CheckPoint : MonoBehaviour {
	public Transform Player;
	public bool enterOnce = true;
	public float showDuration = 3.0f;
	public bool isCheckpoint = false;
	public string txtmessage = "Enter Text";

	private bool alreadyEntered = false;
	private float enteredTime;
	private float currentTime;
	private Text currentMsg;

	void Start(){

		currentMsg = GameObject.Find ("MessageCenter").GetComponent<Text>();
		if (isCheckpoint) {
			txtmessage = "Checkpoint!";
		}
	}

	void FixedUpdate() {
		currentTime = Time.time;
		if (alreadyEntered && currentMsg.text == txtmessage && Time.time - enteredTime > showDuration) {
			currentMsg.text = "";
			if (enterOnce) {
				Destroy (gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if (!alreadyEntered || !enterOnce) {
			enteredTime = Time.time;
			currentMsg.text = txtmessage;

			if(isCheckpoint) {
				
				// also update the respawn position to this checkpoint position;
				PlayerHealth.respawnPoint = transform.position;
				PlayerHealth.respawnPointAngle = transform.eulerAngles;

				// save all the player info at the checkpoint to PlayerPrefs;
				//position
				PlayerPrefs.SetFloat ("x",Player.position.x);
				PlayerPrefs.SetFloat ("y",Player.position.y);
				PlayerPrefs.SetFloat ("z",Player.position.z);
				PlayerPrefs.SetFloat ("Cam_y", Player.eulerAngles.y);
				//health
				//		PlayerPrefs.SetInt ("currentHealth", PlayerHealth.currentHealth);
				//respawnPoint
				PlayerPrefs.SetFloat ("RPx", PlayerHealth.respawnPoint.x);
				PlayerPrefs.SetFloat ("RPy", PlayerHealth.respawnPoint.y);
				PlayerPrefs.SetFloat ("RPz", PlayerHealth.respawnPoint.z);
				PlayerPrefs.SetFloat ("RPA_y", PlayerHealth.respawnPointAngle.y);
				// stamina
			};

			alreadyEntered = true;
		}
	}
		
}
