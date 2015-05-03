using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sphinxgame1
{
    class Background
    {

        private int width;
        private int height;
        private List<Sprite> spriteList;
        private Vector2 scrollVector;

        public Background(int screenWidth, int screenHeight, List<Texture2D> textureList)
        {
            width = screenWidth;
            height = screenHeight;
            spriteList = new List<Sprite>();
            int i = 0;

            foreach (Texture2D texture in textureList)
            {
                spriteList.Add(
                    new Sprite(
                        texture,
                        i * new Vector2(screenWidth, 0),
                        Vector2.Zero,
                        new Rectangle(0, 0, screenWidth, screenHeight),
                        new Vector2(screenWidth / 2, screenHeight / 2)
                        )
                    );
                i++;
            }
        }

        public Vector2 ScrollVector
        {
            set { scrollVector = value; }
        }

        public void Update(GameTime gameTime)
        {
            int ukupno = 0;
            foreach (Sprite sprite in spriteList) ukupno += sprite.FrameWidth;

            foreach (Sprite sprite in spriteList)
            {
                if (sprite.Location.X < -sprite.FrameWidth || sprite.Location.X > (spriteList.Count - 1) * sprite.FrameWidth)
                {
                    sprite.Location = new Vector2((sprite.Location.X + ukupno + sprite.FrameWidth) % ukupno - sprite.FrameWidth, sprite.Location.Y);
                }

                sprite.Velocity = scrollVector;
                sprite.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite sprite in spriteList) sprite.Draw(spriteBatch);
        }
    }
}
