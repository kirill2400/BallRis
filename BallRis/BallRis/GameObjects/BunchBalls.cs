using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace BallRis.GameObjects
{
    class BunchBalls : GameObject
    {
        private Rectangle[,] RectPole;
        private Ball[,] BallPole;
        public Ball[] Balls;
        private Wall[] Walls;
        private Thread moveDown;
        private int rotated = 1;
        private bool[] error = new bool[4];
        public bool _surprise = false;
        public bool ended = false;

        public BunchBalls(Ball[] balls, Rectangle[,] rectpole, Ball[,] ballpole, Wall[] walls, Color color, bool surprise = false)
        {
            moveDown = new Thread(new ThreadStart(MoveDown));
            moveDown.IsBackground = true;
            _surprise = surprise;
            BallPole = ballpole;
            RectPole = rectpole;
            Balls = balls;
            _Rectangle = Balls[0]._Rectangle;
            _Color = color;
            Walls = walls;
            for (int i = 1; i < Balls.Length; i++)
            {
                Balls[i]._Rectangle.X += i * 50;
                Rectangle.Union(ref _Rectangle, ref Balls[i]._Rectangle, out _Rectangle);
            }
            moveDown.Start();
        }

        public void MoveDown()
        {
        start:
            bool canMove = true;
            _Rectangle.Y += 1;
            if (!_Rectangle.Intersects(Walls[2]._Rectangle))
            {
                for (int i = 0; i <= RectPole.GetUpperBound(0); i++)
                    if (canMove)
                        for (int j = 0; j <= RectPole.GetUpperBound(1); j++)
                            if (BallPole[i, j] != null)
                                if (!_Rectangle.Intersects(BallPole[i, j]._Rectangle))
                                    canMove = true;
                                else
                                {
                                    canMove = false;
                                    break;
                                }
                            else canMove = true;
                    else break;
                if (canMove)
                {
                    _Rectangle.Y += 49;
                    foreach (Ball ball in Balls)
                        ball._Rectangle.Y += 50;
                }
                else
                {
                    _Rectangle.Y -= 1;
                    if (_surprise)
                        Balls[1]._Color = _Color;
                    ended = true;
                    moveDown.Abort();
                }
            }
            else
            {
                _Rectangle.Y -= 1;
                if (_surprise)
                    Balls[1]._Color = _Color;
                ended = true;
                moveDown.Abort();
            }
            Thread.Sleep(1000);
            goto start;
        }
        public bool Check(Ball[,] BallPole, ref int Score)
        {
            bool result = false;
            foreach (Ball ball in Balls)
                for (int i = 0; i <= RectPole.GetUpperBound(0); i++)
                    for (int j = 0; j <= RectPole.GetUpperBound(1); j++)
                        if (ball._Rectangle.Contains(RectPole[i, j]))
                        {
                            ball.Line = i;
                            ball.Column = j;
                            BallPole[i, j] = ball;
                        }
            #region Line
            for (int i = 0; i <= RectPole.GetUpperBound(0); i++)
            {
            start:
                bool oneColor = false;
                Color color = Color.White;
                if (BallPole[i, 0] != null)
                    color = BallPole[i, 0]._Color;
                if (!oneColor)
                {
                    for (int j = 0; j <= RectPole.GetUpperBound(1); j++)
                    {
                        if (BallPole[i, j] != null && BallPole[i, j]._Color == color)
                            oneColor = true;
                        else
                        {
                            oneColor = false;
                            break;
                        }
                    }
                }
                if (oneColor)
                {
                    for (int q = i; q < RectPole.GetUpperBound(0); q++)
                        for (int j = 0; j <= RectPole.GetUpperBound(1); j++)
                        {
                            BallPole[q, j] = BallPole[q + 1, j];
                            if (BallPole[q, j] != null)
                            {
                                BallPole[q, j]._Rectangle.Y += 50;
                                BallPole[q, j].Line = q;
                                BallPole[q, j].Column = j;
                            }
                        }
                    Score += 9;
                    result = true;
                    goto start;
                }
            }
            #endregion
            #region Column
            for (int i = 0; i <= RectPole.GetUpperBound(1); i++)
            {
                int counter = 0;
                bool oneColor = false;
                Color color = Color.White;
                if (!oneColor)
                {
                    for (int j = 0; j <= RectPole.GetUpperBound(0); j++)
                    {
                        if (BallPole[j, i] != null)
                        {
                            if (BallPole[j, i]._Color != color)
                            {
                                counter = 0;
                                color = Color.White;
                                oneColor = false;
                                if (color == Color.White)
                                    color = BallPole[j, i]._Color;
                            }
                            if (BallPole[j, i]._Color == color)
                            {
                                oneColor = true;
                                counter++;
                                if (counter == 5)
                                {
                                    counter = 0;
                                    color = Color.White;
                                    for (int q = j - 4; q <= j; q++)
                                    {
                                        BallPole[q, i] = null;
                                    }
                                    for (int q = j; q < RectPole.GetUpperBound(0); q++)
                                        if (BallPole[q, i] != null)
                                        {
                                            BallPole[q, i]._Rectangle.Y += 250;
                                            BallPole[q - 5, i] = BallPole[q, i];
                                            BallPole[q - 5, i].Line = q - 5;
                                            BallPole[q - 5, i].Column = i;
                                            BallPole[q, i] = null;
                                        }
                                    Score += 5;
                                    result = true;
                                    break;
                                }
                                
                            }
                        }
                        else
                        {
                            counter = 0;
                            color = Color.White;
                            oneColor = false;
                        }
                    }
                }
            }
            #endregion
            #region MoveDown
            for (int i = 1; i <= RectPole.GetUpperBound(0); i++)
                for (int j = 0; j <= RectPole.GetUpperBound(1); j++)
                    if (BallPole[i, j] != null && BallPole[i - 1, j] == null)
                    {
                        List<Ball> needToMove = new List<Ball>(5);
                        needToMove.Add(BallPole[i, j]);
                        if (j == 0)
                            for (int q = 1; j + q <= 8; q++)
                                if (BallPole[i, j + q] != null && BallPole[i - 1, j + q] == null)
                                {
                                    needToMove.Add(BallPole[i, j + q]);
                                }
                                else if (BallPole[i, j + q] != null && BallPole[i - 1, j + q] != null)
                                {
                                    needToMove.Clear();
                                    break;
                                }
                                else
                                    break;
                        for (int p = 1; j - p >= 0; p++)
                            if (BallPole[i, j - p] != null && BallPole[i - 1, j - p] == null && j - p > 0)
                                needToMove.Add(BallPole[i, j - p]);
                            else if (BallPole[i, j - p] != null && BallPole[i - 1, j - p] != null)
                            {
                                needToMove.Clear();
                                break;
                            }
                            else if (BallPole[i, j - p] == null || j - p == 0)
                            {
                                if (BallPole[i, j - p] != null)
                                    needToMove.Add(BallPole[i, j - p]);
                                for (int q = 1; j + q <= 8; q++)
                                    if (BallPole[i, j + q] != null && BallPole[i - 1, j + q] == null)
                                        needToMove.Add(BallPole[i, j + q]);
                                    else if (BallPole[i, j + q] != null && BallPole[i - 1, j + q] != null)
                                    {
                                        needToMove.Clear();
                                        break;
                                    }
                                    else
                                        break;
                                break;
                            }
                        foreach (Ball ball in needToMove)
                        {
                            int d = ball.Column;
                            for (int q = ball.Line - 1; q >= 0; q--)
                                if (BallPole[q, d] == null)
                                {
                                    ball._Rectangle.Y += 50;
                                    BallPole[q, d] = ball;
                                    BallPole[q + 1, d] = null;
                                }
                                else
                                    break;
                        }
                    }
            #endregion
            return result;
        }

        public override void Update(SpriteBatch spriteBatch)
        {
            bool canMove = true;
            if ((!_Rectangle.Intersects(Walls[3]._Rectangle)))
            {
                #region Left
                if (Keyboard.GetState().IsKeyDown(Keys.Left) && !pressed[0])
                {
                    _Rectangle.X -= 1;
                    if (!_Rectangle.Intersects(Walls[0]._Rectangle))
                    {
                        for (int i = 0; i <= RectPole.GetUpperBound(0); i++)
                            if (canMove)
                                for (int j = 0; j <= RectPole.GetUpperBound(1); j++)
                                    if (BallPole[i, j] != null)
                                        if (!_Rectangle.Intersects(BallPole[i, j]._Rectangle))
                                            canMove = true;
                                        else
                                        {
                                            canMove = false;
                                            break;
                                        }
                                    else canMove = true;
                            else break;
                        if (canMove)
                        {
                            _Rectangle.X -= 50;
                            foreach (Ball ball in Balls)
                                ball._Rectangle.X -= 50;
                            error[0] = true;
                        }
                        else if (error[0])
                        {
                            error[0] = false;
                            Game1.Error.Play();
                        }
                    }
                    else if (error[0])
                    {
                        error[0] = false;
                        Game1.Error.Play();
                    }
                    _Rectangle.X += 1;
                    pressed[0] = true;

                }
                else if (Keyboard.GetState().IsKeyUp(Keys.Left) && pressed[0])
                    pressed[0] = false;
                #endregion
                #region Right
                if (Keyboard.GetState().IsKeyDown(Keys.Right) && !pressed[1])
                {
                    _Rectangle.X += 1;
                    if (!_Rectangle.Intersects(Walls[1]._Rectangle))
                    {
                        for (int i = 0; i <= RectPole.GetUpperBound(0); i++)
                            if (canMove)
                                for (int j = 0; j <= RectPole.GetUpperBound(1); j++)
                                    if (BallPole[i, j] != null)
                                        if (!_Rectangle.Intersects(BallPole[i, j]._Rectangle))
                                            canMove = true;
                                        else
                                        {
                                            canMove = false;
                                            break;
                                        }
                                    else canMove = true;
                            else break;
                        if (canMove)
                        {
                            _Rectangle.X += 50;
                            foreach (Ball ball in Balls)
                                ball._Rectangle.X += 50;
                            error[1] = true;
                        }
                        else if (error[1])
                        {
                            error[1] = false;
                            Game1.Error.Play();
                        }
                    }
                    else if (error[1])
                    {
                        error[1] = false;
                        Game1.Error.Play();
                    }
                    _Rectangle.X -= 1;
                    pressed[1] = true;
                }
                else if (Keyboard.GetState().IsKeyUp(Keys.Right) && pressed[1])
                    pressed[1] = false;
                #endregion
                #region Down
                if (Keyboard.GetState().IsKeyDown(Keys.Down) && !pressed[2])
                {
                    _Rectangle.Y += 1;
                    if (!_Rectangle.Intersects(Walls[2]._Rectangle))
                    {
                        for (int i = 0; i <= RectPole.GetUpperBound(0); i++)
                            if (canMove)
                                for (int j = 0; j <= RectPole.GetUpperBound(1); j++)
                                    if (BallPole[i, j] != null)
                                        if (!_Rectangle.Intersects(BallPole[i, j]._Rectangle))
                                            canMove = true;
                                        else
                                        {
                                            canMove = false;
                                            break;
                                        }
                                    else canMove = true;
                            else break;
                        if (canMove)
                        {
                            _Rectangle.Y += 50;
                            foreach (Ball ball in Balls)
                                ball._Rectangle.Y += 50;
                            error[2] = true;
                        }
                        else if (error[2])
                        {
                            error[2] = false;
                            Game1.Error.Play();
                        }
                    }
                    else if (error[2])
                    {
                        error[2] = false;
                        Game1.Error.Play();
                    }
                    _Rectangle.Y -= 1;
                    pressed[2] = true;
                }
                else if (Keyboard.GetState().IsKeyUp(Keys.Down) && pressed[2])
                    pressed[2] = false;
                #endregion
                #region Up
                if (Keyboard.GetState().IsKeyDown(Keys.Up) && !pressed[3])
                {
                    Rectangle m_rectangle;
                    if (rotated == 1)
                        m_rectangle = new Rectangle(_Rectangle.X + 50, _Rectangle.Y - 50, 50, 150);
                    else
                        m_rectangle = new Rectangle(_Rectangle.X - 50, _Rectangle.Y + 50, 150, 50);
                    if (!m_rectangle.Intersects(Walls[0]._Rectangle) && !m_rectangle.Intersects(Walls[1]._Rectangle) && !m_rectangle.Intersects(Walls[2]._Rectangle))
                    {
                        for (int i = 0; i <= RectPole.GetUpperBound(0); i++)
                            if (canMove)
                                for (int j = 0; j <= RectPole.GetUpperBound(1); j++)
                                    if (BallPole[i, j] != null)
                                        if (!m_rectangle.Intersects(BallPole[i, j]._Rectangle))
                                            canMove = true;
                                        else
                                        {
                                            canMove = false;
                                            break;
                                        }
                                    else canMove = true;
                            else break;
                        if (canMove)
                        {
                            _Rectangle = m_rectangle;
                            Balls[0]._Rectangle.X += rotated * 50;
                            Balls[0]._Rectangle.Y -= rotated * 50;
                            Balls[2]._Rectangle.X -= rotated * 50;
                            Balls[2]._Rectangle.Y += rotated * 50;
                            rotated = -rotated;
                            error[3] = true;
                        }
                        else if (error[3])
                        {
                            error[3] = false;
                            Game1.Error.Play();
                        }
                    }
                    else if (error[3])
                    {
                        error[3] = false;
                        Game1.Error.Play();
                    }
                    pressed[3] = true;
                }
                else if (Keyboard.GetState().IsKeyUp(Keys.Up) && pressed[3])
                    pressed[3] = false;
                #endregion
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Ball ball in Balls)
                ball.Draw(spriteBatch);
        }
    }
}
