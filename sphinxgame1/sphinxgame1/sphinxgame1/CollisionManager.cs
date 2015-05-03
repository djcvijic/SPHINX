using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace sphinxgame1
{
    class CollisionManager
    {
        public static List<PhysicsObject> ObjectList;

        public CollisionManager()
        {
            ObjectList = new List<PhysicsObject>();
        }

        public int addObject(PhysicsObject newObject)
        {
            ObjectList.Add(newObject);
            return ObjectList.Count - 1;
        }

        public PhysicsObject removeObject(int index)
        {
            PhysicsObject oldObject = ObjectList.ElementAt(index);
            ObjectList.RemoveAt(index);
            return oldObject;
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            PhysicsObject iObject, jObject;
            for (int i = 0; i < ObjectList.Count; i++)
            {
                iObject = ObjectList.ElementAt(i);
                if (iObject.CollisionType != CollisionTypes.NOCOLLIDE)
                {
                    for (int j = i + 1; j < ObjectList.Count; j++)
                    {
                        jObject = ObjectList.ElementAt(j);
                        if ((jObject.CollisionType != CollisionTypes.NOCOLLIDE) &&
                            !(iObject.CollisionType == CollisionTypes.STATIC && jObject.CollisionType == CollisionTypes.STATIC))
                        {
                            if (iObject.CollisionType == CollisionTypes.BOUNCE && jObject.CollisionType == CollisionTypes.BOUNCE)
                            {
                                iObject.Bounce(jObject);
                            } else
                            {
                                iObject.IsBoxColliding = false;
                                jObject.IsBoxColliding = false;

                                Vector2 collisionPoint = iObject.BoxCollision(jObject);
                                if (collisionPoint.Length() != 0)
                                {
                                    iObject.IsBoxColliding = true;
                                    jObject.IsBoxColliding = true;

                                    PhysicsObject bouncingObject;

                                    if (iObject.CollisionType == CollisionTypes.BOUNCE)
                                        bouncingObject = iObject;
                                    else
                                        bouncingObject = jObject;

                                    if (bouncingObject.Velocity.Y > 0)
                                    {
                                        bouncingObject.Velocity = new Vector2(bouncingObject.Velocity.X, 0);
                                        bouncingObject.rollBackY();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
