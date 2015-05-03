using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mario8
{
    class Background
    {
        private Sprite backgroundSprite;

        public Background(Texture2D texture)
        {
            backgroundSprite = new Sprite(
                Vector2.Zero,
                texture,
                new Rectangle(0, 0, 400, 300),
                Vector2.Zero);
        }

        public void Update(GameTime gameTime)
        {
            backgroundSprite.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            backgroundSprite.Draw(spriteBatch);
        }
    }
}
