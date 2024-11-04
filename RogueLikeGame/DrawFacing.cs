using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
    internal class DrawFacing
    {
        public string facing;
        public Texture2D texture;
        public Rectangle Rect;
        public Color color;
        public Vector2 spriteSize;

        AnimationManager amRunningRight;
        AnimationManager amStationaryRight;
        AnimationManager amRunningLeft;
        AnimationManager amStationaryLeft;

        public DrawFacing(string facing, Texture2D texture, Rectangle Rect, Color color, Vector2 spriteSize, int intervalMoving, int intervalStationary) 
        { 
            this.facing = facing;
            this.texture = texture;
            this.Rect = Rect;
            this.color = color;
            this.spriteSize = spriteSize;
            
            amStationaryLeft = new(3, 3, spriteSize, 0, intervalStationary);
            amStationaryRight = new(3, 3, spriteSize, 1, intervalStationary);
            amRunningLeft = new(4, 4, spriteSize, 2, intervalMoving);
            amRunningRight = new(4, 4, spriteSize, 3, intervalMoving);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            switch (facing)
            {
                case ("ls"):
                    spriteBatch.Draw(
                        texture,
                        Rect,
                        amStationaryLeft.GetFrame(),
                        color);
                    amStationaryLeft.Update();
                    break;
                case ("rs"):
                    spriteBatch.Draw(
                        texture,
                        Rect,
                        amStationaryRight.GetFrame(),
                        color);
                    amStationaryRight.Update();
                    break;
                case ("l"):
                    spriteBatch.Draw(
                        texture,
                        Rect,
                        amRunningLeft.GetFrame(),
                        color);
                    amRunningLeft.Update();
                    break;
                case ("r"):
                    spriteBatch.Draw(
                        texture,
                        Rect,
                        amRunningRight.GetFrame(),
                        color);
                    amRunningRight.Update();
                    break;
            }
        }
    }
}
