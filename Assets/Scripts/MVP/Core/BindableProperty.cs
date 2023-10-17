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
        public event Action<T> ChangeValueEvent;

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
                ChangeValueEvent?.Invoke(value);
            }
        }

        [SerializeField] private T _value;

        public void Dispose()
        {
            _value = default;
            ChangeValueEvent = null;
        }
    }

    [Serializable]
    public struct BindableReferenceProperty<T> : IBindableProperty
        where T : class
    {
        public event Action<T> ChangeValueEvent;

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
                ChangeValueEvent?.Invoke(value);
            }
        }

        [SerializeField] private T _value;

        public void Dispose()
        {
            _value = default;
            ChangeValueEvent = null;
        }
    }
}