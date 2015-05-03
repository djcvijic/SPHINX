using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace sphinxgame1
{
    
 

    class Enemy : Player
    {

        float velocityChangeTime = 0.05f, velocityChangeTimeTimer = 0.0f; // sekunde

        public Enemy(Texture2D texture,
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
            Velocity = new Vector2(30,0);
        }

        public virtual void Update(GameTime gameTime, Vector2 scrollVector)
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

            if (Velocity.X < 0) spriteState = SpriteStates.LEFT;
            else if (Velocity.X > 0) spriteState = SpriteStates.RIGHT;
            else spriteState = SpriteStates.STANDING;

            Velocity += deltaVelocity;

            Location += (Velocity + scrollVector) * elapsed;


            if (spriteState == SpriteStates.LEFT)
            {
                leftRunningSprite.Location = Location;
                leftRunningSprite.Update(gameTime);
            }
            else if (spriteState == SpriteStates.RIGHT)
            {
                rightRunningSprite.Location = Location;
                rightRunningSprite.Update(gameTime);
            }
            else
            {
                rightStandingSprite.Location = leftStandingSprite.Location = Location;
            }
        }

        /*public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (spriteState == SpriteStates.LEFT) leftRunningSprite.Draw(spriteBatch);
            else if (spriteState == SpriteStates.RIGHT) rightRunningSprite.Draw(spriteBatch);
            else rightStandingSprite.Draw(spriteBatch);
        }*/
    }
}
