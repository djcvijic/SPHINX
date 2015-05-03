using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // mora da se doda ručno
using Microsoft.Xna.Framework.Graphics; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// Klasa koja modeluje objekte od kojih je sačinjena svaka eksplozija. Particle ima kratak životni vek tokom kojeg se kreće u određenom pravcu,
    /// određenim ubrzanjem i ima osobinu da tokom svog života menja boju od prve prema drugoj. Klasa je izvedena iz klase Sprite, ali specijalizuje 
    /// njenu funkciju dodajući nove osobine poput životnog veka, menjanja boje, itd.
    /// </summary>
    public class Particle : Sprite
    {

        /// <summary>
        /// Vektor ubrzanja
        /// </summary>
        private Vector2 acceleration;

        /// <summary>
        /// Maksinmalni intenzitet brzine koji se može dodeliti objektu. 
        /// </summary>
        private float maxSpeed;

        /// <summary>
        /// Trajanje životnog veka
        /// </summary>
        private int initialDuration;

        /// <summary>
        /// Preostalo vreme života
        /// </summary>
        private int remainingDuration;

        private Color initialColor;
        private Color finalColor;

        /// <summary>
        /// Kreira novu instancu <see cref="Particle"/> klase.
        /// </summary>
        /// <param name="location">Pozicija na ekranu.</param>
        /// <param name="texture">Tekstura ili spritesheet.</param>
        /// <param name="initialFrame">Inicijalni frejm na spritesheetu.</param>
        /// <param name="velocity">Vektor brzine.</param>
        /// <param name="acceleration">Vektor ubrzanja.</param>
        /// <param name="maxSpeed">Maksimalni intenzitet brzine.</param>
        /// <param name="duration">Trajanje životnog veka.</param>
        /// <param name="initialColor">Inicijalna boja.</param>
        /// <param name="finalColor">Finalna boja.</param>
        public Particle(
            Vector2 location, // potrebno za Sprite konstruktor
            Texture2D texture, // potrebno za Sprite konstruktor
            Rectangle initialFrame, // potrebno za Sprite konstruktor
            Vector2 velocity, // potrebno za Sprite konstruktor
            Vector2 acceleration,
            float maxSpeed,
            int duration, 
            Color initialColor,
            Color finalColor
            ) : base(location, texture, initialFrame, velocity)
        {
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
            this.initialDuration = duration;
            this.initialColor = initialColor;
            this.finalColor = finalColor;

            this.remainingDuration = initialDuration;
        }

        /// <summary>
        /// Vraća proteklo vreme života.
        /// </summary>
        /// <value>
        /// Proteklo vreme života.
        /// </value>
        public int ElapsedDuration
        {
            get
            {
                return initialDuration - remainingDuration;
            }
        }

        /// <summary>
        /// Vraća procenat proteklog životnog veka od 0.0 do 1.0.
        /// </summary>
        public float DurationProgress
        {
            get
            {
                return (float)ElapsedDuration / (float)initialDuration;
            }
        }

        /// <summary>
        /// Određuje da li je objekat Particle "živ" odnosno da li je isteklo njegovo predviđeno
        /// vreme za životni vek.
        /// </summary>
        /// <returns>
        ///   <c>true</c> ako je instanca aktivna; inače, <c>false</c>.
        /// </returns>
        public bool IsActive()
        {
            return remainingDuration > 0;
        }

        /// <summary>
        /// Update-uje Particle 60 puta u sekundi. Poziva se u Update() metodi klase
        /// koja koristi ovu klasu. Poziv ove metode omogućava objektu Particle da 
        /// manifestuje svoje osobine poput ubrzanja, menjanja boje, kontrolu životnog veka.
        /// </summary>
        /// <param name="gameTime">Vreme proteklo od prethodnog Update() poziva.</param>
        public override void Update(GameTime gameTime)
        {
            if (IsActive())
            {
                Velocity += acceleration;
                if (Velocity.Length() > maxSpeed) // ako je Particle ubrzan do maksimalne vrednosti brzine
                {
                    Velocity.Normalize(); // Postavi Velocity da ima jediničnu vrednost, ali mu ne menjaj pravac
                    Velocity *= maxSpeed; // Postavi intenzitet vektora na maxSpeed
                }

                /* U zavisnosti od parametra DurationProgress (0.0-1.0) postavlja: 
                 * TintColor na initialColor (DurationProgress == 0),
                 * finalColor (DurationProgress == 1), inače:
                 * interpoliranu boju između initialColor i FinalColor u zavisnosti DurationProgress
                 */

                TintColor = Color.Lerp(
                    initialColor,
                    finalColor,
                    DurationProgress);

                remainingDuration--;
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// Iscrtava Particle na ekranu (60 puta u sekundi).
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch objekat koji ume da iscrtava teksture.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive())
                base.Draw(spriteBatch);
        }
    }
}
