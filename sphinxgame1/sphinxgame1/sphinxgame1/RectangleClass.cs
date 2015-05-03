using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace sphinxgame1
{
    class RectangleClass
    {
        public float X
        {
            get;
            set;
        }
        public float Y
        {
            get;
            set;
        }
        public float Width
        {
            get;
            set;
        }
        public float Height
        {
            get;
            set;
        }
        public Vector2 Location
        {
            get { return new Vector2(X, Y); }
            set { X = value.X; Y = value.Y; }
        }
        public Vector2 Dimensions
        {
            get { return new Vector2(Width, Height); }
            set { Width = value.X; Height = value.Y; }
        }
        public Vector2 Center
        {
            get { return Location + Dimensions / 2; }
            set { Location = value - Dimensions / 2; }
        }
        public RectangleClass(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
        }
        public RectangleClass(Vector2 location, Vector2 dimensions)
        {
            Location = location; Dimensions = dimensions;
        }
        public bool Intersects(RectangleClass otherRectangle)
        {
            Rectangle myRect = new Rectangle((int)X, (int)Y, (int)Width, (int)Height),
            otherRect = new Rectangle((int)otherRectangle.X, (int)otherRectangle.Y, (int)otherRectangle.Width, (int)otherRectangle.Height);
            return myRect.Intersects(otherRect);
        }
    }
}
