using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.Domains.Item.Display;
using GameData.GameDataBridge;
using GameData.Serializer;


namespace Liuguang.mod.Taiwu.BookLibrary;

internal static class BookApi
{
    /// <summary>
    /// 获取书籍接口
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="bookIds"></param>
    /// <param name="amount"></param>
    /// <param name="pageTypes"></param>
    public static void GetBook(int playerId, List<short> bookIds, int amount, byte pageTypes = 0)
    {
        GameDataBridge.AddMethodCall(
            -1, DomainHelper.DomainIds.Character, CharacterDomainHelper.MethodIds.CreateInventoryItem,
            playerId, ItemType.SkillBook, bookIds, amount, pageTypes);
    }

    /// <summary>
    /// 异步调用
    /// </summary>
    /// <param name="onGetAction"></param>
    /// <param name="playerId"></param>
    /// <param name="bookIds"></param>
    /// <param name="amount"></param>
    /// <param name="pageTypes"></param>
    public static void AsyncGetBook(Action<List<ItemDisplayData>> onGetAction, int playerId, List<short> bookIds, int amount, byte pageTypes = 0)
    {
        var dispatcher = SingletonObject.getInstance<AsynchMethodDispatcher>();
        dispatcher.AsynchMethodCall(DomainHelper.DomainIds.Character, CharacterDomainHelper.MethodIds.CreateInventoryItem,
            playerId, ItemType.SkillBook, bookIds, amount, pageTypes,
            (argsOffset, dataPool) =>
            {
                var itemDisplayDataList = new List<ItemDisplayData>();
                Serializer.Deserialize(dataPool, argsOffset, ref itemDisplayDataList);
                onGetAction.Invoke(itemDisplayDataList);
            });
    }
}
