using System;
using MediatR;

namespace Minicommerce.Domain.Common;

    public interface IDomainEventHandler<T> : INotificationHandler<T>
        where T : DomainEvent
    {
    }
 