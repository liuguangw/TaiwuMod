//using GameData.Utilities;
using TaiwuSkillBreakPlate = GameData.Domains.Taiwu.SkillBreakPlate;

namespace Liuguang.mod.Taiwu.BetterLearn;

/// <summary>
/// 突破格重排工具
/// </summary>
internal static class BreakPlateResetTool
{
    /// <summary>
    /// 坐标
    /// </summary>
    internal readonly struct PlatePos
    {
        /// <summary>
        /// 行
        /// </summary>
        public readonly byte rowIndex;
        /// <summary>
        /// 列
        /// </summary>
        public readonly byte colIndex;

        public PlatePos(byte rowIndex, byte colIndex)
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }
    }
    //起点类型
    const sbyte BeginTemplateId = 0;
    //终点类型
    const sbyte EndTemplateId = 1;
    public static void ProcessResetBreakPlate(TaiwuSkillBreakPlate plate)
    {
        //起点、终点、金色格子的位置
        var (beginPos, endPos, bonusPosList) = ParseBreakPlate(plate);
        //AdaptableLog.TagInfo("plate", $"width={plate.Width}, height={plate.Height}");
        //计算目标格子列表
        var posList = CalcPosList(beginPos, endPos, plate.Width, bonusPosList.Count);
        foreach (var currentPos in posList)
        {
            //AdaptableLog.TagInfo("pos", $"row={currentPos.rowIndex}, col={currentPos.colIndex}");
            var currentGrid = plate.Grids[currentPos.rowIndex][currentPos.colIndex];
            //如果此格子已经是金色格子
            if (currentGrid.BonusType >= 0)
            {
                //bonusPosList中移除这个位置
                var bonusPosIndex = PosIndexOf(currentPos, bonusPosList);
                bonusPosList.RemoveAt(bonusPosIndex);
            }
            else
            {
                //取出最后一个金色格子与当前位置的格子交换
                var lastIndex = bonusPosList.Count - 1;
                var srcPos = bonusPosList[lastIndex];
                bonusPosList.RemoveAt(lastIndex);
                SwapGrid(plate, srcPos, currentPos);
            }
        }
    }

    /// <summary>
    /// 获取起点、终点、金色格子的坐标
    /// </summary>
    /// <returns></returns>
    private static (PlatePos, PlatePos, List<PlatePos>) ParseBreakPlate(TaiwuSkillBreakPlate plate)
    {
        //初始化
        var beginPos = new PlatePos(0, 0);
        var endPos = beginPos;
        var bonusPosList = new List<PlatePos>();
        //起点和终点可能有多个
        var beginPosList = new List<PlatePos>(2);
        var endPosList = new List<PlatePos>(2);
        //遍历所有位置
        for (byte rowIndex = 0; rowIndex < plate.Height; rowIndex++)
        {
            for (byte colIndex = 0; colIndex < plate.Width; colIndex++)
            {
                var grid = plate.Grids[rowIndex][colIndex];
                if (grid.TemplateId == BeginTemplateId)
                {
                    //起点
                    beginPosList.Add(new(rowIndex, colIndex));
                }
                else if (grid.TemplateId == EndTemplateId)
                {
                    //终点
                    endPosList.Add(new(rowIndex, colIndex));
                }
                else if (grid.BonusType >= 0)
                {
                    //金色格子
                    bonusPosList.Add(new(rowIndex, colIndex));
                }
            }
        }
        if (beginPosList.Count == 1)
        {
            //只有一个起点
            beginPos = beginPosList[0];
            endPos = SelectOtherPos(beginPos, endPosList);
        }
        else if (endPosList.Count == 1)
        {
            //只有一个终点
            endPos = endPosList[0];
            beginPos = SelectOtherPos(endPos, beginPosList);
        }

        return (beginPos, endPos, bonusPosList);
    }


    /// <summary>
    /// 已知一个点的位置,从列表中选择另一个点的位置
    /// </summary>
    /// <param name="onePos"></param>
    /// <param name="posList"></param>
    /// <returns></returns>
    private static PlatePos SelectOtherPos(PlatePos onePos, List<PlatePos> posList)
    {
        byte? diffValue = null;
        PlatePos distPos = new(0, 0);
        foreach (var pos in posList)
        {
            var diff = GetPosDiff(onePos, pos);
            if (!diffValue.HasValue)
            {
                diffValue = diff;
                distPos = pos;
            }
            //取距离更近的位置
            else if (diff < diffValue.Value)
            {
                diffValue = diff;
                distPos = pos;
            }
        }
        return distPos;
    }

    /// <summary>
    /// 计算两个位置的距离差值
    /// </summary>
    /// <param name="pos0"></param>
    /// <param name="pos1"></param>
    /// <returns></returns>
    private static byte GetPosDiff(PlatePos pos0, PlatePos pos1)
    {
        var diffValue = 0;
        if (pos0.rowIndex >= pos1.rowIndex)
        {
            diffValue += pos0.rowIndex - pos1.rowIndex;
        }
        else
        {
            diffValue += pos1.rowIndex - pos0.rowIndex;
        }
        if (pos0.colIndex >= pos1.colIndex)
        {
            diffValue += pos0.colIndex - pos1.colIndex;
        }
        else
        {
            diffValue += pos1.colIndex - pos0.colIndex;
        }
        return (byte)diffValue;
    }

    /// <summary>
    /// 计算从起点到终点的坐标列表
    /// </summary>
    /// <param name="beginPos">起点位置</param>
    /// <param name="endPos">终点位置</param>
    /// <param name="plateWidth">突破盘的宽度</param>
    /// <param name="bonusTotalCount">需要的位置总个数</param>
    /// <returns></returns>
    private static List<PlatePos> CalcPosList(PlatePos beginPos, PlatePos endPos, int plateWidth, int bonusTotalCount)
    {
        //终点相对于起点的偏移量
        var (offsetRow, offsetCol) = (endPos.rowIndex - beginPos.rowIndex, endPos.colIndex - beginPos.colIndex);
        //AdaptableLog.TagInfo("beginPos", $"row={beginPos.rowIndex}, col={beginPos.colIndex}");
        //AdaptableLog.TagInfo("endPos", $"row={endPos.rowIndex}, col={endPos.colIndex}");
        //AdaptableLog.TagInfo("offset", $"offsetRow={offsetRow}, offsetCol={offsetCol}");
        //初始化
        var posList = new List<PlatePos>(bonusTotalCount);
        var currentPos = beginPos;
        while (posList.Count < bonusTotalCount)
        {
            //没有偏移了
            if (offsetRow == 0 && offsetCol == 0)
            {
                break;
            }
            var nextRowIndex = currentPos.rowIndex;
            //判断应该下还是上
            var rowAction = 0;
            if (offsetRow > 0)
            {
                //下
                rowAction = 1;
                nextRowIndex++;
                offsetRow--;
            }
            else if (offsetRow < 0)
            {
                //上
                rowAction = -1;
                nextRowIndex--;
                offsetRow++;
            }
            //
            var nextColIndex = currentPos.colIndex;
            //判断应该左还是右
            var colAction = 0;
            if (offsetCol > 0)
            {
                //右
                colAction = 1;
            }
            else if (offsetCol < 0)
            {
                //左
                colAction = -1;
            }
            //行没有改变
            if (rowAction == 0)
            {
                nextColIndex = (byte)(nextColIndex + colAction);
                offsetCol -= colAction;
            }
            else if (currentPos.rowIndex % 2 == 0)
            {
                //此行沿着斜线上下只能增大列的位置或者不变
                if (offsetCol > 0)
                {
                    nextColIndex++;
                    offsetCol--;
                }
            }
            else if (currentPos.rowIndex % 2 == 1)
            {
                //此行沿着斜线上下只能减少列的位置或者不变
                //左上或者左下
                if (offsetCol < 0)
                {
                    nextColIndex--;
                    offsetCol++;
                }
                //最后一个不能右上、右下
                else if (currentPos.colIndex + 1 == plateWidth)
                {
                    //改为左上或者左下
                    nextColIndex--;
                    offsetCol++;
                }
            }
            //位置和终点重合了
            if (nextRowIndex == endPos.rowIndex && nextColIndex == endPos.colIndex)
            {
                break;
            }
            currentPos = new(nextRowIndex, nextColIndex);
            posList.Add(currentPos);
        }
        //还有剩余的金色格子
        if (posList.Count > 0 && posList.Count < bonusTotalCount)
        {
            FillPosList(posList, beginPos, endPos, bonusTotalCount);
        }
        return posList;
    }

    /// <summary>
    /// 填充剩余的金色格子位置到列表中
    /// </summary>
    /// <param name="posList"></param>
    /// <param name="beginPos"></param>
    /// <param name="endPos"></param>
    /// <param name="bonusTotalCount"></param>
    private static void FillPosList(List<PlatePos> posList, PlatePos beginPos, PlatePos endPos, int bonusTotalCount)
    {
        var offset = 0;
        var loopSize = posList.Count;
        //不能放置到起点和终点上
        var disabledPosList = new List<PlatePos>() {
            beginPos,endPos
        };
        while (posList.Count < bonusTotalCount)
        {
            //每轮的偏移量
            offset++;
            for (var i = 0; i < loopSize; i++)
            {
                if (posList.Count == bonusTotalCount)
                {
                    break;
                }
                //当前位置前面的点位置
                var currentPos = posList[i];
                var prevPos = beginPos;
                if (i > 0)
                {
                    prevPos = posList[i - 1];
                }
                //AdaptableLog.TagInfo("prevPos", $"row={prevPos.rowIndex}, col={prevPos.colIndex}");
                //AdaptableLog.TagInfo("currentPos", $"row={currentPos.rowIndex}, col={currentPos.colIndex}");
                //同一行
                if (currentPos.rowIndex == prevPos.rowIndex)
                {
                    //优先放在上方
                    int newRowIndex = currentPos.rowIndex - offset;
                    if (newRowIndex < 0)
                    {
                        //上方没有空位了,放到下方
                        newRowIndex = currentPos.rowIndex - newRowIndex;
                    }
                    //根据行类型计算列的位置
                    byte newColIndex;
                    if (currentPos.rowIndex % 2 == 0)
                    {
                        newColIndex = Math.Max(currentPos.colIndex, prevPos.colIndex);
                    }
                    else
                    {
                        newColIndex = Math.Min(currentPos.colIndex, prevPos.colIndex);
                    }
                    PlatePos newPos = new((byte)newRowIndex, newColIndex);
                    //AdaptableLog.TagInfo("newPos.0", $"row={newPos.rowIndex}, col={newPos.colIndex}");
                    if ((PosIndexOf(newPos, disabledPosList) < 0) && (PosIndexOf(newPos, posList) < 0))
                    {
                        posList.Add(newPos);
                    }
                }
                else
                {
                    //不在同一行
                    //默认放在当前位置的左边
                    int newColIndex = currentPos.colIndex - offset;
                    if (newColIndex < 0)
                    {
                        //左边没有空位了,放到右边
                        newColIndex = currentPos.colIndex - newColIndex;
                    }
                    PlatePos newPos = new(currentPos.rowIndex, (byte)newColIndex);
                    //AdaptableLog.TagInfo("newPos.1", $"row={newPos.rowIndex}, col={newPos.colIndex}");
                    if ((PosIndexOf(newPos, disabledPosList) < 0) && (PosIndexOf(newPos, posList) < 0))
                    {
                        posList.Add(newPos);
                    }
                }
            }

        }
    }

    /// <summary>
    /// 交换两个格子的位置
    /// </summary>
    /// <param name="plate"></param>
    /// <param name="srcPos"></param>
    /// <param name="distPos"></param>
    private static void SwapGrid(TaiwuSkillBreakPlate plate, PlatePos srcPos, PlatePos distPos)
    {
        var srcGrid = plate.Grids[srcPos.rowIndex][srcPos.colIndex];
        var distGrid = plate.Grids[distPos.rowIndex][distPos.colIndex];
        plate.Grids[srcPos.rowIndex][srcPos.colIndex] = distGrid;
        plate.Grids[distPos.rowIndex][distPos.colIndex] = srcGrid;
    }

    /// <summary>
    /// 计算pos在列表中的位置,如果不存在返回-1
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="posList"></param>
    /// <returns></returns>
    private static int PosIndexOf(PlatePos pos, List<PlatePos> posList)
    {
        for (var i = 0; i < posList.Count; i++)
        {
            if (posList[i].rowIndex == pos.rowIndex && posList[i].colIndex == pos.colIndex)
            {
                return i;
            }
        }
        return -1;
    }
}
