using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace sphinxgame1
{
    class EnemyX : PlayerX
    {
        float velocityChangeTime = 0.05f, velocityChangeTimeTimer = 0.0f; // sekunde
        private Vector2 scrollVector;

        public EnemyX(Texture2D texture,
            Rectangle leftFrame,
            Rectangle rightFrame,
            Rectangle leftStandingFrame,
            Rectangle rightStandingFrame,
            int frameNumber,
            Vector2 initialLocation,
            int screenWidth,
            int screenHeight)
            : base(texture, leftFrame, rightFrame, leftStandingFrame, rightStandingFrame, frameNumber, initialLocation, screenWidth, screenHeight)
        {
            Velocity = new Vector2(30, 0);
            physObject.Mass = 20.0f;
            physObject.removeForce(0);
        }

        public Vector2 ScrollVector
        {
            set { scrollVector = value; }
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocityChangeTimeTimer += elapsed;

            Vector2 deltaVelocity = Vector2.Zero;

            if (velocityChangeTimeTimer > velocityChangeTime)
            {
                velocityChangeTimeTimer = 0.0f;

                System.Random rand = new System.Random();

                deltaVelocity = new Vector2((float)(rand.Next(-5, 5)), 0);
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

            physObject.Update(gameTime);

            Velocity += deltaVelocity;

            if (Velocity.X < 0) spriteState = SpriteStates.LEFT;
            else if (Velocity.X > 0) spriteState = SpriteStates.RIGHT;
            else spriteState = SpriteStates.STANDING;

            physObject.offsetLocation(elapsed * scrollVector);
        }
    }
}
