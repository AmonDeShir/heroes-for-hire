using System;
using Heroes.Systems.GOAP.Demo;
using UnityEngine;

namespace GOAP.Demo
{
    public class DemoWorldStateManager : MonoBehaviour
    {
        [SerializeField]
        private Transform home;
        
        [SerializeField]
        private Transform work;
        
        [SerializeField]
        private Transform store;
        
        [SerializeField]
        private Transform mine;
        
        private DemoWorldState state;
        public DemoWorldState State => state;

        public void Awake()
        {
            state = new DemoWorldState();
            
            state.RegisterLocation(DemoConsts.HOME, new Vector2(home.position.x, home.position.z));
            state.RegisterLocation(DemoConsts.WORK, new Vector2(work.position.x, work.position.z));
            state.RegisterLocation(DemoConsts.MINE, new Vector2(mine.position.x, mine.position.z));
            state.RegisterLocation(DemoConsts.STORE, new Vector2(store.position.x, store.position.z));
        }
    }
}

