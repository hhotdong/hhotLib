// Credit: https://saens.tistory.com/13
using System;
using UnityEngine;

namespace hhotLib.Common.MVP
{
    public interface IBindableProperty
    {

    }

    [Serializable]
    public struct BindableValueProperty<T> : IBindableProperty
        where T : struct
    {
        public event Action<T> ValueChangedEvent;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value.Equals(value))
                    return;
                _value = value;
                ValueChangedEvent?.Invoke(value);
            }
        }

        [SerializeField] private T _value;

        public void Reset()
        {
            _value = default;
            ValueChangedEvent = null;
        }
    }

    [Serializable]
    public struct BindableReferenceProperty<T> : IBindableProperty
        where T : class
    {
        public event Action<T> ValueChangedEvent;

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != null && _value.Equals(value))
                    return;
                _value = value;
                ValueChangedEvent?.Invoke(value);
            }
        }

        [SerializeField] private T _value;

        public void Reset()
        {
            _value = default;
            ValueChangedEvent = null;
        }
    }
}