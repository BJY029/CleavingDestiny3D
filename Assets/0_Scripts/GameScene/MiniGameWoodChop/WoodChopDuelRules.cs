using UnityEngine;
using System;


[Serializable]
public struct WoodSegment
{
    public float left;
    public float right;

    public float width => right - left;

    public WoodSegment(float left, float right)
    {
        this.left = Mathf.Clamp01(left);
        this.right = Mathf.Clamp01(right);

        if (this.right < this.left)
        {
            (this.left, this.right) = (this.right, this.left);
        }
    }
}

public enum ChopResolveType
{
    Ignored,
    Success,
    Failed
}

public enum KeptWoodSide
{
    None, Left, Right
}

public struct ChopResolve
{
    public ChopResolveType type;
    public int loserIndex;
    public WoodSegment nextSegment;
    public KeptWoodSide keptSide;

    public static ChopResolve Ignored(WoodSegment segment)
    {
        return new ChopResolve
        {
            type = ChopResolveType.Ignored,
            loserIndex = -1,
            nextSegment = segment,
            keptSide = KeptWoodSide.None
        };
    }

    public static ChopResolve Success(WoodSegment segment, KeptWoodSide keptSide)
    {
        return new ChopResolve
        {
            type = ChopResolveType.Success,
            loserIndex = -1,
            nextSegment = segment,
            keptSide = keptSide
        };
    }

    public static ChopResolve Failed(int loserIndex, WoodSegment segment)
    {
        return new ChopResolve
        {
            type = ChopResolveType.Failed,
            loserIndex = loserIndex,
            nextSegment = segment,
            keptSide = KeptWoodSide.None,
        };
    }
}

public class WoodChopDuelRules
{
    public WoodSegment CurrentSegment { get; private set; }
    public int CurrentPlayerIndex { get; private set; }

    private readonly int playerCount;
    private readonly float edgeMargin;
    private readonly float minChoppableWidth;

    public WoodChopDuelRules(int playerCount = 2, float edgeMargin = 0.02f, float minChoppableWidth = 0.06f)
    {
        this.playerCount = playerCount;
        this.edgeMargin = edgeMargin;
        this.minChoppableWidth = minChoppableWidth;

        Reset(0);
    }

    public void Reset(int startPlayerIndex)
    {
        CurrentSegment = new WoodSegment(0f, 1f);
        CurrentPlayerIndex = startPlayerIndex;
    }

    public void Reset(float initialLeft, float initialRight, int startPlayerIndex)
    {
        CurrentSegment = new WoodSegment(initialLeft, initialRight);
        CurrentPlayerIndex = startPlayerIndex;
    }

    public ChopResolve TryChop(int playerIndex, float cutX01)
    {
        if (playerIndex != CurrentPlayerIndex)
            return ChopResolve.Ignored(CurrentSegment);

        if (CurrentSegment.width <= minChoppableWidth)
            return ChopResolve.Failed(CurrentPlayerIndex, CurrentSegment);

        bool isOutsideLog = cutX01 <= CurrentSegment.left + edgeMargin || cutX01 >= CurrentSegment.right - edgeMargin;

        if (isOutsideLog) return ChopResolve.Failed(CurrentPlayerIndex, CurrentSegment);

        WoodSegment leftPlace = new WoodSegment(CurrentSegment.left, cutX01);
        WoodSegment rightPlace = new WoodSegment(cutX01, CurrentSegment.right);

        KeptWoodSide keptSide;
        if (leftPlace.width >= rightPlace.width)
        {
            CurrentSegment = leftPlace;
            keptSide = KeptWoodSide.Left;
        }
        else
        {
            CurrentSegment = rightPlace;
            keptSide = KeptWoodSide.Right;
        }

        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % playerCount;

        return ChopResolve.Success(CurrentSegment, keptSide);
    }

    public ChopResolve FailCurrentPlayer()
    {
        return ChopResolve.Failed(CurrentPlayerIndex, CurrentSegment);
    }
}
