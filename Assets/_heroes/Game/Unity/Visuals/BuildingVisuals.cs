using System.Collections;
using UnityEngine;

namespace Heroes.Game.Buildings
{
    public class BuildingVisuals : MonoBehaviour
    {
        [Header("Roots")]
        [SerializeField] private Transform constructionStagesRoot;
        [SerializeField] private GameObject completeRoot;

        [Header("FX")]
        [SerializeField] private ParticleSystem placementBurstParticles;

        [Header("SFX")] 
        [SerializeField] private AudioSource buildingSfx;
        [SerializeField] private AudioSource sfxAudioSource;
        [SerializeField] private AudioClip placementBurstAudio;

        [Header("Animation")]
        [SerializeField] private float popDuration = 0.25f;
        [SerializeField] private float popHeight = 0.75f;

        private GameObject[] _constructionStages;
        private int _currentStage = -1;
        private BuildingState _currentState;
        private bool _placementBurstPlayed;

        private void Awake()
        {
            _constructionStages = CacheStageObjects(constructionStagesRoot);
        }

        public void RefreshImmediate(BuildingModel model)
        {
            _currentState = model.State;
            _currentStage = GetConstructionStageIndex(model);

            ApplyState(model.State);
            ApplyStageCumulative(_currentStage);
            TryPlayPlacementBurst(model.State);
        }

        public void Refresh(BuildingModel model)
        {
            if (_currentState != model.State)
            {
                _currentState = model.State;
                ApplyState(model.State);
            }

            if (model.State != BuildingState.UnderConstruction)
            {
                if (buildingSfx.isPlaying)
                {
                    buildingSfx.Stop();
                }

                return;
            }

            if (model.State == BuildingState.UnderConstruction)
            {
                if (!buildingSfx.isPlaying)
                {
                    buildingSfx.Play();
                }
            }

            var newStage = GetConstructionStageIndex(model);
            if (newStage != _currentStage)
            {
                ApplyStageCumulative(newStage);
                if (newStage > _currentStage)
                {
                    AnimateStage(newStage);
                }

                _currentStage = newStage;
            }
        }

        private void ApplyState(BuildingState state)
        {
            var showConstruction = state == BuildingState.UnderConstruction;
            var showComplete = state == BuildingState.Completed;

            constructionStagesRoot.gameObject.SetActive(showConstruction);
            completeRoot.SetActive(showComplete);
        }

        private void ApplyStageCumulative(int stage)
        {
            if (_constructionStages == null || _constructionStages.Length == 0)
            {
                return;
            }

            for (var i = 0; i < _constructionStages.Length; i++)
            {
                _constructionStages[i].SetActive(i <= stage);
            }
        }

        private void AnimateStage(int stage)
        {
            if (_constructionStages == null || _constructionStages.Length == 0)
            {
                return;
            }

            if (stage < 0 || stage >= _constructionStages.Length)
            {
                buildingSfx.Stop();
                
                return;
            }

            var target = _constructionStages[stage];
            if (target == null)
            {
                return;
            }

            StopAllCoroutines();
            StartCoroutine(PlayStagePopAnimation(target.transform));
        }

        private IEnumerator PlayStagePopAnimation(Transform target)
        {
            var basePos = target.localPosition;
            var baseScale = target.localScale;

            var startPos = basePos + Vector3.up * popHeight;

            var scaleDuration = popDuration * 0.6f;
            var moveDuration = popDuration * 0.4f;

            target.localScale = Vector3.zero;
            target.localPosition = startPos;

            var time = 0f;

            while (time < scaleDuration)
            {
                time += Time.deltaTime;
                var t = Mathf.Clamp01(time / scaleDuration);

                var scaleT = t * t;

                target.localScale = Vector3.LerpUnclamped(Vector3.zero, baseScale, scaleT);

                yield return null;
            }

            target.localScale = baseScale;

            time = 0f;

            while (time < moveDuration)
            {
                time += Time.deltaTime;
                var t = Mathf.Clamp01(time / moveDuration);

                var moveT = t * t * t;

                target.localPosition = Vector3.LerpUnclamped(startPos, basePos, moveT);

                yield return null;
            }

            target.localPosition = basePos;
        }
        private int GetConstructionStageIndex(BuildingModel model)
        {
            return CalculateStageIndex(model.Health.Current, model.Health.Max, _constructionStages);
        }

        private void TryPlayPlacementBurst(BuildingState state)
        {
            if (_placementBurstPlayed || state != BuildingState.UnderConstruction)
            {
                return;
            }

            _placementBurstPlayed = true;
            if (placementBurstParticles != null)
            {
                placementBurstParticles.Play();
            }
            
            if (sfxAudioSource != null)
            {
                sfxAudioSource.PlayOneShot(placementBurstAudio);
            }
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
