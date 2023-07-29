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
    #endregion

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
    }

    /// <summary>
    /// 加载mod设置
    /// </summary>
    private void loadModSetting()
    {
        DomainManager.Mod.GetSetting(base.ModIdStr, nameof(AttainmentRequirementReduce), ref AttainmentRequirementReduce);
        DomainManager.Mod.GetSetting(base.ModIdStr, nameof(UpgradeMakeItem), ref UpgradeMakeItem);
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
