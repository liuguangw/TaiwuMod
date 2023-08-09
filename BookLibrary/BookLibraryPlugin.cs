using TaiwuModdingLib.Core.Plugin;
using UnityEngine;

namespace Liuguang.mod.Taiwu.BookLibrary;

[PluginConfig("BookLibraryPlugin", "liuguang", "1.0.0.0")]
public class BookLibraryPlugin : TaiwuRemakePlugin
{
    private static GameObject? gameObject;

    public override void Initialize()
    {
        LoadModSetting();
        gameObject = new GameObject("taiwu.BookLibrary");
        gameObject.AddComponent<MainController>();
    }

    public override void Dispose()
    {
        if (gameObject != null)
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }

    public override void OnModSettingUpdate()
    {
        LoadModSetting();
    }

    /// <summary>
    /// 加载mod设置
    /// </summary>
    private void LoadModSetting()
    {
        string HotKeyText = string.Empty;
        ModManager.GetSetting(ModIdStr, nameof(HotKeyText), ref HotKeyText);
        MainController.UpdateHotKey(HotKeyText);
    }

}
