using UnityEngine;
using System.Collections.Generic;
namespace BuilderPattern
{
    public class SlimeProduct : MonoBehaviour
    {

        public int[] SlimeShape { get; set; }

        public float width { get; set; }
        public int points { get; set; }

        public Vector3[] vertices { get; set; }

        public List<GameObject> particles { get; set; }
        public SlimeProduct()
        {

        }
    }
}
