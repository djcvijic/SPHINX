using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // mora da se doda ručno
using Microsoft.Xna.Framework.Graphics; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// Sprite klasa modeluje jedan objekat/sličicu na ekranu. Sadrži teksturu
    /// za sličicu, informaciju o njenoj trenutnoj lokaciji na ekranu, brzini...
    /// Pošto je predviđeno da sličica može da bude animirana, vodi se i 
    /// evidencija o svim frejmovima koji učestvuju u animaciji. U tom slučaju,
    /// kao teksturu je potrebno dostaviti tzv. sprite sheet, koja u sebi sadrži
    /// sve frejmove koji se smenjuju u okviru animacije.
    /// </summary>
    public class Sprite
    {
        #region Privatni atributi klase

        /// <summary>
        /// Tekstura za sprite. Ukoliko želimo da sprite
        /// bude animiran, potrebno je dostaviti teksturu u vidu
        /// sprite sheet-a, inače može se dostaviti i prosta tekstura.
        /// Postavlja se u konstruktoru.
        /// </summary>
        private Texture2D texture;

        /// <summary>
        /// Lista svih frejmova koji učestvuju u animaciji.
        /// Lista frejmova zapravo predstavlja skup pravougoanika (Rectangle)
        /// koji obuhvataju sve frejmove koji učestvuju u animaciji u okviru 
        /// sprite sheet-a. Ukoliko je sprite statičan, lista će sadržati 
        /// samo jedan frejm.
        /// Prvi (ili jedini) frejm se ubacuje već u konstruktoru.
        /// </summary>
        private List<Rectangle> frames = new List<Rectangle>();

        /// <summary>
        /// Širina frejma.
        /// </summary>
        private int frameWidth = 0;

        /// <summary>
        /// Visina frejma.
        /// </summary>
        private int frameHeight = 0;

        /// <summary>
        /// Indeks trenutnog frejma u listi frejmova za animaciju. 
        /// Predstavlja indeks onog frejma u listi koji se trenutno iscrtava na ekranu.
        /// </summary>
        private int currentFrame;

        /// <summary>
        /// Vreme prikazivanja pojedinačnog frejma (u sekundama).
        /// </summary>
        private float frameTime = 0.1f;

        /// <summary>
        /// Pamti proteklo vreme između sukcesivnih Update() poziva,
        /// kako bismo mogli da ispitamo da li treba da pređemo na sledeći
        /// frejm u okviru animacije.
        /// </summary>
        private float timeForCurrentFrame = 0.0f;

        /// <summary>
        /// Pozadinska boja za transparentne teksture.
        /// </summary>
        private Color tintColor = Color.White;

        /// <summary>
        /// Trenutna lokacija sprite-a na ekranu.
        /// Predstavlja koordinate gornjeg levog ugla sprite-a.
        /// Postavlja se u konstruktoru.
        /// </summary>
        private Vector2 location = Vector2.Zero;

        /// <summary>
        /// Vektor brzine sprite-a izražen u pikselima u sekundi. 
        /// Pošto radimo u 2D prostoru, vektor brzine će imati X i Y komponente.
        /// Postavlja se u konstruktoru.
        /// </summary>
        private Vector2 velocity = Vector2.Zero;

        // Dodato
        /// <summary>
        /// Ugao rotacije sprite-a. Uzima vrednosti od 0 do 2Pi.
        /// </summary>
        private float rotation = 0.0f;

        //nikolaADD - tri promenljive
        public int CollisionRadius = 0;
        public int BoundingXPadding = 0;
        public int BoundingYPadding = 0;
        
        #endregion

        /// <summary>
        /// Stvara novu instancu klase <see cref="Sprite"/>.
        /// </summary>
        /// <param name="initialLocation">Početna lokacija na ekranu (gornji levi ugao).</param>
        /// <param name="texture">Prosta tekstura ili sprite sheet za animaciju.</param>
        /// <param name="initialFrame">Početni ili jedini frejm u odnosu na sprite sheet, 
        /// odnosno prostu teksturu.</param>
        /// <param name="velocity">Vektor brzine za sprite [pixels/sec]</param>
        public Sprite (
            Vector2 initialLocation, 
            Texture2D texture, 
            Rectangle initialFrame, 
            Vector2 velocity)
        {
            this.location = initialLocation;
            this.texture = texture;
            this.velocity = velocity;
 
            frames.Add(initialFrame);
            frameWidth = initialFrame.Width;
            frameHeight = initialFrame.Height;
        }

        #region Javna svojstva (properties) klase

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        public int Frame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1); }
            // osiguravamo se u slučaju da se postavlja indeks koji je van opsega liste
            // (ograničavamo vrednost od 0 do broja elemenata liste - 1)
        }

        public float FrameTime
        {
            get { return frameTime; }
            set { frameTime = MathHelper.Max(0, value); }
            // u slučaju da se dostavi negativan broj, postavljamo vreme na 0, 
            // pošto ne može biti negativno.
        }

        /// <summary>
        /// Vraća nam pravougaonik (Rectangle) u odnosu na sprite sheet
        /// koji predstavlja trenutni frejm u animaciji koji je potrebno iscrtati.
        /// U slučaju da je sprite statičan vraća nam pravougaonik koji uokviruje 
        /// taj jedan jedini frejm.
        /// </summary>
        public Rectangle Source
        {
            get { return frames[currentFrame]; }
        }

        /// <summary>
        /// Vraća pravougaonik (Rectangle) u odnosu na lokaciju na ekranu.
        /// Ovaj pravougaonik uokviruje naš sprite na ekranu, i predstavlja mesto
        /// gde je potrebno iscrtati trenutni frejm animiranog ili statičnog sprite-a.
        /// </summary>
        public Rectangle Destination
        {
            get
            {
                return new Rectangle(
                    (int)location.X,
                    (int)location.Y,
                    frameWidth,
                    frameHeight);
            }
        }

        /// <summary>
        /// Vraća centar Destination pravougaonika, odnosno centar našeg sprite-a
        /// na ekranu.
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return location +
                    new Vector2(frameWidth / 2, frameHeight / 2);
            }
        }

        // Dodato
        public float Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                // ograničavamo se na vrednosti od 0 do 2Pi.
                rotation = value % MathHelper.TwoPi;
            }
        }

        #endregion

        #region Javne metode za pristup Sprite klasi

        /// <summary>
        /// Dodaje jedan frejm u listu frejmova koji učestvuju u animaciji.
        /// (Prvi frejm je već dodat kada se pozove konstruktor, pa ukoliko se npr.
        /// animacija sastoji iz 3 različita frejma, potrebno je dodati još 2.
        /// </summary>
        /// <param name="frameRectangle">Pravougaonik koji obuhvata traženi frejm na
        /// sprite sheet-u.</param>
        public void AddFrame(Rectangle frameRectangle)
        {
            frames.Add(frameRectangle);
        }

        //*BojanADD: virtual keyword (jer moramo da redefinisemo metodu u Particles klasi)
        /// <summary>
        /// Update-uje sprite 60 puta u sekundi. Poziva se u Update() metodi klase
        /// koja koristi ovu klasu.
        /// Služi da formira animaciju (šeta kroz listu i smenjuje frejmove koji
        /// se prikazuju na ekranu na svakih frameTime sekundi).
        /// Takođe, služi za kretanje sprite-a po ekranu, tako što množi vektor brzine
        /// sa proteklim vremenom u sekundama i dodaje to na vektor položaja sprite-a
        /// na ekranu.
        /// </summary>
        /// <param name="gameTime">Vreme proteklo od prethodnog Update() poziva.</param>
        public virtual void Update(GameTime gameTime)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeForCurrentFrame += elapsed;

            if (timeForCurrentFrame >= FrameTime)
            {
                currentFrame = (currentFrame + 1) % (frames.Count);
                timeForCurrentFrame = 0.0f;
            }

            location += (velocity * elapsed);
        }

        // Izmenjeno
        //*BojanADD: virtual keyword (jer moramo da redefinisemo metodu u Particles klasi)
        /// <summary>
        /// Iscrtava Sprite na ekranu (60 puta u sekundi).
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch objekat koji ume da iscrtava teksture.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
           /* Ovako je bilo ranije:
            * 
            spriteBatch.Draw(
                Texture,
                Destination,
                Source,
                TintColor);
            */

            /* Pošto nam je potrebno da omogućimo rotaciju sprite-a, potreban nam je Draw()
             * metod koji kao parametar ima ugao rotacije (u radijanima), pa nam više ne odgovara
             * ovo iznad, već pozivamo neki drugi overload-ovani Draw()
             */

            spriteBatch.Draw(
                Texture,        // A texture.
                Center,         // The location (in screen coordinates) to draw the sprite.
                Source,         // A rectangle that specifies (in texels) the source texels from a texture.
                                // Use null to draw the entire texture.
                TintColor,      // The color to tint a sprite. Use Color.White for full color with no tinting.
                Rotation,       // Specifies the angle (in radians) to rotate the sprite about its center.
                new Vector2(frameWidth / 2, frameHeight / 2),   // The sprite origin; the default is (0,0) which represents the upper-left corner.
                1.0f,           // Scale factor.
                SpriteEffects.None, // Effects to apply.
                0.0f                // The depth of a layer. By default, 0 represents the front layer and 1 represents
                                    // a back layer. Use SpriteSortMode if you want sprites to be sorted during
                                    // drawing.
                );
                
        }


        //nikolaADD
        public Rectangle BoundingBoxRect
        {
            get
            {
                return new Rectangle(
                (int)location.X + BoundingXPadding,
                (int)location.Y + BoundingYPadding,
                frameWidth - (BoundingXPadding * 2),
                frameHeight - (BoundingYPadding * 2));
            }
        }

        //nikolaADD
        public bool IsBoxColliding(Rectangle OtherBox)
        {
            return BoundingBoxRect.Intersects(OtherBox);
        }

        //nikolaADD
        public bool IsCircleColliding(Vector2 otherCenter, float
        otherRadius)
        {
            if (Vector2.Distance(Center, otherCenter) <
                 (CollisionRadius + otherRadius))
                return true;
            else
                return false;
        }

        public bool IsArcColliding(Vector2 arcCenter, float arcRadius, double startAngle, double endAngle) // samo za jednostavni kruzni luk; uglovi su izmedju -pi/2 i 3*pi/2 (opseg je jednostavan za realizaciju, a ne otezava mnogo poziv funkcije)
        {
            if (Math.Abs(Vector2.Distance(Center, arcCenter) - CollisionRadius) < arcRadius) // da li ovaj sprajt sece tu kruznicu (sa bilo koje strane)
            {
                Vector2 relativeLocation = Location - arcCenter; // vektor od centra luka do centra this sprajta
                double relativeAngle = Math.Atan2( - relativeLocation.Y, relativeLocation.X); // -y da bih obrnuo y osu (izvini, ali lakse je ovako da bismo radili sa normalnim uglovima, inace bi bio obrnut smer i radili bi sa neprirodnim uglovima)
                if (relativeLocation.X < 0) relativeAngle += Math.PI; // arctan vraca izmedju -pi/2 i pi/2, a ako je x negativan, onda mi treba ugao iz leve polovine trigonometrijskog kruga
                //sad je relativeAngle u opsegu (-pi/2, 3*pi/2)
                if (startAngle < endAngle)
                {
                    //normalan slucaj
                    if (relativeAngle > startAngle && relativeAngle < endAngle) return true;
                }
                    //ovaj drugi slucaj je ako je startAngle npr pi, a endAngle npr 0 (luk kao usta od tuznog smajlija). jasno je da gornji uslov nece biti ispunjen, pa se kolizija malo drugacije racuna 
                else if (relativeAngle > startAngle || relativeAngle < endAngle) return true;
            }
            return false;
        }

        #endregion
    }
}
