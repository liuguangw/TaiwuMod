using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
using GameData.Domains;
using GameData.Domains.Building;

namespace Liuguang.mod.Taiwu.BetterMake;

[PluginConfig("BetterMakePlugin", "liuguang", "1.0.0.0")]
public class BetterMakePlugin : TaiwuRemakePlugin
{
    #region Config
    public static bool AttainmentRequirementReduce = false;
    public static bool UpgradeMakeItem = false;
    public static int BuildingSpaceExtra = 0;
    #endregion

    private bool gameLoaded = false;

    private Harmony? harmony;

    public override void Initialize()
    {
        harmony = Harmony.CreateAndPatchAll(typeof(BetterMakePlugin));
        loadModSetting();
    }

    public override void Dispose()
    {
        harmony?.UnpatchSelf();
    }

    public override void OnModSettingUpdate()
    {
        loadModSetting();
        UpdateBuildingSpaceExtra();
    }

    public override void OnLoadedArchiveData()
    {
        gameLoaded = true;
        UpdateBuildingSpaceExtra();
    }

    /// <summary>
    /// 更新额外建造空间
    /// </summary>
    private void UpdateBuildingSpaceExtra()
    {
        if (gameLoaded)
        {
            DomainManager.Taiwu.SetBuildingSpaceExtraAdd(BuildingSpaceExtra, DomainManager.TaiwuEvent.MainThreadDataContext);
        }
    }

    /// <summary>
    /// 加载mod设置
    /// </summary>
    private void loadModSetting()
    {
        DomainManager.Mod.GetSetting(base.ModIdStr, nameof(AttainmentRequirementReduce), ref AttainmentRequirementReduce);
        DomainManager.Mod.GetSetting(base.ModIdStr, nameof(UpgradeMakeItem), ref UpgradeMakeItem);
        DomainManager.Mod.GetSetting(base.ModIdStr, nameof(BuildingSpaceExtra), ref BuildingSpaceExtra);
    }

    /// <summary>
    /// 制作调整
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BuildingDomain), "GetAttainmentRequirementReduceAndUpgradeMakeItem")]
    public static void Building_GetAttainmentRequirementReduceAndUpgradeMakeItem_PostPatch(ref (int, bool) __result)
    {
        var (reduceVal, upgrade) = __result;
        if (AttainmentRequirementReduce)
        {
            reduceVal += 300;
        }
        if (UpgradeMakeItem)
        {
            upgrade = true;
        }
        __result = (reduceVal, upgrade);
    }

}
