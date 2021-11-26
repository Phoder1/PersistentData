using UnityEngine;

namespace Phoder1.PersistentData
{
    [DefaultExecutionOrder(-999)]
    public abstract class BaseKeySOData<T> : BasePersistentData<T>
    {
        [SerializeField]
        private KeySO _key = default;
        protected override IDataKey Key => _key;
    }
}
