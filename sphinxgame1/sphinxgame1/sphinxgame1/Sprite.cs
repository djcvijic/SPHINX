using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sphinxgame1
{
    class Sprite
    {
        private Texture2D texture;
        private Vector2 location;
        private Vector2 velocity;
        private int currentFrame = 0;
        private List<Rectangle> frames = new List<Rectangle>();
        private float frameTime = 0.1f;
        private float timeForCurrentFrame = 0.0f;
        private int frameWidth;
        private int frameHeight;
        private float scale = 1.0f;

        public Sprite(Texture2D texture,
            Vector2 location,
            Vector2 velocity,
            Rectangle initialFrame)
        {
            this.texture = texture;
            this.location = location;
            this.velocity = velocity;
            frames.Add(initialFrame);
            frameWidth = initialFrame.Width;
            frameHeight = initialFrame.Height;
        }

        public void addFrame(Rectangle newFrame)
        {
            frames.Add(newFrame);
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        public float FrameTime
        {
            get { return frameTime; }
            set { frameTime = value; }
        }

        public int FrameWidth
        {
            get { return frameWidth; }
            set { frameWidth = value; }
        }

        public int FrameHeight
        {
            get { return frameHeight; }
            set { frameHeight = value; }
        }

        public Vector2 Center
        {
            get { return location + new Vector2(frameWidth / 2, frameHeight / 2); }
        }

        public Rectangle Source
        {
            get { return frames[currentFrame]; }
        }

        public Rectangle Destination
        {
            get
            {
                return new Rectangle((int)location.X,
                    (int)location.Y,
                    frameWidth,
                    frameHeight);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeForCurrentFrame += elapsed;
            if (timeForCurrentFrame >= frameTime)
            {
                currentFrame = (currentFrame + 1) % frames.Count;
                timeForCurrentFrame = 0.0f;
            }
            location += (velocity * elapsed);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                Center,
                Source,
                Color.White,
                0.0f,
                new Vector2(frameWidth / 2, frameHeight / 2),
                scale,
                SpriteEffects.None,
                0.0f);
        }
    }
}
