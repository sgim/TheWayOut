﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.Experimental.Networking;
using SimpleJSON;
using System.Linq;

public class HighScores : MonoBehaviour {

	private GameObject nameList;
	private GameObject timeList;
	private JSONNode scores;

	void Start () {
		nameList = GameObject.Find ("NameList");
		timeList = GameObject.Find ("TimeList");
		StartCoroutine (GetScores ());
	}

	void killTheKids(GameObject parent) {
		for (int i = parent.transform.childCount - 1; i >= 0; --i) {
			Destroy (parent.transform.GetChild (i).gameObject);
		}
	}

	void placeScores(ArrayList list) {
		killTheKids (nameList);
		killTheKids (timeList);
		foreach(JSONNode score in list) {
//		for(int i = 0; i < list.Count; i++) {
			GameObject name = new GameObject ();
			GameObject time = new GameObject ();
			name.transform.SetParent (nameList.transform);
			time.transform.SetParent (timeList.transform);
			Text nameText = name.AddComponent<Text> ();
			Text timeText = time.AddComponent<Text> ();
			nameText.text = score["player"]["name"];
			timeText.text = score["time"];
			nameText.font = timeText.font = UnityEngine.Font.CreateDynamicFontFromOSFont ("Arial", 14);
			nameText.horizontalOverflow = timeText.horizontalOverflow = HorizontalWrapMode.Overflow;
			timeText.alignment = TextAnchor.UpperCenter;
		}
	}

	IEnumerator GetScores ()
	{
		using (UnityWebRequest request = UnityWebRequest.Get ("http://localhost:1337/api/times")) {
			yield return request.Send ();
			if (request.isError) {
				Debug.Log (request.error);
			} else {
				scores = JSON.Parse(request.downloadHandler.text);
				FilterScores ("1", "");
//				placeScores (scores);
			}
		}
	}

	void FilterScores(string level, string name) {
		ArrayList filteredScores = new ArrayList();
		for (int i = 0; i < scores.Count; i++) {
			JSONNode score = scores [i];
			if (score["level"]["name"].Value == level && (name == "" || score ["player"] ["name"].Value == name)) {
				filteredScores.Add (score);
			}
		}
		placeScores (filteredScores);
	}

}
