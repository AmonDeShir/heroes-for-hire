using UnityEngine;

namespace Heroes.Game.Buildings
{
    public class BuildingDestructionVisuals : MonoBehaviour
    {
        [Header("Roots")]
        [SerializeField] private Transform destructionStagesRoot;

        private GameObject[] _destructionStages;
        private int _currentStage = -1;

        private void Awake()
        {
            _destructionStages = CacheStageObjects(destructionStagesRoot);
        }

        public void Refresh(BuildingModel model)
        {
            var showDestruction = model.State == BuildingState.Damaged || model.State == BuildingState.Destroyed;
            if (destructionStagesRoot != null)
            {
                destructionStagesRoot.gameObject.SetActive(showDestruction);
            }

            if (!showDestruction)
            {
                return;
            }

            var newStage = GetDestructionStageIndex(model);
            if (newStage != _currentStage)
            {
                ApplyStageImmediate(newStage);
                _currentStage = newStage;
            }
        }

        private void ApplyStageImmediate(int stage)
        {
            if (_destructionStages == null || _destructionStages.Length == 0)
            {
                return;
            }

            for (var i = 0; i < _destructionStages.Length; i++)
            {
                _destructionStages[i].SetActive(i == stage);
            }
        }

        private int GetDestructionStageIndex(BuildingModel model)
        {
            var damaged = model.Health.Max - model.Health.Current;
            return CalculateStageIndex(damaged, model.Health.Max, _destructionStages);
        }

        private static int CalculateStageIndex(float value, float max, GameObject[] stages)
        {
            if (stages == null || stages.Length == 0 || max <= 0f)
            {
                return 0;
            }

            var normalized = value / max;
            
            if (normalized < 0f)
            {
                normalized = 0f;
            }

            if (normalized > 1f)
            {
                normalized = 1f;
            }

            return Mathf.RoundToInt(normalized * (stages.Length - 1));
        }

        private static GameObject[] CacheStageObjects(Transform root)
        {
            if (root == null)
            {
                return System.Array.Empty<GameObject>();
            }

            var count = root.childCount;
            var stages = new GameObject[count];

            for (var i = 0; i < count; i++)
            {
                stages[i] = root.GetChild(i).gameObject;
            }

            return stages;
        }
    }
}


