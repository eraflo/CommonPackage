using System;
using Eraflo.Common.ObjectSystem;
using UnityEngine;

namespace FallGuys.Traps
{
    /// <summary>
    /// Component responsible for detecting collisions and triggers for traps.
    /// Distinguishes between trigger zones (Blower, Launcher detection) and physical impacts (Bumper).
    /// </summary>
    public class TrapDetector : BaseObject
    {
        public Action<Collider> onTriggerEnter;
        public Action<Collider> onTriggerStay;
        public Action<Collider> onTriggerExit;

        public Action<Collision> onCollisionEnter;
        public Action<Collision> onCollisionStay;
        public Action<Collision> onCollisionExit;

        private void OnTriggerEnter(Collider other) => onTriggerEnter?.Invoke(other);
        private void OnTriggerStay(Collider other) => onTriggerStay?.Invoke(other);
        private void OnTriggerExit(Collider other) => onTriggerExit?.Invoke(other);

        private void OnCollisionEnter(Collision collision) => onCollisionEnter?.Invoke(collision);
        private void OnCollisionStay(Collision collision) => onCollisionStay?.Invoke(collision);
        private void OnCollisionExit(Collision collision) => onCollisionExit?.Invoke(collision);
    }
}
