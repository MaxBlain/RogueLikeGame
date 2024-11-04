using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
    internal class EnemySpawning
    {
        private int spawnTimer;
        private int spawnAmount = 1;
        private int lastKillCount = 0;

        private Vector2 position;

        private Texture2D zombieStatic;
        private Texture2D zombieSpriteSheet;
        private Texture2D slimeStatic;
        private Texture2D slimeSpriteSheet;
        private Texture2D skeletonStatic;
        private Texture2D skeletonSpriteSheet;

        private GraphicsDevice graphics;

        private Vector2 zombieSize;
        private Vector2 slimeSize;
        private Vector2 skeletonSize;

        public EnemySpawning(Texture2D zombieStatic, Texture2D zombieSpriteSheet, Texture2D slimeStatic, Texture2D slimeSpriteSheet, Texture2D skeletonStatic, Texture2D skeletonSpriteSheet, GraphicsDevice graphics)
        {
            this.zombieStatic = zombieStatic;
            this.zombieSpriteSheet = zombieSpriteSheet;
            this.slimeStatic = slimeStatic;
            this.slimeSpriteSheet = slimeSpriteSheet;
            this.skeletonStatic = skeletonStatic;
            this.skeletonSpriteSheet = skeletonSpriteSheet;

            this.graphics = graphics;

            zombieSize = new Vector2(23, 37);
            slimeSize = new Vector2(26, 18);
            skeletonSize = new Vector2(21, 34);

            spawnTimer = 0;
        }

        public Rectangle ScreenBounds
        {
            get
            {
                return new Rectangle(
                    -200,
                    -100,
                    graphics.Adapter.CurrentDisplayMode.Width + 400,
                    graphics.Adapter.CurrentDisplayMode.Height + 200
                    );
            }
        }

        public virtual void Update(GameTime gameTime, List<Enemy> enemySprites, int totalEnemyKills)
        {
            spawnTimer++;
            if (totalEnemyKills % 100 == 0 && totalEnemyKills != lastKillCount)
            {
                spawnAmount++;
                lastKillCount = totalEnemyKills;
            }
            if (spawnTimer > 5 + (enemySprites.Count * 3 - totalEnemyKills / 10))
            {
                for (int i = 0; i < spawnAmount; i++)
                {
                    int perimiter = (this.ScreenBounds.Width * 2) + (this.ScreenBounds.Height * 2);
                    Random randomPos = new Random();
                    int spawnPostition = randomPos.Next(perimiter);
                    if (spawnPostition < ScreenBounds.Width)
                    {
                        position.X = spawnPostition;
                        position.Y = ScreenBounds.Top;
                    }
                    else if (spawnPostition < ScreenBounds.Width + ScreenBounds.Height)
                    {
                        position.X = ScreenBounds.Right;
                        position.Y = spawnPostition - ScreenBounds.Width;
                    }
                    else if (spawnPostition < ScreenBounds.Width * 2 + ScreenBounds.Height)
                    {
                        position.X = spawnPostition - ScreenBounds.Width - ScreenBounds.Height;
                        position.Y = ScreenBounds.Bottom;
                    }
                    else
                    {
                        position.X = ScreenBounds.Left;
                        position.Y = spawnPostition - ScreenBounds.Width * 2 - ScreenBounds.Height;
                    }

                    Random randomChar = new Random();
                    int intCharacter = randomChar.Next(3);
                    switch (intCharacter)
                    {
                        case 0:
                            enemySprites.Add(new Enemy(zombieStatic, new Vector2(position.X, position.Y), Color.White, zombieSize, "zombie", new Vector2(0, 0), zombieSpriteSheet));
                            break;
                        case 1:
                            enemySprites.Add(new Enemy(slimeStatic, new Vector2(position.X, position.Y), Color.White, slimeSize, "slime", new Vector2(0, 0), slimeSpriteSheet));
                            break;
                        case 2:
                            enemySprites.Add(new Enemy(skeletonStatic, new Vector2(position.X, position.Y), Color.White, skeletonSize, "skeleton", new Vector2(0, 0), skeletonSpriteSheet));
                            break;
                    }
                }
                spawnTimer = 0;
                Debug.WriteLine("Spawned at : " + position);
            }
        }
    }
}
