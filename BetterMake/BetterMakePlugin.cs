using GameData.Domains;
using GameData.Domains.Building;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;

namespace Liuguang.mod.Taiwu.BetterMake;

[PluginConfig("BetterMakePlugin", "liuguang", "1.0.0.0")]
public class BetterMakePlugin : TaiwuRemakePlugin
{
    #region Config
    /// <summary>
    /// 降低造诣要求
    /// </summary>
    public static bool AttainmentRequirementReduce = false;
    /// <summary>
    /// 无需额外建筑
    /// </summary>
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
        DomainManager.Mod.GetSetting(ModIdStr, nameof(AttainmentRequirementReduce), ref AttainmentRequirementReduce);
        DomainManager.Mod.GetSetting(ModIdStr, nameof(UpgradeMakeItem), ref UpgradeMakeItem);
        DomainManager.Mod.GetSetting(ModIdStr, nameof(BuildingSpaceExtra), ref BuildingSpaceExtra);
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
            reduceVal += 500;
        }
        if (UpgradeMakeItem)
        {
            upgrade = true;
        }
        __result = (reduceVal, upgrade);
    }

    /// <summary>
    /// 制造条件检测
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BuildingDomain), "CheckMakeCondition")]
    public static void Building_CheckMakeCondition_PostPatch(MakeConditionArguments makeConditionArguments,ref bool __result)
    {
        if (!AttainmentRequirementReduce)
        {
            return;
        }
        //角色id检测
        var charId = makeConditionArguments.CharId;
        if (DomainManager.Taiwu.GetTaiwuCharId() == charId)
        {
            __result = true;
        }
    }

}
