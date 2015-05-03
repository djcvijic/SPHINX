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
        Vector2 scrollVector;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        enum GameStates { TITLE, MENU, GAME, PAUSE, GAMEOVER };
        GameStates gameState;
        SpriteFont font;

        private int screenWidth;
        private int screenHeight;

        PlayerX player;
        EnemyX enemy;
        BackgroundX background;
        CollisionManager collisionManager;

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
            font = Content.Load<SpriteFont>("Arial");

            List<Texture2D> bgTextures = new List<Texture2D>();

            bgTextures.Add(Content.Load<Texture2D>(@"Textures\game_background"));
            bgTextures.Add(Content.Load<Texture2D>(@"Textures\Background01"));
            bgTextures.Add(Content.Load<Texture2D>(@"Textures\Background02"));
            bgTextures.Add(Content.Load<Texture2D>(@"Textures\Background03"));
            bgTextures.Add(Content.Load<Texture2D>(@"Textures\Background04"));
            bgTextures.Add(Content.Load<Texture2D>(@"Textures\Background05"));

            background = new BackgroundX(screenWidth, screenHeight, bgTextures);

            Texture2D playerSheet = Content.Load<Texture2D>(@"Textures\ContraSheet1"),
                enemySheet = Content.Load<Texture2D>(@"Textures\enemies_sheet");

            player = new PlayerX(playerSheet,
                new Rectangle(19, 134, 22, 36),
                new Rectangle(239, 134, 22, 36),
                new Rectangle(110, 17, 25, 36),
                new Rectangle(144, 17, 25, 36),
                5,
                new Vector2(130, 360),
                screenWidth,
                screenHeight);
            player.PhysObject.addRectangle(new RectangleClass(player.Location.X, player.Location.Y, 22, 36));
            player.PhysObject.addCircle(player.Location + new Vector2(11, 18), 11);

            enemy = new EnemyX(enemySheet,
                new Rectangle(-2, 330, 33, 16),
                new Rectangle(242, 330, 34, 16),
                new Rectangle(109, 360, 21, 15),
                new Rectangle(141, 360, 21, 15),
                4,
                new Vector2(400, 360),
                screenWidth,
                screenHeight);
            enemy.PhysObject.addRectangle(new RectangleClass(enemy.Location.X, enemy.Location.Y, 21, 15));
            enemy.PhysObject.addCircle(enemy.Location + new Vector2(17, 8), 8);

            collisionManager = new CollisionManager();
            collisionManager.addObject(background.PhysObject);
            collisionManager.addObject(player.PhysObject);
            collisionManager.addObject(enemy.PhysObject);
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

            //Vector2 scrollVector;
            scrollVector = Vector2.Zero;

            if (player.Location.X < 200 && player.Velocity.X < 0)
            {
                scrollVector.X = -player.Velocity.X;
            }
            else if (player.Location.X > 600 && player.Velocity.X > 0)
            {
                scrollVector.X = -player.Velocity.X;
            }
            else scrollVector.X = 0;

            background.ScrollVector = enemy.ScrollVector = scrollVector;

            background.Update(gameTime);

            enemy.Update(gameTime);

            collisionManager.Update(gameTime);

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
            /*spriteBatch.DrawString(font,
                "Player location: " + player.Location +
                "\nPlayer rectangle location: " + player.PhysObject.CollisionRectangles.ElementAt(0).Location +
                "\nPlayer sprite location: " + player.PhysObject.SpriteList.ElementAt(0).Location +
                "\nPlayer velocity: " + player.Velocity +
                "\nBackground location: " + background.PhysObject.Location +
                "\nRectangle location: " + background.PhysObject.CollisionRectangles.ElementAt(0).Location + 
                "\nSprite location: " + background.PhysObject.SpriteList.ElementAt(0).Location,
                Vector2.Zero,
                Color.Red);*/
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
