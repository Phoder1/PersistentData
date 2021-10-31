using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoder1.PersistentData
{
    public abstract class BaseKeySOData<T> : BasePersistentData<T>
    {
        [SerializeField]
        private KeySO _key = default;
        protected override IDataKey Key => _key;
    }
}
