namespace Liuguang.mod.Taiwu.BookLibrary;

/// <summary>
/// 分页类
/// </summary>
internal class Pagination
{
    /// <summary>
    /// 总共有多少条记录
    /// </summary>
    public int ItemTotalCount { get; set; } = 0;
    /// <summary>
    /// 每页最多显示多少条
    /// </summary>
    public int MaxItemsLimit { get; private set; } = 20;
    /// <summary>
    /// 当前是第几页
    /// </summary>
    public int CurrentPage { get; set; } = 1;
    /// <summary>
    /// 总共有多少页
    /// </summary>
    public int TotalPage
    {
        get
        {
            var totalPage = ItemTotalCount / MaxItemsLimit;
            if ((ItemTotalCount % MaxItemsLimit) > 0)
            {
                totalPage++;
            }
            return totalPage;
        }
    }

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrev => CurrentPage > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNext => CurrentPage < TotalPage;
    public Pagination(int itemsLimit)
    {
        MaxItemsLimit = itemsLimit;
    }

    public void GotoPrev()
    {
        CurrentPage -= 1;
    }

    public void GotoNext()
    {
        CurrentPage += 1;
    }
}
