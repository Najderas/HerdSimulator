using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets
{
    public class Flock
    {
        private readonly List<GameObject> _sheeps;

        private Vector3 _leftMost;
        private Vector3 _bottomMost;
        private Vector3 _rightMost;
        private Vector3 _topMost;
        private Vector3 _center;

        public Flock()
        {
            _sheeps = new List<GameObject>();
            _leftMost = new Vector3(float.MaxValue, 0);
            _bottomMost = new Vector3(0, float.MaxValue);
            _rightMost = new Vector3(float.MinValue, 0);
            _topMost = new Vector3(0, float.MinValue);
            _center = new Vector3();
        }

        public void AddSheep(GameObject sheep)
        {
            CountCenter(sheep);
            _sheeps.Add(sheep);
            CountFlockContour(sheep);
        }

        public Vector3 GetCenter()
        {
            return _center;
        }

        public List<GameObject> GetSheeps()
        {
            return _sheeps;
        }

        public Dictionary<string, float> GetFlockBorders()
        {
            return new Dictionary<string, float>
            {
                {"left", _leftMost.x},
                {"right", _rightMost.x},
                {"top", _topMost.y},
                {"bottom", _bottomMost.y}
            };
        }

        public Dictionary<string, Vector3> GetFlockContour()
        {
            return new Dictionary<string, Vector3>
            {
                {"left", _leftMost},
                {"right", _rightMost},
                {"top", _topMost},
                {"bottom", _bottomMost}
            };
        }

        public float GetMaxRadius()
        {
            return GetFlockContour().Select(contour => Vector3.Distance(_center, contour.Value)).Concat(new[] {0f}).Max();
        }

        private void CountCenter(GameObject sheep)
        {
            var x = (_center.x * _sheeps.Count + sheep.transform.position.x) / (_sheeps.Count + 1);
            var y = (_center.y * _sheeps.Count + sheep.transform.position.y) / (_sheeps.Count + 1);
            _center = new Vector3(x, y);
        }

        private void SetLeftMost(Vector3 other)
        {
            if (other.x < _leftMost.x)
            {
                _leftMost = other;
            }
        }

        private void SetBottomMost(Vector3 other)
        {
            if (other.y < _bottomMost.y) {
                _bottomMost = other;
            }
        }

        private void SetRightMost(Vector3 other)
        {
            if (other.x > _rightMost.x) {
                _rightMost = other;
            }
        }

        private void SetTopMost(Vector3 other)
        {
            if (other.y > _topMost.y) {
                _topMost = other;
            }
        }

        private void CountFlockContour(GameObject sheep)
        {
            var position = sheep.transform.position;
            SetBottomMost(position);
            SetLeftMost(position);
            SetRightMost(position);
            SetTopMost(position);
        }
    }
}
