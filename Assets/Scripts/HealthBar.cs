﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MaterialUI;

public class HealthBar : MonoBehaviour {
	public float startPosition;
	public float endPosition;
	public float pedometersteps;
	public RectTransform rTrans;
	public Text steptext;
	public Animator anim;
	private bool isClosed = true;

	// Use this for initialization
	void Awake () {
		DAO database = new DAO ();
		Pedometer ped = database.GetPedometerInfo (FacebookManager.Instance ().user_ID);
		pedometersteps = ped.Total_step;
	 	isClosed = true;
	}

	public void registerSteps(){
		DAO database = new DAO ();
		Pedometer ped = database.GetPedometerInfo (FacebookManager.Instance ().user_ID);
		ped.Total_step += FeaturePedometer.Instance ().stepCnt;
		pedometersteps += FeaturePedometer.Instance ().stepCnt;
		Augmon aug = database.GetAugmonInfo (FacebookManager.Instance ().user_ID);
		aug.Lvl += (int)(FeaturePedometer.Instance ().stepCnt / 100);
		FeaturePedometer.Instance ().stepCnt = 0;
		//update database
		database.UpdateAugmon (aug);
		database.UpdatePedometer (ped);
	}

	public void stepsOnClick(){
		anim.enabled = true;
		registerSteps ();
		if (isClosed) {
			anim.Play ("healthbarSlideOut");
			isClosed = false;
		} else {
			anim.Play ("healthbarSlideIn");
			isClosed = true;
		}
	}

	// Update is called once per frame
	void Update () {
		float length = Mathf.Abs(endPosition-startPosition);
		int delta = FeaturePedometer.Instance().stepCnt;
		steptext.text = "Steps - " + ((pedometersteps + delta) % 100) + "%";
		length = length * (((pedometersteps + delta) % 100)/100);
		if ((int)startPosition > (int)endPosition) {
			rTrans.anchoredPosition = new Vector2(startPosition - length,rTrans.anchoredPosition.y);
		}
		if ((int)startPosition < (int)endPosition) {
			rTrans.anchoredPosition = new Vector2(startPosition + length,rTrans.anchoredPosition.y);
		}
		NavDrawerConfig config = GameObject.Find ("Nav Drawer").GetComponent<NavDrawerConfig> ();
		if (config.isClosed () && !isClosed) {
			stepsOnClick ();
		}
	}
}
