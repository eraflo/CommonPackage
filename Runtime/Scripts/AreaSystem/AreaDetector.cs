using System;
using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace Eraflo.Common.AreaSystem
{
    /// <summary>
    /// Specialized BaseObject for detection areas (Checkpoints, Goals).
    /// Uses the Area Collider slot from BaseObject for its logic.
    /// </summary>
    public class AreaDetector : BaseObject
    {
        public Action<Collider> onTriggerEnter;
        public Action<Collider> onTriggerStay;
        public Action<Collider> onTriggerExit;

        /// <summary>
        /// Reference to the trigger collider used as the detection area.
        /// </summary>
        public Collider Collider => _areaCollider;

        protected override void Awake()
        {
            base.Awake();

            // Ensure our Area Collider is correctly configured as a trigger
            if (_areaCollider != null)
            {
                _areaCollider.isTrigger = true;
            }
            else
            {
                Debug.LogWarning($"[AreaDetector] Area Collider slot is empty on {gameObject.name}. This detector will not work!", this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            onTriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExit?.Invoke(other);
        }
    }
}