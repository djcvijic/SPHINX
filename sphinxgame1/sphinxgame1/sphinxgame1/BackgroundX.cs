using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sphinxgame1
{
    class BackgroundX
    {
        private int width;
        private int height;
        private PhysicsObject physObject;
        private Vector2 scrollVector;

        public PhysicsObject PhysObject
        {
            get { return physObject; }
        }

        public BackgroundX(int screenWidth, int screenHeight, List<Texture2D> textureList)
        {
            width = screenWidth;
            height = screenHeight;
            physObject = new PhysicsObject(Vector2.Zero,
                Vector2.Zero,
                0.0f,
                CollisionTypes.STATIC);
            int i = 0;

            foreach (Texture2D texture in textureList)
            {
                Sprite newSprite = new Sprite(
                    texture,
                    i * new Vector2(screenWidth, 0),
                    Vector2.Zero,
                    new Rectangle(0, 0, screenWidth, screenHeight),
                    new Vector2(screenWidth / 2, screenHeight / 2)
                    );
                physObject.addSprite(newSprite);
                physObject.addRectangle(new RectangleClass(
                    i * screenWidth,
                    400,
                    screenWidth,
                    10));
                i++;
            }
        }

        public Vector2 ScrollVector
        {
            set { scrollVector = value; }
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            int ukupno = 0;
            foreach (Sprite singleSprite in physObject.SpriteList) ukupno += singleSprite.FrameWidth;

            foreach (Sprite singleSprite in physObject.SpriteList)
            {
                if (singleSprite.Location.X < -singleSprite.FrameWidth || singleSprite.Location.X > (physObject.SpriteList.Count - 1) * singleSprite.FrameWidth)
                {
                    singleSprite.Location = new Vector2((singleSprite.Location.X + ukupno + singleSprite.FrameWidth) % ukupno - singleSprite.FrameWidth, singleSprite.Location.Y);
                }
            }
            /*for (int i = 0; i < physObject.CollisionRectangles.Count; i++)
            {
                RectangleClass rect = physObject.CollisionRectangles.ElementAt(i);
                if (rect.Location.X < -width || rect.Location.X > (physObject.CollisionRectangles.Count - 1) * width)
                {
                    Vector2 locVect = new Vector2((rect.Location.X + ukupno + width) % ukupno - width, rect.Location.Y);
                    rect.Location = new Point((int)locVect.X, (int)locVect.Y);
                    physObject.CollisionRectangles.RemoveAt(i);
                    physObject.CollisionRectangles.Insert(i, rect);
                }
            }*/
            foreach (RectangleClass rect in physObject.CollisionRectangles)
            {
                if (rect.Location.X < -width || rect.Location.X > (physObject.CollisionRectangles.Count - 1) * width)
                {
                    rect.Location = new Vector2((rect.Location.X + ukupno + width) % ukupno - width, rect.Location.Y);
                }
            }
            foreach (Circle circ in physObject.CollisionCircles)
            {
                if (circ.Center.X < -width || circ.Center.X > (physObject.CollisionCircles.Count - 1) * width)
                {
                    circ.Center = new Vector2((circ.Center.X + ukupno + width) % ukupno - width, circ.Center.Y);
                }
            }
            
            physObject.Update(gameTime);
            physObject.offsetLocation(elapsed * scrollVector);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite singleSprite in physObject.SpriteList) singleSprite.Draw(spriteBatch);
        }
    }
}
