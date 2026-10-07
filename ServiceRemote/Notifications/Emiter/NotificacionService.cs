using System.Reactive.Subjects;
using ServiceRemote.Dto;
using ServiceRemote.Errors;

namespace ServiceRemote.Notifications.Emiter;
/// <summary>
/// Servicio emisor de notificaciones que envia mensajes en funcion de las acciones de modificacion en el repositorio.
/// Utiliza un subjet como puente de comunicacion entre el servicio y los observadores
/// </summary>
/// <param name="subjet"> Puente de comunicacion IObservable</param>
public class NotificacionService : INotificationService<UserResponseDto,UserDomainError>
{
    private readonly Subject<string> _subjet;

    public NotificacionService(Subject<string> subjet)
    {
        _subjet = subjet;
    }
    
    /// <inheritdoc/>
    public Task NotifyCreated(UserResponseDto obj)
    {
        _subjet.OnNext($"Usuario creado: {obj.ToJson()}");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task NotifyUpdated(UserResponseDto obj)
    {
        _subjet.OnNext($"Usuario actualizado: {obj.ToJson()}");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task NotifyDeleted(UserResponseDto obj)
    {
        _subjet.OnNext($"Usuario eliminado: {obj.ToJson()}");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task NotifyError(UserDomainError error)
    {
        _subjet.OnNext($"Error en el servicio: {error.Message}");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task NotifyEnd()
    {
        _subjet.OnNext($"Fin de la comunicacion, Cerrando servicio de notificaciones");
        _subjet.OnCompleted();
        return Task.CompletedTask;
    }
}