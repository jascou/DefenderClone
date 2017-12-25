using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTest : MonoBehaviour {

	public int[] validColors;
	Rect[] packedAssets;
	Texture2D packedTexture;

	GameObject[] crafts;
	GameObject[] craftsInScene;

	public Material material;
	public float scrollSpeed;
	Vector2 offset=new Vector2();
	void Start () {
		Texture2D[] allGraphics=new Texture2D[19];
		allGraphics[0]=CreateTexture("Defender/Defender");
		for(int i=1;i<13;i++){
			allGraphics[i]=CreateTexture("Crafts/Crafts"+(i).ToString());
		}
		for(int i=13;i<18;i++){
			allGraphics[i]=CreateTexture("Bullets/Bullets"+(i-12).ToString());
		}
		allGraphics[18]=CreateTerrainTexture();
		Resources.UnloadUnusedAssets();
		packedTexture = new Texture2D(8, 8, TextureFormat.ARGB4444, false);
		packedTexture.filterMode=FilterMode.Point;
		packedAssets=packedTexture.PackTextures(allGraphics,1);
		Debug.Log(packedTexture.width+"x"+packedTexture.height);
		
		foreach (Texture2D tex in allGraphics)
		{
			Texture2D.DestroyImmediate(tex, true);
		}
		CreateSprites();
		DuplicateCrafts(90);
    }

    private void DuplicateCrafts(int num)
    {
        craftsInScene=new GameObject[num+12];
		for(int i=0;i<crafts.Length;i++){
			craftsInScene[i]=crafts[i];
		}
		int width=5;
		int height=5;
		Vector2 pos=new Vector2();
		for(int i=crafts.Length;i<crafts.Length+num;i++){
			pos.x=((2*Random.Range(0,2))-1)*Random.Range(0,width);
			pos.y=((2*Random.Range(0,2))-1)*Random.Range(0,height);
			craftsInScene[i]=Instantiate(crafts[0],pos,Quaternion.identity);
		}
    }

    void Update () {
		offset.x+=(scrollSpeed*Time.deltaTime);
		material.mainTextureOffset = offset;
		
		Vector2 mousePos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 pos;
		Vector2 desiredVel;
		foreach(GameObject go in craftsInScene){
			pos=go.transform.localPosition;
			if(Random.Range(0,5)<3){
				desiredVel= ((mousePos-pos).normalized)*6;
			}else{
				desiredVel= ((pos-mousePos).normalized)*6;
			}
			pos+=desiredVel*Time.deltaTime;
			go.transform.localPosition=pos;
		}
    }

    Texture2D CreateTerrainTexture()
    {
        //Random.InitState(23);
		//128x64
		int terrainWidth=256;
		int terrainHeight=16;
		
		int maxDistance=terrainWidth/16;//looking for min 10 points;
		int minDistance=terrainWidth/32;
		int walker=0;
		int minHeight=2;
		int newHeight=Random.Range(0,terrainHeight);
		Vector2 newMarker=new Vector2(walker,newHeight);
		List<Vector2> markers= new List<Vector2>();
		markers.Add(newMarker);
		while (walker<terrainWidth-maxDistance){
			int nextStop=Random.Range(minDistance,maxDistance);
			walker+=nextStop;
			newHeight=Random.Range(minHeight,terrainHeight-minHeight);
			newMarker=new Vector2(walker,newHeight);
			markers.Add(newMarker);
		}
		markers.Add(markers[0]);
		Debug.Log("terrain points "+markers.Count.ToString());
		Texture2D terrainTex= new Texture2D(terrainWidth, terrainHeight, TextureFormat.ARGB4444, false);
		terrainTex.filterMode=FilterMode.Point;
		Vector2 nextMarker;
		int newX=0;
		int newY=0;
		
		for(int i =0;i<markers.Count-1;i++){
			newMarker=markers[i];
			if(i==markers.Count-2){
				nextMarker=markers[markers.Count-1];
				nextMarker.x=terrainWidth;
			}else{
				nextMarker=markers[i+1];
			}
			//Debug.Log(newMarker.ToString()+nextMarker.ToString());
			float slope= (nextMarker.y-newMarker.y)/(nextMarker.x-newMarker.x);
			int delta= (int)(nextMarker.x-newMarker.x);
			for(int j =0;j<delta;j++){
				newX=(int)newMarker.x+j;
				newY=(int)((slope*(newX-newMarker.x))+newMarker.y);
				for (int k = 0; k < terrainHeight; k++) {
					if(k<newY){
						terrainTex.SetPixel(newX, k, Color.white);	
					}else terrainTex.SetPixel(newX, k, Color.clear);
				}
			}
		}
		terrainTex.Apply();
		return terrainTex;
    }

    private void CreateSprites()
    {
        int width=5;
		int height=5;
		GameObject gameObject;
		SpriteRenderer sr;
		Rect newRect;
		Vector2 pos=new Vector2();
		Sprite sprite;
		crafts=new GameObject[12];
		for(int i=0;i<18;i++){
			gameObject=new GameObject();
			sr = gameObject.AddComponent<SpriteRenderer>();
			newRect=ConvertUVToTextureCoordinates(packedAssets[i]);
			sprite= Sprite.Create(packedTexture,newRect,new Vector2(0.5f,0.5f));
			sr.sprite=sprite; 
			pos.x=((2*Random.Range(0,2))-1)*Random.Range(0,width);
			pos.y=((2*Random.Range(0,2))-1)*Random.Range(0,height);
			gameObject.transform.localPosition=pos;
			gameObject.transform.localScale=Vector2.one*10;
			if(i>0&&i<13){
				crafts[i-1]=gameObject;
			}
		}
		gameObject=new GameObject();
		SpriteRenderer terrainRenderer = gameObject.AddComponent<SpriteRenderer>();
		terrainRenderer.material=material;
		newRect=ConvertUVToTextureCoordinates(packedAssets[18]);
		sprite= Sprite.Create(packedTexture,newRect,new Vector2(0.5f,0.5f));
		terrainRenderer.sprite=sprite; 
		gameObject.transform.localScale=Vector2.one*20;
    }

    Texture2D CreateTexture(string textName)
    {
		int invalidTile=-1;
		
		TextAsset textFile = Resources.Load (textName) as TextAsset;
		string[] lines = textFile.text.Split (new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);//split by new line, return
		string[] nums = lines[0].Split(new[] { ',' });//split by ,
		int rows=lines.Length;//number of rows
		int cols=nums.Length;//number of columns
		int[,] levelData = new int[rows, cols];
        for (int i = 0; i < rows; i++) {
			string st = lines[i];
            nums = st.Split(new[] { ',' });
			for (int j = 0; j < cols; j++) {
                int val;
                if (int.TryParse (nums[j], out val)){
                	levelData[i,j] = val;
				}
                else{
                    levelData[i,j] = invalidTile;
				}
            }
        }
		
		levelData=rotateCW(levelData,rows,cols);
		//need to flip rows and cols as we rotated the array
		Texture2D texture = new Texture2D(cols, rows, TextureFormat.ARGB4444, false);
		texture.filterMode=FilterMode.Point;
		for (int i = 0; i < cols; i++) {
			for (int j = 0; j < rows; j++) {
                int val=levelData[i,j];
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
	private Rect ConvertUVToTextureCoordinates(Rect rect)
    {
        return new Rect(rect.x*packedTexture.width,
			rect.y*packedTexture.height,
			rect.width*packedTexture.width,
			rect.height*packedTexture.height
			);
    }

}
