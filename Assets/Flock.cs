using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class Flock
    {
        private readonly List<GameObject> _sheeps;

        public Flock()
        {
            _sheeps = new List<GameObject>();
        }

        public void AddSheep(GameObject sheep)
        {
            _sheeps.Add(sheep);
        }

        public List<GameObject> GetSheeps()
        {
            return _sheeps;
        }
    }
}
