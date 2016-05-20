﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Networking;
using SimpleJSON;

public class State : MonoBehaviour {

	public string currentLevel;
	public JSONNode respawnPoint;
	public string playerName;
	public string playerEmail;
	public string playerid;
	public string url = "http://localhost:1337/";

	JSONNode levels;

	public void LoadScene (string scene) {
		if (scene == "1") {
			currentLevel = "1";
		} else if ( scene == "2"){
			Debug.Log (scene);
			currentLevel = "2";
		} else if (scene == "3"){
			currentLevel = "3";
		}
		DontDestroyOnLoad (transform.gameObject);
		SceneManager.LoadScene (scene);

	}

	// get user info from login 
	public void StoreUser(JSONNode user) {
		playerid = user ["_id"].Value;
		playerName = user ["name"].Value;
		playerEmail = user ["email"].Value;
		StartCoroutine (GetUserInfo ());
	}

	// save user information when exiting to the main menu from the game
	public void SaveUserInfo(){
		StartCoroutine(PostUserInfoWithCheckpoint());
	}

	IEnumerator PostUserInfoWithCheckpoint(){
		WWWForm form = new WWWForm();
		Debug.Log ("currentLevel" + currentLevel);
		form.AddField ("currentLevel", currentLevel);
		form.AddField ("X", PlayerHealth.respawnPoint.x.ToString());
		form.AddField ("Y", PlayerHealth.respawnPoint.y.ToString());
		form.AddField ("Z", PlayerHealth.respawnPoint.z.ToString());
		form.AddField ("Angle", PlayerHealth.respawnPointAngle.y.ToString());
		using (UnityWebRequest request = UnityWebRequest.Post (url + "api/users/" + playerid, form)) {
			yield return request.Send();

			if(request.isError) {
				Debug.Log(request.error);
			}
			else {
				Debug.Log ("updated with " + request.downloadHandler.text);
				StartCoroutine (GetUserInfo ());
			}
		}
	}
		
	// Get user info when loaded to menu from the login;
	IEnumerator GetUserInfo()
	{
		using (UnityWebRequest request = UnityWebRequest.Get (url + "api/users/" + playerid)) {
			yield return request.Send();

			if(request.isError) {
				Debug.Log(request.error);
			}
			else {
				JSONNode CurrentUser = JSON.Parse(request.downloadHandler.text);
				currentLevel = CurrentUser ["currentLevel"] ["name"].Value;
				respawnPoint = CurrentUser ["respawnPoint"]; 
				// If the player has never played before, set the currentLevel to level 1;
				if (currentLevel == "") {
					currentLevel = "1";
//					StartCoroutine (PostUserInfo ());
				}
			}
		}
	}

	//	IEnumerator PostUserInfo(){
	//		WWWForm form = new WWWForm();
	//		form.AddField ("currentLevel", currentLevel);
	//		using (UnityWebRequest request = UnityWebRequest.Post (url + "api/users/" + playerid, form)) {
	//			yield return request.Send();
	//
	//			if(request.isError) {
	//				Debug.Log(request.error);
	//			}
	//			else {
	//				Debug.Log ("updated with " + request.downloadHandler.text);
	//			}
	//		}
	//	}
		
}
