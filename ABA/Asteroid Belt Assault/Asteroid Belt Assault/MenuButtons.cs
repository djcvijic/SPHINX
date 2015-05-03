using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroid_Belt_Assault
{
    public enum ButtonStates { DEFAULT, B1PR, B2PR, B3PR };

    class MenuButtons
    {
        private Sprite button1;
        private Sprite button2;
        private Sprite button3;
        
        private ButtonStates bs;
        private int offset = 250;

        private Rectangle mouseRect; //predstavlja pravougaonik 1x1 oko pozicije mouse pointera

        public ButtonStates BS
        {
            get { return bs; }
            set { bs = value; }
        }

        public MenuButtons(Texture2D texture)
        {
            bs = ButtonStates.DEFAULT;
            mouseRect = new Rectangle(1, 1, 1, 1);

            button1 = new Sprite(new Vector2(50 + offset, 50),
                texture,
                new Rectangle(0, 0, 210, 60),
                Vector2.Zero);
            button1.AddFrame(new Rectangle(300, 0, 210, 60));
            button1.FrameTime = 0;

            button2 = new Sprite(new Vector2(50 + offset, 150),
                 texture,
                 new Rectangle(0, 69, 210, 60),
                 Vector2.Zero);
            button2.AddFrame(new Rectangle(300, 69, 210, 60));
            button2.FrameTime = 0;

            button3 = new Sprite(new Vector2(50 + offset, 250),
                texture,
                new Rectangle(0, 146, 210, 60),
                Vector2.Zero);
            button3.AddFrame(new Rectangle(300, 146, 210, 60));
            button3.FrameTime = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            button1.Draw(spriteBatch);
            button2.Draw(spriteBatch);
            button3.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            mouseRect.X = mouseState.X;
            mouseRect.Y = mouseState.Y;

            //sledeca tri uslova sluze samo da menjaju izgled dugmad ako je mis preko nekog od njih
            if (button1.IsBoxColliding(mouseRect))
            {
                button1.Frame = 1;
            }
            else button1.Frame = 0;

            if (button2.IsBoxColliding(mouseRect))
            {
                button2.Frame = 1;
            }
            else button2.Frame = 0;

            if (button3.IsBoxColliding(mouseRect))
            {
                button3.Frame = 1;
            }
            else button3.Frame = 0;

            //ovaj uslov samo menja stanja dugmeta ako je pritisnuto, ili vraca na difolt ako nista nije pritisnuto
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (button1.IsBoxColliding(mouseRect))
                {
                    bs = ButtonStates.B1PR;
                }

                else if (button2.IsBoxColliding(mouseRect))
                {
                    bs = ButtonStates.B2PR;
                }

                else if (button3.IsBoxColliding(mouseRect))
                {
                    bs = ButtonStates.B3PR;
                }
            }
            else
            {
                bs = ButtonStates.DEFAULT;
            }
        }
    }
}
