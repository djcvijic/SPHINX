using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace sphinxgame1
{
    class Circle
    {
        private Vector2 center;
        private float radius;
        public Circle(Vector2 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }
        public Vector2 Center
        {
            get { return center; }
            set { center = value; }
        }
        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }
    }
}
