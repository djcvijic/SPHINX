using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace sphinxgame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        enum GameStates { TITLE, MENU, GAME, PAUSE, GAMEOVER };
        GameStates gameState;
        Texture2D backgroundSheet, playerSheet, enemySheet;
        Texture2D mBackgroundOneSheet, mBackgroundTwoSheet, mBackgroundThreeSheet, mBackgroundFourSheet, mBackgroundFiveSheet;

        private int screenWidth;
        private int screenHeight;

        Sprite background;
        Player player;
        Enemy enemy;

        Sprite mBackgroundOne;
        Sprite mBackgroundTwo;
        Sprite mBackgroundThree;
        Sprite mBackgroundFour;
        Sprite mBackgroundFive;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            gameState = GameStates.TITLE;
            graphics.PreferredBackBufferHeight = screenHeight = 566;
            graphics.PreferredBackBufferWidth = screenWidth = 800;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundSheet = Content.Load<Texture2D>(@"Textures\game_background");
            mBackgroundOneSheet = Content.Load<Texture2D>(@"Textures\game_background");
            mBackgroundTwoSheet = Content.Load<Texture2D>(@"Textures\Background01");
            mBackgroundThreeSheet = Content.Load<Texture2D>(@"Textures\Background02");
            mBackgroundFourSheet = Content.Load<Texture2D>(@"Textures\Background03");
            mBackgroundFiveSheet = Content.Load<Texture2D>(@"Textures\Background04");

            playerSheet = Content.Load<Texture2D>(@"Textures\ContraSheet1");
            enemySheet = Content.Load<Texture2D>(@"Textures\enemies_sheet");

            background = new Sprite(backgroundSheet,
                Vector2.Zero,
                Vector2.Zero,
                new Rectangle(0, 0, 800, 566),
                new Vector2(800/2, 566 / 2));

            player = new Player(playerSheet,
                new Rectangle(19, 134, 22, 36),
                new Rectangle(239, 134, 22, 36),
                new Rectangle(110, 17, 25, 36),
                new Rectangle(144, 17, 25, 36),
                5,
                new Vector2(130, 360),
                screenWidth,
                screenHeight);

            enemy = new Enemy(enemySheet,
                new Rectangle(-2, 330, 33, 16),
                new Rectangle(242, 330, 34, 16),
                new Rectangle(109, 360, 21, 15),
                new Rectangle(141, 360, 21, 15),
                4,
                new Vector2(400, 360),
                screenWidth,
                screenHeight);


            mBackgroundOne = new Sprite(mBackgroundOneSheet,Vector2.Zero,Vector2.Zero,new Rectangle(0,0,800,566),new Vector2(400,283));
            mBackgroundOne.Scale = 1.0f;

            mBackgroundTwo = new Sprite(mBackgroundTwoSheet, new Vector2(mBackgroundOne.Location.X + mBackgroundOne.FrameWidth * mBackgroundOne.Scale, 0), Vector2.Zero, new Rectangle(0, 0, 800, 566), new Vector2(400, 283));
            mBackgroundTwo.Scale = 1.0f;

            mBackgroundThree = new Sprite(mBackgroundThreeSheet, new Vector2(mBackgroundTwo.Location.X + mBackgroundTwo.FrameWidth * mBackgroundTwo.Scale, 0), Vector2.Zero, new Rectangle(0, 0, 800, 566), new Vector2(400, 283));
            mBackgroundThree.Scale = 1.0f;

            mBackgroundFour = new Sprite(mBackgroundFourSheet, new Vector2(mBackgroundThree.Location.X + mBackgroundThree.FrameWidth * mBackgroundThree.Scale, 0), Vector2.Zero, new Rectangle(0, 0, 800, 566), new Vector2(400, 283));
            mBackgroundFour.Scale = 1.0f;

            mBackgroundFive = new Sprite(mBackgroundFiveSheet, new Vector2(mBackgroundFour.Location.X + mBackgroundFour.FrameWidth * mBackgroundFour.Scale, 0), Vector2.Zero, new Rectangle(0, 0, 800, 566), new Vector2(400, 283));
            mBackgroundFive.Scale = 1.0f;

           
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();
            
            // TODO: Add your update logic here
            int ukupno = mBackgroundOne.FrameWidth + mBackgroundTwo.FrameWidth + mBackgroundThree.FrameWidth + mBackgroundFour.FrameWidth + mBackgroundFive.FrameWidth;

            if (mBackgroundOne.Location.X < -mBackgroundOne.FrameWidth || mBackgroundOne.Location.X > 4 * mBackgroundOne.FrameWidth)
            {
                mBackgroundOne.Location = new Vector2((mBackgroundOne.Location.X + ukupno + mBackgroundOne.FrameWidth) % ukupno - mBackgroundOne.FrameWidth, mBackgroundOne.Location.Y);
            }

            if (mBackgroundTwo.Location.X < -mBackgroundTwo.FrameWidth || mBackgroundTwo.Location.X > 4 * mBackgroundTwo.FrameWidth)
            {
                mBackgroundTwo.Location = new Vector2((mBackgroundTwo.Location.X + ukupno + mBackgroundTwo.FrameWidth) % ukupno - mBackgroundTwo.FrameWidth, mBackgroundTwo.Location.Y);
            }

            if (mBackgroundThree.Location.X < -mBackgroundThree.FrameWidth || mBackgroundThree.Location.X > 4 * mBackgroundThree.FrameWidth)
            {
                mBackgroundThree.Location = new Vector2((mBackgroundThree.Location.X + ukupno + mBackgroundThree.FrameWidth) % ukupno - mBackgroundThree.FrameWidth, mBackgroundThree.Location.Y);
            }

            if (mBackgroundFour.Location.X < -mBackgroundFour.FrameWidth || mBackgroundFour.Location.X > 4 * mBackgroundFour.FrameWidth)
            {
                mBackgroundFour.Location = new Vector2((mBackgroundFour.Location.X + ukupno + mBackgroundFour.FrameWidth) % ukupno - mBackgroundFour.FrameWidth, mBackgroundFour.Location.Y);
            }

            if (mBackgroundFive.Location.X < -mBackgroundFive.FrameWidth || mBackgroundFive.Location.X > 4 * mBackgroundFive.FrameWidth)
            {
                mBackgroundFive.Location = new Vector2((mBackgroundFive.Location.X + ukupno + mBackgroundFive.FrameWidth) % ukupno - mBackgroundFive.FrameWidth, mBackgroundFive.Location.Y);
            }


            Vector2 scrollVector;

            player.Update(gameTime);

            if (player.Location.X < 200 && player.Velocity.X < 0)
            {
                scrollVector = -player.Velocity;
            }
            else if (player.Location.X > 600 && player.Velocity.X > 0)
            {
                scrollVector = -player.Velocity;
            }
            else scrollVector = Vector2.Zero;

            mBackgroundOne.Velocity = mBackgroundTwo.Velocity = mBackgroundThree.Velocity = mBackgroundFour.Velocity = mBackgroundFive.Velocity = scrollVector;

            mBackgroundOne.Update(gameTime);
            mBackgroundTwo.Update(gameTime);
            mBackgroundThree.Update(gameTime);
            mBackgroundFour.Update(gameTime);
            mBackgroundFive.Update(gameTime);

            enemy.Update(gameTime, scrollVector);
            
            background.Update(gameTime);
            

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            // background.Draw(spriteBatch);
            mBackgroundOne.Draw(this.spriteBatch);
            mBackgroundTwo.Draw(this.spriteBatch);
            mBackgroundThree.Draw(this.spriteBatch);
            mBackgroundFour.Draw(this.spriteBatch);
            mBackgroundFive.Draw(this.spriteBatch);
            player.Draw(spriteBatch);
            enemy.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
