using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // mora da se doda ručno
using Microsoft.Xna.Framework.Graphics; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// Objekat koji vodi računa o eksplozijama u sistemu. Eksplozija se sastoji iz dve
    /// vrste Particle objekta (sa teksturom (veći) i bez(manji)) koji će se generisati
    /// u nekom nasumičnom broju kada se generiše efekat eksplozije.
    /// </summary>
    public class ExplosionManager
    {
        /// <summary>
        /// Tekstura za krupnije parče ekplozije
        /// </summary>
        private Texture2D texture;


        /// <summary>
        /// Kešira sve varijante za krupnije parče eksplozije.
        /// </summary>
        private List<Rectangle> pieceRectangles = new List<Rectangle>();


        /// <summary>
        /// Inicijalni frejm za sitne parčiće eksplozije
        /// </summary>
        private Rectangle pointRectangle;

        /// <summary>
        /// Minimalan broj krupnijih parčića eksplozije
        /// </summary>
        private int minPieceCount = 3;
        /// <summary>
        /// Maksimalan broj krupnijih parčića eksplozije
        /// </summary>
        private int maxPieceCount = 6;


        /// <summary>
        /// Minimalan broj sitnijih parčića eksplozije
        /// </summary>
        private int minPointCount = 20;

        /// <summary>
        /// Maksimalan broj sitnij parčića ekplozije
        /// </summary>
        private int maxPointCount = 30;


        /// <summary>
        /// Trajanje eskplozije
        /// </summary>
        private int durationCount = 90;


        /// <summary>
        /// Maksimalna brzina eksplozije
        /// </summary>
        private float explosionMaxSpeed = 30f;


        /// <summary>
        /// Brzina krupnijih parčića (const)
        /// </summary>
        private float pieceSpeedScale = 6f;

        /// <summary>
        /// Minimalna brzina sitnijih parčića
        /// </summary>
        private int pointSpeedMin = 15;

        /// <summary>
        /// Maksimalna brzina sitnijih parčića
        /// </summary>
        private int pointSpeedMax = 30;

        private Color initialColor = new Color(1f, 0.3f, 0f) * 0.5f;
        private Color finalColor = new Color(0f, 0f, 0f, 0f);

        private Random rand = new Random();

        /// <summary>
        /// Lista svih parčića eksplozije u sistemu (i krupnijih i sitnijih)
        /// </summary>
        private List<Particle> particles = new List<Particle>();



        /// <summary>
        /// Kreira novu instancu <see cref="ExplosionManager"/> klase.
        /// </summary>
        /// <param name="texture">Tekstura za krupne parčiće.</param>
        /// <param name="initialFrame">Broj različitih varijanti za krupnije eksplozije.</param>
        /// <param name="pieceCount">Broj različitih varijanti za krupnije eksplozije.</param>
        /// <param name="pointRectangle">Velicina sitnih parčića eksplozije.</param>
        public ExplosionManager(
            Texture2D texture,
            Rectangle initialFrame,
            int pieceCount, 
            Rectangle pointRectangle
            )
        {
            this.texture = texture;
            for (int i = 0; i < pieceCount; i++)
            {
                pieceRectangles.Add(new Rectangle(
                    initialFrame.X + (initialFrame.Width*i),
                    initialFrame.Y,
                    initialFrame.Width,
                    initialFrame.Height
                    ));
            }
            this.pointRectangle = pointRectangle;
        }

        /// <summary>
        /// Vraća nasumični vektor sa intenzitetom scale.
        /// </summary>
        /// <param name="scale">Intezitet vektora.</param>
        /// <returns></returns>
        private Vector2 randomDirection(float scale)
        {
            Vector2 direction;
            do
            {
                direction = new Vector2(
                    rand.Next(0, 101) - 50,
                    rand.Next(0, 101) - 50
                    );
            } while (direction.Length() == 0); // ako je slučajno Vector2.Zero, ponovi

            direction.Normalize(); // random pravac, intenzitet 1
            direction *= scale; // skaliraj sa scale, da bi dobili vektor željenog intenziteta

            return direction;
        }

        /// <summary>
        /// Dodaje eksplozije sacinjene od dve varijante Particle sprajtova,
        /// sa teksturom (veće) i bez (manje).
        /// </summary>
        /// <param name="location">Pozicija gde se dešava eksplozija.</param>
        /// <param name="momentum">Impuls (komponenta kretanja ekplozije).</param>
        public void AddExplosion(Vector2 location, Vector2 momentum)
        {
            //lokacija parceta ekplozije
            Vector2 pieceLocation = location - new Vector2(
                pieceRectangles[0].Width / 2,
                pieceRectangles[0].Height / 2
            );

            // stvaramo od 3 do 6 parčeta eksplozije
            int pieces = rand.Next(minPieceCount, maxPieceCount + 1);
            for (int i = 0; i < pieces; i++)
            {
                particles.Add(new Particle(
                    pieceLocation,
                    texture,
                    pieceRectangles[rand.Next(0, pieceRectangles.Count)], // jedna od tri varijanti inicijalnog frejma
                    randomDirection(pieceSpeedScale) + momentum, // vektor brzine konstantnog inteziteta u random pravcu + impuls eksplozije
                    Vector2.Zero, // ubrzanje
                    explosionMaxSpeed, // maksimalna brzina
                    durationCount, // trajanje eksplozije (koliko Update-a)
                    initialColor, // početna i
                    finalColor)); // završna boja
            }

            // stvaramo od 20 do 30 čestica eksplozija
            int points = rand.Next(minPointCount, maxPointCount + 1);
            for (int i = 0; i < points; i++)
            {
                particles.Add(new Particle(
                    location,
                    texture,
                    pointRectangle, // inicijalni frejm
                    randomDirection((float)rand.Next(pointSpeedMin, pointSpeedMax)) + momentum, // vektor brzine random inteziteta u random pravcu + impuls eksplozije
                    Vector2.Zero,
                    explosionMaxSpeed,
                    durationCount,
                    initialColor,
                    finalColor));
            }

            // pusti zvuk eksplozije
            SoundManager.PlayExplosion();
        }


        /// <summary>
        /// Update-uje sve elemente eksplozije i vodi računa o njihovom životnom veku
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].IsActive())
                    particles[i].Update(gameTime);
                else
                    particles.RemoveAt(i);
            }
        }


        /// <summary>
        /// Iscrtava sve elemente eksplozije
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particles)
                particle.Draw(spriteBatch);
        }
    }
}
