using System;
using System.Threading.Tasks;
using System.Timers;
using FastRide.Client.Contracts;
using Microsoft.AspNetCore.Components.Authorization;

namespace FastRide.Client.BackgroundService;

public class JobExecutedEventArgs : EventArgs;

public class SendCurrentGeolocationService : IDisposable
{
    private readonly ISender _sender;

    private readonly Task<AuthenticationState> authenticatonState;
    private bool _running;

    private Timer _timer;

    public SendCurrentGeolocationService(ISignalRFactory factory, Task<AuthenticationState> authenticatonState)
    {
        this.authenticatonState = authenticatonState;
        _sender = factory.GetSenderAsync().GetAwaiter().GetResult();
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
        var auth = authenticatonState.Result;
        _sender.NotifyUserGeolocationAsync(auth.User.Claims)
        OnJobExecuted();
    }
}