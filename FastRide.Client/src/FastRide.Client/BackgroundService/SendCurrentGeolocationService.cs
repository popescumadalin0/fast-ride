using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;
using FastRide.Client.Contracts;
using FastRide.Server.Contracts.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Timer = System.Timers.Timer;

namespace FastRide.Client.BackgroundService;

public class JobExecutedEventArgs : EventArgs;

public class SendCurrentGeolocationService : IDisposable
{
    private readonly Task<AuthenticationState> _authenticatonState;
    private readonly IGeolocationService _geolocationService;
    private readonly ISender _sender;
    private bool _running;

    private Timer _timer;

    public SendCurrentGeolocationService(ISender sender, Task<AuthenticationState> authenticatonState,
        IGeolocationService geolocationService)
    {
        this._authenticatonState = authenticatonState;
        _geolocationService = geolocationService;
        _sender = sender;
    }

    public void Dispose()
    {
        if (_running)
        {
            _timer.Dispose();
        }
    }

    public event EventHandler<JobExecutedEventArgs> JobExecuted;

    private void OnJobExecuted()
    {
        JobExecuted?.Invoke(this, new JobExecutedEventArgs());
    }

    public void StartExecuting()
    {
        if (!_running)
        {
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Elapsed += HandleTimer;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            _running = true;
        }
    }

    private void HandleTimer(object source, ElapsedEventArgs e)
    {
        var auth = _authenticatonState.Result;
        var userId = auth.User.Claims.Single(x => x.Type == "sub").Value;
        var groupName = auth.User.Claims.Single(x => x.Type == ClaimTypes.GroupSid).Value;
        var geolocation = _geolocationService.GetLocationAsync().GetAwaiter().GetResult();
        _sender.NotifyUserGeolocationAsync(userId,
                groupName,
                new Geolocation()
                {
                    Latitude = geolocation.Latitude,
                    Longitude = geolocation.Longitude,
                })
            .GetAwaiter()
            .GetResult();

        OnJobExecuted();
    }
}