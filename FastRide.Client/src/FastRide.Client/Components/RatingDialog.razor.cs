using System;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FastRide.Client.Components;

public partial class RatingDialog : IDisposable
{
    [Inject] private ISignalRService SignalRService { get; set; }

    [Parameter] public string InstanceId { get; set; }

    [Inject] private IDialogService DialogService { get; set; }

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    private int _rating;

    public void Dispose()
    {
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        StateHasChanged();
    }

    private async Task NoRatingAsync()
    {
        await SignalRService.SendRatingAsync(InstanceId, 0);
        MudDialog.Close(DialogResult.Ok(true));
    }


    private async Task RatingChangedAsync(int rating)
    {
        _rating = rating;

        await SignalRService.SendRatingAsync(InstanceId, rating);

        MudDialog.Close(DialogResult.Ok(true));
    }
}