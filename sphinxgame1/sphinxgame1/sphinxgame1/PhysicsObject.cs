using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sphinxgame1
{
    enum CollisionTypes { NOCOLLIDE, BOUNCE, STATIC };

    class PhysicsObject
    {
        public static float Gravity = 2000;

        private float mass;
        private Vector2 acceleration;
        private List<Vector2> forceList;
        private List<Sprite> spriteList;
        private Sprite currentSprite;
        private List<Circle> collisionCircles;
        private List<RectangleClass> collisionRectangles;
        CollisionTypes myType;
        Vector2 location;
        Vector2 velocity;
        Vector2 prevLocation;

        public bool IsBoxColliding = false;
        public bool IsCircleColliding = false;

        public static Vector2 rectCenter(RectangleClass rect)
        {
            return rect.Location + rect.Dimensions / 2;
        }

        public PhysicsObject (Vector2 location,
            Vector2 velocity,
            float mass,
            CollisionTypes collisionType)
        {
            this.location = location;
            this.velocity = velocity;
            this.mass = mass;
            forceList = new List<Vector2>();
            spriteList = new List<Sprite>();
            myType = collisionType;
            collisionCircles = new List<Circle>();
            collisionRectangles = new List<RectangleClass>();
        }

        public int addSprite(Sprite newSprite)
        {
            if (currentSprite == null) currentSprite = newSprite;
            newSprite.Velocity = Vector2.Zero;
            spriteList.Add(newSprite);
            return spriteList.Count - 1;
        }

        public List<Sprite> SpriteList
        {
            get { return spriteList; }
            set { spriteList = value; }
        }

        public int addForce(Vector2 newForce)
        {
            forceList.Add(newForce);
            return forceList.Count - 1;
        }

        public void removeForce(int index)
        {
            forceList.RemoveAt(index);
        }

        public void addGravity()
        {
            forceList.Add(mass * Gravity * Vector2.UnitY);
        }

        public int addCircle(Vector2 newCenter, float newRadius)
        {
            collisionCircles.Add(new Circle(newCenter, newRadius));
            return collisionCircles.Count - 1;
        }

        public int addRectangle(RectangleClass newRectangle)
        {
            collisionRectangles.Add(newRectangle);
            return collisionRectangles.Count - 1;
        }

        public bool FreezeX
        {
            get;
            set;
        }

        public bool FreezeY
        {
            get;
            set;
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public Vector2 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector2 MaxVelocity
        {
            get;
            set;
        }

        public int CurrentSprite
        {
            set { currentSprite = spriteList.ElementAt(value); }
        }

        public CollisionTypes CollisionType
        {
            get { return myType; }
        }

        public List<RectangleClass> CollisionRectangles
        {
            get { return collisionRectangles; }
        }

        public List<Circle> CollisionCircles
        {
            get { return collisionCircles; }
        }

        public void offsetLocation(Vector2 offset)
        {
            location += offset;
            foreach (Sprite sprite in spriteList)
                sprite.Location += offset;
            foreach (RectangleClass rectangle in collisionRectangles)
                rectangle.Location += offset;
            foreach (Circle circle in collisionCircles)
                circle.Center += offset;
        }

        public void rollBackY()
        {
            offsetLocation((prevLocation - location) * Vector2.UnitY);
        }

        public Vector2 BoxCollision(PhysicsObject otherObject)
        {
            if (myType != CollisionTypes.NOCOLLIDE)
            {
                foreach (RectangleClass myRectangle in collisionRectangles)
                    foreach (RectangleClass otherRectangle in otherObject.collisionRectangles)
                        if (myRectangle.Intersects(otherRectangle))
                        {
                            return (rectCenter(myRectangle) + rectCenter(otherRectangle)) / 2;
                        }
            }
            return Vector2.Zero;
        }

        public void Bounce(PhysicsObject otherObject)
        {
            if (myType != CollisionTypes.NOCOLLIDE)
            {
                foreach (Circle myCircle in collisionCircles)
                    foreach (Circle otherCircle in otherObject.collisionCircles)
                        if ((myCircle.Radius + otherCircle.Radius) > Vector2.Distance(myCircle.Center, otherCircle.Center))
                        {
                            float totalMass = mass + otherObject.mass;
                            Vector2 cOfMass = (Velocity + otherObject.Velocity) / 2;
                            Vector2 normal1 = otherCircle.Center - myCircle.Center;
                            normal1.Normalize();
                            Vector2 normal2 = myCircle.Center - otherCircle.Center;
                            normal2.Normalize();

                            float myQuotient, otherQuotient;
                            if (totalMass == 0) myQuotient = otherQuotient = 1;
                            else
                            {
                                myQuotient = mass / totalMass;
                                otherQuotient = otherObject.mass / totalMass;
                            }

                            Velocity -= cOfMass * myQuotient;
                            Velocity = Vector2.Reflect(Velocity, normal1);
                            Velocity += cOfMass * myQuotient;
                            otherObject.Velocity -= cOfMass * otherQuotient;
                            otherObject.Velocity = Vector2.Reflect(otherObject.Velocity, normal2);
                            otherObject.Velocity += cOfMass * otherQuotient;

                            return;
                        }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            prevLocation = location;

            acceleration = Vector2.Zero;
            if (mass > 0) foreach (Vector2 force in forceList)
                {
                    acceleration += force / mass;
                }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity += elapsed * acceleration;

            if (velocity.X > MaxVelocity.X) velocity.X = MaxVelocity.X;
            if (velocity.Y > MaxVelocity.Y) velocity.Y = MaxVelocity.Y;

            Vector2 offset = elapsed * velocity;

            if (FreezeX) offset.X = 0;
            if (FreezeY) offset.Y = 0;

            offsetLocation(offset);

            if (currentSprite != null) currentSprite.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (currentSprite != null) currentSprite.Draw(spriteBatch);
        }
    }
}
