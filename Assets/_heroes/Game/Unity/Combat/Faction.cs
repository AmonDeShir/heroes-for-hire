using UnityEngine;

namespace Heroes.Game.Combat
{
    public class Faction : MonoBehaviour
    {
        public TeamType Team = TeamType.Heroes;
        public int Level = 1;
    }

    public enum TeamType
    {
        Heroes = 0,
        Enemies = 1,
    }
}
