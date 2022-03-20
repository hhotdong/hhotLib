// https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events
// https://medium.com/codex/rts-interlude-1-introducing-an-event-system-unity-c-14c121fb8ed

using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using hhotLib.Common;

namespace hhotLib
{
    public class EventManager : Singleton<EventManager>
    {
        private readonly Dictionary<string, UnityEvent> events = new Dictionary<string, UnityEvent>();
        private readonly Dictionary<string, UnityEvent<CustomEventArgs>> paramEvents = new Dictionary<string, UnityEvent<CustomEventArgs>>();
        private readonly Dictionary<string, Queue<UnityAction>> eventQueues = new Dictionary<string, Queue<UnityAction>>();

        protected override void OnAwake()
        {
            events.Clear();
            paramEvents.Clear();
            eventQueues.Clear();
        }

        protected override void OnDestroySingleton()
        {
            foreach (var item in events)
            {
                item.Value.RemoveAllListeners();
            }

            foreach (var item in paramEvents)
            {
                item.Value.RemoveAllListeners();
            }

            foreach (var item in eventQueues)
            {
                item.Value.Clear();
            }

            events.Clear();
            paramEvents.Clear();
            eventQueues.Clear();
        }

        public void Register(string eventName, UnityAction listener)
        {
            if (events.TryGetValue(eventName, out UnityEvent evt))
            {
                evt.AddListener(listener);
            }
            else
            {
                evt = new UnityEvent();
                evt.AddListener(listener);
                events.Add(eventName, evt);
            }
        }

        public void Unregister(string eventName, UnityAction listener)
        {
            if (events.TryGetValue(eventName, out UnityEvent evt))
            {
                evt.RemoveListener(listener);
            }
        }

        public void TriggerEvent(string eventName)
        {
            if (events.TryGetValue(eventName, out UnityEvent evt))
            {
                evt.Invoke();
            }
        }

        public void RegisterParamEvent(string eventName, UnityAction<CustomEventArgs> listener)
        {
            if (paramEvents.TryGetValue(eventName, out UnityEvent<CustomEventArgs> evt))
            {
                evt.AddListener(listener);
            }
            else
            {
                evt = new UnityEvent<CustomEventArgs>();
                evt.AddListener(listener);
                paramEvents.Add(eventName, evt);
            }
        }

        public void UnregisterParamEvent(string eventName, UnityAction<CustomEventArgs> listener)
        {
            if (paramEvents.TryGetValue(eventName, out UnityEvent<CustomEventArgs> evt))
            {
                evt.RemoveListener(listener);
            }
        }

        public void TriggerParamEvent(string eventName, CustomEventArgs param)
        {
            if (paramEvents.TryGetValue(eventName, out UnityEvent<CustomEventArgs> evt))
            {
                evt.Invoke(param);
            }
        }

        public void RegisterQueuedEvent(string eventName, UnityAction callback)
        {
            if (eventQueues.TryGetValue(eventName, out Queue<UnityAction> queue))
            {
                queue.Enqueue(callback);
            }
            else
            {
                queue = new Queue<UnityAction>();
                queue.Enqueue(callback);
                eventQueues.Add(eventName, queue);
            }
        }

        public void ClearQueuedEvent(string eventName)
        {
            if (eventQueues.TryGetValue(eventName, out Queue<UnityAction> queue))
            {
                queue.Clear();
            }
        }

        public void TriggerQueuedEvent(string eventName, bool triggerAll)
        {
            if (eventQueues.TryGetValue(eventName, out Queue<UnityAction> queue))
            {
                if (triggerAll)
                {
                    StartCoroutine(TriggerAllEvents(queue));
                }
                else
                {
                    queue.Dequeue()?.Invoke();
                }
            }
            else
            {
                Debug.LogError($"No event found for {eventName}!");
            }

            IEnumerator TriggerAllEvents(Queue<UnityAction> q)
            {
                for (int i = 0; i < q.Count; i++)
                {
                    q.Dequeue()?.Invoke();
                    yield return null;
                }
            }
        }
    }
}