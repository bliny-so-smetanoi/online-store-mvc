using MediatR;

namespace Application.Abstractions; 

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent { }