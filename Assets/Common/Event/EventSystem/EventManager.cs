// Credit: https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events
// Credit: https://medium.com/codex/rts-interlude-1-introducing-an-event-system-unity-c-14c121fb8ed

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace hhotLib.Common
{
    public class EventManager : Singleton<EventManager>
    {
        public delegate void EventDelegate<T>(T e) where T : CustomEventArgs;
        private delegate void EventDelegateInternal(CustomEventArgs e);

        private readonly Dictionary<string, UnityEvent>              paramlessEvents           = new Dictionary<string, UnityEvent>();
        private readonly Dictionary<Type, EventDelegateInternal>     paramEventsDelegates      = new Dictionary<Type, EventDelegateInternal>();
        private readonly Dictionary<Delegate, EventDelegateInternal> paramEventsDelegateLookup = new Dictionary<Delegate, EventDelegateInternal>();
        private readonly Dictionary<string, Queue<UnityAction>>      eventQueues               = new Dictionary<string, Queue<UnityAction>>();

        public void Register(string eventName, UnityAction listener)
        {
            if (paramlessEvents.TryGetValue(eventName, out UnityEvent evt))
                evt.AddListener(listener);
            else
            {
                evt = new UnityEvent();
                evt.AddListener(listener);
                paramlessEvents.Add(eventName, evt);
            }
        }

        public void Unregister(string eventName, UnityAction listener)
        {
            if (paramlessEvents.TryGetValue(eventName, out UnityEvent evt))
                evt.RemoveListener(listener);
        }

        public void TriggerEvent(string eventName)
        {
            if (paramlessEvents.TryGetValue(eventName, out UnityEvent evt))
                evt.Invoke();
            else
                Debug.LogWarning($"Event({eventName}) not found!");
        }

        public void RegisterParamEvent<T>(EventDelegate<T> del) where T : CustomEventArgs
        {
            if (paramEventsDelegateLookup.ContainsKey(del))
                return;

            EventDelegateInternal internalDelegate = (e) => del((T)e);
            paramEventsDelegateLookup[del] = internalDelegate;

            if (paramEventsDelegates.TryGetValue(typeof(T), out EventDelegateInternal tempDel))
            {
                tempDel += internalDelegate;
                paramEventsDelegates[typeof(T)] = tempDel;
            }
            else
                paramEventsDelegates[typeof(T)] = internalDelegate;
        }

        public void UnregisterParamEvent<T>(EventDelegate<T> del) where T : CustomEventArgs
        {
            if (paramEventsDelegateLookup.TryGetValue(del, out EventDelegateInternal internalDelegate) == false)
                return;

            if (paramEventsDelegates.TryGetValue(typeof(T), out EventDelegateInternal tempDel) == false)
                return;
            
            paramEventsDelegateLookup.Remove(del);
            tempDel -= internalDelegate;

            if (tempDel == null)
                paramEventsDelegates.Remove(typeof(T));
            else
                paramEventsDelegates[typeof(T)] = tempDel;
        }

        public void TriggerParamEvent(CustomEventArgs args)
        {
            if (paramEventsDelegates.TryGetValue(args.GetType(), out EventDelegateInternal del))
                del.Invoke(args);
            else
                Debug.LogWarning($"{args.GetType()} has no listeners");
        }

        public bool HasParamEventListener<T>(EventDelegate<T> del) where T : CustomEventArgs
        {
            return paramEventsDelegateLookup.ContainsKey(del);
        }

        public void RegisterQueuedEvent(string eventName, UnityAction callback)
        {
            if (eventQueues.TryGetValue(eventName, out Queue<UnityAction> queue))
                queue.Enqueue(callback);
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
                queue.Clear();
        }

        public void TriggerQueuedEvent(string eventName, bool triggerAll, bool triggerOneByOne)
        {
            if (eventQueues.TryGetValue(eventName, out Queue<UnityAction> queue) == false)
            {
                Debug.LogWarning($"Event({eventName}) not found!");
                return;
            }

            if (triggerAll)
            {
                if (triggerOneByOne)
                    StartCoroutine(TriggerAllQueuedEventsOneByOne(queue));
                else
                {
                    while (queue.Count > 0)
                        queue.Dequeue().Invoke();
                }
                return;
            }

            queue.Dequeue().Invoke();
        }

        public bool HasQueuedEvent(string eventName)
        {
            return eventQueues.TryGetValue(eventName, out Queue<UnityAction> queue) && queue.Count > 0;
        }

        public void Dispose()
        {
            foreach (var item in paramlessEvents)
                item.Value.RemoveAllListeners();

            foreach (var item in eventQueues)
                item.Value.Clear();

            paramlessEvents          .Clear();
            paramEventsDelegates     .Clear();
            paramEventsDelegateLookup.Clear();
            eventQueues              .Clear();
        }

        private IEnumerator TriggerAllQueuedEventsOneByOne(Queue<UnityAction> queue)
        {
            while (queue.Count > 0)
            {
                queue.Dequeue().Invoke();
                yield return null;
            }
        }

        protected override void OnAwake()
        {
            paramlessEvents          .Clear();
            paramEventsDelegates     .Clear();
            paramEventsDelegateLookup.Clear();
            eventQueues              .Clear();
        }

        protected override void OnDestroySingleton()
        {
            Dispose();
        }
    }
}