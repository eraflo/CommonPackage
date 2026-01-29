using System.Collections.Generic;
using UnityEngine;

namespace Eraflo.Common.ObjectSystem
{
    public class VisualPreviewDrawer : MonoBehaviour
    {
        [SerializeField] private Material _lineMaterial;
        [SerializeField] private float _defaultWidth = 0.05f;

        private List<LineRenderer> _linePool = new List<LineRenderer>();
        private List<LineRenderer> _activeLines = new List<LineRenderer>();
        private int _poolPointer = 0;

        public void Clear()
        {
            foreach (var line in _activeLines)
            {
                line.positionCount = 0;
                line.gameObject.SetActive(false);
            }
            _activeLines.Clear();
            _poolPointer = 0;
        }

        public void DrawBox(Vector3 center, Vector3 size, Color color, Matrix4x4 matrix)
        {
            var line = GetLine(color);
            line.positionCount = 16;
            
            Vector3 h = size * 0.5f;
            Vector3[] p = {
                new Vector3(-h.x, -h.y, -h.z), new Vector3(h.x, -h.y, -h.z), new Vector3(h.x, h.y, -h.z), new Vector3(-h.x, h.y, -h.z), // front
                new Vector3(-h.x, -h.y, -h.z), // back to start
                new Vector3(-h.x, -h.y, h.z), new Vector3(h.x, -h.y, h.z), new Vector3(h.x, h.y, h.z), new Vector3(-h.x, h.y, h.z), // back
                new Vector3(-h.x, -h.y, h.z), // back to start
                new Vector3(h.x, -h.y, h.z), new Vector3(h.x, -h.y, -h.z), // side
                new Vector3(h.x, h.y, -h.z), new Vector3(h.x, h.y, h.z), // side
                new Vector3(-h.x, h.y, h.z), new Vector3(-h.x, h.y, -h.z) // side
            };

            for (int i = 0; i < p.Length; i++) p[i] = matrix.MultiplyPoint(center + p[i]);
            line.SetPositions(p);
        }

        public void DrawSphere(Vector3 center, float radius, Color color, Matrix4x4 matrix)
        {
            DrawCircle(center, radius, color, matrix, Vector3.up);
            DrawCircle(center, radius, color, matrix, Vector3.right);
            DrawCircle(center, radius, color, matrix, Vector3.forward);
        }

        public void DrawCircle(Vector3 center, float radius, Color color, Matrix4x4 matrix, Vector3 normal)
        {
            var line = GetLine(color);
            int segments = 24;
            line.positionCount = segments + 1;
            
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, normal);
            Vector3[] p = new Vector3[segments + 1];
            for (int i = 0; i <= segments; i++)
            {
                float angle = i * (360f / segments) * Mathf.Deg2Rad;
                Vector3 local = rot * new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                p[i] = matrix.MultiplyPoint(center + local);
            }
            line.SetPositions(p);
        }

        public void DrawArc(Vector3 center, float radius, float angleRange, Color color, Matrix4x4 matrix)
        {
            var line = GetLine(color);
            int segments = 12;
            line.positionCount = segments + 2; // +1 for center, +1 for loop-ish

            Vector3[] p = new Vector3[segments + 2];
            p[0] = matrix.MultiplyPoint(center);
            
            float angleStep = (angleRange * 2f) / segments;
            for (int i = 0; i <= segments; i++)
            {
                float currentAngle = -angleRange + (i * angleStep);
                Vector3 rotDir = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward;
                p[i + 1] = matrix.MultiplyPoint(center + rotDir * radius);
            }
            line.SetPositions(p);
        }

        public void DrawLine(Vector3 start, Vector3 end, Color color, Matrix4x4 matrix)
        {
            var line = GetLine(color);
            line.positionCount = 2;
            line.SetPosition(0, matrix.MultiplyPoint(start));
            line.SetPosition(1, matrix.MultiplyPoint(end));
        }

        private LineRenderer GetLine(Color color)
        {
            LineRenderer line;
            if (_poolPointer < _linePool.Count)
            {
                line = _linePool[_poolPointer];
            }
            else
            {
                GameObject go = new GameObject("VisualLine");
                go.transform.SetParent(transform);
                line = go.AddComponent<LineRenderer>();
                line.material = _lineMaterial ?? new Material(Shader.Find("Sprites/Default"));
                line.startWidth = line.endWidth = _defaultWidth;
                line.useWorldSpace = true;
                _linePool.Add(line);
            }

            line.gameObject.SetActive(true);
            line.startColor = line.endColor = color;
            _activeLines.Add(line);
            _poolPointer++;
            return line;
        }

        private void OnDisable() => Clear();
    }
}
