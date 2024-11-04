using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;

namespace RogueLikeGame
{
    public class RogueLikeGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D playerSpriteSheet;

        Texture2D zombieStatic;
        Texture2D zombieSpriteSheet;
        Texture2D slimeStatic;
        Texture2D slimeSpriteSheet;
        Texture2D skeletonStatic;
        Texture2D skeletonSpriteSheet;

        Texture2D floorTexture;

        Texture2D levelUpCard;
        List<Rectangle> levelUpRectangles;
        int[,] levelUpLists;
        string[] levelUpStrings;
        SpriteFont font;

        // Create sprites
        Player player;
        List<Enemy> enemySprites;

        // Weapons
        List<Missile> arrows;
        List<Missile> magic;

        Vector2 magicSize;
        Vector2 arrowSize;

        int healthRegenCounter = 0;
        int healthRegen = 1;
        int globalGracePeriod = 0;

        EnemySpawning enemySpawning;

        DrawBackground drawBackground;

        int totalEnemyKills = 0;
        int levelingRequirement = 10;
        int levelingCounter = 0;

        bool paused = false;
        bool gameOver = false;

        public RogueLikeGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load floor texture and draw
            floorTexture = Content.Load<Texture2D>("floor");
            drawBackground = new DrawBackground(_graphics);

            // Load level up textures
            levelUpCard = Content.Load<Texture2D>("level up card");
            levelUpRectangles = new List<Rectangle>();
            for (int i = 1; i < 4; i++)
            {
                levelUpRectangles.Add(new Rectangle((GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2) - (GraphicsDevice.Adapter.CurrentDisplayMode.Width / 8),
                    ((GraphicsDevice.Adapter.CurrentDisplayMode.Height / 4) * i) - (GraphicsDevice.Adapter.CurrentDisplayMode.Height / 16),
                    GraphicsDevice.Adapter.CurrentDisplayMode.Width / 4,
                    GraphicsDevice.Adapter.CurrentDisplayMode.Height / 8));
            }
            font = Content.Load<SpriteFont>("fonts/font");

            // List containing enemy sprites and creating enemy textures
            enemySprites = new List<Enemy>();
            zombieStatic = Content.Load<Texture2D>("zombie static");
            zombieSpriteSheet = Content.Load<Texture2D>("zombie sprite sheet");
            slimeStatic = Content.Load<Texture2D>("slime static");
            slimeSpriteSheet = Content.Load<Texture2D>("slime sprite sheet");
            skeletonStatic = Content.Load<Texture2D>("skeleton static");
            skeletonSpriteSheet = Content.Load<Texture2D>("skeleton sprite sheet");

            // Player textures
            Texture2D playerStatic = Content.Load<Texture2D>("player static");
            playerSpriteSheet = Content.Load<Texture2D>("player sprite sheet");

            // Changeable player settings
            string character = "wizard";

            // Sprite sizes
            Vector2 playerSize = new Vector2(34, 44);

            // Missile sprite sizes
            magicSize = new Vector2(5, 5);
            arrowSize = new Vector2(5, 3);

            // Create the player sprite and the load the starting weapon
            player = new Player(playerStatic, new Vector2(((GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2) - (playerStatic.Width * 2)), ((GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2) - (playerStatic.Height * 2))), Color.White, playerSize, character, playerSpriteSheet);
            arrows = new List<Missile>();
            magic = new List<Missile>();

            // Load enemy spawning
            enemySpawning = new EnemySpawning(zombieStatic, zombieSpriteSheet, slimeStatic, slimeSpriteSheet, skeletonStatic, skeletonSpriteSheet, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (paused == false)
            {
                // Update player
                player.Update(gameTime);

                // Update enemies
                foreach (Enemy enemy in enemySprites)
                {
                    EnemyTick(gameTime, enemy);
                    enemy.Update(gameTime);
                }

                //Player health check and regen
                if (player.health <= 0)
                {
                    paused = true;
                    gameOver = true;
                }
                HealthRegen();

                // Weapon logic and enemy health
                foreach (Weapon weapon in player.weapons)
                {
                    switch (weapon.name)
                    {
                        case "sword":
                            MeleeHit(weapon);
                            break;

                        case "staff":
                            ShootStaff(weapon);
                            KillOutofBounds(magic);
                            break;

                        case "bow":
                            ShootBow(weapon);
                            KillOutofBounds(arrows);
                            break;
                    }
                }

                // Update Spawn timer
                enemySpawning.Update(gameTime, enemySprites, totalEnemyKills);

                KillEnemies();
            }
            else
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    ApplyLevelUp();
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw player and enemies
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            drawBackground.Draw(_spriteBatch, floorTexture);

            player.Draw(_spriteBatch);

            foreach (Enemy enemy in enemySprites)
            {
                enemy.Draw(_spriteBatch);
            }

            foreach (Weapon weapon in player.weapons)
            {
                switch (weapon.name)
                {
                    case "sword":
                        break;

                    case "staff":
                        // Draw any missiles
                        foreach (Missile orb in magic)
                        {
                            orb.Draw(_spriteBatch);
                        }
                        break;

                    case "bow":
                        // Draw any arrows
                        foreach (Missile arrow in arrows)
                        {
                            arrow.Draw(_spriteBatch);
                        }
                        break;
                }
            }

            _spriteBatch.DrawString(font, "Total Kills : " + totalEnemyKills, new Vector2(20, 20), Color.White);
            _spriteBatch.DrawString(font, "Health : " + player.health + " / " + player.maxHealth, new Vector2(20, 140), Color.White);
            _spriteBatch.DrawString(font, "Kills until next level up : " + (levelingRequirement - levelingCounter), new Vector2(20, 80), Color.White);

            if (paused == true && gameOver == false)
            {
                for (int i = 0; i < 3; i++)
                {
                    _spriteBatch.Draw(levelUpCard, levelUpRectangles[i], Color.White);
                    _spriteBatch.DrawString(font, WrapText(font, levelUpStrings[i], (float)(levelUpRectangles[i].Width * 0.8)), new Vector2(levelUpRectangles[i].Left + (float)(levelUpRectangles[i].Width * 0.1), levelUpRectangles[i].Top + (float)(levelUpRectangles[i].Height * 0.15)), Color.White);
                }
            }
            else if (gameOver == true)
            {
                _spriteBatch.DrawString(font, "GAME OVER", new Vector2((GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2) - 128, (GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2) - 128), Color.Red);
                _spriteBatch.DrawString(font, "Press Esc to Exit", new Vector2((GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2) - 182, (GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2) - 64), Color.Red);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // Calculate velocity of object A towards object B with given speed
        private Vector2 TrackingAtoB(float aX, float aY, float bX, float bY, int speed)
        {
            double angle = Math.Atan((bY - aY) / (bX - aX));
            if (bX > aX)
            {
                Vector2 velocity = new Vector2((float)(Math.Cos(angle) * speed), (float)(Math.Sin(angle) * speed));
                return velocity;
            }
            else
            {
                Vector2 velocity = new Vector2((float)(Math.Cos(angle) * -speed), (float)(Math.Sin(angle) * -speed));
                return velocity;
            }
        }

        // Find the closest target which a bullet will aim for
        private int ClosestTarget(Vector2 position)
        {
            int target = 0;
            float distance = 99999;
            for (int i = 0; i < enemySprites.Count; i++)
            {
                float distanceTemp = Vector2.Distance(position, enemySprites[i].position);
                if (distanceTemp < distance)
                {
                    distance = distanceTemp;
                    target = i;
                }
            }
            return target;
        }

        // Applies projectile logic
        private void Projectile(List<Missile> missiles, Weapon weapon)
        {
            foreach (Missile missile in missiles.ToList())
            {
                missile.position += missile.velocity;
                DamageEnemies(weapon, missile, missiles);
            }
        }

        // Applies melle logic
        private void MeleeHit(Weapon weapon)
        {

        }

        // Check enemy health and kill if 0
        private void KillEnemies()
        {
            List<Sprite> killList = new();
            foreach (Enemy enemy in enemySprites)
            {
                if (enemy.health <= 0)
                {
                    killList.Add(enemy);
                    totalEnemyKills++;
                    levelingCounter++;
                    if (levelingCounter >= levelingRequirement)
                    {
                        levelingRequirement = (int)(levelingRequirement * 1.2);
                        levelingCounter = 0;
                        LevelUp();
                    }
                }
            }
            foreach (Enemy enemy in killList)
            {
                enemySprites.Remove(enemy);
            }
        }

        // Damage enemies hit by bullets
        private void DamageEnemies(Weapon weapon, Missile missile, List<Missile> missiles)
        {
            List<Sprite> killList = new();
            foreach (Enemy enemy in enemySprites)
            {
                if (enemy.Rect.Contains(missile.Rect.Center))
                {
                    killList.Add(missile);
                    enemy.health -= weapon.damage;
                }
            }
            EmptyKillList(killList, missiles);
        }

        // Kill out of bounds bullets
        private void KillOutofBounds(List<Missile> missiles)
        {
            List<Sprite> killList = new();
            foreach (Missile missile in missiles)
            {
                // Kill the bullet if it goes off screen
                if (missile.Rect.Right < 0 | missile.Rect.Left > GraphicsDevice.Adapter.CurrentDisplayMode.Width | missile.Rect.Bottom < 0 | missile.Rect.Top > GraphicsDevice.Adapter.CurrentDisplayMode.Height)
                {
                    killList.Add(missile);
                }
            }
            EmptyKillList(killList, missiles);
        }

        // Empty the kill list of missiles
        private void EmptyKillList(List<Sprite> killList, List<Missile> missiles)
        {
            foreach (Missile missile in killList)
            {
                missiles.Remove(missile);
            }
        }

        // Club logic
        private void MeleeHit()
        {

        }

        // Staff logic
        private void ShootStaff(Weapon weapon)
        {
            if (weapon.reloadCounter >= weapon.reloadSpeed && enemySprites.Count > 0)
            {
                // Create a new missile which to be shot
                magic.Add(new Missile(Content.Load<Texture2D>("orb"), new Vector2(player.Rect.Center.X, player.Rect.Center.Y), Color.White, magicSize, "orb"));
                magic.Last().target = ClosestTarget(player.position);
                magic.Last().velocity = TrackingAtoB(magic.Last().position.X, magic.Last().position.Y, enemySprites[magic.Last().target].Rect.Center.X, enemySprites[magic.Last().target].Rect.Center.Y, weapon.projectileSpeed);
                weapon.reloadCounter = 0;
            }
            Projectile(magic, weapon);
            weapon.reloadCounter++;
        }

        // Bow logic
        private void ShootBow(Weapon weapon)
        {
            if (weapon.reloadCounter >= weapon.reloadSpeed)
            {
                // Create a new missile which to be shot with correct orientation
                if (player.facing == "l" | player.facing == "ls")
                {
                    arrows.Add(new Missile(Content.Load<Texture2D>("arrow left"), new Vector2(player.Rect.Center.X, player.Rect.Center.Y), Color.White, arrowSize, "arrow"));
                    arrows.Last().velocity = new Vector2(-weapon.projectileSpeed, 0);
                }
                else if (player.facing == "r" | player.facing == "rs")
                {
                    arrows.Add(new Missile(Content.Load<Texture2D>("arrow right"), new Vector2(player.Rect.Center.X, player.Rect.Center.Y), Color.White, arrowSize, "arrow"));
                    arrows.Last().velocity = new Vector2(weapon.projectileSpeed, 0);
                }
                weapon.reloadCounter = 0;
            }
            Projectile(arrows, weapon);
            weapon.reloadCounter++;
        }

        // Player health regen
        private void HealthRegen()
        {
            healthRegenCounter++;
            if (player.health != player.maxHealth && healthRegenCounter >= player.healthRegenRate)
            {
                player.health += healthRegen;
                healthRegenCounter = 0;
                if (player.health > player.maxHealth)
                {
                    player.health = player.maxHealth;
                }
            }
        }

        // Enemy update
        private void EnemyTick(GameTime gameTime, Enemy enemy)
        {
            // Update grace period
            if (enemy.gracePeriod < enemy.gracePeriodLength)
            {
                enemy.gracePeriod++;
            }
            if (globalGracePeriod < 5)
            {
                globalGracePeriod++;
            }

            // Move enemies towards player until collision
            if (enemy.Rect.Contains(player.Rect.Center) == false)
            {
                Vector2 velocity = TrackingAtoB(enemy.Rect.Center.X, enemy.Rect.Center.Y, player.Rect.Center.X, player.Rect.Center.Y, enemy.speed);
                enemy.target = new Vector2(player.Rect.Center.X, player.Rect.Center.Y);
                enemy.position += velocity;
                enemy.Update(gameTime);
            }

            if (enemy.facing.Contains("s") != true && enemy.Rect.Contains(player.Rect.Center) == true)
            {
                enemy.facing += "s";
            }
            else if (enemy.facing.Contains("s") == true && enemy.Rect.Contains(player.Rect.Center) == false)
            {
                enemy.facing = enemy.facing.Remove(1);
            }

            // Collision logic
            if (enemy.Rect.Contains(player.Rect.Center) && enemy.gracePeriod == enemy.gracePeriodLength && globalGracePeriod == 5)
            {
                player.health -= enemy.damage;
                enemy.gracePeriod = 0;
                globalGracePeriod = 0;
            }
        }

        // Level up card choices
        private void LevelUp()
        {
            paused = true;
            levelUpStrings = new string[3];
            levelUpLists = new int[3, 3];
            Random random = new Random();

            // Generate list of 3 differnt random upgrades
            while (levelUpStrings[0] == levelUpStrings[1] | levelUpStrings[0] == levelUpStrings[2] | levelUpStrings[1] == levelUpStrings[2])
            {
                for (int i = 0; i < 3; i++)
                {
                    // Weapon or player to be upgraded
                    levelUpLists[0, i] = random.Next(player.weapons.Count + 1);
                    // Upgrade type
                    if (levelUpLists[0, i] == 0)
                    {
                        levelUpLists[1, i] = random.Next(4);
                    }
                    else
                    {
                        levelUpLists[1, i] = random.Next(3);
                    }
                    // Rarity of upgrade and uniqe outcome if new weapon
                    if (levelUpLists[0, i] <= player.weapons.Count - 1 && levelUpLists[1, i] == 3)
                    {
                        levelUpLists[2, i] = random.Next(availableWeapons().Count);
                    }
                    else
                    {
                        levelUpLists[2, i] = random.Next(3);
                    }

                    // Weapn Upgrades
                    if (levelUpLists[0, i] <= player.weapons.Count - 1)
                    {
                        switch (levelUpLists[1, i])
                        {
                            // Damage
                            case 0:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        levelUpStrings[i] = "Increase " + player.weapons[levelUpLists[0, i]].name + " damage by 10%";
                                        break;
                                    case 2:
                                        levelUpStrings[i] = "Increase " + player.weapons[levelUpLists[0, i]].name + " damage by 20%";
                                        break;
                                }
                                break;
                            // Reload Speed
                            case 1:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        levelUpStrings[i] = "Increase " + player.weapons[levelUpLists[0, i]].name + " reload speed by 10%";
                                        break;
                                    case 2:
                                        levelUpStrings[i] = "Increase " + player.weapons[levelUpLists[0, i]].name + " reload speed by 20%";
                                        break;
                                }
                                break;
                            // Projectile Speed
                            case 2:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        levelUpStrings[i] = "Increase " + player.weapons[levelUpLists[0, i]].name + " projectile speed by 10%";
                                        break;
                                    case 2:
                                        levelUpStrings[i] = "Increase " + player.weapons[levelUpLists[0, i]].name + " projectile speed by 20%";
                                        break;
                                }
                                break;
                            case 3:
                            // New Weapon
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                        levelUpStrings[i] = "Add new weapon bow";
                                        break;
                                }
                                break;
                        }
                    }
                    // Player Upgrades
                    else
                    {
                        switch (levelUpLists[1, i])
                        {
                            // Health
                            case 0:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                        levelUpStrings[i] = "Increase maximum and current health by 5";
                                        break;
                                    case 1:
                                        levelUpStrings[i] = "Increase maximum and current health by 10";
                                        break;
                                    case 2:
                                        levelUpStrings[i] = "Increase maximum and current health by 20";
                                        break;
                                }
                                break;
                            // Speed
                            case 1:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        levelUpStrings[i] = "Increase speed by 1";
                                        break;
                                    case 2:
                                        levelUpStrings[i] = "Increase speed by 2";
                                        break;
                                }
                                break;
                            // Health Regen Rate
                            case 2:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        levelUpStrings[i] = "Increase health regen rate by 10%";
                                        break;
                                    case 2:
                                        levelUpStrings[i] = "Increase health regen rate by 20%";
                                        break;
                                }
                                break;
                            // Health Regen Amount
                            case 3:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        levelUpStrings[i] = "Increase health regen amount by 1";
                                        break;
                                    case 2:
                                        levelUpStrings[i] = "Increase health regen amount by 2";
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        // Apply level up chosen card
        public void ApplyLevelUp()
        {
            for (int i = 0; i < 3; i++)
            {
                if (levelUpRectangles[i].Contains(Mouse.GetState().Position))
                {
                    paused = false;

                    // Weapn Upgrades
                    if (levelUpLists[0, i] <= player.weapons.Count - 1)
                    {
                        switch (levelUpLists[1, i])
                        {
                            // Damage
                            case 0:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        player.weapons[levelUpLists[0, i]].damage = (int)(player.weapons[levelUpLists[0, i]].damage * 1.1);
                                        break;
                                    case 2:
                                        player.weapons[levelUpLists[0, i]].damage = (int)(player.weapons[levelUpLists[0, i]].damage * 1.2);
                                        break;
                                }
                                break;
                            // Reload Speed
                            case 1:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        player.weapons[levelUpLists[0, i]].reloadSpeed = (int)(player.weapons[levelUpLists[0, i]].reloadSpeed * 0.9);
                                        break;
                                    case 2:
                                        player.weapons[levelUpLists[0, i]].reloadSpeed = (int)(player.weapons[levelUpLists[0, i]].reloadSpeed * 0.8);
                                        break;
                                }
                                break;
                            // Projectile Speed
                            case 2:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        player.weapons[levelUpLists[0, i]].projectileSpeed = (int)(player.weapons[levelUpLists[0, i]].projectileSpeed * 1.1);
                                        break;
                                    case 2:
                                        player.weapons[levelUpLists[0, i]].projectileSpeed = (int)(player.weapons[levelUpLists[0, i]].projectileSpeed * 1.2);
                                        break;
                                }
                                break;
                            case 3:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                        player.weapons.Add(new Weapon("bow", 150, 100, 0, 10));
                                        break;
                                }
                                break;
                        }
                    }
                    // Player Upgrades
                    else
                    {
                        switch (levelUpLists[1, i])
                        {
                            // Health
                            case 0:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                        player.maxHealth += 5;
                                        player.health += 5;
                                        break;
                                    case 1:
                                        player.maxHealth += 10;
                                        player.health += 10;
                                        break;
                                    case 2:
                                        player.maxHealth += 20;
                                        player.health += 20;
                                        break;
                                }
                                break;
                            // Speed
                            case 1:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        player.speed += 1;
                                        break;
                                    case 2:
                                        player.speed += 2;
                                        break;
                                }
                                break;
                            // Health Regen Rate
                            case 2:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        player.healthRegenRate = (int)(player.healthRegenRate * 0.9);
                                        break;
                                    case 2:
                                        player.healthRegenRate = (int)(player.healthRegenRate * 0.8);
                                        break;
                                }
                                break;
                            // Health Regen Amount
                            case 3:
                                switch (levelUpLists[2, i])
                                {
                                    case 0:
                                    case 1:
                                        healthRegen += 1;
                                        break;
                                    case 2:
                                        healthRegen += 2;
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
        }

        // String wrapping
        public string WrapText(SpriteFont font, string text, float maxLineWidth)
        {
            string[] words = text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            return sb.ToString();
        }

        public List<string> availableWeapons()
        {
            List<string> usedWeaponList = new List<string>();
            List<string> availableWeaponList = new List<string>();
            foreach (Weapon weapon in player.weapons)
            {
                usedWeaponList.Add(weapon.name);
            }
            if (usedWeaponList.Contains("sword") && usedWeaponList.Contains("staff"))
            {
                availableWeaponList.Add("magical blade");
            }
            if (usedWeaponList.Contains("sword") && usedWeaponList.Contains("bow"))
            {
                availableWeaponList.Add("dagger shot");
            }
            if (usedWeaponList.Contains("staff") && usedWeaponList.Contains("bow"))
            {
                availableWeaponList.Add("triple wand");
            }
            
            return availableWeaponList;
        }
    }
}
