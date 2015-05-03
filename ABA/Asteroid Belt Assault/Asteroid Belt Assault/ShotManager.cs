using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // mora da se doda ručno
using Microsoft.Xna.Framework.Graphics; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// Klasa ShotManager vodi evidenciju o ispaljenim pucnjima igrača ili
    /// neprijatelja. Takođe, potrebno je obezbediti da metak ispaljen od strane
    /// neprijatelja ne može da pogodi neprijatelja, i da igrač ne može
    /// sam sebe da povredi ukoliko je to uopšte moguće, a to će biti kasnije implementirano.
    /// </summary>
    public class ShotManager
    {
        #region Privatni atributi klase

        /// <summary>
        /// Lista svih shot-ova (metkova) koji su trenutno na ekranu.
        /// Svaki metak je sprite za sebe.
        /// </summary>
        private List<Sprite> shots = new List<Sprite>();

        //*BojanADD:
        public List<Sprite> Shots
        {
            get { return shots; }
        }

        /// <summary>
        /// Tekstura ili SpriteSheet za (animirani) pucanj.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Početni frejm u animaciji (u odnosu na SpriteSheet).
        /// </summary>
        private Rectangle initialFrame;

        /// <summary>
        /// Ukupan broj frejmova u animaciji.
        /// </summary>
        private int frameNumber;

        /// <summary>
        /// Intenzitet brzine kretanja.
        /// </summary>
        private float speed;

        /// <summary>
        /// Površina ekrana. Potrebna nam je da bismo mogli da iz liste
        /// izbacimo sve metkove koji su napustili vidljivu površinu ekrana.
        /// </summary>
        private Rectangle screenBounds;

        //BojanADD:
        private int collisionRadius;

        //*BojanADD
        private bool playerFired;

        #endregion

        /// <summary>
        /// Stvara novu instancu <see cref="ShotManager"/> klase.
        /// </summary>
        /// <param name="texture">Sprite sheet za pucanj(metak).</param>
        /// <param name="initialFrame">Inicijalni frejm za animaciju.</param>
        /// <param name="frameNumber">Ukupan broj frejmova u animaciji.</param>
        /// <param name="shotSpeed">Brzina kretanja pucnja.</param>
        /// <param name="screenBounds">Pravougaonik koji predstavlja vidljivu površinu ekrana.</param>
        public ShotManager(
            Texture2D texture,
            Rectangle initialFrame,
            int frameNumber,
            int collisionRadius,
            float shotSpeed,
            Rectangle screenBounds,
            bool playerFired) //*BojanADD
        {
            /* Uz pomoć parametara iz konstruktora inicijalizujemo
             * sva potrebna privatna polja u klasi. Sve ove podatke moramo
             * da čuvamo negde jer će nam biti potrebni za pravljenje sprite-ova
             * za metkove, a to radimo kad god se ispaljuje metak, a ne
             * ovde u konstruktoru, za razliku od npr. StarField-a.
             */
            this.texture = texture;
            this.initialFrame = initialFrame;
            this.frameNumber = frameNumber;
            this.speed = shotSpeed;
            this.collisionRadius = collisionRadius;
            this.screenBounds = screenBounds;

            this.playerFired = playerFired; //*BojanADD
        }

        #region Javne metode za pristup klasi ShotManager

        /// <summary>
        /// Poziva se kada je potrebno ispaliti metak.
        /// </summary>
        /// <param name="location">Lokacija gde treba da se pojavi metak na ekranu.</param>
        /// <param name="velocity">Podatak u kom smeru treba da se kreće metak
        ///                        (jedinični vektor brzine).</param>
        public void FireShot(
            Vector2 location,
            Vector2 velocity)
        {
            /* Pravimo novi sprite za metak, prosleđujemo
             * odgovarajuće informacije */
            Sprite thisShot = new Sprite(
                location,
                texture,
                initialFrame,
                velocity);

            thisShot.Velocity *= speed;

            // Dodajemo ostale frejmove za animaciju
            for (int x = 1; x < frameNumber; x++)
            {
                thisShot.AddFrame(new Rectangle(
                    initialFrame.X + (initialFrame.Width * x),
                    initialFrame.Y,
                    initialFrame.Width,
                    initialFrame.Height));
            }

            //BojanADD:
            thisShot.CollisionRadius = collisionRadius;
            // Ubacujemo tek napravljeni sprite u listu sprite-ova
            shots.Add(thisShot);


            //*BojanADD:
            if (playerFired)
                SoundManager.PlayPlayerShot();
            else
                SoundManager.PlayEnemyShot();
        }

        /// <summary>
        /// Poziva se u okviru Update() metode klase koja koristi
        /// ovu klasu (60 puta u sekundi). Ovu klasu će da koristi
        /// klasa Ship i klasa koja će predstavljati neprijateljske
        /// brodove, i u svom Update-u će da poziva ovaj Update().
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            /* For petlja ispituje da li je neki metak izašao sa vidljivog
             * dela ekrana, i ako jeste, vadi ga iz liste. Petlja ide unazad,
             * da bi se izbeglo preskakanje elemenata pri vađenju iz liste */
            for (int x = shots.Count - 1; x >= 0; x--)
            {
                shots[x].Update(gameTime);
                if (!screenBounds.Intersects(shots[x].Destination))
                {
                    shots.RemoveAt(x);
                }
            }
        }

        /// <summary>
        /// Iscrtava pucnje na ekranu. Poziva se u okviru Draw-a klase
        /// koja predstavlja objekat koji može da puca.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch objekat koji ume da iscrtava teksture.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite shot in shots)
            {
                shot.Draw(spriteBatch);
            }
        } 

        #endregion
    }
}
