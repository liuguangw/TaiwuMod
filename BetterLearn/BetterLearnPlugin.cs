using GameData.Domains;
using GameData.Domains.Taiwu;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;
//using GameData.Utilities;
using TaiwuSkillBreakPlate = GameData.Domains.Taiwu.SkillBreakPlate;

namespace Liuguang.mod.Taiwu.BetterLearn;

[PluginConfig("BetterLearnPlugin", "liuguang", "1.0.0.0")]
public class BetterLearnPlugin : TaiwuRemakePlugin
{
    #region Config
    public static bool BreakNotCostedStep = false;
    public static bool FastPractice = false;
    public static bool ResetBreakPlate = false;
    #endregion

    private Harmony? harmony;

    public override void Initialize()
    {
        harmony = Harmony.CreateAndPatchAll(typeof(BetterLearnPlugin));
        loadModSetting();
        //AdaptableLog.Info("BetterLearnPlugin 初始化");
    }

    public override void Dispose()
    {
        harmony?.UnpatchSelf();
        //AdaptableLog.Info("BetterLearnPlugin 销毁");
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
        DomainManager.Mod.GetSetting(ModIdStr, nameof(BreakNotCostedStep), ref BreakNotCostedStep);
        DomainManager.Mod.GetSetting(ModIdStr, nameof(FastPractice), ref FastPractice);
        DomainManager.Mod.GetSetting(ModIdStr, nameof(ResetBreakPlate), ref ResetBreakPlate);
    }

    /// <summary>
    /// 显示全部格子
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TaiwuDomain), "InitSkillBreakPlate")]
    public static void Taiwu_InitSkillBreakPlate_PostPatch(TaiwuSkillBreakPlate plate)
    {
        foreach (var gridItems in plate.Grids)
        {
            foreach (var grid in gridItems)
            {
                //全部格子显示
                grid.State = 0;
            }
        }
        if (ResetBreakPlate)
        {
            BreakPlateResetTool.ProcessResetBreakPlate(plate);
        }
    }

    /// <summary>
    /// 突破基础几率调整
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TaiwuDomain), "UpdateBreakPlateTotalStepAndSuccessRate")]
    public static void Taiwu_UpdateBreakPlateTotalStepAndSuccessRate_PostPatch(short skillTemplateId, TaiwuDomain __instance)
    {
        var plate = __instance.GetElement_SkillBreakPlateDict(skillTemplateId);
        plate.BaseSuccessRate = byte.MaxValue;
    }

    /// <summary>
    /// 突破不增加次数
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TaiwuDomain), "SelectSkillBreakGrid")]
    public static void Taiwu_SelectSkillBreakGrid_PostPatch(short skillId, TaiwuDomain __instance)
    {
        if (!BreakNotCostedStep)
        {
            return;
        }
        var plate = __instance.GetElement_SkillBreakPlateDict(skillId);
        plate.CostedStepCount = 0;
    }

    /// <summary>
    /// 修习一次100%
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(TaiwuDomain), "CalcPracticeResult")]
    public static void Taiwu_CalcPracticeResult_PostPatch(ref int __result)
    {
        if (!FastPractice)
        {
            return;
        }
        __result = 100;
    }
}
