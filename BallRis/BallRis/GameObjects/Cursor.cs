using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BallRis.GameObjects
{
    class Cursor : GameObject
    {
        public Cursor(Texture2D CursorTexture)
        {
            _Texture = CursorTexture;
            _Color = Color.White;
        }
        MouseState mouse;
        public override void Draw(SpriteBatch spriteBatch)
        {
            mouse = Mouse.GetState();
            spriteBatch.Draw(_Texture, new Rectangle(mouse.X, mouse.Y, 35, 35), _Color);
        }
    }
}
