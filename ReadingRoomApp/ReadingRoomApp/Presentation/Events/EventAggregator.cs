using System;
using System.Collections.Generic;

namespace ReadingRoomApp.Presentation.Events
{
    public interface IEventAggregator
    {
        void Subscribe<TEvent>(Action<TEvent> action);
        void Unsubscribe<TEvent>(Action<TEvent> action);
        void Publish<TEvent>(TEvent eventToPublish);
    }

    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<object>> _subscribers = new Dictionary<Type, List<object>>();

        public void Subscribe<TEvent>(Action<TEvent> action)
        {
            var eventType = typeof(TEvent);
            if (!_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType] = new List<object>();
            }

            _subscribers[eventType].Add(action);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> action)
        {
            var eventType = typeof(TEvent);
            if (_subscribers.ContainsKey(eventType))
            {
                _subscribers[eventType].Remove(action);
            }
        }

        public void Publish<TEvent>(TEvent eventToPublish)
        {
            var eventType = typeof(TEvent);
            if (!_subscribers.ContainsKey(eventType))
                return;

            foreach (var subscriber in _subscribers[eventType].ToArray())
            {
                var action = (Action<TEvent>)subscriber;
                action(eventToPublish);
            }
        }
    }
}