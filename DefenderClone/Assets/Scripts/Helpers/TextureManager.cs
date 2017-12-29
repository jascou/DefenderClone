using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureManager {
	int[] validColors;
	Rect[] packedAssets;
	Texture2D packedTexture;
	Texture2D[] allGraphics;
	int textureIndex;
	Texture2D terrainTex= new Texture2D(8, 8, TextureFormat.ARGB4444, false);
	Dictionary<string,int> textureDict;

    public Texture2D PackedTexture
    {
        get
        {
            return packedTexture;
        }
    }

    public Texture2D TerrainTex
    {
        get
        {
            return terrainTex;
        }
    }
	/*
	Manages textures which are all dynamically created at runtime, packed and referenced via dictionary
	 */
    public TextureManager(int howManyTextures){
		validColors=new int[2];
		allGraphics=new Texture2D[howManyTextures];
		textureIndex=0;
		textureDict=new Dictionary<string, int>();
	}
	public Rect GetTextureRectByName(string textureName){
		//Debug.Log("getting "+textureName);
		textureName=textureName.ToLower();
		int textureIndex;
		Rect textureRect=new Rect(0,0,0,0);
		if(textureDict.TryGetValue(textureName, out textureIndex)){
			if(textureIndex==-1){
				textureRect=new Rect(0,0,terrainTex.width,terrainTex.height);
			}else{
				textureRect=ConvertUVToTextureCoordinates(packedAssets[textureIndex]);
			}
		}else{
			Debug.Log("no such texture "+textureName);
		}
		return textureRect;
	}
	public void LoadTextArtToTexture(string textPath){
		string[] textureName=textPath.Split("/"[0]);
		textureDict.Add(textureName[textureName.Length-1].ToLower(),textureIndex);
		allGraphics[textureIndex++]=CreateArtTexture(textPath);
	}

	public void LoadTerrainTexture(int terrainWidth){
		Texture2D.DestroyImmediate(terrainTex, true);
		textureDict.Add("terrain",-1);
		CreateTerrainTexture(terrainWidth);
	}
	//packs all textures into single texture to help reduce draw calls
    public void PackTextures()
    {
		packedTexture = new Texture2D(8, 8, TextureFormat.ARGB4444, false);
		packedTexture.filterMode=FilterMode.Point;
		packedTexture.wrapMode=TextureWrapMode.Clamp;
		packedAssets=packedTexture.PackTextures(allGraphics,1);
		Debug.Log("packed textures to "+packedTexture.width+"x"+packedTexture.height);
		
		foreach (Texture2D tex in allGraphics)
		{
			Texture2D.DestroyImmediate(tex, true);
		}
		allGraphics=null;
    }
	//brute creation of the background texture connecting random points and filling everything below it
	void CreateTerrainTexture(int terrainWidth)
    {
        //Random.InitState(23);
		//256x16
		int terrainHeight=16;
		int interval=terrainWidth/terrainHeight;
		int maxDistance=terrainWidth/interval;//looking for min 10+ points;
		int minDistance=terrainWidth/(2*interval);
		int walker=0;
		int minHeight=terrainHeight/8;
		int newHeight=Random.Range(minHeight,terrainHeight-minHeight);
		Vector2 marker=new Vector2(walker,newHeight);
		List<Vector2> markers= new List<Vector2>();
		markers.Add(marker);
		while (walker<terrainWidth-maxDistance){
			int nextStop=Random.Range(minDistance,maxDistance);
			walker+=nextStop;
			newHeight=Random.Range(minHeight,terrainHeight-minHeight);
			marker=new Vector2(walker,newHeight);
			markers.Add(marker);
		}
		markers.Add(markers[0]);
		Debug.Log("terrain created with points - "+markers.Count.ToString());
		terrainTex= new Texture2D(terrainWidth, terrainHeight, TextureFormat.ARGB4444, false);
		terrainTex.filterMode=FilterMode.Point;
		terrainTex.wrapModeV=TextureWrapMode.Clamp;
		Vector2 nextMarker;
		int newX=0;
		int newY=0;
		
		for(int i =0;i<markers.Count-1;i++){
			marker=markers[i];
			if(i==markers.Count-2){
				nextMarker=markers[markers.Count-1];
				nextMarker.x=terrainWidth;
			}else{
				nextMarker=markers[i+1];
			}
			float slope= (nextMarker.y-marker.y)/(nextMarker.x-marker.x);
			int delta= (int)(nextMarker.x-marker.x);
			for(int j =0;j<delta;j++){
				newX=(int)marker.x+j;
				newY=(int)((slope*(newX-marker.x))+marker.y);
				for (int k = 0; k < terrainHeight; k++) {
					if(k<newY){
						terrainTex.SetPixel(newX, k, Color.white);	
					}else terrainTex.SetPixel(newX, k, Color.clear);
				}
			}
		}
		terrainTex.Apply();
    }

	//load art text from resources and create individual enemies and bullets
	Texture2D CreateArtTexture(string textName)
    {
		int invalidTile=-1;
		
		TextAsset textFile = Resources.Load (textName) as TextAsset;
		string[] lines = textFile.text.Split (new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);//split by new line, return
		string[] nums = lines[0].Split(new[] { ',' });//split by ,
		int rows=lines.Length;//number of rows
		int cols=nums.Length;//number of columns
		int[,] artData = new int[rows, cols];
        for (int i = 0; i < rows; i++) {
			string st = lines[i];
            nums = st.Split(new[] { ',' });
			for (int j = 0; j < cols; j++) {
                int val;
                if (int.TryParse (nums[j], out val)){
                	artData[i,j] = val;
				}
                else{
                    artData[i,j] = invalidTile;
				}
            }
        }
		
		artData=rotateCW(artData,rows,cols);
		//need to flip rows and cols as we rotated the array
		Texture2D texture = new Texture2D(cols, rows, TextureFormat.ARGB4444, false);
		texture.filterMode=FilterMode.Point;
		texture.wrapMode=TextureWrapMode.Clamp;
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
                int val=artData[i,j];
				if(val!=invalidTile){
					if(val==validColors[0]){
						texture.SetPixel(i, j, Color.black);
					}else{
						texture.SetPixel(i, j, Color.white);
					}
				}else texture.SetPixel(i, j, Color.clear);
			}
		}
		texture.Apply();
		return texture;
    }
	//needed to rotate array clockwise due to origin difference for texture2d
	int[,] rotateCW(int[,] arr,int rows,int cols) {
		int M = rows;
		int N = cols;
		int[,] ret = new int[N,M];
		for (int r = 0; r < M; r++) {
			for (int c = 0; c < N; c++) {
				ret[c,M-1-r] = arr[r,c];
			}
		}
		return ret;
	}
	//needed to convert UV (0,1) coordinates to texture pixel coordinates
	private Rect ConvertUVToTextureCoordinates(Rect rect)
    {
        return new Rect(rect.x*packedTexture.width,
			rect.y*packedTexture.height,
			rect.width*packedTexture.width,
			rect.height*packedTexture.height
		);
    }
}
