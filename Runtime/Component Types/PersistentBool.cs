using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoder1.PersistentData
{
    [AddComponentMenu("Phoder1/Persistent Data/Bool data")]
    public class PersistentBool : BaseKeySOData<bool> 
    {
        [SerializeField]
        bool _defaultValue;
        protected override bool DefaultValue => _defaultValue;
    }
}
