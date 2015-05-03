using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // mora da se doda ručno
using Microsoft.Xna.Framework.Graphics; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{

    /// <summary>
    /// Klasa Enemy predstavlja jedan neprijateljski brod. Kao takva nece biti
    /// direktno instancirana u Game1 klasi vec u EnemyManager klasi. Kompletna logika
    /// upravljanja neprijateljskim brodom smestena je u EnemyManager klasu. Ovde, 
    /// samo brinemo o izracunavanju parametara koje su potrebne sprite-u - lokacija,
    /// brzina, pravac kretanja...
    /// </summary>

    public class Enemy
    {
        #region Privatni atributi klase

        /// <summary>
        /// Sprite koji predstavlja neprijatelja.
        /// </summary>
        private Sprite enemySprite;

        /// <summary>
        /// Vektor koji u zbiru sa vektorom location predstavlja poziciju 
        /// pucaljke neprijatelja.
        /// </summary>
        private Vector2 gunOffset = new Vector2(25, 25);

        /// <summary>
        /// Red u koji smestamo sve tacke na nasoj putanji kroz svemir.
        /// Objansnjenje: Putanje kojima se krecu neprijatelji su unapred 
        /// napravljene u klasi EnemyManager. Svaka putanja se sastoji od
        /// niza tacaka (waypoints) izmedju kojih ce se neprijateljski brod
        /// pravolinijski kretati.
        /// </summary>
        private Queue<Vector2> waypoints = new Queue<Vector2>();

        /// <summary>
        /// Najbliza tacka na putanji prema kojoj se krecemo
        /// (u cijem pravcu idemo). Inicijalno je postavljena na nulti vektor
        /// ali vec ukonstruktoru klase dobija pocetnu vrednost.
        /// </summary>
        private Vector2 currentWaypoint = Vector2.Zero;

        /// <summary>
        /// Brzina kojom se krece neprijateljski brod. Ova brzina je manja
        /// od brzine kojom se krece igrac.
        /// </summary>
        private float speed = 120f;

        /// <summary>
        /// Postavljanje okvira oko Enemy-a koji potreban za izracunavanje
        /// kolizije.
        /// </summary>
        private int enemyRadius = 15;

        /// <summary>
        /// Za kretanje neprijateljskog broda odgovorna je Sprite klasa, tj. 
        /// pozivom Update metode nase Sprite klase realizovace se kretanje.
        /// Vektor previousLocation cuva lokaciju broda pre poslednjeg poziva Update-a.
        /// Inicijalno postavljen je na nultu vrednost.
        /// </summary>
        private Vector2 previousLocation = Vector2.Zero;

        //*BojanADD:
        /// <summary>
        /// Da li je neprijatelj uništen ili ne.
        /// Za postavljanje ove promenljive na true je zadužena CollisionManager klasa, i zbog toga je public.
        /// </summary>
        public bool Destroyed = false;

        #endregion



        /// <summary>
        /// Stvara novu instancu <see cref="Enemy"/> klase.
        /// </summary>
        /// <param name="texture">Sprite sheet za brod.</param>
        /// <param name="initialLocation">Početni vektor položaja na ekranu.</param>
        /// <param name="initialFrame">Inicijalni frame za animaciju.</param>
        /// <param name="frameCount">Ukupan broj frame-ova u animaciji.</param>
        public Enemy(
            Texture2D texture,
            Vector2 initialLocation,
            Rectangle initialFrame,
            int frameCount)
        {
            //kreiramo Sprite za neprijateljski brod i dajemo mu parametre prosledjene u konstruktoru
            enemySprite = new Sprite(initialLocation, texture, initialFrame, Vector2.Zero);


            //ucitavamo sve frame-ove osim prvog sa SpriteSheet.png da bi dobili odgovarajucu animaciju
            for (int i = 1; i < frameCount; i++)
            {
                enemySprite.AddFrame(new Rectangle(
                initialFrame.X + (initialFrame.Width * i),
                initialFrame.Y,
                initialFrame.Width,
                initialFrame.Height));
            }

            //inicijalizacija preostalih privatnih atributa
            previousLocation = initialLocation;
            currentWaypoint = initialLocation;
            enemySprite.CollisionRadius = enemyRadius;
        }


        #region Javna svojstva(properties) klase Enemy

        //bice korisena u EnemyManage klasi

        public Sprite Sprite
        {
            get { return enemySprite; }
        }

        public Vector2 GunOffset
        {
            get { return gunOffset; }
        }

        #endregion

        #region Javne metode klase Enemy

        /// <summary>
        /// Metoda pomocu koje ubacujemo nove tacke na putanji u red "waypoints".
        /// </summary>
        /// <param name="waypoint">Tacka na putanji koju treba ubaciti u queue.</param>
        public void AddWaypoint(Vector2 waypoint)
        {
            waypoints.Enqueue(waypoint);
        }

        /// <summary>
        /// Proveravamo da li smo dosli do sledece tacke na putanji. Ovo radimo tako sto merimo
        /// rastojanje izmedju tacke na putanji i naseg sprite-a. Ako je ovo rastojanje manje od
        /// polovine sirine sprite-a (ako je brod dovoljno blizu) metoda vraca TRUE, inace povratna
        /// vrednost je FALSE. 
        /// </summary>
        public bool WaypointReached()
        {
            if (Vector2.Distance(enemySprite.Location, currentWaypoint) < (float)enemySprite.Source.Width/2)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Proverava da li se brod nalazi u okvirima vidljivog dela ekrana, ondnosno
        /// da li treba voditi evidenciju o njemu i iscrtavati ga, ili ne.
        /// </summary>
        public bool IsActive()
        {
            //*BojanADD: (dodali smo destroyed promenljivu koja se kontroliše iz CollisionManager-a)
            if (Destroyed)
                return false;

            if (waypoints.Count > 0)
                return true;

            if (WaypointReached())
                return false;

            return true;
        }

        /// <summary>
        /// Poziva se u okviru Update() metode klase koja koristi ovu klasu (60 puta u sekundi). 
        /// Potrebno je izracunati novi vektor brzine neprijateljskog broda.
        /// </summary>
        /// <param name="gameTime">Vreme proteklo od prethodnog Update() poziva.</param>
        public void Update(GameTime gameTime)
        {
            if (IsActive())
            {
                //racunamo novi vektor pravca
                Vector2 heading = currentWaypoint - enemySprite.Location;
                
                //ukoliko je vektor pravca razlicit od nultog vektora vrsimo njegovu noramlizaciju
                //i tako dobijamo jedinicni vektor pravca
                if (heading != Vector2.Zero)
                    heading.Normalize();

                //vektor brzine dobijamo jednostavnim mnozenjem jedinicnog vektora pravca 
                //odgovarajucim intezitetom
                heading *= speed;

                //prosledjujemo izracunati vektor brzine klasi Sprite
                enemySprite.Velocity = heading;

                //cuvamo trenutnu lokaciju
                previousLocation = enemySprite.Location;

                //pozivamo Update Sprite klase - ovo prouzrokuje kretanje broda
                enemySprite.Update(gameTime);

                //Potrebno je rotirati neprijateljski brod u zavisnosti od pravca u kome se krece.
                //Ugao koji racunamo je ugao izmedju vektora "heading" i "X ose" - a funkcija je Atan2 (mnogo bezbednija od Atan)
                enemySprite.Rotation = (float)Math.Atan2(heading.Y, heading.X);

                //Ako smo dosli do jedne tacke, uzimamo narednu tacku iz reda prema kojoj treba da se krecemo
                if (WaypointReached())
                    if (waypoints.Count > 0)
                        currentWaypoint = waypoints.Dequeue();
            }
        }


        /// <summary>
        /// Poziva se u okviru Draw() metode klase koja koristi ovu klasu. 
        /// Potrebno je iscrtati neprijateljski brod ali samo ukoliko je aktivan - 
        /// ukoliko se nalazi na ekranu.
        /// </summary>
        /// <param name="spriteBatcj">SpriteBatch objekat koji ume da iscrtava teksture.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive())
                enemySprite.Draw(spriteBatch);
        }

        #endregion
    }
}
