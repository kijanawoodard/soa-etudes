using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PublicWebApp.Infrastructure
{
    public interface ISubscribeHandlers
    {
        void Subscribe<TMessage>(Func<TMessage, Task> handler);
        void Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> handler);
    }

    public interface IMediator
    {
        Task Send<TMessage>(TMessage message);
        Task<TResult> Send<TMessage, TResult>(TMessage message);
    }

    public class Mediator : ISubscribeHandlers, IMediator
    {
        private readonly IDictionary<Type, Delegate> _subscriptions;

        public void Subscribe<TMessage>(Func<TMessage, Task> handler)
        {
            Subscribe<TMessage, Task>(async message =>
            {
                await handler(message);
                return new TaskCompletionSource<Unit>().Task;
            });
        }

        public void Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> handler)
        {
            _subscriptions.Add(typeof(TMessage), handler);
        }

        public async Task Send<TMessage>(TMessage message)
        {
            await Send<TMessage, Unit>(message);
        }

        public async Task<TResult> Send<TMessage, TResult>(TMessage message)
        {
            Delegate value;
            if (!_subscriptions.TryGetValue(typeof(TMessage), out value))
                throw new ApplicationException(string.Format("No Handler subscribed for message {0}.", typeof(TMessage).Name));

            var handler = value as Func<TMessage, Task<TResult>>;
            if (handler == null) throw new ApplicationException(string.Format("The handler subscribed for {0} does not have result type of {1}.", typeof(TMessage).Name, typeof(TResult).Name));

            return await handler(message);
        }

        public Mediator()
        {
            _subscriptions = new SortedDictionary<Type, Delegate>();
        }

        class Unit { }
    }
}