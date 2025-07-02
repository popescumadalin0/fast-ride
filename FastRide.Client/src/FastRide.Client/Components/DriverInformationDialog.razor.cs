using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Enums;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Sdk.Contracts;
using FastRide.Server.Sdk.Refit;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FastRide.Client.Components;

public partial class DriverInformationDialog : IDisposable
{
    [Parameter] public UserIdentifier UserIdentifier { get; set; }

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    [Inject] private ISnackbar SnackBar { get; set; }

    private User _user = new();

    protected override async Task OnInitializedAsync()
    {
        var user = await FastRideApiClient.GetUserAsync(UserIdentifier);

        if (!user.Success)
        {
            SnackBar.Add("Cannot see the driver!", Severity.Warning);
            MudDialog.Close(DialogResult.Ok(true));
            return;
        }

        _user = user.Response;
        
        var httpClient = new HttpClient();

        var url = _user.PictureUrl;

        using var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, url);
        requestMessage.Headers.Accept.ParseAdd("*/*");
        requestMessage.Headers.AcceptEncoding.ParseAdd("gzip, deflate, br");
        requestMessage.Headers.UserAgent.ParseAdd("PostmanRuntime/7.44.1");

        var response = await httpClient.SendAsync(requestMessage);

        if (response.StatusCode == HttpStatusCode.OK)
        {

            var bytes = await response.Content.ReadAsByteArrayAsync();
            _user.PictureUrl =
                $"data:image/jpg;base64, {Convert.ToBase64String(bytes)}";
        }
        else
        {
            _user.PictureUrl = "default-image.png";
        }
    }

    public void Dispose()
    {
        MudDialog?.Dispose();
        SnackBar?.Dispose();
    }
}