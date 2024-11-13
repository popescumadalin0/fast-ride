using System;
using System.Threading;
using FastRide.Client.Contracts;

namespace FastRide.Client.BackendServices;

public sealed class BlazorServiceAccessor : IBlazorServiceAccessor
{
    private static readonly AsyncLocal<BlazorServiceHolder> CurrentServiceHolder = new();

    /// <summary>
    /// Gets or sets the current IServiceProvider.
    /// </summary>
    public IServiceProvider Services
    {
        get => CurrentServiceHolder.Value!.Services!;
        set
        {
            if (CurrentServiceHolder.Value is { } holder)
            {
                // Clear the current IServiceProvider trapped in the AsyncLocal.
                holder.Services = null;
            }
            
            // Use object indirection to hold the IServiceProvider in an AsyncLocal
            // so it can be cleared in all ExecutionContexts when it's cleared.
            CurrentServiceHolder.Value = new() { Services = value };
        }
    }

    private sealed class BlazorServiceHolder
    {
        public IServiceProvider? Services { get; set; }
    }
}