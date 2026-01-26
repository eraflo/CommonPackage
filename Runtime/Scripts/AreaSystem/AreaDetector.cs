using System;
using ObjectSystem;
using UnityEngine;


namespace AreaSystem
{
    [RequireComponent(typeof(Collider))]
    public class AreaDetector : BaseObject
    {
        private Collider _collider;
        private AreaSO _area;

        public Action<Collider> onTriggerEnter;
        public Action<Collider> onTriggerStay;
        public Action<Collider> onTriggerExit;

        public void SetSize(Vector3 size)
        {
            if (_collider is BoxCollider box) box.size = size;
        }

        public void SetRadius(float radius)
        {
            if (_collider is SphereCollider sphere) sphere.radius = radius;
        }

        public void SetHeight(float height)
        {
            if (_collider is CapsuleCollider capsule) capsule.height = height;
        }

        public void SetDirection(int direction)
        {
            if (_collider is CapsuleCollider capsule) capsule.direction = direction;
        }

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;

            _area = _config as AreaSO;
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

        private void OnDrawGizmosSelected()
        {
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

            switch (config.Shape)
            {
                case AreaShape.Box:
                    Gizmos.DrawCube(Vector3.zero, config.AreaSize);
                    Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.8f);
                    Gizmos.DrawWireCube(Vector3.zero, config.AreaSize);
                    break;
                case AreaShape.Sphere:
                    Gizmos.DrawSphere(Vector3.zero, config.Radius);
                    Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.8f);
                    Gizmos.DrawWireSphere(Vector3.zero, config.Radius);
                    break;
                case AreaShape.Capsule:
                    DrawCapsuleGizmo(config.Radius, config.CapsuleHeight, config.CapsuleDirection, baseColor);
                    break;
            }

            Gizmos.matrix = oldMatrix;
        }

        private void DrawCapsuleGizmo(float radius, float height, int direction, Color color)
        {
            // Simple visual representation of a capsule using wire spheres and lines
            // or just a custom primitive if we want to be fancy. 
            // For now, let's use a WireCapsule approach or simple spheres/lines.

            Vector3 point1 = Vector3.zero;
            Vector3 point2 = Vector3.zero;
            float offset = Mathf.Max(0, (height / 2f) - radius);

            if (direction == 0) { point1.x = offset; point2.x = -offset; } // X
            else if (direction == 1) { point1.y = offset; point2.y = -offset; } // Y
            else { point1.z = offset; point2.z = -offset; } // Z

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