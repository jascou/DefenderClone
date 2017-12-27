using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>{
	protected GameManager() { } // guarantee this will be always a singleton only - can't use the constructor!

	public const string DEFENDER_BULLET = "herobullet", BULLET1 = "bullets5", BULLET2 = "bullets2", BULLET3 = "bullets3";
	public const string BULLET4 = "bullets4", BULLET5 = "bullets1", ENEMY1 = "crafts1", ENEMY2 = "crafts2", ENEMY3 = "crafts3";
    public const string ENEMY4 = "crafts4", ENEMY5 = "crafts5", ENEMY6 = "crafts6", ENEMY7 = "crafts7", ENEMY8 = "crafts8";
    public const string ENEMY9 = "crafts9", ENEMY10 = "crafts10", ENEMY11 = "crafts11", ENEMY12 = "crafts12";

	int currentLevel;
	int currentDifficulty;

	public TextureManager textureManager;
	public static Vector2 topBottomLimits;
	public static Vector2 leftRightLimits;
	public static Vector2 screenLeftRightLimits;
	public static GameObject twinColliderPrefab;
	public static GameObject singleColliderPrefab;

	public void Initialise(GameObject singlePrefabObj, GameObject twinprefabObject){
		Instance.currentLevel=1;
		Instance.textureManager= new TextureManager(18);
		Instance.textureManager.LoadTextArtToTexture("Defender/Defender");
		for(int i=0;i<12;i++){
			Instance.textureManager.LoadTextArtToTexture("Crafts/Crafts"+(i+1).ToString());
		}
		for(int i=0;i<5;i++){
			Instance.textureManager.LoadTextArtToTexture("Bullets/Bullets"+(i+1).ToString());
		}
		Resources.UnloadUnusedAssets();
		Instance.textureManager.PackTextures();
		singleColliderPrefab=singlePrefabObj;
		twinColliderPrefab=twinprefabObject;
	}

    internal void InitLevel(float left, float top, float right, float bottom)
    {
        Instance.textureManager.LoadTerrainTexture(256);
		Instance.currentDifficulty=Instance.getDifficulty();
		topBottomLimits.x=top;
		topBottomLimits.y=bottom;
		leftRightLimits.x=left;
		leftRightLimits.y=right;
		screenLeftRightLimits.x=-400;
		screenLeftRightLimits.y=400;
    }

    private int getDifficulty()
    {
        return 1;
    }
}
