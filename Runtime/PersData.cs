using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phoder1.PersistentData
{
    /// <summary>
    /// A class for managing persistent data
    /// </summary>
    /// <typeparam name="T">The type of data the class manages</typeparam>
    public static class PersData<T>
    {
        private static Dictionary<IDataKey, PersistentData> _dataDict = null;
        ///<summary>If a value with that key already exists it will be overriden.</summary>
        /// <param name="key">The key to access the data in the dictionary.</param>
        /// <param name="value">The value to assign by default.</param>
        /// <param name="isPersistent">Whether the data is persistent between scenes, otherwise will be cleared when ALL scenes are unloaded.</param>
        public static void SetValue(IDataKey key, T value = default)
        {
            if (key == null)
                return;

            if (TryGetData(key, out var data))
                data.Value = value;
            else
                AddToDictionary(key, new PersistentData(value));
        }
        public static void SetValue(IDataKey key, T value = default, bool isPersistent = true)
        {
            if (key == null)
                return;

            if (TryGetData(key, out var data))
            {
                data.Value = value;
                SetPersistent(data, isPersistent);
            }
            else
            {
                AddToDictionary(key, new PersistentData(value, isPersistent));
            }
        }

        public static bool TryGetValue(IDataKey key, out T value)
        {
            if (TryGetData(key, out var val))
            {
                value = val.Value;
                return true;
            }

            value = default;
            return false;

        }
        public static T GetValue(IDataKey key)
        {
            if (TryGetValue(key, out var T))
                return T;

            return default;
        }
        public static void SetPersistent(IDataKey key, bool isPersistent)
        {
            if (key == null)
                return;

            if (TryGetData(key, out var data))
            {
                data.Persistent = isPersistent;
            }
            else
            {
                data = PersistentData.Default;
                SetPersistent(data, isPersistent);
                AddToDictionary(key, data);
            }
        }
        public static bool TryGetPersistent(IDataKey key, out bool isPersistent)
        {
            if (TryGetData(key, out var val))
            {
                isPersistent = val.Persistent;
                return true;
            }

            isPersistent = true;
            return false;

        }
        public static bool GetPersistent(IDataKey key)
        {
            if (TryGetPersistent(key, out var isPersistent))
                return isPersistent;

            return default;
        }
        /// <summary>
        /// OnValueChange(PreviousValue, NewValue)
        /// </summary>
        public static void OnValueChangedSubscribe(IDataKey key, Action<T, T> action)
        {
            if (key == null)
                return;

            if (TryGetData(key, out var data))
                data.OnValueChanged += action;
            else
            {
                var defData = PersistentData.Default;
                AddToDictionary(key, defData);
                defData.OnValueChanged += action;
            }
        }

        public static void OnValueChangedUnsubscribe(IDataKey key, Action<T, T> action)
        {
            if (key == null)
                return;

            if (TryGetData(key, out var data))
                data.OnValueChanged -= action;
        }
         
        public static void ClearNonePersistentData()
        {
            if (_dataDict == null || _dataDict.Count == 0)
                return;

            List<IDataKey> keysToRemove = new List<IDataKey>();

            foreach (var dataPair in _dataDict)
                if (!dataPair.Value.Persistent)
                    keysToRemove.Add(dataPair.Key);

            keysToRemove.ForEach((x) => _dataDict.Remove(x));

#if UNITY_EDITOR
            Debug.Log("Cleared none-persistent data.");
#endif
        }
        public static void ClearAllData()
        {
            if (_dataDict == null || _dataDict.Count == 0)
                return;

            IDataKey[] keys = new IDataKey[_dataDict.Count];

            _dataDict.Keys.CopyTo(keys, 0);

            Array.ForEach(keys, (x) => _dataDict.Remove(x));

#if UNITY_EDITOR
            Debug.Log("Cleared all data.");
#endif
        }
        #region Internal
        private static void SetPersistent(PersistentData data, bool isPersistent)
        {
            if (data == null)
                return;

            data.Persistent = isPersistent;
        }
        private static void AddToDictionary(IDataKey key, PersistentData data)
        {
            if (key == null)
                return;

            if (_dataDict == null)
                _dataDict = new Dictionary<IDataKey, PersistentData>();

            _dataDict.Add(key, data);
        }

        private static bool TryGetData(IDataKey key, out PersistentData data)
        {
            if (_dataDict != null && key != null && _dataDict.Count > 0 && _dataDict.TryGetValue(key, out var _data))
            {
                data = _data;
                return true;
            }

            data = default;
            return false;
        }

        #endregion

        #region Data class
        private class PersistentData
        {
            public PersistentData(T value = default, bool persistent = true)
            {
                Value = value;
                _persistent = persistent;
            }
            private bool _persistent = true;
            private T _value = default;

            /// <summary>
            /// The current data assigned to the key.
            /// </summary>
            public T Value
            {
                get => _value;
                set
                {
                    if (_value.Equals(value))
                        return;

                    OnValueChanged?.Invoke(_value, value);

                    _value = value;
                }
            }

            /// <summary>
            /// OnValueChange(PreviousValue, NewValue)
            /// </summary>
            public event Action<T, T> OnValueChanged = default;

            public static PersistentData Default => new PersistentData();

            public bool Persistent { get => _persistent; set => _persistent = value; }
        }
        #endregion
    }

    public interface IDataKey { }
    public static class PersDataHelper
    {
        public static void SetValue<T>(this IDataKey key, T value = default)
            => PersData<T>.SetValue(key, value);
        public static void SetValue<T>(this IDataKey key, T value = default, bool isPersistent = true)
            => PersData<T>.SetValue(key, value, isPersistent);
        public static bool TryGetValue<T>(this IDataKey key, out T value)
            => PersData<T>.TryGetValue(key, out value);
        public static T GetValue<T>(this IDataKey key)
            => PersData<T>.GetValue(key);
        public static void SetPersistent<T>(this IDataKey key, bool isPersistent)
            => PersData<T>.SetPersistent(key, isPersistent);

        /// <summary>
        /// OnValueChange(PreviousValue, NewValue)
        /// </summary>
        public static void OnValueChangedSubscribe<T>(this IDataKey key, Action<T, T> action)
            => PersData<T>.OnValueChangedSubscribe(key, action);

        public static void OnValueChangedUnsubscribe<T>(this IDataKey key, Action<T, T> action)
            => PersData<T>.OnValueChangedUnsubscribe(key, action);

        public static bool TryGetPersistent<T>(this IDataKey key, out bool isPersistent)
            => PersData<T>.TryGetPersistent(key, out isPersistent);
        public static bool GetPersistent<T>(this IDataKey key)
            => PersData<T>.GetPersistent(key);
    }
}
