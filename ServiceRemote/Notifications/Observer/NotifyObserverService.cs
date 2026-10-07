using System.Reactive.Subjects;

namespace ServiceRemote.Notifications.Observer;

/// <summary>
/// Implementacion del servio de Observador de notificaciones
/// </summary>
public class NotifyObserverService : INotifyObserverService
{
    private Subject<string> _subjet;
    private IDisposable _suscripcion;

    public NotifyObserverService(Subject<string> subjet)
    {
        _subjet = subjet;
        _suscripcion = _subjet.Subscribe(mens => Console.WriteLine(mens));
    }
    
    /// <inheritdoc/>
    public void Dispose()
    {
        _suscripcion.Dispose();
    }
}