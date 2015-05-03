using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics; // mora da se doda ručno
using Microsoft.Xna.Framework; // mora da se doda ručno
using Microsoft.Xna.Framework.Audio; // mora da se doda ručno
using Microsoft.Xna.Framework.Content; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// Javna statička klasa uz pomoću koje ćemo koristiti zvučne efekte u
    /// video igri. Klasa je static da bi svaka druga klasa u sistemu mogla da joj
    /// pristupi bez prethodnog instaciranja.
    /// </summary>
    public static class SoundManager
    {
        /// <summary>
        /// Lista zvučnih efekata koje ćemo koristiti.
        /// </summary>
        private static List<SoundEffect> explosions = new List<SoundEffect>();

        /// <summary>
        /// Broj zvučnih efekata eksplozije.
        /// </summary>
        private static int explosionCount = 4;

        /// <summary>
        /// Zvučni efekat za pucanje igrača.
        /// </summary>
        private static SoundEffect playerShot;

        /// <summary>
        /// Zvučni efekat za pucanje neprijatelja.
        /// </summary>
        private static SoundEffect enemyShot;

        /// <summary>
        /// Objekat uz pomoću kojeg ćemo da aktiviramo jedan od 4
        /// moguća zvučna efekta ekplozije.
        /// </summary>
        private static Random rand = new Random();

        /// <summary>
        /// Inicijalizacija resursa (poziva se iz Game1 klase).
        /// Povezuje .wav fajlove koji predstavljaju zvučni efekat
        /// sa promenljivama u klasi.
        /// </summary>
        /// <param name="content">The content.</param>
        public static void Initialize(ContentManager content)
        {
            try
            {
                playerShot = content.Load<SoundEffect>(@"Sounds\Shot1");
                enemyShot = content.Load<SoundEffect>(@"Sounds\Shot2");

                /* dodaj sve zvučne efekte za ekplozije u listu */
                for (int i = 1; i <= explosionCount; i++)
                {
                    explosions.Add(
                        content.Load<SoundEffect>(@"Sounds\Explosion" + i.ToString()));
                }
            }
            catch
            {
                Debug.Write("SoundManager Initialization Failed");
            }
        }

        /// <summary>
        /// Pušta jedan od četiri moguća zvuk eksplozije.
        /// </summary>
        public static void PlayExplosion()
        {
            try
            {
                explosions[rand.Next(0, explosionCount)].Play();
            }
            catch
            {
                Debug.Write("PlayExplosion Failed");
            }
        }

        /// <summary>
        /// Pušta zvuk pucanja igrača.
        /// </summary>
        public static void PlayPlayerShot()
        {
            try
            {
                playerShot.Play();
            }
            catch
            {
                Debug.Write("PlayPlayerShot Failed");
            }
        }

        /// <summary>
        /// Pušta zvuk pucanja neprijatelja.
        /// </summary>
        public static void PlayEnemyShot()
        {
            try
            {
                enemyShot.Play();
            }
            catch
            {
                Debug.Write("PlayEnemyShot Failed");
            }
        }

    }
}
