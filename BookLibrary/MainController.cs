using UnityEngine;

namespace Liuguang.mod.Taiwu.BookLibrary;

internal class MainController : MonoBehaviour
{
    #region HotKey
    private static KeyCode HotKey = KeyCode.F5;
    public static void UpdateHotKey(string HotKeyText)
    {
        if (!Enum.TryParse(HotKeyText, out HotKey))
        {
            HotKey = KeyCode.F5;
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