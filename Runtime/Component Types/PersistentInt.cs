using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoder1.PersistentData
{
    [AddComponentMenu("Phoder1/Persistent Data/Int data")]
    public class PersistentInt : BaseKeySOData<int> 
    {
        [SerializeField]
        int _defaultValue;
        protected override int DefaultValue => _defaultValue;
    }
}
