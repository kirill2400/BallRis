using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallRis.GameObjects
{
    class Wall : GameObject
    {
        public Wall(Texture2D Texture, Rectangle Rectangle, Color color)
        {
            _Texture = Texture;
            _Rectangle = Rectangle;
            _Color = color;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
