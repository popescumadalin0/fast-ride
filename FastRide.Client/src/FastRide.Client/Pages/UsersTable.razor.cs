using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FastRide.Server.Contracts.Models;
using FastRide.Server.Sdk.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace FastRide.Client.Pages;

public partial class UsersTable
{
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private IFastRideApiClient FastRideApiClient { get; set; }

    private string _searchString = "";
    private User _user = null;
    private User _beforeUser;
    private IEnumerable<User> _users = new List<User>();

    protected override async Task OnInitializedAsync()
    {
        var users = await FastRideApiClient.GetUsersAsync();

        if (!users.Success)
        {
            Snackbar.Add($"Something went wrong: {users.ResponseMessage}", Severity.Error);
            return;
        }

        _users = users.Response;
    }

    private void BackupItem(object element)
    {
        _beforeUser = new User
        {
            Identifier = ((User)element).Identifier,
            UserType = ((User)element).UserType,
            Rating = ((User)element).Rating,
            PictureUrl = ((User)element).PictureUrl,
            PhoneNumber = ((User)element).PhoneNumber,
            UserName = ((User)element).UserName
        };
    }

    private async Task ItemHasBeenCommittedAsync(MouseEventArgs args)
    {
        var response = await FastRideApiClient.UpdateUserAsync(new UpdateUserPayload()
        {
            PhoneNumber = _user.PhoneNumber,
            UserType = _user.UserType,
            User = _user.Identifier
        });
        if (response.Success)
        {
            Snackbar.Add("User saved!");
        }
        else
        {
            Snackbar.Add($"Something went wrong: {response.ResponseMessage}", Severity.Error);
            ResetItemToOriginalValues(_user);
        }
    }

    private void ResetItemToOriginalValues(object element)
    {
        ((User)element).Identifier = _beforeUser.Identifier;
        ((User)element).UserType = _beforeUser.UserType;
        ((User)element).Rating = _beforeUser.Rating;
        ((User)element).PictureUrl = _beforeUser.PictureUrl;
        ((User)element).PhoneNumber = _beforeUser.PhoneNumber;
        ((User)element).UserName = _beforeUser.UserName;
    }

    private bool FilterFunc(User element)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;
        if (element.Identifier.NameIdentifier.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.Identifier.Email.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.PhoneNumber.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (element.UserName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }
}