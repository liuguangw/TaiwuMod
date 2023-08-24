using TaiwuSkillBreakPlate = GameData.Domains.Taiwu.SkillBreakPlate;

namespace Liuguang.mod.Taiwu.BetterLearn;

internal static class BreakPlateResetTool
{
    //起点类型
    const sbyte BeginTemplateId = 0;
    //终点类型
    const sbyte EndTemplateId = 1;
    public static void ProcessResetBreakPlate(TaiwuSkillBreakPlate plate)
    {
        var beginPosList = new List<(byte, byte)>(2);
        var endPosList = new List<(byte, byte)>(2);
        var bonusPosList = new List<(byte, byte)>();
        for (byte rowIndex = 0; rowIndex < plate.Height; rowIndex++)
        {
            for (byte colIndex = 0; colIndex < plate.Width; colIndex++)
            {
                var grid = plate.Grids[rowIndex][colIndex];
                if (grid.TemplateId == BeginTemplateId)
                {
                    //起点
                    beginPosList.Add((rowIndex, colIndex));
                }
                else if (grid.TemplateId == EndTemplateId)
                {
                    //终点
                    endPosList.Add((rowIndex, colIndex));
                }
                else if (grid.BonusType >= 0)
                {
                    //金色格子
                    bonusPosList.Add((rowIndex, colIndex));
                }
            }
        }
        (byte, byte) beginPos = (0, 0);
        (byte, byte) endPos = (0, 0);
        if (beginPosList.Count == 1)
        {
            beginPos = beginPosList[0];
            endPos = SelectOtherPos(beginPos, endPosList);
        }
        else if (endPosList.Count == 1)
        {
            endPos = endPosList[0];
            beginPos = SelectOtherPos(endPos, beginPosList);
        }
        //从起点向终点移动的方向
        var toRight = beginPos.Item2 < endPos.Item2;
        var toBottom = beginPos.Item1 < endPos.Item1;
        //新的金色格子位置列表
        var bonusTotalCount = bonusPosList.Count;
        var currentBonusCount = 0;
        var newBonusPosList = new List<(byte, byte)>(bonusTotalCount);
        //当前位置
        var currentPos = beginPos;
        while (currentBonusCount < bonusTotalCount)
        {
            if (currentPos == endPos)
            {
                break;
            }
            if (!HasNextPos(currentPos, toRight, toBottom, plate.Height, plate.Width))
            {
                break;
            }
            currentPos = GetNextPos(currentPos, toRight, toBottom);
            var currentGrid = plate.Grids[currentPos.Item1][currentPos.Item2];
            if (currentGrid.TemplateId == BeginTemplateId || currentGrid.TemplateId == EndTemplateId)
            {
                break;
            }
            newBonusPosList.Add(currentPos);
            currentBonusCount++;
            //如果此格子已经是金色格子
            if (currentGrid.BonusType >= 0)
            {
                //bonusPosList中移除这个位置
                for (var i = 0; i < bonusPosList.Count; i++)
                {
                    if (bonusPosList[i] == currentPos)
                    {
                        bonusPosList.RemoveAt(i);
                        break;
                    }
                }
            }
            else
            {
                var lastIndex = bonusPosList.Count - 1;
                var srcPos = bonusPosList[lastIndex];
                bonusPosList.RemoveAt(lastIndex);
                SwapGrid(plate, srcPos, currentPos);
            }
        }
        //还有剩余的金色格子
        byte offset = 0;
        var baseBonusCount = currentBonusCount;
        while (currentBonusCount < bonusTotalCount)
        {
            offset++;
            for (var i1 = 0; i1 < baseBonusCount; i1++)
            {
                currentPos = newBonusPosList[i1];
                if (toRight)
                {
                    currentPos.Item2 += offset;
                }
                else
                {
                    currentPos.Item2 -= offset;
                }
                var currentGrid = plate.Grids[currentPos.Item1][currentPos.Item2];
                if (currentGrid.TemplateId == BeginTemplateId || currentGrid.TemplateId == EndTemplateId)
                {
                    continue;
                }
                newBonusPosList.Add(currentPos);
                currentBonusCount++;
                //如果此格子已经是金色格子
                if (currentGrid.BonusType >= 0)
                {
                    //bonusPosList中移除这个位置
                    for (var i = 0; i < bonusPosList.Count; i++)
                    {
                        if (bonusPosList[i] == currentPos)
                        {
                            bonusPosList.RemoveAt(i);
                            break;
                        }
                    }
                }
                else
                {
                    var lastIndex = bonusPosList.Count - 1;
                    var srcPos = bonusPosList[lastIndex];
                    bonusPosList.RemoveAt(lastIndex);
                    SwapGrid(plate, srcPos, currentPos);
                }
                //
                if (currentBonusCount>= bonusTotalCount)
                {
                    break;
                }
            }
        }
    }


    /// <summary>
    /// 已知一个点的位置,从列表中选择另一个点的位置
    /// </summary>
    /// <param name="onePos"></param>
    /// <param name="posList"></param>
    /// <returns></returns>
    private static (byte, byte) SelectOtherPos((byte, byte) onePos, List<(byte, byte)> posList)
    {
        (byte, byte) tmpPos = (0, 0);
        byte diffValue = 0;
        byte GetDiff(byte v1, byte v2)
        {
            if (v1 >= v2)
            {
                return (byte)(v1 - v2);
            }
            return (byte)(v2 - v1);
        }
        foreach (var pos in posList)
        {
            var diff = (byte)(GetDiff(onePos.Item1, pos.Item1) + GetDiff(onePos.Item2, pos.Item2));
            if (diff > diffValue)
            {
                diffValue = diff;
                tmpPos = pos;
            }
        }
        return tmpPos;
    }


    private static bool HasNextPos((byte, byte) currentPos, bool toRight, bool toBottom, byte rowCount, byte colCount)
    {
        var (rowIndex, colIndex) = currentPos;
        if (toBottom)
        {
            byte nextRowIndex = (byte)(rowIndex + 1);
            if (nextRowIndex >= rowCount)
            {
                return false;
            }

        }
        else if (rowIndex == 0)
        {
            return false;
        }
        //
        if (toRight)
        {
            if (rowIndex % 2 == 0)
            {
                var nextColIndex = (byte)(colIndex + 1);
                if (nextColIndex >= colCount)
                {
                    return false;
                }
            }
        }
        else
        {
            if ((rowIndex % 2 != 0) && (colIndex == 0))
            {
                return false;
            }
        }
        return true;
    }

    private static (byte, byte) GetNextPos((byte, byte) currentPos, bool toRight, bool toBottom)
    {
        var (rowIndex, colIndex) = currentPos;
        byte nextRowIndex;
        byte nextColIndex = colIndex;
        if (toBottom)
        {
            nextRowIndex = (byte)(rowIndex + 1);
        }
        else
        {
            nextRowIndex = (byte)(rowIndex - 1);
        }
        if (rowIndex % 2 == 0)
        {
            if (toRight)
            {
                nextColIndex = (byte)(colIndex + 1);
            }
        }
        else
        {
            if (!toRight)
            {
                nextColIndex = (byte)(colIndex - 1);
            }
        }
        return (nextRowIndex, nextColIndex);
    }

    /// <summary>
    /// 交换两个格子的位置
    /// </summary>
    /// <param name="plate"></param>
    /// <param name="srcPos"></param>
    /// <param name="distPos"></param>
    private static void SwapGrid(TaiwuSkillBreakPlate plate, (byte, byte) srcPos, (byte, byte) distPos)
    {
        var srcGrid = plate.Grids[srcPos.Item1][srcPos.Item2];
        var distGrid = plate.Grids[distPos.Item1][distPos.Item2];
        plate.Grids[srcPos.Item1][srcPos.Item2] = distGrid;
        plate.Grids[distPos.Item1][distPos.Item2] = srcGrid;
    }
}
