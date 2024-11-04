using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
    internal class Enemy : Sprite
    {
        public int damage;
        public int health;
        public int speed;
        
        public string character;

        public string facing;

        public Vector2 target;

        public int gracePeriod = 0;
        public int gracePeriodLength;

        DrawFacing drawFacing;

        public Enemy(Texture2D texture, Vector2 position, Color color, Vector2 spriteSize, string character, Vector2 target, Texture2D enemySpriteSheet) : base(texture, position, color, spriteSize)
        {
            this.target = target;
            this.spriteSize = spriteSize;
            this.character = character;

            int intervalMoving = 15;
            int intervalStationary = 45;

            this.color = Color.White;

            facing = "r";

            if (character == "zombie")
            {
                this.damage = 5;
                this.health = 100;
                this.speed = 2;
                this.gracePeriodLength = 40;
                intervalMoving = 15;
                intervalStationary = 45;
            }
            else if (character == "slime")
            {
                this.damage = 2;
                this.health = 100;
                this.speed = 1;
                this.gracePeriodLength = 8;
                intervalMoving = 30;
                intervalStationary = 45;
            }
            else if (character == "skeleton")
            {
                this.damage = 5;
                this.health = 100;
                this.speed = 2;
                this.gracePeriodLength = 40;
                intervalMoving = 15;
                intervalStationary = 45;
            }

            drawFacing = new DrawFacing(facing, enemySpriteSheet, Rect, color, spriteSize, intervalMoving, intervalStationary);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (facing.Contains("s") == false && Rect.Left < target.X)
            {
                facing = "r";
            }
            else if (facing.Contains("s") == false && Rect.Right > target.X)
            {
                facing = "l";
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
