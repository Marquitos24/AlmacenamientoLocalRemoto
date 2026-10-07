namespace ServiceRemote.Notifications.Observer;

/// <summary>
/// Contrato de servicio para la gestión de observadores de notificaciones.
/// </summary>
public interface INotifyObserverService
{
    /// <summary>
    /// Funcion de desuscripción del servicio de observadores de notificaciones.
    /// </summary>
    public void Dispose();
}