using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelExplosion
{
	int numPixelsMax=6;
	int numPixelsMin=3;
	float life=1;
    float elapsedTime=0;
	public bool isTobeRemoved;
	List<PixelParticle> pixels;
    public PixelExplosion(GameObject pixelGO,Vector2 initialPosition,Color color)
    {
		int numPixels=numPixelsMin+Random.Range(0,numPixelsMax-numPixelsMin);
		pixels=new List<PixelParticle>();
		for(int i=0;i<numPixels;i++){
			pixels.Add(new PixelParticle(SimplePool.Spawn(pixelGO,initialPosition,Quaternion.identity),initialPosition));
			pixels[i].paint=color;
		}
    }
	public bool Tick(){
        elapsedTime+=Time.deltaTime;
        if(elapsedTime>=life)isTobeRemoved=true;
		if(isTobeRemoved)return false;
		return true;
	}
	public void Move(float deltaX, float deltaY){
		foreach(PixelParticle pp in pixels){
			pp.Move(deltaX,deltaY);
		}
	}
	public void Remove(){
		foreach(PixelParticle pp in pixels){
			SimplePool.Despawn(pp.displayObject);
		}
		pixels.Clear();
	}
}
class PixelParticle : MovableObject
{
	float damping=0.96f;
	float speed;
	public PixelParticle(GameObject gameObject, Vector2 initialPosition) : base(gameObject, initialPosition)
    {
		speed=250;
        velocity=Random.insideUnitCircle*speed;
    }
	public override void Move(float deltaX, float deltaY){//slows to halt
        velocity*=damping;
		base.Move(deltaX+(velocity.x*Time.deltaTime),deltaY+(velocity.y*Time.deltaTime));
	}
}
