using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallRis.GameObjects
{
    class GUI : GameObject
    {
        private SpriteFont _SpriteFont;
        private object[][] _Sprite;
        public void SetScore(int value)
        {
            _Sprite[4][0] = "Счёт: " + value;
        }
        public void SetLives(int value)
        {
            _Sprite[5][0] = "Жизни: " + value;
        }

        public GUI(SpriteFont SpriteFont, Color Color)
        {
            _SpriteFont = SpriteFont;
            _Color = Color;
            _Sprite = new object[][]{
                new object[] { "↑ Повернуть", new Vector2(halfWidth - 9, halfHeight - 225) },
                new object[] { "Влево\n       ←", new Vector2(halfWidth - 295, halfHeight - 45) },
                new object[] { " Вправо\n→", new Vector2(halfWidth + 223, halfHeight - 45) },
                new object[] { "Опустить ↓", new Vector2(halfWidth - 119, halfHeight + 200) },
                new object[] { "Счёт: " + 0, new Vector2(halfWidth - 295, halfHeight - 229) },
                new object[] { "Жизни: " + 3, new Vector2(halfWidth + 200, halfHeight - 229) }
            };
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _Sprite.Length; i++)
            {
                if (i < 4)
                {
                    spriteBatch.DrawString(_SpriteFont, (string)_Sprite[i][0], (Vector2)_Sprite[i][1], _Color);
                    continue;
                }
                spriteBatch.DrawString(_SpriteFont, (string)_Sprite[i][0], (Vector2)_Sprite[i][1], Color.Pink);
            }
        }
    }
}
