using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class URLRawImage : RawImage
{
    [SerializeField] string m_Url = null;
    public string url {
        get {
            return m_Url;
        }
        set {
            if (!string.IsNullOrEmpty (m_Url) && CacheManager.Instance)
                CacheManager.Instance.DestroyTexture (m_Url);
            m_Url = value;
            if (!string.IsNullOrEmpty(m_Url) && CacheManager.Instance)
                CacheManager.Instance.DownloadTexture(m_Url, FileType.rawImage, DownloadTextureCallback);
        }
    }
    void Start()
    {
        base.Start();
        if (!string.IsNullOrEmpty(m_Url))
        {
            url = m_Url;
        }
    }
    void DownloadTextureCallback (Texture dtexture) {
        texture = dtexture;
        AspectRatioFitter fitter = GetComponent<AspectRatioFitter> ();
        if (fitter != null) {
            fitter.aspectRatio = texture.width / (float)texture.height;
        }
    }
    protected override void OnDestroy () {
        if (!string.IsNullOrEmpty (m_Url))
            if(CacheManager.Instance) CacheManager.Instance.DestroyTexture (m_Url);
    }
}
#if UNITY_EDITOR
[CustomEditor (typeof (URLRawImage))]
public class URLRamImageEditor : UnityEditor.UI.RawImageEditor {
    SerializedProperty m_Url;
    protected override void OnEnable () {
        base.OnEnable ();
        m_Url = serializedObject.FindProperty ("m_Url");
    }
    public override void OnInspectorGUI () {
        serializedObject.Update ();
        EditorGUILayout.PropertyField (m_Url);
        serializedObject.ApplyModifiedProperties ();
        base.OnInspectorGUI ();
    }
}
#endif