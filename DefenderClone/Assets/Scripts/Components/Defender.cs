﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender : MovableObject
{
    float lastFiredTime;
	float firingDelay=0.1f;

	/*
	defender specific behaviour
	 */
    public Defender(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
		isFacingRight=true;
    }
	public void MoveTo(Vector2 newPos){
		position=Vector2.Lerp(position,newPos,0.05f);
		FixPosition();
		displayObject.transform.localPosition=position;
	}

	//wait for firing delay
	public bool isReadyToFireAgain(){
		if(Time.timeSinceLevelLoad-lastFiredTime>firingDelay){
			return true;
		}
		return false;
	}
	public void SetFireTime(){
		lastFiredTime=Time.timeSinceLevelLoad;
	}
	//get/set direction
    public bool isFacingRight
    {
        get
        {
            return (Mathf.Sign(displayObject.transform.localScale.x)==1);
        }

        set
        {
            Vector2 localScale=displayObject.transform.localScale;
			if(value){
				localScale.x=Mathf.Abs(localScale.x);
			}else{
				localScale.x=-1*Mathf.Abs(localScale.x);
			}
			displayObject.transform.localScale=localScale;
        }
    }
}
