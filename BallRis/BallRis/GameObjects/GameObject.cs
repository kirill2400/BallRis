using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallRis.GameObjects
{
    class GameObject : IGameObject
    {
        public Texture2D _Texture;
        public Rectangle _Rectangle;
        public Color _Color;
        public int halfWidth = Game1.halfWidth;
        public int halfHeight = Game1.halfHeight;
        public bool[] pressed = new bool[4];

        public virtual void Update(SpriteBatch spriteBatch) { }
        public virtual void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(_Texture, _Rectangle, _Color);
        }
    }
}
