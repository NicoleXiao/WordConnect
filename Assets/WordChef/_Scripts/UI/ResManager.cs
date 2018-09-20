using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.IO;

/// <summary>
/// 图片资源加载 (图集支持Starling/Sparrow格式)
/// author: zhouzhanglin

/// </summary>
public class ResManager : MonoBehaviour
{
	//读取文件的路径，如果是写文件，不需要加file://前缀
	class PathUtil{
		#if UNITY_EDITOR
		public static string streamingAssetPath = "file://" + Application.streamingAssetsPath + "/";
		public static string persistentDataPath = "file://" + Application.persistentDataPath + "/";
		public static string temporaryCachePath = "file://" + Application.temporaryCachePath + "/";
		#elif UNITY_ANDROID
		public static string streamingAssetPath = Application.streamingAssetsPath + "/";
		public static string persistentDataPath = "file://" + Application.persistentDataPath + "/";
		public static string temporaryCachePath = "file://" + Application.temporaryCachePath + "/";
		#else
		public static string streamingAssetPath = "file://" + Application.streamingAssetsPath + "/";
		public static string persistentDataPath = "file://" + Application.persistentDataPath + "/";
		public static string temporaryCachePath = "file://" + Application.temporaryCachePath + "/";
		#endif
	}

	public static string streamingAssetPath {
		get{
			return PathUtil.streamingAssetPath;
		}
	}

	public static string persistentDataPath {
		get{
			return PathUtil.persistentDataPath;
		}
	}

	public static string temporaryCachePath {
		get{
			return PathUtil.temporaryCachePath;
		}
	}

	private static ResManager m_instance;
	public static ResManager Instance {
		get {
			if (m_instance == null) {
				GameObject go = new GameObject ();
				m_instance = go.AddComponent<ResManager> ();
			}
			return m_instance;
		}
	}

	public static bool IsInited(){
		return m_instance != null;
	}

	void Awake ()
	{
		if (m_instance != null) {
			Destroy (gameObject);
			return;
		}

		name = "[ResManager]";
		m_instance = this;
		DontDestroyOnLoad (gameObject);
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += delegate(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) {
			if(disposeAssetsOnSceneLoad && mode== UnityEngine.SceneManagement.LoadSceneMode.Single){
				DisposeAll();
			}
		};
		maxLoadingCount = Mathf.Clamp (maxLoadingCount,1,20);

        if(debug){
            print ("ResManager>>>>> persistentDataPath :" + Application.persistentDataPath);
        }
	}

	/// <summary>
	/// 返回的资源类型
	/// </summary>
	public enum AssetType
	{
		Sprite,
		//贴图
		Texture2D,
		//图集
		Sprites,
		//声音
		AudioClip,
		//文本
		Text,
		//二进制
		Bytes
	}

	/// <summary>
	/// 资源的路径
	/// </summary>
	public enum AssetPath
	{
		StreamingAssets,
		Resources,
		PersistentData,
		TemporaryCache
	}

	[System.Serializable]
	public class Asset
	{
		public string url;
		public AssetType type;
		public AssetPath path;
		[HideInInspector]
		public Dictionary<string,Sprite> sprites;
		
		private Sprite _sprite;
		public Sprite sprite{
			get{ 
				if(_sprite == null && texture != null){
					_sprite = CreateSprite(Vector2.one*0.5f);
				}
				return _sprite;
			}
			set{ _sprite = value;}
		}

		[HideInInspector]
		public Texture2D texture;
		[HideInInspector]
		public AudioClip audioClip;
		[HideInInspector]
		public string text;
		[HideInInspector]
		public byte[] bytes;

		//for sprite
		public SpriteMeshType meshType = SpriteMeshType.FullRect;
		//for texture2d
		public bool textureReadonly = true;
		//for texture2d
		public TextureWrapMode warpMode = TextureWrapMode.Clamp;
		//use search path
		public bool useSearchPath = true;

		public string pathName{
			get{
				if(path==AssetPath.StreamingAssets) return "StreamingAssets";
				else if(path==AssetPath.PersistentData) return "PersistentData";
				else if(path==AssetPath.TemporaryCache) return "TemporaryCache";
				return "Resources";
			}
		}
		
		public bool cached = false;

		public Asset ()
		{
		}

		public Asset (string url, AssetType type, AssetPath path = AssetPath.StreamingAssets, bool cached = false)
		{
			this.url = url;
			this.type = type;
			this.path = path;
			this.cached = cached;
		}

		/// <summary>
		/// 通过Asset中的Texture来创建 Sprite
		/// </summary>
		/// <returns>Sprite.</returns>
		/// <param name="pivot">原点.</param>
		public Sprite CreateSprite(Vector2 pivot){
			if(texture!=null){
				return Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),pivot,100,0,SpriteMeshType.FullRect);
			}
			return null;
		}

		//dispose myself
		public void Dispose(){
			ResManager.Instance.DisposeAsset(this);
		}
	}

	public string searchPath = "";
	
	public bool disposeAssetsOnSceneLoad = true;
	//优先加载加密文件还是一般文件
	public bool priorityLoadEncrypted = false;
	//一次最多几个同时加载
	[Range(1,20)]
	public int maxLoadingCount = 5;
	public bool debug = true;
	//加载完成的
	private Dictionary<string,Asset> m_LoadedKV = new Dictionary<string, Asset> ();
	//加载失败的
	private Dictionary<string,Asset> m_LoadFailedKV = new Dictionary<string, Asset>();
	//正在加载的对象
	private Dictionary<string,Asset> m_LoadingAssets = new Dictionary<string, Asset>();
	private int m_CurrentCount;

	/// <summary>
	/// Loads the group assets.
	/// </summary>
	/// <param name="assetGroup">Asset group.</param>
	/// <param name="onLoaded">On loaded.</param>
	/// <param name="onProgress">On progress.</param>
	public void LoadGroup (Asset[] assetGroup, Action<Asset[]> onLoaded, Action<Asset[],float> onProgress = null)
	{
		int count = assetGroup.Length;
		for (int i = 0; i < assetGroup.Length; ++i) {
			LoadAsset (assetGroup [i], delegate(Asset asset) {
				count--;
				assetGroup[assetGroup.Length-count-1] = asset;
				if (onProgress != null) {
					onProgress (assetGroup, 1f - (float)count / assetGroup.Length);
				}
				if (count == 0) {
					if (onLoaded != null)
						onLoaded (assetGroup);
				}
			},delegate(Asset asset) {
				count--;
				if (onProgress != null) {
					onProgress (assetGroup, 1f - (float)count / assetGroup.Length);
				}
				if (count == 0) {
					if (onLoaded != null)
						onLoaded (assetGroup);
				}
			});
		}
	}


	/// <summary>
	/// Loads the asset.
	/// </summary>
	/// <param name="asset">Asset.</param>
	/// <param name="onLoaded">On loaded call back.</param>
	/// <param name="onLoadFailed">On load failed call back.</param>
	public void LoadAsset (Asset asset, Action<Asset> onLoaded,Action<Asset> onLoadFailed = null)
	{
		string path = asset.pathName+asset.url;
		if(m_LoadFailedKV.ContainsKey(path)){
			if(onLoadFailed!=null){
				onLoadFailed(asset);
			}
			return;
		}
		if (m_LoadedKV.ContainsKey (path)) {
			Asset loaded = m_LoadedKV [path];
			if (loaded.textureReadonly != asset.textureReadonly ){
				Debug.LogError("已加载:"+asset.url+",但textureReadonly不一致!");
			}
			if (onLoaded != null)
				onLoaded (loaded);
		} else if(m_LoadingAssets.ContainsKey(path)){
			
			StartCoroutine(WaitLoad(asset,onLoaded,onLoadFailed));

		} else {
			StartCoroutine (LoadingAsset (asset, onLoaded,onLoadFailed));
		}
	}

	IEnumerator WaitLoad(Asset asset, Action<Asset> onLoaded,Action<Asset> onLoadFailed = null){
		bool isSuccess = true;
		string path = asset.pathName+asset.url;
		while(true){
			if(m_LoadFailedKV.ContainsKey(path)){
				isSuccess = false;
				break;
			}

			if (!m_LoadedKV.ContainsKey (path)) {
				yield return new WaitForFixedUpdate();
			}else break;
		}
		if(isSuccess){
			if(onLoaded!=null) onLoaded (m_LoadedKV [path]);
		}else{
			if(onLoadFailed!=null) onLoadFailed(asset);
		}
	}
	
	private string GetPathName(AssetPath path){
		if(path== AssetPath.StreamingAssets) return "StreamingAssets";
		if(path== AssetPath.PersistentData) return "PersistentData";
		else if(path==AssetPath.TemporaryCache) return "TemporaryCache";
		return "Resources";
	}
	private string GetPath(Asset asset){
        string search = asset.useSearchPath ? this.searchPath : "";
        if (File.Exists(Application.persistentDataPath + "/" + search + asset.url ) 
            || File.Exists(Application.persistentDataPath + "/" + search + asset.url + ".txt" ) )
        {
            return persistentDataPath + search;
        }

        if(asset.path== AssetPath.StreamingAssets) return streamingAssetPath+search;
        if(asset.path== AssetPath.PersistentData) return persistentDataPath+search;
		if(asset.path== AssetPath.TemporaryCache) return temporaryCachePath;
		return this.searchPath;
	}

	/// <summary>
	/// Gets the asset.
	/// </summary>
	/// <returns>The asset.</returns>
	/// <param name="url">URL.</param>
	public Asset GetAsset (string url,AssetPath path = AssetPath.StreamingAssets)
	{
		string pathName = GetPathName(path);

		if (m_LoadedKV.ContainsKey (pathName+url))
			return m_LoadedKV [pathName+url];
		return null;
	}
	
	/// <summary>
	/// Disposes the asset.
	/// </summary>
	/// <param name="sprite">Sprite.</param>
	public void DisposeAsset(Sprite sprite){
		if(sprite!=null && sprite.texture!=null){
			DisposeAsset(sprite.texture);
		}
	}
	
	/// <summary>
	/// Disposes the asset by texture.
	/// </summary>
	/// <param name="texture">Texture.</param>
	public void DisposeAsset(Texture texture){
		if (texture!=null){
			foreach(Asset asset in m_LoadedKV.Values){
				if (asset!=null && asset.texture == texture){
					DisposeAsset(asset);	
					break;
				}
			}
		}
	}

	/// <summary>
	/// Disposes the asset.
	/// </summary>
	/// <param name="url">URL.</param>
	public void DisposeAsset (string url,AssetPath path = AssetPath.StreamingAssets)
	{
		string pathName = GetPathName(path);

		if (m_LoadedKV.ContainsKey (pathName+url)) {
			DisposeAsset (m_LoadedKV [pathName+url]);
		}
	}

	/// <summary>
	/// Disposes the asset.
	/// </summary>
	/// <param name="asset">Asset.</param>
	public void DisposeAsset (Asset asset)
	{
		if (asset.path == AssetPath.StreamingAssets || asset.path == AssetPath.PersistentData) {
			
			if (asset.sprite)
				DestroyImmediate (asset.sprite, false);
			if (asset.sprites != null) {
				foreach (Sprite s in asset.sprites.Values) {
					if(s) DestroyImmediate (s, false);
				}
				asset.sprites = null;
			}
			if (asset.texture) {
				DestroyImmediate (asset.texture, false);
			}
			if(asset.audioClip){
				DestroyImmediate(asset.audioClip,false);
			}
			asset.bytes = null;
			asset.text = null;

		} else if (asset.path == AssetPath.Resources) {
			
			if (asset.sprite)
				Resources.UnloadAsset (asset.sprite);
			if (asset.texture)
				Resources.UnloadAsset (asset.texture);
			if (asset.sprites != null) {
				foreach (Sprite s in asset.sprites.Values) {
					if(s) DestroyImmediate (s, false);
				}
				asset.sprites = null;
			}
			if(asset.audioClip){
				Resources.UnloadAsset(asset.audioClip);
			}
			asset.bytes = null;
			asset.text = null;
		}
		if (m_LoadedKV.ContainsKey (asset.pathName+asset.url)) {
			m_LoadedKV.Remove (asset.pathName+asset.url);
		}
		print("ResManager>>>>> Dispose Asset:"+asset.url);
	}
	
	/// <summary>
	/// 移除，但不是dipose
	/// </summary>
	/// <param name="url">URL.</param>
	/// <param name="path">Path.</param>
	public void RemoveAssetDontDispose (string url,AssetPath path = AssetPath.StreamingAssets)
	{
		string pathName = GetPathName(path);
		url = pathName+url;

		if (m_LoadedKV.ContainsKey (url)) {
			m_LoadedKV.Remove(url);
		}
		if (m_LoadFailedKV.ContainsKey (url)) {
			m_LoadFailedKV.Remove (url);
		}
	}

	/// <summary>
	/// 移除失败的信息
	/// </summary>
	/// <param name="url">URL.</param>
	/// <param name="path">Path.</param>
	public void RemoveFailedAsset(string url,AssetPath path){
		if (m_LoadFailedKV.ContainsKey (GetPathName (path) + url)) {
			m_LoadFailedKV.Remove (GetPathName (path) + url);
		}
	}
	/// <summary>
	/// 清除所有的失败消息
	/// </summary>
	public void ClearFailedAssets(){
		m_LoadFailedKV.Clear ();
	}

	/// <summary>
	/// Disposes all.
	/// </summary>
	/// <param name="containCache">If set to <c>true</c> contain cache.</param>
	public void DisposeAll (bool containCache = false)
	{
		StopAllCoroutines ();
		m_CurrentCount = 0;
		List<Asset> assets = new List<Asset> ();
		foreach (Asset asset in m_LoadedKV.Values) {
			if (containCache) {
				assets.Add (asset);
			} else if (!asset.cached) {
				assets.Add (asset);
			}
		}
		for (int i = 0; i < assets.Count; ++i) {
			DisposeAsset (assets [i]);
		}
		m_LoadingAssets.Clear();
		m_LoadFailedKV.Clear ();
	}

	IEnumerator LoadingAsset (Asset asset, Action<Asset> onLoaded,Action<Asset> onLoadFailed = null)
	{
		if(m_LoadFailedKV.ContainsKey(asset.pathName+asset.url))
		{
			if(onLoadFailed!=null) onLoadFailed(asset);
		}
		else
		{
			while (m_CurrentCount >= maxLoadingCount) {
				yield return 0;
			}

			if (m_LoadedKV.ContainsKey (asset.pathName+asset.url)) {
				Asset loaded = m_LoadedKV [asset.pathName+asset.url];
				if (loaded.textureReadonly != asset.textureReadonly ){
					Debug.LogError("ResManager>>>>> 已加载:"+asset.url+",但textureReadonly不一致!");
				}
				if (onLoaded != null)
					onLoaded (loaded);
			}
			else
			{
				++m_CurrentCount;
				if(debug){
					print ("ResManager>>>>> Load CurrentCount :" + m_CurrentCount);
				}
				if (asset.path != AssetPath.Resources) {
					if(debug){
						print ("ResManager>>>>> Load Start :" + asset.pathName +"->" + asset.url);
					}
					string url = GetPath(asset) + asset.url;

					m_LoadingAssets[asset.pathName+asset.url] = asset;
					bool isReversed = priorityLoadEncrypted ? true : false;
					
					string realUrl = url + (priorityLoadEncrypted ? ".txt" : "");
					if(asset.type == AssetType.Sprites){
						realUrl = url;
						isReversed = false;
					}
					WWW www = new WWW (realUrl);
					while (!www.isDone) {
						yield return 0;
					}
					
					if (www.error != null && www.error.Length > 0) {
						www.Dispose ();
						isReversed = priorityLoadEncrypted ? false : true;
						www = new WWW (url + (priorityLoadEncrypted ? "" : ".txt"));
						while (!www.isDone) {
							yield return 0;
						}
					}	
					if(m_LoadingAssets.ContainsKey(asset.pathName+asset.url))  m_LoadingAssets.Remove(asset.pathName+asset.url);
					if (www.error == null || www.error.Length == 0) {
						if(debug){
							print ("ResManager>>>>> Load Success :" + asset.pathName +"->" + asset.url);
						}
						if (asset.type == AssetType.Sprite || asset.type == AssetType.Texture2D) {
							if (isReversed) {
								Texture2D texture = new Texture2D (2, 2,TextureFormat.RGBA32,false);
								byte[] bytes=ReverseBytes (www.bytes);
								texture.LoadImage (bytes, asset.textureReadonly);
								asset.texture = texture;
								bytes = null;
							} else {
								asset.texture = asset.textureReadonly ? www.textureNonReadable : www.texture;
							}
							asset.texture.name = asset.url.Substring(asset.url.LastIndexOf("/")+1);
						}


						if (asset.type == AssetType.Sprite) {
							asset.texture.wrapMode = TextureWrapMode.Clamp;
							asset.sprite = Sprite.Create (asset.texture, new Rect (0f, 0f, asset.texture.width, asset.texture.height), Vector2.one * 0.5f, 100, 1, asset.meshType);
							asset.sprite.name = asset.texture.name;				
							HandleLoadedAsset(asset,onLoaded);

						} else if (asset.type == AssetType.Texture2D) {
							
							asset.texture.wrapMode = asset.warpMode;
							HandleLoadedAsset(asset,onLoaded);

						} else if (asset.type == AssetType.Sprites && asset.url.LastIndexOf (".xml") == asset.url.Length-4) {

							yield return StartCoroutine(LoadingSpritesByXML (www.text, asset, onLoaded,onLoadFailed));

						}else if(asset.type == AssetType.AudioClip){
                            asset.audioClip = www.GetAudioClip(false,true);
							asset.audioClip.name = asset.url;
							HandleLoadedAsset(asset,onLoaded);

						}else if(asset.type == AssetType.Text){
							asset.text = www.text;
							HandleLoadedAsset(asset,onLoaded);
						}else if(asset.type == AssetType.Bytes){
							asset.bytes = www.bytes;
							HandleLoadedAsset(asset,onLoaded);
						}

					} else {
						HandleAssetFail(asset,onLoadFailed);
					}
					www.Dispose ();

				} else{

					m_LoadingAssets[asset.pathName + asset.url] = asset;	
					if (asset.type == AssetType.Sprite) {

						ResourceRequest rr = Resources.LoadAsync<Sprite> (asset.url);
						yield return rr;
						if(m_LoadingAssets.ContainsKey(asset.pathName + asset.url))  m_LoadingAssets.Remove(asset.pathName + asset.url);
						if (rr.isDone && rr.asset != null ) {
							asset.sprite = rr.asset as Sprite;
							if (asset.sprite) {
								asset.texture = asset.sprite.texture;
								HandleLoadedAsset(asset,onLoaded);
							}
						}else{
							HandleAssetFail(asset,onLoadFailed);
						}

					} else if (asset.type == AssetType.Texture2D) {

						ResourceRequest rr = Resources.LoadAsync<Texture2D> (asset.url);
						yield return rr;
						if(m_LoadingAssets.ContainsKey(asset.pathName + asset.url))  m_LoadingAssets.Remove(asset.pathName + asset.url);
						if (rr.isDone && rr.asset !=null) {
							asset.texture = rr.asset as Texture2D;
							HandleLoadedAsset(asset,onLoaded);
						}else{
							HandleAssetFail(asset,onLoadFailed);
						}

					} else if (asset.type == AssetType.Sprites) {

						if(m_LoadingAssets.ContainsKey(asset.pathName + asset.url))  m_LoadingAssets.Remove(asset.pathName + asset.url);
						Sprite[] sprites = Resources.LoadAll<Sprite> (asset.url);
						if (sprites != null && sprites.Length > 0) {
							asset.sprites = new Dictionary<string, Sprite> ();
							foreach (Sprite s in sprites) {
								asset.sprites [s.name] = s;
							}
							HandleLoadedAsset(asset,onLoaded);
						} else {
							HandleAssetFail(asset,onLoadFailed);
						}
					}else if(asset.type == AssetType.AudioClip){
						ResourceRequest rr = Resources.LoadAsync<AudioClip> (asset.url);
						yield return rr;
						asset.audioClip = rr.asset as AudioClip;
						if(asset.audioClip==null) HandleAssetFail(asset,onLoadFailed);
						else HandleLoadedAsset(asset,onLoaded);

					}else if(asset.type == AssetType.Text){
						ResourceRequest rr = Resources.LoadAsync<TextAsset> (asset.url);
						yield return rr;
						TextAsset ta = (rr.asset as TextAsset);
						if(ta) asset.text = ta.text;
						if(ta==null) HandleAssetFail(asset,onLoadFailed);
						else HandleLoadedAsset(asset,onLoaded);

					}else if(asset.type == AssetType.Bytes){
						ResourceRequest rr = Resources.LoadAsync<TextAsset> (asset.url);
						yield return rr;
						TextAsset ta = (rr.asset as TextAsset);
						if(ta) asset.bytes = ta.bytes;
						if(ta==null) HandleAssetFail(asset,onLoadFailed);
						else HandleLoadedAsset(asset,onLoaded);
					}
				}
				--m_CurrentCount;
				if(debug){
					print ("ResManager>>>>> Load CurrentCount :" + m_CurrentCount);
				}
			}
		}
	}
	void HandleLoadedAsset(Asset asset,Action<Asset> onLoaded){
		m_LoadedKV [asset.pathName+asset.url] = asset;
		if(m_LoadFailedKV.ContainsKey(asset.pathName+asset.url)){
			m_LoadFailedKV.Remove(asset.pathName+asset.url);
		}
		if (onLoaded != null)
			onLoaded (asset);
	}
	void HandleAssetFail(Asset asset,Action<Asset> onLoadFailed){
		if(debug){
			Debug.LogWarning  ("ResManager>>>>> Load error :"  + asset.pathName +"->"+ asset.url);
		}
		m_LoadFailedKV[asset.pathName+asset.url] = asset;
		if(onLoadFailed!=null) onLoadFailed(asset);
	}

	IEnumerator LoadingSpritesByXML (string config, Asset asset, Action<Asset> onLoaded,Action<Asset> onLoadFailed = null)
	{
		string atlasPath = asset.url.Substring (0, asset.url.LastIndexOf (".xml")) + ".png";
		bool isReversed = priorityLoadEncrypted ? true : false;
		string url = GetPath(asset) + atlasPath;
		WWW www = new WWW (url + (priorityLoadEncrypted ? ".txt" : ""));
		while (!www.isDone) {
			yield return 0;
		}
		if (www.error != null && www.error.Length > 0) {
			www.Dispose ();
			isReversed = priorityLoadEncrypted ? false : true;
			www = new WWW (url + (priorityLoadEncrypted ? "" : ".txt"));
			while (!www.isDone) {
				yield return 0;
			}
		}

		if (www.error == null || www.error.Length == 0) {
			if (isReversed) {
				Texture2D texture = new Texture2D (2, 2,TextureFormat.RGBA32,false);
				texture.wrapMode = TextureWrapMode.Clamp;
				byte[] bytes=ReverseBytes (www.bytes);
				texture.LoadImage (bytes, asset.textureReadonly);
				asset.texture = texture;
				bytes = null;
			} else {
				asset.texture = asset.textureReadonly ? www.textureNonReadable : www.texture;
			}
			ReadSpritesByXML(config,asset);
			HandleLoadedAsset(asset,onLoaded);

		} else {
			HandleAssetFail(asset,onLoadFailed);
		}
		www.Dispose ();
	}

	public static byte[] ReverseBytes (byte[] bytes)
	{
		Array.Reverse(bytes);
		return bytes;
	}

	void ReadSpritesByXML(string config,Asset asset){

		XmlDocument xmlDoc = new XmlDocument ();
		xmlDoc.LoadXml (config);
		XmlNode root = xmlDoc.SelectSingleNode ("TextureAtlas");
		XmlNodeList nodeList = root.ChildNodes;

		asset.sprites = new Dictionary<string, Sprite> ();
		//遍历所有子节点
		foreach (XmlNode xn in nodeList) {
			if (!(xn is XmlElement))
				continue;
			XmlElement xe = (XmlElement)xn;
			string frameName = xe.GetAttribute ("name").Replace ('/', '_');
			float x = float.Parse (xe.GetAttribute ("x"));
			float y = float.Parse (xe.GetAttribute ("y"));
			float w = float.Parse (xe.GetAttribute ("width"));
			float h = float.Parse (xe.GetAttribute ("height"));
			Sprite s = Sprite.Create (asset.texture, new Rect (x, asset.texture.height - h - y, w, h), Vector2.one * 0.5f, 100, 1, asset.meshType);
			s.name = frameName;
			asset.sprites [frameName] = s;
		}
	}





	#region  最简单基础的图片加载

	/// <summary>
	/// 从StreamingAsset目录下加载Sprite (不会缓存图片，相同图片会重复加载)
	/// 如果要缓存加载图片或对下载数量进行限制，请使用LoadAsset 或 LoadGroup接口
	/// </summary>
	/// <param name="url">取StreamingAsset下的图片路径.</param>
	/// <param name="onLoaded">加载后的回调.</param>
	/// <param name="pivot">Sprite的原点.</param>
	/// <param name="isAsyncInIOS">IOS中读取图片是否为异步.</param>
	/// <param name="useSearchPath">是否使用预设的search path.</param>
	/// <param name="meshType">默认为FullRect.</param>
	public void LoadSpriteFromStreamingAsset(string url,Action<Sprite> onLoaded,Vector2 pivot,bool isAsyncInIOS = true, Action<string> onLoadFailed = null,bool useSearchPath = true,SpriteMeshType meshType = SpriteMeshType.FullRect)
	{
		LoadTexture2DFromStreamingAsset (url, delegate(Texture2D t) {
		Sprite s = Sprite.Create(t,new Rect(0,0,t.width,t.height),pivot,100,0,meshType);
			if(onLoaded!=null) onLoaded(s);
		}, isAsyncInIOS, false,onLoadFailed,useSearchPath);
	}

	/// <summary>
	/// 从StreamingAsset目录下加载Texture (不会缓存图片，相同图片会重复加载)
	/// 如果要缓存加载图片或对下载数量进行限制，请使用LoadAsset 或 LoadGroup接口
	/// </summary>
	/// <param name="url">取StreamingAsset下的图片路径.</param>
	/// <param name="onLoaded">加载后的回调.</param>
	/// <param name="isAsyncInIOS">IOS中读取图片是否为异步.</param>
	/// <param name="readOnly">材质是否包含像素值.</param>
	/// <param name="useSearchPath">是否使用预设的search path.</param>
	/// 
	public void LoadTexture2DFromStreamingAsset(string url,Action<Texture2D> onLoaded,bool isAsyncInIOS = true,bool readOnly = true, Action<string> onLoadFailed = null,bool useSearchPath = true){
		string searchPath = useSearchPath ? this.searchPath : "";

        string rootFolder = persistentDataPath;
        string realPath = Application.persistentDataPath + "/" + searchPath + url;
        if (!File.Exists(realPath) && !File.Exists(realPath + ".txt"))
        {
            realPath = Application.streamingAssetsPath + "/" + searchPath + url;
            rootFolder = streamingAssetPath; 
        }
 
		#if UNITY_IOS

		if(isAsyncInIOS)
		{
            StartCoroutine(WWWSyncLoadTexture2D(rootFolder+ searchPath +url,onLoaded,readOnly,onLoadFailed));
		}
		else
		{
			bool isHaveNoEncrypted = File.Exists(realPath);
			bool isHaveEncrypted = File.Exists(realPath+".txt");
			if(isHaveNoEncrypted==false && isHaveEncrypted==false){
				if (debug) {
					Debug.LogWarning ("ResManager>>>>> LoadTexture2DFromStreamingAsset Fail:" + url);
				}
				if(onLoadFailed!=null) {
					onLoadFailed(url);
				}
				return;
			}
			byte[] bytes = File.ReadAllBytes(isHaveNoEncrypted? realPath : realPath+".txt");
			if(bytes!=null){
				if(isHaveEncrypted){
					bytes = ReverseBytes(bytes);
				}
				Texture2D texture = new Texture2D (2, 2,TextureFormat.RGBA32,false);
				texture.wrapMode = TextureWrapMode.Clamp;
				texture.LoadImage (bytes,readOnly);
				if(onLoaded!=null){
					onLoaded(texture);
				}
			}
		}
		#else
        StartCoroutine(WWWSyncLoadTexture2D(rootFolder+searchPath+url,onLoaded,readOnly,onLoadFailed));
		#endif
	}

	IEnumerator WWWSyncLoadTexture2D(string url,Action<Texture2D> onLoaded,bool readOnly = true, Action<string> onLoadFailed = null){
		WWW www = new WWW ( priorityLoadEncrypted ? url+".txt":url );
        while (!www.isDone) {
			yield return null;
		}
        if (www.error == null || www.error.Length == 0) {
			GetTexture(www,priorityLoadEncrypted,onLoaded,readOnly);
            Debug.LogWarning ("ResManager>>>>> LoadTexture2DFromStreamingAsset success:" + url);
        } else{
            www = new WWW ( priorityLoadEncrypted ? url:url+".txt" );
			while (!www.isDone) {
				yield return null;
			}

			if (www.error == null || www.error.Length == 0) {
				GetTexture(www,!priorityLoadEncrypted,onLoaded,readOnly);
			}
			else
			{
				if (debug) {
					//Debug.LogWarning ("ResManager>>>>> LoadTexture2DFromStreamingAsset Fail:" + url);
				}
				if(onLoadFailed!=null) {
					onLoadFailed(url);
				}
			}
			
		}
	}

	void GetTexture(WWW www ,bool isEncrypted , Action<Texture2D> onLoaded,bool readOnly = true){
		if(isEncrypted){
			if(onLoaded!=null) {
				Texture2D texture = new Texture2D (2, 2,TextureFormat.RGBA32,false);
				texture.wrapMode = TextureWrapMode.Clamp;
				byte[] bytes=ReverseBytes (www.bytes);
				texture.LoadImage (bytes, readOnly);
				bytes = null;
				onLoaded(texture);
			}
		}
		else
		{
			if(onLoaded!=null) {
				Texture2D texture = readOnly ? www.textureNonReadable : www.texture;
				texture.wrapMode = TextureWrapMode.Clamp;
				onLoaded(texture);
			}
		}
		www.Dispose();
	}
	#endregion
}
