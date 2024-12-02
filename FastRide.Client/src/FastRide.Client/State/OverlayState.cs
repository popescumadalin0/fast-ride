using System;
using FastRide.Client.Models;

namespace FastRide.Client.State;

public class OverlayState
{
    private bool _dataLoading = false;
    
    public event Action OnChange;
    
    public bool DataLoading
    {
        get => _dataLoading;
        set
        {
            _dataLoading = value;
            OnChange?.Invoke();
        }
    }
}