using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
    internal class Sprite
    {
        // Scale factor
        private static readonly float scale = 4f;

        public Texture2D texture;
        public Vector2 position;
        public Color color;
        public Vector2 spriteSize;

        // Scale the sprite texture
        public Rectangle Rect
        {
            get
            {
                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    (int)spriteSize.X * (int)scale,
                    (int)spriteSize.Y * (int)scale
                    );
            }
        }

        public Sprite(Texture2D texture, Vector2 position, Color color, Vector2 spriteSize)
        {
            this.texture = texture;
            this.position = position;
            this.color = color;
            this.spriteSize = spriteSize;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, color);
        }
    }
}
