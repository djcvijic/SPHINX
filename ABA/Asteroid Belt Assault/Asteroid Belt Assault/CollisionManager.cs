using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; // mora da se doda ručno

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// Klasa koja je sposobna da detektuje kolizije između svih objekata u igri
    /// osim asteroid-asteroid, jer asteroid poseduje svoju logiku za koliziju koja
    /// ukljucuje i bouncing-off. 
    /// </summary>
    public class CollisionManager
    {
        /// <summary>
        /// Referenca na asteroide u sistemu
        /// </summary>
        private Asteroids asteroids;

        private HUD hud;

        /// <summary>
        /// Referenca na igrača
        /// </summary>
        private Ship player1, player2;

        /// <summary>
        /// Referenca na neprijateljske brodove
        /// </summary>
        private EnemyManager enemyManager;

        /// <summary>
        /// Referenca na efekte eksplozije
        /// </summary>
        private ExplosionManager explosionManager;



        /// <summary>
        /// Vektor pozicije kojeg ćemo koristiti kad hoćemo da neki objekat
        /// uklonimo sa ekrana. Kada objekat dobije koordinate koje su van
        /// vidiljivog opsega ekrana, njegova Update metoda će se pobrinuti 
        /// da ga izbriše iz liste aktivnih objekata. Za neprijateljski brod
        /// logika je malo drugačija, potrebno je postaviti njegovo svojstvo
        /// Destroyed na true, da bi IsActive bilo false, nakon čega sledi
        /// brisanje iz liste.
        /// </summary>
        private Vector2 offScreen = new Vector2(-500, -500);


        /// <summary>
        /// Kreira novi objekat <see cref="CollisionManager"/> klase.
        /// </summary>
        /// <param name="asteroids">Referenca na asteroide u sistemu.</param>
        /// <param name="player">Referenca na igrača.</param>
        /// <param name="enemyManager">Referenca na neprijateljske brodove.</param>
        /// <param name="explosionManager">Referenca na efekte eksplozije.</param>
        public CollisionManager(
            Asteroids asteroids,
            Ship p1, 
            Ship p2,
            EnemyManager enemyManager,
            ExplosionManager explosionManager,
            HUD hud)
        {
            this.asteroids = asteroids;
            this.player1 = p1;
            this.player2=p2; 
            this.enemyManager = enemyManager;
            this.explosionManager = explosionManager;
            this.hud = hud;
        }


        /// <summary>
        /// Proverava da li je došlo do kolizije metka koje je ispalio igrač
        /// sa neprijateljskim brodom, nakon čega sledi uništenje neprijatelja.
        /// </summary>
        private void checkShotToEnemyCollisions()
        {
          
            foreach (Sprite shot in player1.ShotManager.Shots)
            {
                foreach (Enemy enemy in enemyManager.Enemies)
                {
                    if (shot.IsCircleColliding(
                        enemy.Sprite.Center,
                        enemy.Sprite.CollisionRadius))
                    {
                        shot.Location = offScreen;
                        enemy.Destroyed = true;
                        hud.Score += 10;
                        explosionManager.AddExplosion(
                            enemy.Sprite.Center,
                            enemy.Sprite.Velocity / 10);
                    }
                }
            }
            foreach (Sprite shot in player2.ShotManager.Shots)
            {
                foreach (Enemy enemy in enemyManager.Enemies)
                {
                    if (shot.IsCircleColliding(
                        enemy.Sprite.Center,
                        enemy.Sprite.CollisionRadius))
                    {
                        shot.Location = offScreen;
                        enemy.Destroyed = true;
                        hud.Score += 10;
                        explosionManager.AddExplosion(
                            enemy.Sprite.Center,
                            enemy.Sprite.Velocity / 10);
                    }
                }
            }
        }

        private void checkShotToAsteroidCollisions()
        {
            // player shots
            foreach (Sprite shot in player1.ShotManager.Shots)
            {
                foreach (Sprite asteroid in asteroids.asteroids)
                {
                    if (shot.IsCircleColliding(
                        asteroid.Center,
                        asteroid.CollisionRadius))
                    {
                        shot.Location = offScreen;
                        explosionManager.AddExplosion(
                            asteroid.Center,
                            asteroid.Velocity / 5);
                        asteroid.Location = offScreen;
                        hud.Score += 5;
                    }
                }
            }
            foreach (Sprite shot in player2.ShotManager.Shots)
            {
                foreach (Sprite asteroid in asteroids.asteroids)
                {
                    if (shot.IsCircleColliding(
                        asteroid.Center,
                        asteroid.CollisionRadius))
                    {
                        shot.Location = offScreen;
                        explosionManager.AddExplosion(
                            asteroid.Center,
                            asteroid.Velocity / 5);
                        asteroid.Location = offScreen;
                        hud.Score += 5;
                    }
                }
            }

            // enemy shots
            foreach (Sprite shot in enemyManager.enemyShotManager.Shots)
            {
                foreach (Sprite asteroid in asteroids.asteroids)
                {
                    if (shot.IsCircleColliding(
                        asteroid.Center,
                        asteroid.CollisionRadius))
                    {
                        shot.Location = offScreen;
                        explosionManager.AddExplosion(
                            asteroid.Center,
                            asteroid.Velocity / 5);
                        asteroid.Location = offScreen;
                    }
                }
            }
        }

        // *** Neprijateljski metak sa igračem,

        /// <summary>
        /// Proverava da li je došlo do kolizije metka koje je ispalio neprijatelj
        /// sa igracem, nakon čega sledi uništenje igraca.
        /// </summary>
        private void checkEnemyShotToPlayerCollision()
        {
            foreach (Sprite shot in enemyManager.enemyShotManager.Shots)
            {


                if (shot.IsCircleColliding(
                    player1.Sprite.Center,
                    player1.Sprite.CollisionRadius))
                {
                    shot.Location = offScreen;
                    player1.Destroyed = true;
                    explosionManager.AddExplosion(
                        player1.Sprite.Center,
                        player1.Sprite.Velocity / 5);
                }
                if (shot.IsCircleColliding(
                    player2.Sprite.Center,
                    player2.Sprite.CollisionRadius))
                {
                    shot.Location = offScreen;
                    player2.Destroyed = true;
                    explosionManager.AddExplosion(
                        player2.Sprite.Center,
                        player2.Sprite.Velocity / 5);
                }

            }
        }


        // *** Neprijateljski brod sa igračem,

        /// <summary>
        /// Proverava da li je došlo do kolizije neprijatelja
        /// sa igracem, nakon čega sledi uništenje igraca i neprijatelja
        /// </summary>
        private void checkEnemyToPlayerCollision()
        {
            foreach (Enemy enemy in enemyManager.Enemies)
            {


                if (enemy.Sprite.IsCircleColliding(
                    player1.Sprite.Center,
                    player1.Sprite.CollisionRadius))
                {
                    enemy.Destroyed = true;
                    player1.Destroyed = true;
                    explosionManager.AddExplosion(
                        player1.Sprite.Center,
                        player1.Sprite.Velocity / 5);
                    explosionManager.AddExplosion(
                        enemy.Sprite.Center,
                        enemy.Sprite.Velocity / 5);
                }
                if (enemy.Sprite.IsCircleColliding(
                    player2.Sprite.Center,
                    player2.Sprite.CollisionRadius))
                {
                    enemy.Destroyed = true;
                    player2.Destroyed = true;
                    explosionManager.AddExplosion(
                        player2.Sprite.Center,
                        player2.Sprite.Velocity / 5);
                    explosionManager.AddExplosion(
                        enemy.Sprite.Center,
                        enemy.Sprite.Velocity / 5);
                }

            }
        }
        // *** Asteroid sa igračem

        /// <summary>
        /// Proverava da li je došlo do kolizije metka koje je ispalio igrač
        /// sa asteroidom, nakon čega sledi uništenje asteroida i igraca
        /// </summary>
        private void checkAsteroidsToPlayerCollisions()
        {
            foreach (Sprite asteroid in asteroids.asteroids)
            {
                if (asteroid.IsCircleColliding(
                    player1.Sprite.Center,
                    player1.Sprite.CollisionRadius))
                {
                    /*player1.Destroyed = true;
                    explosionManager.AddExplosion(
                        player1.Sprite.Center,
                        player1.Sprite.Velocity / 5);
                    explosionManager.AddExplosion(
                        asteroid.Center,
                        asteroid.Velocity / 5);
                    asteroid.Location = offScreen;*/

                    asteroid.Velocity = player1.Sprite.Velocity * 1.5f;
                }
                if (asteroid.IsCircleColliding(
                    player2.Sprite.Center,
                    player2.Sprite.CollisionRadius))
                {
                    player2.Destroyed = true;
                    explosionManager.AddExplosion(
                        player2.Sprite.Center,
                        player2.Sprite.Velocity / 5);
                    explosionManager.AddExplosion(
                        asteroid.Center,
                        asteroid.Velocity / 5);
                    asteroid.Location = offScreen;
                }
            }
        }


        /// <summary>
        /// Proverava da li je došlo do kolizije neprijatelja
        /// sa asteroidom, nakon čega sledi uništenje asteroida i neprijatelja
        /// </summary>
        private void checkEnemyToAsteroidCollision()
        {
            foreach (Enemy enemy in enemyManager.Enemies)
            {
                foreach (Sprite asteroid in asteroids.asteroids)
                {
                    if (enemy.Sprite.IsCircleColliding(
                        asteroid.Center,
                        asteroid.CollisionRadius))
                    {
                        enemy.Destroyed = true;
                        asteroid.Location = offScreen;
                        explosionManager.AddExplosion(
                            asteroid.Center,
                            asteroid.Velocity / 5);
                        explosionManager.AddExplosion(
                            enemy.Sprite.Center,
                            enemy.Sprite.Velocity / 5);
                    }
                }

            }
        }



        /// <summary>
        /// Proverava sve kolizije u sistemu i preduzima odgovarajuče akcije, za sada samo jedna opcija.
        /// </summary>
        public void CheckCollisions()
        {
            checkShotToEnemyCollisions();
            checkShotToAsteroidCollisions(); // <--- proverava i metke igraca i metke neprijatelja
            checkEnemyShotToPlayerCollision();
            checkEnemyToPlayerCollision();
            checkAsteroidsToPlayerCollisions();
            checkEnemyToAsteroidCollision();
        }
    }
}
