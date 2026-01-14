using System;
using UnityEngine;
using UnityUtils;

namespace GOAP
{
    [RequireComponent(typeof(SphereCollider))]
    public class Sensor : MonoBehaviour
    {
        [SerializeField]
        private float m_detectionRadius = 5f;

        [SerializeField]
        [Tooltip("In seconds")]
        private float m_timerInterval = 1f;
        
        [SerializeField]
        private string m_targetTag = "Player";
        
        public Vector3 TargetPosition => _target ? _target.transform.position : Vector3.zero;
        public bool IsTargetInRange => TargetPosition != Vector3.zero;
        
        private SphereCollider _collider;
        private GameObject _target;
        private Vector3 _lostTargetPosition;
        private Timer _timer;
        
        public event Action OnTargetChange = delegate { };

        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
            _collider.radius = m_detectionRadius;
        }

        private void Start()
        {
            _timer = new Timer(m_timerInterval);
            _timer.OnTimeOut += () => UpdateTargetPosition(_target.OrNull());
            
            _timer.Start();
        }

        private void UpdateTargetPosition(GameObject target = null)
        {
            _target = target;

            if (IsTargetInRange && (_lostTargetPosition != TargetPosition || _lostTargetPosition != Vector3.zero))
            {
                _lostTargetPosition = TargetPosition;
                OnTargetChange();
            }
        }

        private void Update()
        {
            _timer.Tick(Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(m_targetTag))
            {
                return;
            }
            
            UpdateTargetPosition();
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(m_targetTag))
            {
                return;
            }
            
            UpdateTargetPosition();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = IsTargetInRange ? Color.red : Color.green;
            Gizmos.DrawWireSphere(TargetPosition, m_detectionRadius);
        }
    }
}