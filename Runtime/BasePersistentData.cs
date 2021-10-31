using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Phoder1.PersistentData
{
    public abstract class BasePersistentData<T> : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<T> OnValueChanged;
        protected abstract IDataKey Key { get; }

        private void OnEnable()
        {
            OnValueChangedSubscribe(ValueChanged);
#if UNITY_EDITOR
            if (Key.TryGetValue<T>(out var _val))
                Debug.Log($"Value exists, Value = {_val}");
            else
                Debug.Log($"Value doesn't exists! Value defaulted to {_val}");
#endif
        }
        private void OnDisable()
        {
            OnValueChangedUnsubscribe(ValueChanged);
        }
        [ShowInInspector, HideInEditorMode]
        public T Value
        {
            get => GetValue();
            set => SetValue(value);
        }
        [ShowInInspector, HideInEditorMode]
        public bool Persistent
        {
            get => GetPersistent();
            set => SetPersistent(value);
        }
        #region IDataKey extensions
        public void SetValue(T value = default)
            => Key.SetValue(value);
        public void SetValue(T value = default, bool isPersistent = true)
            => Key.SetValue(value, isPersistent);
        public bool TryGetValue(out T value)
            => Key.TryGetValue(out value);
        public T GetValue()
            => Key.GetValue<T>();
        public void SetPersistent(bool isPersistent)
            => Key.SetPersistent<T>(isPersistent);

        /// <summary>
        /// OnValueChange(PreviousValue, NewValue)
        /// </summary>
        public void OnValueChangedSubscribe(Action<T, T> action)
            => Key.OnValueChangedSubscribe(action);

        public void OnValueChangedUnsubscribe(Action<T, T> action)
            => Key.OnValueChangedUnsubscribe(action);

        public bool TryGetPersistent(out bool isPersistent)
            => Key.TryGetPersistent<T>(out isPersistent);
        public bool GetPersistent()
            => Key.GetPersistent<T>();
        #endregion
        #region Internal
        private void ValueChanged(T previousValue, T newValue)
        {
            OnValueChanged?.Invoke(newValue);
        }
        #endregion
    }
}
