﻿using BuildingBlocks.Exceptions;

namespace BuildingBlocks.Utilities;

/// <summary>
/// Allows action to be executed when it is disposed
/// </summary>
public struct ActionDisposable : IDisposable
{
    readonly Action _action;

    public static readonly ActionDisposable Empty = new ActionDisposable(() => { });

    public ActionDisposable(Action action)
    {
        Guard.AgainstNull(action, nameof(action));

        _action = action;
    }

    public void Dispose()
    {
        _action();
    }
}