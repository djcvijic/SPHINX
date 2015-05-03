using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // mora da se doda ručno
using Microsoft.Xna.Framework.Graphics; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// Klasa modeluje zvezdano polje u pozadini igrice. Zvezdano polje se
    /// sastoji iz određenog broja zvezda koje se kreću konstantnom brzinom po ekranu.
    /// </summary>
    public class StarField
    {

        #region Privatni atributi klase

        /// <summary>
        /// Lista sprite-ova. Svaki sprite služi da modeluje jednu zvezdu.
        /// </summary>
        private List<Sprite> stars = new List<Sprite>();

        /// <summary>
        /// Širina ekrana.
        /// Potrebna nam je iz razloga što se zvezde kreću po celukopnom
        /// prostoru za igru.
        /// Postavlja se u konstruktoru.
        /// </summary>
        private int screenWidth;

        /// <summary>
        /// Visina ekrana.
        /// Potrebna nam je iz razloga što se zvezde kreću po celukopnom
        /// prostoru za igru.
        /// Postavlja se u konstruktoru.
        /// </summary>
        private int screenHeight;

        /// <summary>
        /// Generator pseudo-slučajnih brojeva koji koristimo da 
        /// postavimo zvezde na nekom slučajnom mestu na ekranu, i
        /// da joj dodelimo slučajnu boju.
        /// </summary>
        private Random rand = new Random();

        /// <summary>
        /// Boje za zvezde.
        /// </summary>
        private Color[] colors = { Color.White, Color.Yellow,
                                   Color.Wheat, Color.WhiteSmoke,
                                   Color.SlateGray };

        #endregion

        /// <summary>
        /// Stvara novu instancu <see cref="StarField"/> klase.
        /// </summary>
        /// <param name="screenWidth">Širina ekrana.</param>
        /// <param name="screenHeight">Visina ekrana.</param>
        /// <param name="starCount">Ukupan broj zvezda na ekranu.</param>
        /// <param name="starVelocity">Vektor brzine svih zvezda.</param>
        /// <param name="texture">Tekstura odakle ekstraktujemo zvezde.</param>
        /// <param name="frameRectangle">Pravougaonik u odnosu na teksturu koji označava
        /// deo teksture koji koristimo za zvezdu.</param>
        public StarField (
            int screenWidth,
            int screenHeight,
            int starCount,
            Vector2 starVelocity,
            Texture2D texture,
            Rectangle frameRectangle)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            // dodajemo zvezde (njih starCount) u listu sprite-ova
            for (int x = 0; x < starCount; x++)
            {
                stars.Add(new Sprite(
                    new Vector2(rand.Next(0, screenWidth), // X koordinatu položaja postavljamo na slučajnu vrednost od 0 do širine ekrana
                        rand.Next(0, screenHeight)), // Y koordinatu položaja postavljamo na slučajnu vrednost od 0 do visine ekrana
                    texture, // prosleđujemo teksturu
                    frameRectangle, // prosleđujemo uokvirujući pravougaonik u okviru teksture
                    starVelocity)); // prosleđujemo vektor brzine

                // uzimamo jednu od boja namenjenih za zvezde
                Color starColor = colors[rand.Next(0, colors.Count())];
                // pravimo da svaka zvezda bude semi-transparentna
                starColor *= (float)(rand.Next(30, 80) / 100f);
                // postavljamo boju poslednje dodate zvezde na upravo definisanu boju
                stars[stars.Count - 1].TintColor = starColor; 
            }
        }

        #region Javne metode za pristup StarField klasi

        /// <summary>
        /// Update-uje položaj zvezda 60 puta u sekundi. Poziva se u Update() metodi klase
        /// koja koristi ovu klasu.
        /// </summary>
        /// <param name="gameTime">Vreme proteklo od prethodnog Update() poziva.</param>
        public void Update(GameTime gameTime)
        {
            foreach (Sprite star in stars)
            {
                star.Update(gameTime);
                // ukoliko vektor položaja neke od zvezda izadje izvan opsega visine ekrana,
                // postavlja se nov položaj na vrh ekrana sa slučajnom X koordinatom.
                if (star.Location.Y > screenHeight)
                {
                    star.Location = new Vector2(rand.Next(0, screenWidth), 0);
                }
            }
        }

        /// <summary>
        /// Iscrtava zvezde (60 puta u sekundi). Za svaku zvezdu poziva Draw() metod Sprite klase.
        /// Ovaj Draw() metod će pozivati glavna Game1.cs klasa u svojoj Draw() metodi.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch objekat koji ume da iscrtava teksture.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite star in stars)
            {
                star.Draw(spriteBatch);
            }
        }

        #endregion
    }
}
