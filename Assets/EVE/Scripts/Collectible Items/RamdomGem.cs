﻿using UnityEngine;
using System.Collections;
using Assets.EVE.Scripts.Utils;

public class RamdomGem : MonoBehaviour {

	[Header("Colors")]
	public bool blue = true;
	public bool red = true;
	public bool green = true;
	public bool white = true;

		
	[Header("Shapes")]
	public bool oldSingle = true;
	public bool pear = true;
	public bool rose = true;
	public bool round = true;

	private ArrayList fileNames;


	// Use this for initialization
	void Start () {
		fileNames = new ArrayList ();
		PossibleSelections ();
		var index = Random.Range (0, fileNames.Count-1);

		var go = GameObjectUtils.InstatiatePrefab((string)fileNames[index]);
	    go.transform.position = transform.position;
	    go.transform.rotation = Random.rotation;
        go.transform.localScale += go.transform.localScale;
        go.gameObject.GetComponent<CollectibleItem>().id = this.gameObject.name;
		Destroy (this.gameObject);
	}
	
	private void PossibleSelections(){
		if (blue) {
			if (oldSingle) {
				fileNames.Add ("OldSingleCut_Blue");
			}
			if (pear) {
				fileNames.Add ("PearCut_Blue");
			}
			if (rose) {
				fileNames.Add ("RoseCut_Blue");
			}
			if (round) {
				fileNames.Add ("RoundCut_Blue");
			}
		}
		if (red) {
			if (oldSingle) {
				fileNames.Add ("OldSingleCut_Red");
			}
			if (pear) {
				fileNames.Add ("PearCut_Red");
			}
			if (rose) {
				fileNames.Add ("RoseCut_Red");
			}
			if (round) {
				fileNames.Add ("RoundCut_Red");
			}
		}
		if (green) {
			if (oldSingle) {
				fileNames.Add ("OldSingleCut_Green");
			}
			if (pear) {
				fileNames.Add ("PearCut_Green");
			}
			if (rose) {
				fileNames.Add ("RoseCut_Green");
			}
			if (round) {
				fileNames.Add ("RoundCut_Green");
			}
		}
		if (white) {
			if (oldSingle) {
				fileNames.Add ("OldSingleCut_White");
			}
			if (pear) {
				fileNames.Add ("PearCut_White");
			}
			if (rose) {
				fileNames.Add ("RoseCut_White");
			}
			if (round) {
				fileNames.Add ("RoundCut_White");
			}
		}
		if((!red && !white && !blue &&!green)||(!pear&&!oldSingle&&!round&&!rose)){
			Debug.LogWarning("No type of gem selected!");
		}
	}
}
