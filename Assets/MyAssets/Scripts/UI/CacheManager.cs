using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class CacheManager : MonoBehaviour {
    static string pathBase;
    Dictionary<string, Texture> texturesCache = new Dictionary<string, Texture> ();
    public static CacheManager Instance;
    void Awake()
    {
        pathBase = Application.persistentDataPath + "/Cache/";
        Instance = this;
    }
    void Start()
    {
        SetupCacheDirectories();
    }
    void SetupCacheDirectories()
    {
        if (!Directory.Exists(pathBase + "Textures"))
            Directory.CreateDirectory(pathBase + "Textures");
        if (!Directory.Exists(pathBase + "RawImages"))
            Directory.CreateDirectory(pathBase + "RawImages");
    }
    public static bool IsCashed(string url, FileType type)
    {
        return File.Exists(GetFilePath(url, type));
    }
    public static void Cache(FileType type, string url, byte[] data)
    {
        string path = GetFilePath(url, type);
        File.WriteAllBytes(path, data);
    }
    public static string GetCacheURI(string url, FileType type)
    {
        string path = GetFilePath(url, type);
        if (File.Exists(path))
            return "file:///" + path;
        return null;
    }
    public static string GetFilePath(string url, FileType type)
    {
        string fileName = url.GetHashCode() + url.Substring(url.LastIndexOf("/") + 1).Replace (' ', '_').Replace ("%", "_");
        return GetFilePath (fileName, url, type);
    }
    public static string GetFilePath (string fileName, string url, FileType type) {
        string folder = "";
        switch (type) {
            case FileType.texture:
                folder = "Textures/";
                break;
            case FileType.rawImage:
                folder = "RawImages/";
                break;
            default:
                break;
        }
        return pathBase + folder + fileName;
    }
    public void DownloadTexture(string url, FileType type, Action<Texture> callback )
    {
        string fileName = url.GetHashCode () + url.Substring (url.LastIndexOf ("/") + 1).Replace (' ', '_').Replace("%","_");
        if (texturesCache.ContainsKey (fileName) && texturesCache[fileName] != null) {
            callback (texturesCache[fileName]);
        } else {
            string cachePath = GetFilePath (fileName, url, type);
            bool isCached = File.Exists (cachePath);
            if (isCached) {
                Texture2D texture = new Texture2D (2, 2);
                texture.LoadImage (File.ReadAllBytes (cachePath), true);
                texturesCache.Add (fileName, texture);
                callback (texture);
            } else {
                StartCoroutine (DownloadTexture (fileName, cachePath, url, callback));
            }
        }
    }
    IEnumerator DownloadTexture(string fileName, string cachePath, string url, Action<Texture> callback)
    {
        if (texturesCache.ContainsKey (fileName)) {
            yield return new WaitUntil (() => texturesCache[fileName] != null);
            callback (texturesCache[fileName]);
            yield break;
        }
        texturesCache.Add (fileName, null);
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error + ":" + url);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                texturesCache[fileName] = texture;
                callback (texture);
                File.WriteAllBytes (cachePath, www.downloadHandler.data);
            }
        }
    }
    public void DestroyTexture (string url) {
        string fileName = url.GetHashCode () + url.Substring (url.LastIndexOf ("/") + 1).Replace (' ', '_').Replace ("%", "_");
        if (texturesCache.ContainsKey (fileName)) {
            DestroyImmediate (texturesCache[fileName]);
            texturesCache.Remove (fileName);
        }
    }
}
public enum FileType
{
    texture,
    rawImage
}
