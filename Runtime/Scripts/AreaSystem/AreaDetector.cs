using System;
using Eraflo.Common.ObjectSystem;
using UnityEngine;


namespace Eraflo.Common.AreaSystem
{
    [RequireComponent(typeof(Collider))]
    public class AreaDetector : BaseObject
    {
        [SerializeField] private Collider _colliderTrigger;
        private AreaSO _area;

        public Action<Collider> onTriggerEnter;
        public Action<Collider> onTriggerStay;
        public Action<Collider> onTriggerExit;

        public Collider Collider => _collider;

        public void SetCenter(Vector3 center)
        {
            if (_colliderTrigger is BoxCollider box) box.center = center;
            else if (_colliderTrigger is SphereCollider sphere) sphere.center = center;
            else if (_colliderTrigger is CapsuleCollider capsule) capsule.center = center;
        }

        public void SetSize(Vector3 size)
        {
            if (_colliderTrigger is BoxCollider box) box.size = size;
        }

        public void SetRadius(float radius)
        {
            if (_colliderTrigger is SphereCollider sphere) sphere.radius = radius;
        }

        public void SetHeight(float height)
        {
            if (_colliderTrigger is CapsuleCollider capsule) capsule.height = height;
        }

        public void SetDirection(int direction)
        {
            if (_colliderTrigger is CapsuleCollider capsule) capsule.direction = direction;
        }

        protected override void Awake()
        {
            base.Awake();
            if(_colliderTrigger == null)
            {
                if (!TryGetComponent(out _colliderTrigger))
                {
                    Debug.LogError($"[AreaDetector] Collider trigger is missing on {gameObject.name}!", this);
                    return;
                }
            }
            _colliderTrigger.isTrigger = true;

            _area = _config as AreaSO;
            SyncTriggerCollider();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            SyncTriggerCollider();
        }

        public void SyncTriggerCollider()
        {
            _area = _config as AreaSO;
            if (_area == null || _colliderTrigger == null) return;

            SetCenter(_area.Center);
            if (_area.Shape == AreaShape.Box) SetSize(_area.AreaSize);
            else if (_area.Shape == AreaShape.Sphere) SetRadius(_area.Radius);
            else if (_area.Shape == AreaShape.Capsule)
            {
                SetRadius(_area.Radius);
                SetHeight(_area.CapsuleHeight);
                SetDirection(_area.CapsuleDirection);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // TODO: Check if Player or not

            onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            // TODO: Check if Player or not

            onTriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            // TODO: Check if Player or not

            onTriggerExit?.Invoke(other);
        }

        private void OnDrawGizmos()
        {
            DrawAreaGizmos(false);
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            DrawAreaGizmos(true);
        }

        private void DrawAreaGizmos(bool selected)
        {
            AreaSO config = _config as AreaSO;
            if (config == null) return;

            Color baseColor = config.GizmoColor;
            Gizmos.color = selected ? new Color(baseColor.r, baseColor.g, baseColor.b, 0.4f) : new Color(baseColor.r, baseColor.g, baseColor.b, 0.1f);

            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            Vector3 center = config.Center;

            switch (config.Shape)
            {
                case AreaShape.Box:
                    Gizmos.DrawCube(center, config.AreaSize);
                    Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.8f);
                    Gizmos.DrawWireCube(center, config.AreaSize);
                    break;
                case AreaShape.Sphere:
                    Gizmos.DrawSphere(center, config.Radius);
                    Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.8f);
                    Gizmos.DrawWireSphere(center, config.Radius);
                    break;
                case AreaShape.Capsule:
                    DrawCapsuleGizmo(center, config.Radius, config.CapsuleHeight, config.CapsuleDirection, baseColor);
                    break;
            }

            Gizmos.matrix = oldMatrix;
        }

        private void DrawCapsuleGizmo(Vector3 center, float radius, float height, int direction, Color color)
        {
            Vector3 point1 = center;
            Vector3 point2 = center;
            float offset = Mathf.Max(0, (height / 2f) - radius);

            if (direction == 0) { point1.x += offset; point2.x -= offset; } // X
            else if (direction == 1) { point1.y += offset; point2.y -= offset; } // Y
            else { point1.z += offset; point2.z -= offset; } // Z

            Gizmos.DrawSphere(point1, radius);
            Gizmos.DrawSphere(point2, radius);

            Gizmos.color = new Color(color.r, color.g, color.b, 0.8f);
            Gizmos.DrawWireSphere(point1, radius);
            Gizmos.DrawWireSphere(point2, radius);

            // Draw connecting lines
            // (Ignoring complexity for now, these spheres already give a good idea of the volume)
        }
    }
}