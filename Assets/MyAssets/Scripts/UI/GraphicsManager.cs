using UnityEngine;

public class GraphicsManager: MonoBehaviour
{
    void Start()
    {
#if MOBILE_INPUT
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(Constants.GRAPHICS_LEVEL_PREFS, 0));
        if (!PlayerPrefs.HasKey(Constants.GRAPHICS_LEVEL_PREFS))
            ILogger.Instance.ShowMessage("Graphics is set to <B>low</B>, you can change that in the graphics settings", LoggerType.info);
#else
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(Constants.GRAPHICS_LEVEL_PREFS, 2));
        if (!PlayerPrefs.HasKey(Constants.GRAPHICS_LEVEL_PREFS))
            ILogger.Instance.ShowMessage("Graphics is set to <B>High</B>, you can change that in the graphics settings", LoggerType.info);
#endif
        PlayerPrefs.SetInt(Constants.GRAPHICS_LEVEL_PREFS, QualitySettings.GetQualityLevel());
        Application.targetFrameRate = 300;
    }
    public void SetQualitySettings(int level)
    {
        if (level == QualitySettings.GetQualityLevel()) return;
#if !MOBILE_INPUT
        if (level > 2)
            ILogger.Instance.ShowMessage("Graphics level is set to High now, this may affect your performance!");
#endif
        PlayerPrefs.SetInt(Constants.GRAPHICS_LEVEL_PREFS, level);
        QualitySettings.SetQualityLevel(level);
    }
}
