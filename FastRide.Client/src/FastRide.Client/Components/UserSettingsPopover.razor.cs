using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FastRide.Client.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MudBlazor;

namespace FastRide.Client.Components;

public partial class UserSettingsPopover
{
    private bool _openProfileSettings;

    [Parameter] public AuthenticationState Context { get; set; }

    [Inject] private IDialogService DialogService { get; set; }


    private string _profileImage;

    protected override async Task OnInitializedAsync()
    {
        var httpClient = new HttpClient();

        var url = Context.User.Claims.Single(c => c.Type == "picture").Value;

        using var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, url);
        requestMessage.Headers.Accept.ParseAdd("*/*");
        requestMessage.Headers.AcceptEncoding.ParseAdd("gzip, deflate, br");
        requestMessage.Headers.UserAgent.ParseAdd("PostmanRuntime/7.44.1");

        var response = await httpClient.SendAsync(requestMessage);

        if (response.StatusCode == HttpStatusCode.OK)
        {

            var bytes = await response.Content.ReadAsByteArrayAsync();
            _profileImage =
                $"data:image/jpg;base64, {Convert.ToBase64String(bytes)}";
        }
        else
        {
            _profileImage = "default-image.png";
        }
    }


    private async Task OpenChangeUserSettingsAsync()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        await DialogService.ShowAsync<ChangeUserSettingsDialog>("Change user settings", options);

        _openProfileSettings = false;
    }

    private void OpenAccountMenu()
    {
        _openProfileSettings = !_openProfileSettings;
    }
}