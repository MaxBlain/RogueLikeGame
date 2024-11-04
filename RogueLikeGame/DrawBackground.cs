using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLikeGame
{
    internal class DrawBackground
    {
        private GraphicsDeviceManager graphics;

        public DrawBackground(GraphicsDeviceManager graphics) 
        {
            this.graphics = graphics;
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D floorTexture)
        {
            for (int i = 0; i < graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Width; i+=floorTexture.Width)
            {
                for (int j = 0; j < graphics.GraphicsDevice.Adapter.CurrentDisplayMode.Height; j += floorTexture.Height)
                {
                    spriteBatch.Draw(floorTexture, new Vector2(i, j), Color.White);
                }
            }
        }
    }
}
