using UnityEngine;

namespace Liuguang.mod.Taiwu.BookLibrary;

internal class MainController : MonoBehaviour
{
    #region HotKey
    private static KeyCode HotKey = KeyCode.Home;
    public static void UpdateHotKey(string HotKeyText)
    {
        if (!Enum.TryParse(HotKeyText, out HotKey))
        {
            HotKey = KeyCode.Home;
        }
    }
    #endregion

    private MainWindow? mainWindow;
    //private int currentPage = 1;

    public void Awake()
    {
        if (mainWindow == null)
        {
            mainWindow = new MainWindow();
            mainWindow.InitUI();
        }
    }

    public void OnDestroy()
    {
        mainWindow?.DestroyUI();
    }

    public void Update()
    {
        if (Input.GetKeyUp(HotKey))
        {
            mainWindow?.SwitchActiveStatus();
        }
    }
}