using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BallRis.GameObjects
{
    class MainMenuScreen : GameObject
    {
        private Texture2D ButtonTexture;
        private Rectangle ButtonRectangle;
        private SpriteFont _SpriteFont;
        public bool StartGame;

        public MainMenuScreen(Texture2D pixelTexture, Texture2D ButtonTexture, SpriteFont SpriteFont, Color Color)
        {
            _Texture = pixelTexture;
            this.ButtonTexture = ButtonTexture;
            _Color = Color;
            _SpriteFont = SpriteFont;
            _Rectangle = new Rectangle(0, 0, halfWidth * 2, halfHeight * 2);
            this.ButtonRectangle = new Rectangle(halfWidth - 170, halfHeight - 33, 341, 66);
        }
        MouseState mouse;
        public override void Update(SpriteBatch spriteBatch)
        {
            mouse = Mouse.GetState();
            if (Game1.Active &&
                mouse.LeftButton == ButtonState.Pressed &&
                mouse.X > ButtonRectangle.X && mouse.X < ButtonRectangle.X + ButtonRectangle.Width &&
                mouse.Y > ButtonRectangle.Y && mouse.Y < ButtonRectangle.Y + ButtonRectangle.Height)
                StartGame = true;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(ButtonTexture, ButtonRectangle, Color.White);
        }
    }
}
