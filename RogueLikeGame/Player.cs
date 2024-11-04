using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
    internal class Player : Sprite
    {
        Texture2D playerSpriteSheet;

        public int maxHealth;
        public int health;
        public int speed;
        public int healthRegenRate;
        public List<Weapon> weapons;
        public string facing = "r";

        DrawFacing drawFacing;

        public Player(Texture2D texture, Vector2 position, Color color, Vector2 spriteSize, string character, Texture2D playerSpriteSheet) : base(texture, position, color, spriteSize)
        {
            // Create stats dictionary depending on character
            
            weapons = new List<Weapon>();

            int intervalMoving = 9;
            int intervalStationary = 30;

            if (character == "knight")
            {
                this.maxHealth = 80;
                this.speed = 5;
                this.healthRegenRate = 300;
                weapons.Add(new Weapon("sword", 10, 100, 0, 1));
                intervalMoving = 9;
                intervalStationary = 30;
            }
            else if (character == "wizard")
            {
                this.maxHealth = 50;
                this.speed = 5;
                this.healthRegenRate = 500;
                weapons.Add(new Weapon("staff", 30, 25, 0, 6));
                intervalMoving = 9;
                intervalStationary = 30;
            }
            else if (character == "rogue")
            {
                this.maxHealth = 50;
                this.speed = 7;
                this.healthRegenRate = 500;
                weapons.Add(new Weapon("bow", 150, 100, 0, 10));
                intervalMoving = 7;
                intervalStationary = 30;
            }
            this.health = this.maxHealth;
            this.playerSpriteSheet = playerSpriteSheet;
            this.spriteSize = spriteSize;

            drawFacing = new DrawFacing(facing, playerSpriteSheet, Rect, color, spriteSize, intervalMoving, intervalStationary);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Keyboard interaction
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                position.Y -= speed;
                if (facing.Contains("s"))
                {
                    facing = facing.Remove(1);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                position.Y += speed;
                if (facing.Contains("s"))
                {
                    facing = facing.Remove(1);
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                position.X -= speed;
                facing = "l";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                position.X += speed;
                facing = "r";
            }
            if (facing.Contains("s") != true)
            { 
                if (Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.Right) && Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.Down))
                {
                    facing += "s";
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Left) && Keyboard.GetState().IsKeyDown(Keys.Right) && Keyboard.GetState().IsKeyUp(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.Down))
                {
                    facing += "s";
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Down) && Keyboard.GetState().IsKeyDown(Keys.Up) && Keyboard.GetState().IsKeyUp(Keys.Left) && Keyboard.GetState().IsKeyUp(Keys.Right))
                {
                    facing += "s";
                }
            }
            drawFacing.facing = facing;
            drawFacing.Rect = Rect;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            drawFacing.Draw(spriteBatch);
        }
    }
}
