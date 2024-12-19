using System.Threading.Tasks;

namespace FastRide.Client.Contracts;

public interface ISignalRFactory
{
    Task<ISender> GetSenderAsync();

    Task<IObserver> GetObserverAsync();
}