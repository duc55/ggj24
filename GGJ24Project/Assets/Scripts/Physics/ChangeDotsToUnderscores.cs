using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChangeDotsToUnderscores : MonoBehaviour {


	// GameObjects[] childObj = new GameObject[0];
	public Transform gameObjToChangeDotsToUnderscores;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(gameObjToChangeDotsToUnderscores != null)
		{
			foreach (Transform part in gameObjToChangeDotsToUnderscores.GetComponentsInChildren<Transform>())
			{
				part.name = part.name.Replace(".", "_");
				// charProfile.bodyPartsToConfigureList.Add(part.name);
			}
			gameObjToChangeDotsToUnderscores = null;
		}
		
	}
}
