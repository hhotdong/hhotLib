using System;
using System.Reflection;
using UnityEngine;

namespace hhotLib.Common.MVP
{
    public abstract class ScriptableObjectModel : ScriptableObject
    {
        public abstract void Reset();

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying == false)
                return;

            try
            {
                NotifyBindablePropertyChanged();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// 에디터에서 플레이 도중에 모델의 BindableProperty를 변경하는 경우 변경사항을 알리는 이벤트를 호출한다.
        /// </summary>
        private void NotifyBindablePropertyChanged()
        {
            FieldInfo[] fieldInfos = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fi in fieldInfos)
            {
                object bindableProperty = fi.GetValue(this);
                if (bindableProperty == null || (bindableProperty is not IBindableProperty))
                    continue;

                Type bindablePropertyType = bindableProperty.GetType();
                var eventDelegate = (MulticastDelegate)bindablePropertyType.GetField("ChangeValueEvent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bindableProperty);
                if (eventDelegate != null)
                {
                    object val = bindablePropertyType.GetField("_value", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(bindableProperty);
                    foreach (var handler in eventDelegate.GetInvocationList())
                        handler.Method.Invoke(handler.Target, new object[] { val });
                }
            }
        }
    }
#endif
}