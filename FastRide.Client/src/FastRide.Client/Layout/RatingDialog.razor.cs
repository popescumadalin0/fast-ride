using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.SignalRModels;
using Microsoft.AspNetCore.Components;

namespace FastRide.Client.Layout;

public partial class RatingDialog : IDisposable
{
    [Inject] private ISignalRService SignalRService { get; set; }

    private int _rating;
    private bool _open;

    private string _instanceId = string.Empty;

    public void Dispose()
    {
        SignalRService.SendRating -= OpenRatingDialog;
    }

    protected override async Task OnInitializedAsync()
    {
        SignalRService.SendRating += OpenRatingDialog;

        await base.OnInitializedAsync();
    }

    private async Task NoRatingAsync()
    {
        _open = false;
        await SignalRService.SendRatingAsync(_instanceId, 0);
    }

    private async Task OpenRatingDialog(RatingRequest request)
    {
        _open = true;
        _instanceId = request.InstanceId;
        StateHasChanged();
    }

    private async Task RatingChangedAsync(int rating)
    {
        _rating = rating;

        await SignalRService.SendRatingAsync(_instanceId, rating);

        await Task.Delay(2000);

        _open = false;
        StateHasChanged();
    }
}