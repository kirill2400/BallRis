using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BallRis.GameObjects
{
    class GameOverScreen : GameObject
    {
        private Texture2D ButtonTexture;
        private Rectangle ButtonRectangle;
        private Vector2[] Vectors;
        private SpriteFont _SpriteFont;
        private bool NewGame;
        public bool GetNewGame()
        {
            if (NewGame)
            {
                NewGame = false;
                return true;
            }
            return NewGame;
        }
        public void SetNewGame(bool value)
        {
            NewGame = value;
        }

        public GameOverScreen(Texture2D Texture, Texture2D ButtonTexture, SpriteFont SpriteFont, Color Color)
        {
            _Texture = Texture;
            this.ButtonTexture = ButtonTexture;
            _Color = Color;
            _SpriteFont = SpriteFont;
            _Rectangle = new Rectangle(0, 0, halfWidth * 2, halfHeight * 2);
            this.ButtonRectangle = new Rectangle(halfWidth - 170, halfHeight - 33, 341, 66);
            Vectors = new Vector2[] {
                new Vector2(ButtonRectangle.X + 82, ButtonRectangle.Y - 102),
                new Vector2(ButtonRectangle.X + 104, ButtonRectangle.Y - 68),
                new Vector2(ButtonRectangle.X, ButtonRectangle.Y - 34),
                new Vector2(ButtonRectangle.X + 88, ButtonRectangle.Y - 34)
            };
        }
        MouseState mouse;
        public override void Update(SpriteBatch spriteBatch)
        {
            mouse = Mouse.GetState();
            if (Game1.Active &&
                mouse.LeftButton == ButtonState.Pressed &&
                mouse.X > ButtonRectangle.X && mouse.X < ButtonRectangle.X + ButtonRectangle.Width &&
                mouse.Y > ButtonRectangle.Y && mouse.Y < ButtonRectangle.Y + ButtonRectangle.Height)
                SetNewGame(true);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(_SpriteFont, "Игра окончена!", Vectors[0], Color.Yellow);
            spriteBatch.DrawString(_SpriteFont, "Ваш счёт: " + Game1.GlobalScore, Vectors[1], Color.Yellow);
            if (Game1.GlobalScore > Game1.HighScore)
                spriteBatch.DrawString(_SpriteFont, "Вы установили новый рекорд!", Vectors[2], Color.Yellow);
            else
                spriteBatch.DrawString(_SpriteFont, "Ваш рекорд: " + Game1.HighScore, Vectors[3], Color.Yellow);
            spriteBatch.Draw(ButtonTexture, ButtonRectangle, Color.White);
        }
    }
}
