using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // mora da se doda ručno
using Microsoft.Xna.Framework.Graphics; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// Klasa koja modeluje asteroide koji mogu da se sudare sa igracem.
    /// </summary>
    public class Asteroids
    {

        #region Privatni atributi klase

        /// <summary>
        /// podrazumevana visina i sirina prozora u okviru koga ce se asteoridi kretati
        /// 
        /// posto u konstruktoru postavljamo nove vrednosti ova dva parametra njihova
        /// pocetna vrednost nije od znacaja
        /// </summary>
        private int screenWidth = 800;
        private int screenHeight = 600;
        /// <summary>
        /// atribut koji ce nam omoguciti kasnje prosirivanje prvobitnog prozora
        /// </summary>
        private int screenPadding = 10;

        /// <summary>
        /// pocetni frame - frame od koga pocinje animacija
        /// </summary>
        private Rectangle initialFrame;

        /// <summary>
        /// broj frame-ova koji ucestvuju u animaciji
        /// </summary>
        private int frameNumber;


        /// <summary>
        /// texutra za sprite-ove
        /// </summary>
        private Texture2D texture;



        /// <summary>
        /// lista svih asteroida - svaki asteroid je sprite
        /// </summary>
        public List<Sprite> asteroids = new List<Sprite>();


        /// <summary>
        /// atributi za minimalnu i maksimalnu brzinu kretanja asteroida
        /// </summary>
        private int minSpeed = 60;
        private int maxSpeed = 120;


        /// <summary>
        /// atribut za nasumicno odabrane vrednosti koje se koriste pri kreiranju novih asteroida
        /// </summary>
        private Random rand = new Random(); 

        #endregion

        /// <summary>
        /// Konstruktor klase Asteroids<see cref="Asteroids"/>
        /// </summary>
        /// <param name="asteroidCount">Broj asteroida koji ce biti kreirani u igrici</param>
        /// <param name="texture">Tekstura za sprite-ove.</param>
        /// <param name="initialFrame">Pocetni frame</param>
        /// <param name="asteroidFrames">Broj frame-ova.</param>
        /// <param name="screenWidth">Sirina prozora igrice.</param>
        /// <param name="screenHeight">Visina prozora igrice.</param>
        public Asteroids(
            int asteroidCount,
            Texture2D texture,
            Rectangle initialFrame,
            int asteroidFrames,
            int screenWidth,
            int screenHeight)
        {
            //dodeljivanje parametara prosledjenih u konstruktor atributima klase
            this.texture = texture;
            this.initialFrame = initialFrame;
            this.frameNumber = asteroidFrames;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            for (int x = 0; x < asteroidCount; x++)
            {

                //pravljenje jednog asteroida - pozivanje konstruktora klase Sprite sa neophodnim parametrima
                Sprite newAsteroid = new Sprite(
                new Vector2(-500, -500),            //lokacija
                texture,                            //tekstura
                initialFrame,                       //pocetni frame
                Vector2.Zero);                      //vektor brzine


                //dodavanje svih frame-ova sa teksture u listu frame-ova datog sprite-a 
                for (int t = 1; t < frameNumber; t++)
                {
                    newAsteroid.AddFrame(new Rectangle(
                    initialFrame.X + (initialFrame.Width * t),
                    initialFrame.Y,
                    initialFrame.Width,
                    initialFrame.Height));
                }

                //postavljanje pocetnog ugla rotacije na nasumicnu vrednost izmedju 0-360
                newAsteroid.Rotation = MathHelper.ToRadians((float)rand.Next(0, 360));
                
                //nikolaADD - dodata vrednost atributa CollisionRadius objektu newAsteroid
                newAsteroid.CollisionRadius = 15;

                //dodavanje novog asteroida u listu asteroida
                asteroids.Add(newAsteroid);
            }
        }


        //nikolaADD -  dodata metoda BounceAsteroids
        private void BounceAsteroids(Sprite asteroid1, Sprite asteroid2)
        {
            {
                Vector2 cOfMass = (asteroid1.Velocity + asteroid2.Velocity) / 2;
                Vector2 normal1 = asteroid2.Center - asteroid1.Center;
                normal1.Normalize();
                Vector2 normal2 = asteroid1.Center - asteroid2.Center;
                normal2.Normalize();
                asteroid1.Velocity -= cOfMass;
                asteroid1.Velocity = Vector2.Reflect(asteroid1.Velocity, normal1);
                asteroid1.Velocity += cOfMass;
                asteroid2.Velocity -= cOfMass;
                asteroid2.Velocity = Vector2.Reflect(asteroid2.Velocity, normal2);
                asteroid2.Velocity += cOfMass;
            }
        }

        #region Privatne pomoćne metode koje se koriste samo u ovoj klasi

        //BojanREDEF: popravljena metoda u slucaju da se dobije location koji je vec u koliziji sa ostalim asteroidima
        /// <summary>
        /// Ideja je da asteroidi ulaze u vidljivo polje sa tri strane (sa leve strane, sa vrha, sa desne strane). 
        /// Ukoliko bi asteroidi ulazili i sa dna, cesto bi dolazili u situaciju da igrac izgubi zivot, jer
        /// bi ga asteroid na taj nacin iznenadio. Na pocetku vektor location postavljamo na 0, pa kasnije nasumicno
        /// biramo jedno od tri moguca mesta za stvaranje asteroida.
        /// </summary>
        /// <returns></returns>
        private Vector2 randomLocation()
        {
            Vector2 location = Vector2.Zero;
            bool isCollided = false;
            int tryCount = 0;

            do
            {
                tryCount++;
                switch (rand.Next(0, 3))
                {
                    //asteroidi ce biti stvarani iza leve ivice prozora, po celoj visini
                    case 0:
                        location.X = -initialFrame.Width;
                        location.Y = rand.Next(0, screenHeight);
                        break;

                    //asteroidi ce biti stvarani iza desne ivice prozora, po celoj visini
                    case 1:
                        location.X = screenWidth;
                        location.Y = rand.Next(0, screenHeight);
                        break;

                    //asteroidi ce biti stvarani iznad gornje ivice prozora, po celoj sirini
                    case 2:
                        location.X = rand.Next(0, screenWidth);
                        location.Y = -initialFrame.Height;
                        break;

                }

                for (int i = 0; i < asteroids.Count; i++)
                {
                    if (asteroids[i].IsCircleColliding(new Vector2(location.X + initialFrame.Width / 2, location.Y + initialFrame.Height / 2), asteroids[i].CollisionRadius))
                        isCollided = true;
                }

                if (tryCount == 5 && isCollided == true)
                {
                    location = new Vector2(-500, -500);
                    isCollided = false;
                }
                    
            } while (isCollided);
            
            return location;
        }



        /// <summary>
        /// Metoda koja nam govori da li je neki asteroid u vidljivom polju ili ne. Ipak necemo posmatrati
        /// samo vidljivo polje, nego prosireno polje. Prosirivanje se vrsi za atribut screenPadding.
        /// </summary>
        /// <param name="asteroid">The asteroid.</param>
        /// <returns>
        ///   <c>true</c> ako je asteroid u prosirenom polju; inace,<c>false</c>.
        /// </returns>
        private bool isOnScreen(Sprite asteroid)
        {
            if (asteroid.Destination.Intersects(
                new Rectangle(
                    -screenPadding,
                    -screenPadding,
                    screenWidth + screenPadding * 2,
                    screenHeight + screenPadding * 2)
                    )
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// Biramo nasumican pravac i intezitet vektora brzine
        /// </summary>
        /// <returns></returns>
        private Vector2 randomVelocity()
        {

            //definisanje nasumicnog pravca vektora brzine
            Vector2 velocity = new Vector2(
                rand.Next(0, 101) - 50,
                rand.Next(0, 101) - 50);

            // Normalizacija vektora brzine
            velocity.Normalize();

            //definisanje nasumicnog inteziteta vektora brzine
            velocity *= rand.Next(minSpeed, maxSpeed);
            return velocity;
        } 

        #endregion

        #region Javne metode za pristup klasi Ship

        /// <summary>
        /// U update metodi prolazimo kroz listu asteroida(sprite-ova) i za svaki pojedinacno pozivamo metodu Update()
        /// Nakon toga proveravamo da li mozda asteroid izasao iz prosienog polja, i ako jeste stvaramo ga na 
        /// novoj lokaciji sa novim parametrima
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            foreach (Sprite asteroid in asteroids)
            {
                asteroid.Update(gameTime);
                if (!isOnScreen(asteroid))
                {
                    asteroid.Location = randomLocation();
                    asteroid.Velocity = randomVelocity();
                }
            }


            //nikolaADD - dodata for petlja
            for (int x = 0; x < asteroids.Count; x++)
            {
                for (int y = x + 1; y < asteroids.Count; y++)
                {
                    if (asteroids[x].IsCircleColliding(
                    asteroids[y].Center,
                    asteroids[y].CollisionRadius))
                    {
                        BounceAsteroids(asteroids[x], asteroids[y]);
                    }
                }
            }

        }


        /// <summary>
        /// Klasican prolazak kroz listu asteroida(sprite-ova) i za svaki pojedinacno pozivamo njegovu Draw metodu
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite asteroid in asteroids)
            {
                asteroid.Draw(spriteBatch);
            }
        } 

        #endregion

    }
}
