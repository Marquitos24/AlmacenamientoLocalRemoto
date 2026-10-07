using ServiceRemote.Errors;

namespace ServiceRemote.Notifications.Emiter;

/// <summary>
/// Interfaz que define un servicio de notificaciones para recursos de tipo T y errores de tipo TError.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TError"></typeparam>
public interface INotificationService<T, TError>
{
    /// <summary>
    /// Notifica de la creacion de un recurso.
    /// </summary>
    /// <param name="obj">El objeto que representa el recurso creado.</param>
    Task NotifyCreated(T obj);
    
    /// <summary>
    /// Notifica de la actualizacion de un recurso.
    /// </summary>
    /// <param name="obj">El objeto que representa el recurso modificado.</param>
    Task NotifyUpdated(T obj);

    /// <summary>
    /// Notifica de la eliminacion de un recurso.
    /// </summary>
    /// <param name="obj">El objeto que representa el recurso eliminado.</param>
    Task NotifyDeleted(T obj);
    
    /// <summary>
    /// Notifica un error en el repositorio.
    /// </summary>
    /// /// <param name="error">El error detectado.</param>
    Task NotifyError(TError error);

    /// <summary>
    /// Notifica el fin de la comunicacion del servicio de notificaciones.
    /// </summary>
    Task NotifyEnd();
}