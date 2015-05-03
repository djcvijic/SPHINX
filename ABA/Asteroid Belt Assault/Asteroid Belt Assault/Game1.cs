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

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        HUD hud;
        Texture2D spriteSheet;
        Texture2D menuSheet;
        MenuButtons menuButtons;
        Sprite gameOver;
        StarField starField;
        Ship ship1, ship2;
        Asteroids asteroids;
        EnemyManager enemyManager;
        ExplosionManager explosionManager;
        CollisionManager collisionManager;
        Vector2 pauseLoc;

        int screenWidth;
        int screenHeight;
        int gameOverTimer;

        
        enum states { MENU, GAME, DESTROYED, PAUSE};
        private states state;

        public SpriteFont Font { get; set; }

        bool pauseKeyDown = false;

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
            screenWidth = this.Window.ClientBounds.Width;
            screenHeight = this.Window.ClientBounds.Height;

            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            hud = new HUD(screenWidth-200, 0);
            hud.Font = Content.Load<SpriteFont>("nasFont");
            pauseLoc = new Vector2(screenWidth/2-10, screenHeight/2);
            Font = Content.Load<SpriteFont>("nasFont");
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteSheet = Content.Load<Texture2D>(@"Textures\SpriteSheet");
            menuSheet = Content.Load<Texture2D>(@"Textures\AsterMenus");

            //stvaranje dugmica glavnog menija
            menuButtons = new MenuButtons(menuSheet);

            gameOver = new Sprite(new Vector2(300, 200),
                menuSheet,
                new Rectangle(0, 225, 210, 60),
                Vector2.Zero);

            // stvaranje objekta StarField
            starField = new StarField(
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height,
                200,
                new Vector2(0f, 30f),
                spriteSheet,
                new Rectangle(0, 450, 2, 2));

            // stvaranje objekta Ship
            ship1 = new Ship(
                spriteSheet,
                new Rectangle(0, 150, 50, 50),
                3,
                new Vector2((float)screenWidth / 2 - 25, screenHeight - 50),
                screenWidth,
                screenHeight, Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Space);

            ship2 = new Ship(
               spriteSheet,
               new Rectangle(0, 360, 50, 50),
               3,
               new Vector2((float)screenWidth / 2 - 55, screenHeight - 50),
               screenWidth,
               screenHeight, Keys.W, Keys.S, Keys.A, Keys.D, Keys.X);

            // stvaranje objekta Asteroids
            asteroids = new Asteroids(
                10, 
                spriteSheet, 
                new Rectangle(0, 0, 50, 50), 
                20, 
                screenWidth, 
                screenHeight);

            // stvaranje objekta EnemyManager
            enemyManager = new EnemyManager(
                spriteSheet,
                new Rectangle(0, 200, 50, 50),
                6,
                ship1,
                ship2, 
                new Rectangle(0, 0, screenWidth, screenHeight));

            //stvaranje objekta ExplosionManager
            explosionManager = new ExplosionManager(
                spriteSheet,
                new Rectangle(0, 100, 50, 50),
                3,
                new Rectangle(0, 450, 2, 2));

            //stvaranje objekta CollisionManager
            collisionManager = new CollisionManager(
                asteroids,
                ship1,
                ship2, 
                enemyManager,
                explosionManager, hud);

            //inicijalizuj SoundManager
            SoundManager.Initialize(Content);

            state = states.MENU;
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

        protected void checkPause()
        {
            bool pauseKeyDownThisFrame = Keyboard.GetState().IsKeyDown(Keys.P);
            if (!pauseKeyDown && pauseKeyDownThisFrame)
            {
                if (state!=states.PAUSE)
                    state = states.PAUSE;
                else
                    state = states.GAME;
            }
            pauseKeyDown = pauseKeyDownThisFrame;
        }
        

        protected override void Update(GameTime gameTime)
        {
                      
            starField.Update(gameTime);

           

        
                switch (state)
                {
                    case states.MENU:
                        {
                            IsMouseVisible = true;
                            menuButtons.Update(gameTime);
                            if (menuButtons.BS == ButtonStates.B1PR)
                            {
                                IsMouseVisible = false;
                                state = states.GAME;
                            }
                            if (menuButtons.BS == ButtonStates.B1PR)
                            {
                                
                            }
                            if (menuButtons.BS == ButtonStates.B3PR)
                            {
                                this.Exit();
                            }
                            break;
                        }
                    case states.GAME:
                        {
                            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                                state = states.MENU;

                            checkPause();

                            if (ship1.Destroyed == true && ship2.Destroyed == true)
                            {
                                state = states.DESTROYED;
                                gameOverTimer = 5000;
                                ship1.Destroyed = false;
                                ship2.Destroyed = false;
                            }

                            asteroids.Update(gameTime);
                            Vector2 initialShipLoc = new Vector2((float)screenWidth / 2 - 25, screenHeight - 50);
                            ship1.Update(gameTime);
                            ship2.Update(gameTime);
                            enemyManager.Update(gameTime);
                            collisionManager.CheckCollisions();
                            explosionManager.Update(gameTime);
                            break;
                        }
                    case states.DESTROYED:
                        {
                            gameOverTimer -= gameTime.ElapsedGameTime.Milliseconds;
                            if (gameOverTimer <= 0) { LoadContent(); }
                            break;
                        }
                    case states.PAUSE:
                        {
                            checkPause();
                            break;
                        }

                
            }
                base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            starField.Draw(spriteBatch);



                switch (state)
                {
                    case (states.MENU):
                        {
                            menuButtons.Draw(spriteBatch);
                            break;
                        }
                    case (states.GAME):
                        {
                            asteroids.Draw(spriteBatch);
                            ship1.Draw(spriteBatch);
                            ship2.Draw(spriteBatch);
                            enemyManager.Draw(spriteBatch);
                            explosionManager.Draw(spriteBatch);
                            hud.Draw(spriteBatch);
                            break;
                        }
                    case (states.DESTROYED):
                        {
                            gameOver.Draw(spriteBatch);
                            break;
                        }
                    case(states.PAUSE):
                    {
                        spriteBatch.DrawString(
                            Font,
                            "PAUSED",
                            pauseLoc,
                            Color.White);
                        break;
                    }
                }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
