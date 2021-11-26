using UnityEngine;

namespace Phoder1.PersistentData
{
    [AddComponentMenu("Phoder1/Persistent Data/Gameobject data")]
    public class PersistentGameobject : BaseKeySOData<GameObject> 
    {
        [SerializeField]
        GameObject _defaultValue;
        protected override GameObject DefaultValue => _defaultValue;
    }
}
