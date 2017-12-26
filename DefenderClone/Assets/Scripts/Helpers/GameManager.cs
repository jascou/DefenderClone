using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>{
	protected GameManager() { } // guarantee this will be always a singleton only - can't use the constructor!

	int currentLevel;
	int currentDifficulty;

	public TextureManager textureManager;

	public void Initialise(){
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
	}

	public void InitLevel(){
		Instance.textureManager.LoadTerrainTexture(256);
		Instance.currentDifficulty=Instance.getDifficulty();
	}

    private int getDifficulty()
    {
        return 1;
    }
}
