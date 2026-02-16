using UnityEngine;

namespace Heroes.Goap.Runtime.World
{
    public class GoapLocationMarker : MonoBehaviour
    {
        public LocationSO LocationType;

        void OnEnable()
        {
            GoapWorldState.RegisterLocation(this);
        }

        void OnDisable()
        {
            GoapWorldState.UnregisterLocation(this);
        }
    }
}
