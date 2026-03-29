using UnityEngine;

namespace Heroes.Tools.BuildingCreator
{
    public class BuildingCreatorPreview : MonoBehaviour
    {
        [SerializeField] private Camera captureCamera;
        [SerializeField] private Transform previewRoot;

        public Camera CaptureCamera => captureCamera;
        public Transform PreviewRoot => previewRoot;

        public void PlayConstructionSequence(float delaySeconds)
        {
            if (!Application.isPlaying)
            {
                TriggerAnimator("Build");
                return;
            }
            TriggerAnimator("Build");
        }

        public void PlayDestructionSequence(float delaySeconds)
        {
            if (!Application.isPlaying)
            {
                TriggerAnimator("Destroy");
                return;
            }
            TriggerAnimator("Destroy");
        }
        private void TriggerAnimator(string trigger)
        {
            if (previewRoot == null || previewRoot.childCount == 0)
            {
                return;
            }

            var animator = previewRoot.GetChild(0).GetComponentInChildren<Animator>();
            if (animator == null)
            {
                return;
            }

            animator.ResetTrigger(trigger);
            animator.SetTrigger(trigger);
        }
    }
}
