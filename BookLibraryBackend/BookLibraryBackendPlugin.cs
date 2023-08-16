using GameData.Common;
using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using HarmonyLib;
using TaiwuModdingLib.Core.Plugin;

namespace Liuguang.mod.Taiwu.BookLibraryBackend;

[PluginConfig("BookLibraryBackend", "liuguang", "1.0.0.0")]
public class BookLibraryBackendPlugin : TaiwuRemakePlugin
{

    private Harmony? harmony;

    public override void Initialize()
    {
        harmony = Harmony.CreateAndPatchAll(typeof(BookLibraryBackendPlugin));
    }

    public override void Dispose()
    {
        harmony?.UnpatchSelf();
    }

    /// <summary>
    /// 劫持CharacterDomain的CallMethod, 如果是工具调用的(5参数)则执行特别处理.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(CharacterDomain), "CallMethod")]
    public static bool Character_CallMethod_PrePatch(Operation operation, RawDataPool argDataPool, RawDataPool returnDataPool, DataContext context, ref int __result)
    {
        int argsOffset = operation.ArgsOffset;
        var methodId = operation.MethodId;
        if (methodId == CharacterDomainHelper.MethodIds.CreateInventoryItem)
        {
            var argsCount = operation.ArgsCount;
            if (argsCount != 5)
            {
                return true;
            }
            int charId = 0;
            argsOffset += Serializer.Deserialize(argDataPool, argsOffset, ref charId);
            sbyte itemType = 0;
            argsOffset += Serializer.Deserialize(argDataPool, argsOffset, ref itemType);
            short templateId = 0;
            argsOffset += Serializer.Deserialize(argDataPool, argsOffset, ref templateId);
            int amount = 0;
            argsOffset += Serializer.Deserialize(argDataPool, argsOffset, ref amount);
            byte pageTypes = 0;
            argsOffset += Serializer.Deserialize(argDataPool, argsOffset, ref pageTypes);
            if (itemType == ItemType.SkillBook)
            {
                var itemDisplayDataList = CreateInventoryItem(context, charId, templateId, amount, pageTypes);
                __result = Serializer.Serialize(itemDisplayDataList, returnDataPool);
                //false 不继续执行原来的CallMethod函数
                return false;
            }
        }
        //true表示后续继续执行
        return true;
    }

    private static List<ItemDisplayData> CreateInventoryItem(DataContext context, int charId, short templateId, int amount, byte pageTypes)
    {
        var character = DomainManager.Character.GetElement_Objects(charId);
        var itemDomain = DomainManager.Item;
        var itemDisplayDataList = new List<ItemDisplayData>(amount);
        for (int i = 0; i < amount; i++)
        {
            var itemKey = itemDomain.CreateSkillBook(context, templateId, pageTypes, 5);
            //修改耐久值
            var bookInfo = itemDomain.GetElement_SkillBooks(itemKey.Id);
            var durability = Config.SkillBook.Instance[templateId].MaxDurability;
            bookInfo.SetMaxDurability(durability, context);
            bookInfo.SetCurrDurability(durability, context);
            //添加到背包
            character.AddInventoryItem(context, itemKey, 1);
            itemDisplayDataList.Add(new(bookInfo, 1, charId));
        }
        return itemDisplayDataList;
    }

}
