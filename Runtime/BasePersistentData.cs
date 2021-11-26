using System;
using UnityEngine;
using UnityEngine.Events;

namespace Phoder1.PersistentData
{
    [DefaultExecutionOrder(-999)]
    public abstract class BasePersistentData<T> : MonoBehaviour
    {
        [SerializeField]
        private T _defaultValue = default;
        [SerializeField]
        private bool _defaultPersistent = false;
        [Tooltip("If set to true, will override any existing value back to default on awake")]
        [SerializeField]
        private bool _overrideValue;

        [SerializeField]
        private UnityEvent<T> OnValueChanged;
        protected abstract IDataKey Key { get; }
        protected virtual T DefaultValue => _defaultValue;

        private void Awake()
        {
            if (_overrideValue)
                SetToDefault();
            else
                Init();
        }

        private void OnEnable()
        {
            OnValueChangedSubscribe(ValueChanged);
#if UNITY_EDITOR
            if (TryGetValue(out var _val))
                Debug.Log($"Value exists, Value = {_val}", this);
            else
                Debug.Log($"Value doesn't exists! Value defaulted to {_val}", this);
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
                if (TryGetValue(out T val))
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
                if (TryGetPersistent(out bool persistent))
                    return persistent;

                Init();
                return _defaultPersistent;
            }
            set => SetPersistent(value);
        }
        private void Init()
        {
            if (!TryGetValue(out var val))
                SetToDefault();
        }
        private void SetToDefault() => SetValue(DefaultValue, _defaultPersistent);
        private void ValueChanged(T previousValue, T newValue) => OnValueChanged?.Invoke(newValue);
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
    }
}
