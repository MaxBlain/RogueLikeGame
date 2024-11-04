using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
    internal class Missile : Sprite
    {
        public int target;
        public Vector2 velocity;

        public Missile(Texture2D texture, Vector2 position, Color color, Vector2 spriteSize, string type) : base(texture, position, color, spriteSize) { }
    }
}
