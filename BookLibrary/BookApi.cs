using GameData.Domains;
using GameData.Domains.Character;
using GameData.Domains.Item;
using GameData.GameDataBridge;


namespace Liuguang.mod.Taiwu.BookLibrary;

internal static class BookApi
{
    /// <summary>
    /// 获取书籍接口
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="bookId"></param>
    /// <param name="amount"></param>
    /// <param name="pageTypes"></param>
    public static void GetBook(int playerId, short bookId, int amount, byte pageTypes = 0)
    {
        GameDataBridge.AddMethodCall(
            -1, DomainHelper.DomainIds.Character, CharacterDomainHelper.MethodIds.CreateInventoryItem,
            playerId, ItemType.SkillBook, bookId, amount, pageTypes);
    }
}
