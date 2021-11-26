using System;
using UnityEngine;
using UnityEngine.Events;

namespace Phoder1.PersistentData
{
    public abstract class BasePersistentData<T> : MonoBehaviour
    {
        [SerializeField]
        private bool _defaultPersistent;

        [SerializeField]
        private UnityEvent<T> OnValueChanged;
        protected abstract IDataKey Key { get; }
        protected virtual T DefaultValue => default;

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            if (!TryGetValue(out var val))
                SetValue(DefaultValue, _defaultPersistent);
        }

        private void OnEnable()
        {
            OnValueChangedSubscribe(ValueChanged);
#if UNITY_EDITOR
            if (TryGetValue(out var _val))
                Debug.Log($"Value exists, Value = {_val}");
            else
                Debug.Log($"Value doesn't exists! Value defaulted to {_val}");
#endif
        }
        private void OnDisable()
        {
            OnValueChangedUnsubscribe(ValueChanged);
        }
        public T Value
        {
            get
            {
                if(TryGetValue(out T val))
                    return val;

                Init();
                return DefaultValue;
            }
            set => SetValue(value, Persistent);
        }
        public bool Persistent
        {
            get
            {
                if(TryGetPersistent(out bool persistent))
                    return persistent;

                Init();
                return _defaultPersistent;
            }
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
