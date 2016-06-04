using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Flock
    {
        private readonly List<GameObject> _sheeps;
        private Dictionary<string, Vector3> _flockContourVectors;
        private Dictionary<string, float> _flockBorderPoints;

        private Vector3 _leftMost;
        private Vector3 _bottomMost;
        private Vector3 _rightMost;
        private Vector3 _topMost;

        public Flock()
        {
            _sheeps = new List<GameObject>();
        }

        public void AddSheep(GameObject sheep)
        {
            _sheeps.Add(sheep);
            _flockContourVectors = CountFlockContour();
            _flockBorderPoints = CountFlockBorders();
        }

        public List<GameObject> GetSheeps()
        {
            return _sheeps;
        }

        public Dictionary<string, float> GetFlockBorders()
        {
            return _flockBorderPoints;
        }

        private Dictionary<string, float> CountFlockBorders()
        {
            if (_flockContourVectors != null)
            {
                return new Dictionary<string, float>
                {
                    {"left", _leftMost.x},
                    {"right", _rightMost.x},
                    {"top", _topMost.y},
                    {"bottom", _bottomMost.y}
                };
            }

            var left = 20f;
            var bottom = 20f;
            var right = 20f;
            var top = -20f;

            foreach (var sheep in _sheeps)
            {
                var x = sheep.transform.position.x;
                var y = sheep.transform.position.y;

                left = Mathf.Min(left, x);
                right = Mathf.Max(right, x);
                top = Mathf.Max(top, y);
                bottom = Mathf.Min(bottom, y);
            }

            return new Dictionary<string, float>
            {
                {"left", left},
                {"right", right},
                {"top", top},
                {"bottom", bottom}
            };
        }

        public Dictionary<string, Vector3> GetFlockContour()
        {
            return _flockContourVectors;
        }

        private Dictionary<string, Vector3> CountFlockContour()
        {
            _leftMost = new Vector3(20f, 0);
            _bottomMost = new Vector3(0, 20f);
            _rightMost = new Vector3(-20f, 0);
            _topMost = new Vector3(0, -20f);

            foreach (var sheep in _sheeps)
            {
                var x = sheep.transform.position.x;
                var y = sheep.transform.position.y;

                if (x < _leftMost.x)
                {
                    _leftMost = sheep.transform.position;
                }
                if (x > _rightMost.x)
                {
                    _rightMost = sheep.transform.position;
                }
                if (y > _topMost.y)
                {
                    _topMost = sheep.transform.position;
                }
                if (y < _bottomMost.y)
                {
                    _bottomMost = sheep.transform.position;
                }
            }

            return new Dictionary<string, Vector3>
            {
                {"left", _leftMost},
                {"right", _rightMost},
                {"top", _topMost},
                {"bottom", _bottomMost}
            };
        }
    }
}
