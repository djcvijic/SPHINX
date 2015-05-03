using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sphinxgame1
{
    class Player
    {
        private Sprite leftRunningSprite;
        private Sprite rightRunningSprite;
        private Sprite leftStandingSprite;
        private Sprite rightStandingSprite;
        private enum SpriteStates { LEFT, RIGHT, STANDING };
        SpriteStates spriteState = SpriteStates.STANDING;
        SpriteStates oldState = SpriteStates.RIGHT;
        private Rectangle areaLimit;
        private Vector2 velocity;
        private Vector2 location;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        public Player(Texture2D texture,
            Rectangle leftFrame,
            Rectangle rightFrame,
            Rectangle leftStandingFrame,
            Rectangle rightStandingFrame,
            int frameNumber,
            Vector2 initialLocation,
            int screenWidth,
            int screenHeight)
        {
            rightRunningSprite = new Sprite(texture,
                initialLocation,
                Vector2.Zero,
                rightFrame,
                new Vector2(rightFrame.Width / 2, rightFrame.Height / 2));
            leftRunningSprite = new Sprite(texture,
                initialLocation,
                Vector2.Zero,
                leftFrame,
                new Vector2(leftFrame.Width / 2, leftFrame.Height / 2));
            leftStandingSprite = new Sprite(texture,
                 initialLocation,
                 Vector2.Zero,
                 leftStandingFrame,
                new Vector2(leftStandingFrame.Width / 2, leftStandingFrame.Height / 2));
            rightStandingSprite = new Sprite(texture,
                initialLocation,
                Vector2.Zero,
                rightStandingFrame,
                new Vector2(rightStandingFrame.Width / 2, rightStandingFrame.Height / 2));

            leftRunningSprite.Scale = rightRunningSprite.Scale = leftStandingSprite.Scale = rightStandingSprite.Scale = 2.0f;
            leftRunningSprite.FrameTime = rightRunningSprite.FrameTime = leftStandingSprite.FrameTime = rightStandingSprite.FrameTime = 0.1f;

            areaLimit = new Rectangle(0, 0, screenWidth, 390);

            for (int i = 1; i < frameNumber; i++)
            {
                leftRunningSprite.addFrame(new Rectangle(leftFrame.X + i * 24,
                     leftFrame.Y,
                     leftFrame.Width,
                     leftFrame.Height));
                rightRunningSprite.addFrame(new Rectangle(rightFrame.X - i * 24,
                     rightFrame.Y,
                     rightFrame.Width,
                     rightFrame.Height));
            }

            location = initialLocation;
            velocity = Vector2.Zero;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (spriteState == SpriteStates.LEFT) leftRunningSprite.Draw(spriteBatch);
            else if (spriteState == SpriteStates.RIGHT) rightRunningSprite.Draw(spriteBatch);
            else if (oldState == SpriteStates.LEFT) leftStandingSprite.Draw(spriteBatch);
            else rightStandingSprite.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            bool leftPressed = Keyboard.GetState().IsKeyDown(Keys.Left);
            bool rightPressed = Keyboard.GetState().IsKeyDown(Keys.Right);

            if (leftPressed && !rightPressed)
            {
                oldState = spriteState = SpriteStates.LEFT;
                velocity = new Vector2(-400, 0);
            }

            else if (rightPressed && !leftPressed)
            {
                oldState = spriteState = SpriteStates.RIGHT;
                velocity = new Vector2(400, 0);
            }
            else
            {
                spriteState = SpriteStates.STANDING;
                velocity = Vector2.Zero;
            }

            if ((!(velocity.X < 0 && location.X < 200)) && (!(velocity.X > 0 && location.X > 600)))
                location += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;


            if (spriteState == SpriteStates.LEFT)
            {
                leftRunningSprite.Location = location;
                leftRunningSprite.Update(gameTime);
            }
            else if (spriteState == SpriteStates.RIGHT)
            {
                rightRunningSprite.Location = location;
                rightRunningSprite.Update(gameTime);
            }
            else
            {
                rightStandingSprite.Location = leftStandingSprite.Location = location;
            }

        }
    }
}
