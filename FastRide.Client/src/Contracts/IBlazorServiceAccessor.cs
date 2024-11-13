using System;

namespace FastRide.Client.Contracts;

public interface IBlazorServiceAccessor
{
    /// Gets or sets the current IServiceProvider.
    IServiceProvider Services { get; set; }
}