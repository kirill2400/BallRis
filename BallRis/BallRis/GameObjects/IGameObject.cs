using Microsoft.Xna.Framework.Graphics;

namespace BallRis.GameObjects
{
    interface IGameObject
    {
        void Update(SpriteBatch spriteBatch);
        void Draw(SpriteBatch spriteBatch);
    }
}
