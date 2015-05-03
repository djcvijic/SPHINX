using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // mora da se doda ručno
using Microsoft.Xna.Framework.Graphics; // mora da se doda ručno
using Microsoft.Xna.Framework.Input; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{

    /// <summary>
    /// Klasa Ship predstavlja brod kojim upravlja igrač. Brodom
    /// se upravlja uz pomoć strelica gore, dole, levo i desno, a prostor
    /// za pomeranje je ograničen na donju polovinu ekrana. Brod može i da puca,
    /// a to se dešava kada korisnik pritisne taster Space.
    /// </summary>
    public class Ship
    {

        #region Privatni atributi klase

        /// <summary>
        /// Sprite koji predstavlja brod.
        /// </summary>
        private Sprite ship;
        public bool Destroyed = false;

        /// <summary>
        /// Brzina (skalarna) sa kojom ćemo pomnožiti
        /// jedinični vektor brzine pri pomeraju.
        /// Podrazumevano 160 piksela u sekundi, 
        /// može da se menja preko svojstva.
        /// </summary>
        private float speed = 160.0f;

        /// <summary>
        /// Prostor u kome je dozvoljeno da se kreće brod.
        /// </summary>
        private Rectangle areaLimit;

        // Dodato:

        /// <summary>
        /// Vector2 offset pucaljke u odnosu na gornji levi
        /// ugao, tj. lokaciju broda,
        /// </summary>
        private Vector2 gunOffset = new Vector2(25, 10);

        /// <summary>
        /// Minimalni vremenski interval između dva pucnja.
        /// </summary>
        private float shotTime = 0.2f;

        /// <summary>
        /// Promenljiva u kojoj pamtimo proteklo vreme od prethodnog
        /// pucnja, na osnovu koje možemo da vidimo da li je proteklo
        /// minimalno zahtevano vreme, ili ne.
        /// </summary>
        private float timeForCurrentShot = 0.0f;

        /// <summary>
        /// Objekat koji nam omogućava pucanje. Njega ćemo da koristimo
        /// pri svakom pritisku Space tastera, a on poseduje logiku da 
        /// obezbedi iscrtavanje i kretanje metka na ekranu.
        /// </summary>
        private ShotManager shotManager;

        private Keys gore, dole, levo, desno, pucanje;

        #endregion

        /// <summary>
        /// Stvara novu instancu <see cref="Ship"/> klase.
        /// </summary>
        /// <param name="texture">Sprite sheet za brod.</param>
        /// <param name="initialFrame">Inicijalni frejm za animaciju.</param>
        /// <param name="frameNumber">Ukupan broj frejmova u animaciji.</param>
        /// <param name="initialLocation">Početni vektor položaja na ekranu.</param>
        /// <param name="screenWidth">Širina ekrana.</param>
        /// <param name="screenHeight">Visina ekrana.</param>
        public Ship(
            Texture2D texture,
            Rectangle initialFrame,
            int frameNumber,
            Vector2 initialLocation,
            int screenWidth,
            int screenHeight, 
            Keys gore, 
            Keys dole, 
            Keys levo,
            Keys desno, 
            Keys pucanje)
        {
            ship = new Sprite(initialLocation, texture, initialFrame, Vector2.Zero);
            areaLimit = new Rectangle(0, screenHeight / 2, screenWidth, screenHeight/2);

            // pošto animaciju sačinjavaju 3 frejma, a jedan je već dodat u okviru konstruktora
            // za sprite, potrebno je dodati još 2 susedna frejma sa sprite sheet-a.
            for (int i = 1; i < frameNumber; i++)
            {
                ship.AddFrame(new Rectangle(
                    initialFrame.X + initialFrame.Width * i,
                    initialFrame.Y,
                    initialFrame.Width,
                    initialFrame.Height));
            }

            // (dodato)
            // instanciramo objekat shotManager
            shotManager = new ShotManager(
                texture,                        //  tekstura
                new Rectangle(0, 300, 5, 5),    //  polje na SpriteSheet.png gde se nalazi prvi frame
                4,                              //  ukupan broj frejmova
                2,
                250f,                           //  brzina metka (pixels/sec), brze nego neprijateljev metak
                new Rectangle(0, 0, screenWidth, screenHeight), //  vidljiva površina ekrana  
                true);   //*BojanADD
            this.desno = desno;
            this.levo = levo;
            this.dole = dole;
            this.gore = gore;
            this.pucanje = pucanje;
            this.Sprite.CollisionRadius = 15;
            
        }

        #region Privatne pomoćne metode koje se koriste samo u ovoj klasi

        /// <summary>
        /// Poziva se kada korisnik pritisne Space taster. Potrebno je proveriti da li je od prethodnog
        /// pucnja proteklo minimalno vreme između dva pucnja, i ako jeste metak se ispaljuje u pravcu
        /// prema gore i resetuje tajmer za minimalno vreme.
        /// </summary>
        private void fireShot()
        {
            if (timeForCurrentShot >= shotTime)
            {
                shotManager.FireShot(
                    ship.Location + gunOffset,
                    new Vector2(0, -1));

                timeForCurrentShot = 0.0f;
            }
        } 

        #endregion

        #region Javna svojstva (properties) klase

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        } 

        //BojanADD:
        public Sprite Sprite
        {
            get { return ship; }
        }

        //BojanADD:
        public Vector2 Location
        {
            get { return ship.Location; }
            set { ship.Location = value; }
        }

        public ShotManager ShotManager
        {
            get
            {
                return shotManager;
            }
        }

        #endregion

        #region Javne metode za pristup klasi Ship

        /// <summary>
        /// Poziva se u okviru Update() metode klase koja koristi
        /// ovu klasu (60 puta u sekundi). Potrebno je reagovati na
        /// poteze igrača, odnosno pokrenuti brod u zavisnosti od toga
        /// koju strelicu na tastaturi je pritisnuo igrač, ili ispaliti metak
        /// ukoliko je pritisnut taster Space.
        /// </summary>
        /// <param name="gameTime">Vreme proteklo od prethodnog Update() poziva.</param>
        public void Update(GameTime gameTime)
        { 

            // Pozivamo Update() objekta shotManager, da bi mogla da se ažurira pozicija
            // metka na ekranu.
            shotManager.Update(gameTime);

            // Akumuliramo proteklo vreme od poslednjeg Update() poziva da bismo mogli da 
            // proverimo da li je proteklo željeno minimalno vreme.
            timeForCurrentShot += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsActive())
            {
                ship.Velocity = Vector2.Zero;   // vektor brzine broda resetujemo na 0
                // da se brzina ne bi povećavala nekontrolisano.

                // učitavamo status svih karaktera sa tastature
                KeyboardState keyboardState = Keyboard.GetState();

                // u zavisnosti od toga koji taster na tastaturi je pritisnut
                // povećavamo brzinu za jediničnu vrednost u određenom smeru.
                if (keyboardState.IsKeyDown(gore))
                {
                    ship.Velocity += new Vector2(0, -1);
                }

                if (keyboardState.IsKeyDown(dole))
                {
                    ship.Velocity += new Vector2(0, 1);
                }

                if (keyboardState.IsKeyDown(levo))
                {
                    ship.Velocity += new Vector2(-1, 0);
                }

                if (keyboardState.IsKeyDown(desno))
                {
                    ship.Velocity += new Vector2(1, 0);
                }

                if (keyboardState.IsKeyDown(pucanje))
                {
                    fireShot();
                }

                ship.Velocity.Normalize();
                ship.Velocity *= speed;
                /* Normalizujemo vrednost jediničnog vektora brzine i svaku komponentu množimo
                 * određenom skalarnom vrednošću (intenzitetom) kako bi se brod zaista kretao željenom brzinom.
                 * Normalizaciju je potrebno uraditi iz razloga što, ukoliko igrač u isto vreme
                 * pritisne dva tastera (npr. strelicu gore, i strelicu levo) intenzitet jediničnog vektora
                 * brzine neće biti 1, već 1.41, pa bi se brže kretao po dijagonali nego inače */

                // kaskadno pozivamo Update() metodu klase Sprite, kako bi ona obavila
                // sve što je potrebno za sam sprite.
                ship.Update(gameTime);

                // sprečavamo brod da izađe izvan dozvoljenog pravouganika areaLimit
                Vector2 location = ship.Location;

                if (location.X < areaLimit.X)
                    location.X = areaLimit.X;

                if (location.X + ship.Destination.Width > areaLimit.Width)
                    location.X = areaLimit.Width - ship.Destination.Width;

                if (location.Y < areaLimit.Y)
                    location.Y = areaLimit.Y;

                if (location.Y + ship.Destination.Height > areaLimit.Y + areaLimit.Height)
                    location.Y = areaLimit.Y + areaLimit.Height - ship.Destination.Height;

                ship.Location = location;
            }
            else
                ship.Location = new Vector2(-500, -500);
        }

        /// <summary>
        /// Iscrtava brod na ekranu (60 puta u sekundi). Poziva Draw() metod klase Sprite objekta Ship,
        /// a ovu metodu će pozivati glavna Game1.cs klasa u svojoj Draw() metodi.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch objekat koji ume da iscrtava teksture.</param>
        public void Draw(SpriteBatch spriteBatch)
        {

            // pozivamo Draw() metode i shotManager-a i samog broda jer je potrebno da se iscrtaju
            // i brod i pucnji na ekranu.
            shotManager.Draw(spriteBatch);
            if(IsActive())
            ship.Draw(spriteBatch);
        }

        public bool IsActive()
        {
            //*BojanADD: (dodali smo destroyed promenljivu koja se kontroliše iz CollisionManager-a)
            if (Destroyed)
                return false;
            else
                return true;
        }

        #endregion
    }
}
