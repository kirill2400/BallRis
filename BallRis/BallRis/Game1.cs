using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BallRis.GameObjects;

namespace BallRis
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        #region Статичные поля
        static public bool Active;//Статичное поле boolean Игра имеет фокус?
        static public int GlobalScore;//Статичное поле текущего счета
        static public int HighScore;//Статичное поле лучшего рекорда
        static public int halfWidth;//Половина ширины экрана
        static public int halfHeight;//Половина высоты экрана
        #endregion
        private Wall[] walls = new Wall[4];//Массив стен
        private Ball[] ball = new Ball[3];//Массив шаров для связки шаров
        private BunchBalls Balls;//Поле экземпляра связки шаров
        private SpriteFont Font1;//Поле шрифта
        private GUI gui;//Поле экземпляра класса интерфейса GUI
        private Cursor Cursor;//Поле экземпляра класса курсора

        #region Поля, связанные с состоянием игры
        private enum GameState { MainMenu, NewGame, InGame, GameOver }//Перечисления состояний игры
        private enum GameLevel { Easy = 2, Medium, Hard }//Перечисления сложностей игры
        private GameState gameState = GameState.MainMenu;//Поле свойства состояния игры
        private GameState GetGameState()
        {
            return gameState;
        }
        private void SetGameState(GameState value)
        {
            if (value == GameState.NewGame)
            {
                SetScore(0);
                gui.SetScore(0);
                if (GlobalScore > HighScore)
                    HighScore = GlobalScore;
                GlobalScore = 0;
                counter = 0;
                SetLives(3);
                gui.SetLives(3);
                gameState = GameState.InGame;
                gameLevel = GameLevel.Easy;
                OverSounds[1].Play(1f, 0f, 0f);
                MediaPlayer.Play(Content.Load<Song>("BackgroundSounds/" + rnd.Next(5).ToString()));
                Spawn();
            }
            else
                gameState = value;
        }
        private GameLevel gameLevel = GameLevel.Easy;//Поле уровеня сложности
        private MainMenuScreen MainMenuScreen;//Поле для экземпляра класса главного меню
        private GameOverScreen GameOverScreen;//Поле для экземпляра класса проигранной игры
        #endregion

        #region Вспомогающие поля для игровой логики
        private Texture2D Texture_Ball;
        private Color[] colors = new Color[] { Color.DarkRed, Color.DarkGreen, Color.DarkBlue, Color.DarkKhaki };//Массив цветов
        static public Random rnd = new Random();//Экземпляр класса случайности
        private int Score = 0;//Поле количества очков
        private int GetScore()
        {
            return Score;
        }
        private void SetScore(int value)
        {
            if (gameLevel == GameLevel.Easy && Score + value >= 45)
                gameLevel += 1;
            else if (gameLevel == GameLevel.Medium && Score + value >= 135)
                gameLevel += 1;
            if (value == 0)
                Score = 0;
            Score += ((int)gameLevel - 1) * value;
            gui.SetScore(Score);
        }
        private int Lives = 3;//Поле количества жизней
        private int GetLives()
        {
            return Lives;
        }
        private void SetLives(int value)
        {
            Lives = value;
            gui.SetLives(Lives);
        }
        private int counter;//Счётчик связок шаров, предназначенный для ловушки
        private Rectangle[,] RectPole;//массив прямоугольников
        private Ball[,] BallPole;//массив шаров
        #endregion
        #region Поля звуковых эффектов
        static public SoundEffect Error;
        static public SoundEffect[] FadeSounds;//Звуки исчезания шаров
        static public SoundEffect[] SlapSounds;//Звуки падения шаров
        static public SoundEffect[] SurpriseSounds;//Звуки ловушки
        static private SoundEffect[] OverSounds;//Звуки вступления и завершения
        #endregion

        /// <summary>
        /// Метод инициализации класса игры
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 650;
            graphics.PreferredBackBufferHeight = 450;
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }
        /// <summary>
        /// Метод загрузки контента
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            halfWidth = spriteBatch.GraphicsDevice.Viewport.Width / 2;//Присваивание переменной размер в половину ширины экрана
            halfHeight = spriteBatch.GraphicsDevice.Viewport.Height / 2;//Присваивание переменной размер в половину высоты экрана
            RectPole = new Rectangle[8, 9];//Инициализация массив скелетов
            BallPole = new Ball[8, 9];//Инициализация массива шаров
            for (int i = 0; i <= RectPole.GetUpperBound(0); i++)
                for (int j = 0; j <= RectPole.GetUpperBound(1); j++)
                    RectPole[i, j] = new Rectangle(halfWidth + (int)((-4.5 + j) * 50), halfHeight - (int)((-4 + i) * 50) - 50, 50, 50);//Заполнения массива скелетов
            Texture_Ball = Content.Load<Texture2D>("ball");//Загрузка текстуры шара в переменную
            #region Инициализация стен
            var pixel = Content.Load<Texture2D>("pixel");//Загрузка текстуры пикселя и заполнение массива стен, с помощью этой текстуры
            walls[0] = new Wall(pixel, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width - (halfWidth + 225), spriteBatch.GraphicsDevice.Viewport.Height), Color.Olive);
            walls[1] = new Wall(pixel, new Rectangle(halfWidth + 225, 0, spriteBatch.GraphicsDevice.Viewport.Width - (halfWidth + 150), spriteBatch.GraphicsDevice.Viewport.Height), Color.Olive);
            walls[2] = new Wall(pixel, new Rectangle(0, halfHeight + 200, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height - (halfHeight + 175)), Color.Olive);
            walls[3] = new Wall(pixel, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height - (halfHeight + 200)), Color.Olive);
            #endregion
            #region Инициализация интерфейса и прочего
            Font1 = Content.Load<SpriteFont>("SpriteFont");//Загрузка шрифта
            gui = new GUI(Font1, Color.White);//Создание экземпляра класса GUI
            MainMenuScreen = new MainMenuScreen(pixel, Content.Load<Texture2D>("StartGame"), Font1, Color.Olive);//создание экземпляра класса MainMenuScreen
            GameOverScreen = new GameOverScreen(pixel, Content.Load<Texture2D>("StartNewGame"), Font1, Color.Olive);//Создание экземпляра класса GameOverScreen
            Cursor = new Cursor(Content.Load<Texture2D>("Cursor"));//Создание экземпляра класса Cursor
            #endregion
            #region Загрузка музыки и звуковых эффектов
            Error = Content.Load<SoundEffect>("Error");
            FadeSounds = new SoundEffect[6];
            for (int i = 0; i < FadeSounds.Length; i++)
                FadeSounds[i] = Content.Load<SoundEffect>("OtherSounds/Fade/" + i.ToString());
            SlapSounds = new SoundEffect[5];
            for (int i = 0; i < SlapSounds.Length; i++)
                SlapSounds[i] = Content.Load<SoundEffect>("OtherSounds/Slap/" + i.ToString());
            SurpriseSounds = new SoundEffect[4];
            for (int i = 0; i < SurpriseSounds.Length; i++)
                SurpriseSounds[i] = Content.Load<SoundEffect>("OtherSounds/Surprise/" + i.ToString());
            OverSounds = new SoundEffect[] { Content.Load<SoundEffect>("GameOver"), Content.Load<SoundEffect>("StartOver") };
            MediaPlayer.Volume = 0.2f;
            MediaPlayer.MediaStateChanged += ChangeMusic;
            #endregion
        }
        /// <summary>
        /// Метод разгрузки контента
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload(); //Разгрузка всего контента, что был загружен
            spriteBatch.Dispose();
            this.Dispose();//Разгрузка всех ресурсов используемых классом для сборщика мусора
        }

        /// <summary>
        /// Метод обновления
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))//Выход из игры при нажатии Escape
                this.Exit();
            Game1.Active = this.IsActive;//Запись в переменную значение имеет ли игра фокус
            switch (GetGameState())
            {
                case GameState.MainMenu:
                    #region MainMenu
                    MainMenuScreen.Update(spriteBatch);//Прорисовка главного меню
                    if (MainMenuScreen.StartGame)//При нажатии кнопки начать игру
                        SetGameState(GameState.NewGame);//Новая игра
                    break;
                    #endregion
                case GameState.InGame:
                    #region InGame
                    if (!Balls.ended)//Если связка шаров ещё в полете
                        Balls.Update(spriteBatch);//Метод обновления связки шаров
                    else
                    {//Иначе
                        int Score = 0;//Временная переменная для получения очков, если шары уничтожатся
                        if (Balls.Check(BallPole, ref Score))//Метод проверки, если возвращает true, то некие шары уничтожились
                            Game1.FadeSounds[Game1.rnd.Next(Game1.FadeSounds.Length)].Play(1f, 0f, 0f);//Играть случайный звук уничтожения шаров
                        else if (Balls._surprise) //Иначе если связка шаров с ловушкой и вернет false
                            Game1.SurpriseSounds[Game1.rnd.Next(Game1.SurpriseSounds.Length)].Play(1f, 0f, 0f);//Играть случайный звук злобы
                        else //Иначе если вернет false и связка шаров без ловушки
                            Game1.SlapSounds[Game1.rnd.Next(Game1.SlapSounds.Length)].Play(1f, 0f, 0f);//Играть случайный звук падения шаров
                        if (Score != 0)//Если количество очков во временной переменной изменилось
                            SetScore(Score);//Прибавляем очки
                        for (int i = 3; i < 6; i++)//Цикл проверки блокированности пути для вылета новой связки
                            if (BallPole[7, i] != null)//Если путь заблокирован, то
                            {
                                BallPole = new Ball[8, 9];//Очистка поля
                                Balls.Balls = null;//Удаление временных шаров связки
                                if (GetLives() == 1)//Если жизни кончились, то игра проиграна
                                {
                                    SetGameState(GameState.GameOver);//Присваивание статуса конца игры
                                    GlobalScore = GetScore();//Присваивание статическому полю количество набранных очков
                                    OverSounds[0].Play(1f, 0f, 0f);//Играть звук резкого торможения темы
                                    MediaPlayer.Stop();//Остановить воспроизведение темы
                                }
                                else //Иначе
                                {
                                    SetLives(GetLives() - 1);//Минус жизнь
                                    Game1.SurpriseSounds[Game1.rnd.Next(Game1.SurpriseSounds.Length)].Play(1f, 0f, 0f);//Играть случайный звук злобы
                                }
                                break;
                            } //Цикл кончается
                        if (GetGameState() != GameState.GameOver)//Если игра не проиграна, создавать новую связку
                            Spawn();
                    }
                    #endregion
                    break;
                case GameState.GameOver:
                    #region GameOver
                    GameOverScreen.Update(spriteBatch);//Метод обновления меню конца игры
                    if (GameOverScreen.GetNewGame())//Проверка желания начать новую игру
                        SetGameState(GameState.NewGame);//Начинаем новую игру
                    #endregion
                    break;
            }
            
            base.Update(gameTime);
        }
        /// <summary>
        /// Метод прорисовки
        /// </summary>
        protected override void Draw(GameTime gameTime) 
        {
            GraphicsDevice.Clear(Color.DarkOliveGreen);//Очистка экрана и заполнение определенным цветом
            spriteBatch.Begin();//Начало процесса прорисовки
            switch (GetGameState())//Переключатель зависимый от состояния игры
            {
                case GameState.MainMenu://Главное меню
                    MainMenuScreen.Draw(spriteBatch);//Прорисовка главного меню
                    break;
                case GameState.InGame://В игре
                    Balls.Draw(spriteBatch);//Прорисовка связки шаров
                    foreach (Ball ball in BallPole)
                        if (ball != null)
                            ball.Draw(spriteBatch);//Прорисовка каждого шара, который уже приземлился когда-то
                    foreach (Wall wall in walls)
                        wall.Draw(spriteBatch);//Прорисовка стен
                    gui.Draw(spriteBatch);//Прорисовка интерфейса
                    break;
                case GameState.GameOver://Меню проигранной игры
                    GameOverScreen.Draw(spriteBatch);//Прорисовка меню проигранной игры
                    break;
            }
            Cursor.Draw(spriteBatch);//Прорисовка игрового курсора
            spriteBatch.End();//Завершение процесса прорисовки

            base.Draw(gameTime);
        }

        /// <summary>
        /// Метод создания связки шаров
        /// </summary>
        protected void Spawn()
        {
            Color color = colors[rnd.Next((int)gameLevel)];//Случайный цвет для всей связки
            counter++;//Счетчик плюс один
            for (int i = 0; i < ball.Length; i++)
                ball[i] = new Ball(Texture_Ball, new Rectangle(halfWidth - 75, halfHeight - 300, 50, 50), color);
            bool surprise = false;
            if (counter == 9)//Если истина, то делаем ловушку
            {
                int index = rnd.Next((int)gameLevel);//Запоминаем номер случайного индекса для цвета ловушки
                while (colors[index] == color)//Цикл для выбора цвета шара-ловушки не совпадающий с цветом остальных двух шаров
                    index = rnd.Next((int)gameLevel);
                color = colors[index];//Цвет для шара ловушки
                counter = 0;//Обнуление счётчика для ловушки
                surprise = true;//ставим флаг ловушки
            }
            Balls = new BunchBalls(ball, RectPole, BallPole, walls, color, surprise);//Создание новой связки
        }
        /// <summary>
        /// Метод смены музыки по окончании предыдущей
        /// </summary>
        protected void ChangeMusic(object sender, EventArgs args)
        {
            if (MediaPlayer.State == MediaState.Stopped && GetGameState() == GameState.InGame)
                MediaPlayer.Play(Content.Load<Song>("BackgroundSounds/" + rnd.Next(5).ToString()));
        }
    }
}
