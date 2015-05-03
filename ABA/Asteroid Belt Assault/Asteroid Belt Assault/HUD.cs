using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid_Belt_Assault
{
    public class HUD
    {
        private Vector2 pozicija;

        public SpriteFont Font { get; set; }

        public int Score { get; set; }

        public HUD(int x, int y)
        {
            pozicija = new Vector2(x, y);
             Score = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the Score in the top-left of screen
            spriteBatch.DrawString(
                Font,                          // SpriteFont
                "Score: " + Score.ToString(),  // Text
                pozicija,                      // Position
                Color.White);                  // Tint
        }
    }
}
