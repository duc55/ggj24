//attach this to the clothing GO you want to attach (the GO with the SkinnedMeshRenderer)
//drag the main character body you want to attach it to (the GO with the SkinnedMeshRenderer)
//the clothing rig must have been the same rig the character's body used. the body parts (transforms) must be named the same thing

using System;
using System.Collections.Generic;
using UnityEngine;

	// [ExecuteInEditMode]
	public class AddItemToArmature : MonoBehaviour {

		public Material materialToApplyToMesh;
		public SkinnedMeshRenderer bodyToAttachTo; //the character's main body
		public bool getTargetArmatureBones;
//		public Transform[] targetBonesArray = new Transform[0];
//		public SkinnedMeshRenderer meshToAttach; //the clothing you want to attach
		public List<SkinnedMeshRenderer> meshesToAttachList = new List<SkinnedMeshRenderer>(); //the clothing you want to attach
//		public bool getItemArmatureBones;
//		public Transform[] itemBonesArray = new Transform[0];
		
		
		public bool moveMeshesToTargetArmature;
//		public Transform[] newBoneTransformsToUseArray = new Transform[0];
//		public List<Transform> boneTransformsToUseList = new List<Transform>();
//		public List<Transform> boneTransformsToUseList = new List<Transform>();
		
//		[Header("ATTACH")]
//		public bool throwAwayOldArmature;
//		public bool attachItemToTargetBody;


	public GameObject originalGameObject;

	void CloneGameObject()
	{
		// Create a new instance of the original GameObject
		GameObject newGameObject = Instantiate(originalGameObject);

		// Set the new GameObject's position and rotation to match the original
		newGameObject.transform.position = originalGameObject.transform.position;
		newGameObject.transform.rotation = originalGameObject.transform.rotation;

		// Set the new GameObject as a child of the original's parent
		newGameObject.transform.SetParent(originalGameObject.transform.parent);
	}
	
	private void OnValidate()
	{
		if (moveMeshesToTargetArmature)
		{
			moveMeshesToTargetArmature = false;
			
			
			ChangeDotsToUnderscores(meshesToAttachList[0].transform.root);
	//				meshesToAttachList[0].bones = newBoneTransformsToUseArray;
			foreach (var item in meshesToAttachList)
			{
				item.bones = bodyToAttachTo.bones;
				item.rootBone = bodyToAttachTo.rootBone; //point it to the new root bone.
				if(materialToApplyToMesh)
				{
					item.material = materialToApplyToMesh;
				}
				item.quality = SkinQuality.Bone4;
				item.transform.SetParent(bodyToAttachTo.transform.parent, false); //move the clothing over to the body GO
			}
			
		}
	}

void Update () 
		{
//			if (getTargetArmatureBones)
//			{
//				getTargetArmatureBones = false;
//				targetBonesArray = bodyToAttachTo.bones;
//			}





// 			if (moveMeshesToTargetArmature)
// 			{
// 				moveMeshesToTargetArmature = false;
// 				ChangeDotsToUnderscores(meshesToAttachList[0].transform.root);
// //				meshesToAttachList[0].bones = newBoneTransformsToUseArray;
// 				foreach (var item in meshesToAttachList)
// 				{
// 					item.bones = bodyToAttachTo.bones;
// 					item.rootBone = bodyToAttachTo.rootBone; //point it to the new root bone.
// 					if(materialToApplyToMesh)
// 					{
// 						item.material = materialToApplyToMesh;
// 					}
// 					item.quality = SkinQuality.Bone4;
// 					item.transform.SetParent(bodyToAttachTo.transform.parent, false); //move the clothing over to the body GO
// 				}
// 			}
			
			
			
			
			
			
			
			
			
			
//			if (getItemArmatureBones)
//			{
//				getItemArmatureBones = false;
//				foreach (var item in meshesToAttachList[0].bones)
//				{
//					item.name = item.name.Replace(".", "_");
//				}
//				itemBonesArray = meshesToAttachList[0].bones;
//				newBoneTransformsToUseArray = meshesToAttachList[0].bones;
////				itemBonesArray = new Transform[0];
////				foreach (var item in meshesToAttachList)
////				{
////					for (int i = 0; i < UPPER; i++)
////					{
////						
////					}
////					foreach (var bone in item.bones)
////					{
////						if (!itemBonesArray.Contains(bone))
////						{
////							itemBonesArray.Add(bone);
////						}
////					}
////				}
////				itemBonesArray = meshToAttach.bones;
//			}
//			if(attachItemToTargetBody)
//			{
//				attachItemToTargetBody = false;
//				foreach (var sm in meshesToAttachList)
//				{
//					CopyArmature(sm);
//				}
//			}
//			if(meshToAttach != null)
//			{
//				CopyArmature();
//			}
			// if(bodyToAttachTo != null)
			// {
			// 	CopyArmature();
			// }
			// if(!clothingToAttach)
			// {
			// 	clothingToAttach = GetComponent<SkinnedMeshRenderer>();
			// }
		}


//		void SetMeshBones(SkinnedMeshRenderer sm, Transform rootBone, Transform[] bones)
//		{
//			sm.bones = bones;
//			sm.rootBone = rootBone;
//		}
//
//		void CopyArmature(SkinnedMeshRenderer meshToAttach)
//		{
//			ChangeDotsToUnderscores(meshToAttach.transform.root);
////			Dictionary<string, Transform> targetBonesDict = new Dictionary<string, Transform>(); //main body bones
////			Transform[] targetBonesArray = bodyToAttachTo.bones;
//
////			foreach (Transform bone in bodyToAttachTo.bones) //make a dict for easy transform lookup
////			{
////				targetBonesDict[bone.name] = bone;
////			}
//			List<Transform> replacementBones = new List<Transform>(); //this will hold
//			GameObject cloneItem = Instantiate(meshToAttach.gameObject);
//			SkinnedMeshRenderer clonedSkinnedMeshRenderer = cloneItem.GetComponent<SkinnedMeshRenderer>();
////			foreach(Transform bone in clonedSkinnedMeshRenderer.bones)
////			{
////				if(targetBonesDict.ContainsKey(bone.name))
////				{
////					replacementBones.Add(targetBonesDict[bone.name]);
////				}
////			}
//			clonedSkinnedMeshRenderer.bones = replacementBones.ToArray(); //convert the list to an array and assign
////			GameObject throwAway = clonedSkinnedMeshRenderer.transform.root.gameObject;
//			// clothingToAttach.transform.SetParent(this.transform); //move the clothing over to the body GO
//			clonedSkinnedMeshRenderer.transform.SetParent(bodyToAttachTo.transform.parent, false); //move the clothing over to the body GO
////			clonedSkinnedMeshRenderer.transform.SetParent(this.transform, false); //move the clothing over to the body GO
//			clonedSkinnedMeshRenderer.rootBone = bodyToAttachTo.rootBone; //point it to the new root bone.
//			if(materialToApplyToMesh)
//			{
//				clonedSkinnedMeshRenderer.material = materialToApplyToMesh;
//			}
//			clonedSkinnedMeshRenderer.quality = SkinQuality.Bone4;
////			clonedSkinnedMeshRenderer = null; //clear 
//			// todo the old heirarchy the shirt was moved from can be deleted
////			if(throwAwayOldArmature)
////			{
////				DestroyImmediate(throwAway);
////			}
//		}
//
//		// void CopyArmature()
//		// {
//		// 	// ChangeDotsToUnderscores();
//		// 	Dictionary<string, Transform> bones = new Dictionary<string, Transform>(); //main body bones
//		// 	foreach (Transform bone in bodyToAttachTo.bones) //make a dict for easy transform lookup
//		// 	{
//		// 		bones[bone.name] = bone;
//		// 	}
//		// 	List<Transform> replacementBones = new List<Transform>(); //this will hold
//		// 	foreach(Transform bone in clothingToAttach.bones)
//		// 	{
//		// 		if(bones.ContainsKey(bone.name))
//		// 		{
//		// 			replacementBones.Add(bones[bone.name]);
//		// 		}
//		// 	}
//		// 	clothingToAttach.bones = replacementBones.ToArray(); //convert the list to an array and assign
//		// 	clothingToAttach.transform.SetParent(bodyToAttachTo.transform, false); //move the clothing over to the body GO
//		// 	clothingToAttach.rootBone = bodyToAttachTo.rootBone; //point it to the new root bone.
//		// 	bodyToAttachTo = null; //clear 
//		// 	Destroy(this.transform.root.gameObject);
//		// }
		void ChangeDotsToUnderscores(Transform t)
		{
			foreach (Transform part in t.GetComponentsInChildren<Transform>())
			{
				part.name = part.name.Replace(".", "_");
				// charProfile.bodyPartsToConfigureList.Add(part.name);
			}
		}
	}

		// public SkinnedMeshRenderer CopyArmature(string skinName)
		// public SkinnedMeshRenderer CopyArmature()
		// {
		// 	// SkinnedMeshRenderer skinInFBX = ItemLibrary.Get().playerFullFBX.transform.Find (skinName).gameObject.GetComponent<SkinnedMeshRenderer> ();
		// 	SkinnedMeshRenderer skinInFBX = bodyToAttachTo;
		// 	// Mesh skin = skinInFBX.sharedMesh;
		// 	Transform[] bones = skinInFBX.bones;
		// 	Transform[] replacementBones = new Transform[bones.Length];
		// 	for (int i = 0; i < bones.Length; i++) {
		// 		Transform b = GetBone (bones [i].gameObject.name);
			
		// 		replacementBones [i] = b;
		// 	}
		// 	GameObject go = new GameObject ();
		// 	go.transform.SetParent (transform, false);
		// 	SkinnedMeshRenderer smr = go.AddComponent<SkinnedMeshRenderer> ();
		// 	smr.sharedMesh = skin;
		// 	smr.bones = replacementBones;
		// 	// if (inUI) {
		// 	// 	go.layer = 5;
		// 	// }
		// 	return smr;
		// }
		// public SkinnedMeshRenderer CopyArmature(string skinName)
		// {
		// 	// SkinnedMeshRenderer skinInFBX = ItemLibrary.Get().playerFullFBX.transform.Find (skinName).gameObject.GetComponent<SkinnedMeshRenderer> ();
		// 	SkinnedMeshRenderer skinInFBX = bodyToAttachTo;
		// 	Mesh skin = skinInFBX.sharedMesh;
		// 	Transform[] bones = skinInFBX.bones;
		// 	Transform[] replacementBones = new Transform[bones.Length];
		// 	for (int i = 0; i < bones.Length; i++) {
		// 		Transform b = GetBone (bones [i].gameObject.name);
			
		// 		replacementBones [i] = b;
		// 	}
		// 	GameObject go = new GameObject ();
		// 	go.transform.SetParent (transform, false);
		// 	SkinnedMeshRenderer smr = go.AddComponent<SkinnedMeshRenderer> ();
		// 	smr.sharedMesh = skin;
		// 	smr.bones = replacementBones;
		// 	// if (inUI) {
		// 	// 	go.layer = 5;
		// 	// }
		// 	return smr;
		// }