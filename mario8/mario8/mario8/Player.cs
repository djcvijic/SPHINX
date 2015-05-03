using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mario8
{
    class Player
    {
        private Sprite playerSprite;
        private Rectangle areaLimit;

        public Player(Texture2D texture, int screenWidth, int screenHeight)
        {
            playerSprite = new Sprite(
                new Vector2(50,100),
                texture,
                new Rectangle(21,334,29,48),
                Vector2.Zero);
            playerSprite.Acceleration = new Vector2(0, 0.5f);
            areaLimit = new Rectangle(0, 0, screenWidth, (int) (screenHeight * 0.8f));
        }

        public void Update(GameTime gameTime)
        {
            playerSprite.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                playerSprite.Velocity = new Vector2(-4,playerSprite.Velocity.Y);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                playerSprite.Velocity = new Vector2(4, playerSprite.Velocity.Y);
            }
            else
            {
                playerSprite.Velocity = new Vector2(0, playerSprite.Velocity.Y);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && (playerSprite.Location.Y > 0.75 * areaLimit.Height))
            {
                playerSprite.Velocity = new Vector2(playerSprite.Velocity.X, -10);
            }

            playerSprite.Velocity += playerSprite.Acceleration;
            playerSprite.Location += playerSprite.Velocity;
            // spreèavamo brod da izaðe izvan dozvoljenog pravouganika areaLimit
            Vector2 location = playerSprite.Location;

            /*if (location.X < areaLimit.X)
                location.X = areaLimit.X;

            if (location.X + playerSprite.Destination.Width > areaLimit.Width)
                location.X = areaLimit.Width - playerSprite.Destination.Width;

            if (location.Y < areaLimit.Y)
                location.Y = areaLimit.Y;

            if (location.Y + playerSprite.Destination.Height > areaLimit.Y + areaLimit.Height)
                location.Y = areaLimit.Y + areaLimit.Height - playerSprite.Destination.Height;*/

            playerSprite.Location = location;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            playerSprite.Draw(spriteBatch);
        }
    }
}
