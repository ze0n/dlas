using System;

namespace Debug.Like.A.Scientist
{
    public interface IDebugSession
    {
        string Id { get; }
        DateTime CreatedAt { get; }
        DateTime? FinishedAt { get; }
        DebugSessionStatus Status { get; }
        DebugSettings Settings { get; }
    }
}
