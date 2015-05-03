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

        private int screenWidth;
        private int screenHeight;

        Player player;
        Enemy enemy;
        Background background;

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
            screenHeight = graphics.PreferredBackBufferHeight = 566;
            screenWidth = graphics.PreferredBackBufferWidth = 800;

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


            // TODO: use this.Content to load your game content here
            List<Texture2D> bgTextures = new List<Texture2D>();

            bgTextures.Add(Content.Load<Texture2D>(@"Textures\game_background"));
            bgTextures.Add(Content.Load<Texture2D>(@"Textures\Background01"));
            bgTextures.Add(Content.Load<Texture2D>(@"Textures\Background02"));
            bgTextures.Add(Content.Load<Texture2D>(@"Textures\Background03"));
            bgTextures.Add(Content.Load<Texture2D>(@"Textures\Background04"));
            bgTextures.Add(Content.Load<Texture2D>(@"Textures\Background05"));

            background = new Background(screenWidth, screenHeight, bgTextures);

            Texture2D playerSheet = Content.Load<Texture2D>(@"Textures\ContraSheet1"),
                enemySheet = Content.Load<Texture2D>(@"Textures\enemies_sheet");

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
            player.Update(gameTime);

            Vector2 scrollVector;

            if (player.Location.X < 200 && player.Velocity.X < 0)
            {
                scrollVector = -player.Velocity;
            }
            else if (player.Location.X > 600 && player.Velocity.X > 0)
            {
                scrollVector = -player.Velocity;
            }
            else scrollVector = Vector2.Zero;

            background.ScrollVector = enemy.ScrollVector = scrollVector;

            background.Update(gameTime);

            enemy.Update(gameTime);

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
            background.Draw(spriteBatch);
            player.Draw(spriteBatch);
            enemy.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
