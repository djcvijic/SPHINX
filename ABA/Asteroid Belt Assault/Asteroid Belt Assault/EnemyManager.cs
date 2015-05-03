using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // mora da se doda ručno
using Microsoft.Xna.Framework.Graphics; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{
    public class EnemyManager
    {
        #region Privatni atributi klase
        // Keširamo sledeća 3 atributa radi lakšeg pravljenja neprijateljskih brodova (kao kod ShotManager-a).
        //************************************************************
        /// <summary>
        /// Tekstura ili SpriteSheet za (animirani) brod.
        /// </summary>
        private Texture2D texture;
        /// <summary>
        /// Početni frejm u animaciji (u odnosu na SpriteSheet).
        /// </summary>
        private Rectangle initialFrame;
        /// <summary>
        /// Ukupan broj frejmova u animaciji.
        /// </summary>
        private int frameCount;
        //************************************************************

        /// <summary>
        /// Lista neprijatelja trenutno aktivnih u igri.
        /// </summary>
        private List<Enemy> enemies = new List<Enemy>();

        /// <summary>
        /// Treba nam jedan ShotManager jer neprijatelji imaju mogućnost pucanja (kao kod Ship klase).
        /// </summary>
        public ShotManager enemyShotManager;

        /// <summary>
        /// Potreban nam je i igrač (Ship objekat) jer su nam potrebne njegove koordinate kako bi
        /// neprijatelji mogli da pucaju ka njemu.
        /// </summary>
        private Ship player1, player2;

        // Brojači:
        // Minimalan i maksimalan broj neprijatelja po jednom talasu (uzećemo neki broj iz ovog opsega pomoću Random objekta).
        //************************************************************
        private int minShipsPerWave = 5;
        private int maxShipsPerWave = 8;
        //***********************************************************

        // Tajmeri:
        // Postoje dva tajmera: jedan meri koliko vremena je prošlo od prethodnog talasa neprijatelja,
        //                      drugi meri koliko vremena je prošlo od prethodnog spawnovanja (stvaranja)
        //                                                             neprijatelja u okviru tog talasa.
        // Tu je i treshhold za oba tajmera (koliko minimalno vremena mora da protekne izmedju dva talasa,
        // odnosno dva spawn-a.
        private float nextWaveTimer = 0.0f;
        private float nextWaveWaitTime = 8.0f; // 8 sekuundi
        private float shipSpawnTimer = 0.0f;
        private float shipSpawnWaitTime = 0.5f; // pola sekunde

        /// <summary>
        /// Verovatnoća pucanja neprijatelja na igrača.
        /// </summary>
        /// U okviru svakog Update() poziva, uzimamo random vrednost od 0-100 i pitamo da li je manja od shipShotChance. 
        /// Ukoliko nasumično izabrana vrednost bude bila manja od 0.2, neprijatelj će ispaliti metak na igrača. 
        /// Vrednost 0.2 se možda čini veoma malom verovatnoćnom, ali Update() se poziva otprilike oko 60 puta u sekundi, 
        /// pa je, praktično, šansa da neprijatelj ispali metak u jednoj sekundi oko 12% (a tih neprijatelja ima 5-8 po talasu).
        private float shipShotChance = 0.2f;

        /// <summary>
        /// Lista svih putanja koje neprijatelj može da ima.
        /// </summary>
        /// Pošto je putanja praktično lista koordinata (waypoint-a), onda imamo listu listi Vector2 objekata.
        private List<List<Vector2>> pathWaypoints = new List<List<Vector2>>();

        /// <summary>
        /// Skup vrednosti key->value gde je key redni broj putanje, a key broj neprijatelja 
        /// koje treba stvoriti na toj putanji.
        /// </summary>
        /// Sa MSDN: The Dictionary(Of TKey, TValue) generic class provides a mapping from a set of keys to a set of values. 
        /// Each addition to the dictionary consists of a value and its associated key. 
        /// Retrieving a value by using its key is very fast, close to O(1), 
        /// because the Dictionary(Of TKey, TValue) class is implemented as a hash table.
        private Dictionary<int,int> waveSpawns = new Dictionary<int,int>();

        //private bool active = true;

        private Random rand = new Random();
        #endregion

        //*BojanADD: treba nam jer ćemo morati da mu pristupimo iz CollisionManager-a
        public List<Enemy> Enemies
        {
            get { return enemies; }
        }

        //public bool Active
        //{
        //    get { return active; }
        //    set { active = value; }
        //}

        /// <summary>
        /// Sets up waypoints.
        /// </summary>
        /// Za svaku od 4 moguće putanje koje smo predvideli da neprijatelj može da koristi,
        /// popunjavamo pathWaypoints listu. Dakle potrebno je napraviti novu putanju, dodati
        /// vektore koje će činiti waypoint-e za tu putanju, i upravo napravljenu putanju dodati
        /// u listu putanji pathWaypoints. Za svaku putanju se inicijalizuje broj neprijatelja 
        /// trenutno na njoj na 0.
        private void setUpWaypoints()
        {
            List<Vector2> path0 = new List<Vector2>();
            path0.Add(new Vector2(850, 300));
            path0.Add(new Vector2(-100, 300));
            pathWaypoints.Add(path0);
            waveSpawns[0] = 0;

            List<Vector2> path1 = new List<Vector2>();
            path1.Add(new Vector2(-50, 225));
            path1.Add(new Vector2(850, 225));
            pathWaypoints.Add(path1);
            waveSpawns[1] = 0;

            List<Vector2> path2 = new List<Vector2>();
            path2.Add(new Vector2(-100, 50));
            path2.Add(new Vector2(150, 50));
            path2.Add(new Vector2(200, 75));
            path2.Add(new Vector2(200, 125));
            path2.Add(new Vector2(150, 150));
            path2.Add(new Vector2(150, 175));
            path2.Add(new Vector2(200, 200));
            path2.Add(new Vector2(600, 200));
            path2.Add(new Vector2(850, 600));
            pathWaypoints.Add(path2);
            waveSpawns[2] = 0;

            List<Vector2> path3 = new List<Vector2>();
            path3.Add(new Vector2(600, -100));
            path3.Add(new Vector2(600, 250));
            path3.Add(new Vector2(580, 275));
            path3.Add(new Vector2(500, 250));
            path3.Add(new Vector2(500, 200));
            path3.Add(new Vector2(450, 175));
            path3.Add(new Vector2(400, 150));
            path3.Add(new Vector2(-100, 150));
            pathWaypoints.Add(path3);
            waveSpawns[3] = 0;
        }

        /// <summary>
        /// Stvara novu instancu <see cref="EnemyManager"/> klase.
        /// </summary>
        /// <param name="texture">Tekstura (sprite sheet) za neprijateljski brod.</param>
        /// <param name="initialFrame">Početni frejm u animaciji.</param>
        /// <param name="frameCount">Ukupna broj frejmova.</param>
        /// <param name="player">Ship (igrač).</param>
        /// <param name="screenBounds">Granice vidljive površine ekrana za igru (treba nam za ShotManager-a).</param>
        public EnemyManager(
            Texture2D texture,
            Rectangle initialFrame,
            int frameCount,
            Ship p1,
            Ship p2,
            Rectangle screenBounds)
        {
            // Keširamo potrebne vrednosti za kasnije kreiranje neprijateljskih brodova
            this.texture = texture;
            this.initialFrame = initialFrame;
            this.frameCount = frameCount;
            this.player1 = p1;
            this.player2 = p2;

            // Kreiramo ShotManager
            enemyShotManager = new ShotManager(
                texture, // tekstura za metak (na istom je spriteSheetu kao i neprijatelj 
                         // pa smo prosleđujemo ref.)
                new Rectangle(0, 300, 5, 5), // frejm na spritesheetu
                4, // broj frejmova u anim.
                2, // collisionRadius za metak (DODATO!)
                150f, // intenzitet brzine metka
                screenBounds,
                false);

            setUpWaypoints();
        }

        /// <summary>
        /// Stvara neprijatelja na putanji path.
        /// </summary>
        /// <param name="path">Redni broj putanje u pathWaypoints na kome treba da se stvori
        /// neprijatelj.</param>
        private void spawnEnemy(int path)
        {
            Enemy thisEnemy = new Enemy(
                texture,
                pathWaypoints[path][0], // inicijalna lokacija: prvi waypoint na putanji
                initialFrame,
                frameCount);

            // Dodaju se svi waypoint-i u upravo napravljeni Enemy objekat
            // da bi znao kuda treba da se kreće.
            for (int i = 0; i < pathWaypoints[path].Count(); i++)
            {
                thisEnemy.AddWaypoint(pathWaypoints[path][i]);
            }
            enemies.Add(thisEnemy); // Dodajemo neprijatelja u našu listu trenutno aktivnih neprijatelja
                                    // u igri.
        }


        /// <summary>
        /// Startuje talas neprijatelja.
        /// </summary>
        /// <param name="waveType">Broj talasa (indeks putanje na kojoj hoćemo
        /// da se stratuje talas).</param>
        private void spawnWave(int waveType)
        {
            waveSpawns[waveType] +=
                rand.Next(minShipsPerWave, maxShipsPerWave + 1);
        }


        /// <summary>
        /// Update-uje talase neprijatelja i same neprijatelje u okviru tog talasa.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// Vodi računa o onim tajmerima i na osnovu njih startuje talase
        /// i spawn-uje neprijatelje. Ovaj "privatni" update() će se pozivati u okviru onog
        /// pravog Update() na koji smo navikli.
        private void updateWaveSpawns(GameTime gameTime)
        {
            shipSpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds; // akumuliramo proteklo vreme
            if (shipSpawnTimer > shipSpawnWaitTime) // ako je prošao minimalni zahtevani interval za neprijatelje
            {
                for (int i = waveSpawns.Count - 1; i >= 0; i--) // proveravamo za sve putanje
                {
                    if (waveSpawns[i] > 0) // da li je potrebno stvoriti još nekog neprijatelja na putanji
                    {
                        waveSpawns[i]--; // dekrementiraj za jedan, jer ćemo upravo sada stvoriti neprijatelja
                        spawnEnemy(i); // stvori neprijatelja na toj putanji
                    }
                }
                shipSpawnTimer = 0f; // resetuj tajmer, pa broj ispočetka
            }

            nextWaveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds; // akumuliramo proteklo vreme
            if (nextWaveTimer > nextWaveWaitTime) // ako je prošao minimalni zahtevni interval za talase 
            {
                spawnWave(rand.Next(0, pathWaypoints.Count)); // startuj talas
                nextWaveTimer = 0f; // resetuj tajmer, broj ispočetka
            }
        }


        /// <summary>
        /// Poziva se 60 puta u sekundi iz klase koja koristi ovu klasu (Game.cs).
        /// U njoj takođe treba pozvate sve Update() svih klasa koje koristi ova klasa,
        /// a koje u sebi imaju neki Sprite, tj. Update() metodu.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public void Update(GameTime gameTime)
        {
            enemyShotManager.Update(gameTime);

            for (int i = enemies.Count - 1; i >= 0; i--) // prolazimo kroz sve neprijatelje
            {
                enemies[i].Update(gameTime); // pozivamo njihov Update()
                if (enemies[i].IsActive() == false) // ukoliko im je stanje sada postalo neaktivno, brišemo ih
                {
                    enemies.RemoveAt(i);
                }
                else // u suprotnom, brod ispaljuje metak prema igraču sa verovatnoćom shipShotChance.
                {
                    if ((float)rand.Next(0, 1000) / 10 <= shipShotChance) // ako je verovatnoća ispunjena
                    {
                     

                        Vector2 fireLoc = enemies[i].Sprite.Location; // koordinate neprijatelja
                        fireLoc += enemies[i].GunOffset; // gde se pojavljuje metak (treba nam za ShotManager.FireShot())
                        Ship meta;
                        if (rand.Next(0, 100) < 50 && player1.IsActive())
                            meta = player1;
                        else if (player2.IsActive())
                            meta = player2;
                        else
                            meta = player1;

                        Vector2 shotDirection = meta.Sprite.Center - fireLoc; // pravac u kome treba pucati (treba nam za ShotManager.FireShot())
                        shotDirection.Normalize(); // normalizacija, jer nam je potreban samo jednični pravac, koji se kasnije množi sa intenzitetom (speed) metka.

                        enemyShotManager.FireShot( // ispaljujemo metak
                            fireLoc,
                            shotDirection);
                    }
                }
            }

            //if (Active)
            updateWaveSpawns(gameTime); // pozivamo onaj "privatni" update() iznad koji nam vrši logiku vezanu za tajmere, talase, neprijatelje... 
        }

        /// <summary>
        /// Iscrtava sve neprijatelje na ekranu.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            enemyShotManager.Draw(spriteBatch);
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
    }
}
