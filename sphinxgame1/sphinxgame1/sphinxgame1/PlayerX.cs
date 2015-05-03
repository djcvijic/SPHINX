using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sphinxgame1
{
    class PlayerX
    {
        protected PhysicsObject physObject;
        protected enum SpriteStates { LEFT, RIGHT, STANDING };
        protected SpriteStates spriteState = SpriteStates.STANDING;
        protected SpriteStates oldState = SpriteStates.RIGHT;

        public PhysicsObject PhysObject
        {
            get { return physObject; }
        }

        public Vector2 Velocity
        {
            get { return physObject.Velocity; }
            set { physObject.Velocity = value; }
        }

        public Vector2 Location
        {
            get { return physObject.Location; }
            set { physObject.Location = value; }
        }

        public PlayerX(Texture2D texture,
            Rectangle leftFrame,
            Rectangle rightFrame,
            Rectangle leftStandingFrame,
            Rectangle rightStandingFrame,
            int frameNumber,
            Vector2 initialLocation,
            int screenWidth,
            int screenHeight)
        {
            Sprite rightRunningSprite = new Sprite(texture,
                initialLocation,
                Vector2.Zero,
                rightFrame,
                new Vector2(rightFrame.Width / 2, rightFrame.Height / 2));
            Sprite leftRunningSprite = new Sprite(texture,
                initialLocation,
                Vector2.Zero,
                leftFrame,
                new Vector2(leftFrame.Width / 2, leftFrame.Height / 2));
            Sprite leftStandingSprite = new Sprite(texture,
                 initialLocation,
                 Vector2.Zero,
                 leftStandingFrame,
                new Vector2(leftStandingFrame.Width / 2, leftStandingFrame.Height / 2));
            Sprite rightStandingSprite = new Sprite(texture,
                initialLocation,
                Vector2.Zero,
                rightStandingFrame,
                new Vector2(rightStandingFrame.Width / 2, rightStandingFrame.Height / 2));

            leftRunningSprite.Scale = rightRunningSprite.Scale = leftStandingSprite.Scale = rightStandingSprite.Scale = 2.0f;
            leftRunningSprite.FrameTime = rightRunningSprite.FrameTime = leftStandingSprite.FrameTime = rightStandingSprite.FrameTime = 0.1f;

            for (int i = 1; i < frameNumber; i++)
            {
                leftRunningSprite.addFrame(new Rectangle(leftFrame.X + i * (leftFrame.Width + 2),
                     leftFrame.Y,
                     leftFrame.Width,
                     leftFrame.Height));
                rightRunningSprite.addFrame(new Rectangle(rightFrame.X - i * (rightFrame.Width + 2),
                     rightFrame.Y,
                     rightFrame.Width,
                     rightFrame.Height));
            }

            physObject = new PhysicsObject(initialLocation,
                Vector2.Zero,
                85.0f,
                CollisionTypes.BOUNCE);
            physObject.addSprite(rightRunningSprite);
            physObject.addSprite(leftRunningSprite);
            physObject.addSprite(rightStandingSprite);
            physObject.addSprite(leftStandingSprite);
            physObject.addGravity();
            physObject.MaxVelocity = 2000 * Vector2.One;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            physObject.Draw(spriteBatch);
        }

        public virtual void Update(GameTime gameTime)
        {
            bool leftPressed = Keyboard.GetState().IsKeyDown(Keys.Left);
            bool rightPressed = Keyboard.GetState().IsKeyDown(Keys.Right);
            bool upPressed = Keyboard.GetState().IsKeyDown(Keys.Up);
            bool downPressed = Keyboard.GetState().IsKeyDown(Keys.Down);

            if (leftPressed && !rightPressed)
            {
                oldState = spriteState = SpriteStates.LEFT;
                Velocity = new Vector2(-400, Velocity.Y);
            }

            else if (rightPressed && !leftPressed)
            {
                oldState = spriteState = SpriteStates.RIGHT;
                Velocity = new Vector2(400, Velocity.Y);
            }
            else
            {
                spriteState = SpriteStates.STANDING;
                Velocity = Vector2.UnitY * Velocity.Y;
            }

            //bool isColliding = false;
            //foreach (PhysicsObject physObj in CollisionManager.ObjectList)
            //    if ((physObj != physObject) && (physObject.isBoxColliding(physObj).Length() > 0))
            //        isColliding = true;

            if (upPressed && !downPressed && physObject.IsBoxColliding)
            {
                Velocity = new Vector2(Velocity.X, - 750);
            }

            if (spriteState == SpriteStates.LEFT)
            {
                physObject.CurrentSprite = 1;
            }
            else if (spriteState == SpriteStates.RIGHT)
            {
                physObject.CurrentSprite = 0;
            }
            else if (oldState == SpriteStates.LEFT)
            {
                physObject.CurrentSprite = 3;
            }
            else
            {
                physObject.CurrentSprite = 2;
            }

            if ((Velocity.X < 0 && Location.X < 200) || (Velocity.X > 0 && Location.X > 600))
                physObject.FreezeX = true;
            else physObject.FreezeX = false;

            physObject.Update(gameTime);
        }
    }
}
