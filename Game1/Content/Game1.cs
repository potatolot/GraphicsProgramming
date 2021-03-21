using GraphicsProgramming;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Lesson _currentLesson;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            // Lesson1
            //_currentLesson = new Lesson1();

            // Lesson2
            //_currentLesson = new Lesson2();

            // Lesson3
            //_currentLesson = new Lesson3();

            // HW Lesson3
           // _currentLesson = new HWLesson3();

            // Lesson4 (Terrain)
            //_currentLesson = new Lesson4();

            // Lesson5
            //_currentLesson = new Lesson5();

            // Lesson6
            _currentLesson = new Lesson6();
        }

        protected override void Initialize()
        {
            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            

            _graphics.ApplyChanges();

            // TODO: Add your initialization logic here
            _currentLesson.Initialize();
            base.Initialize();
           
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _currentLesson.LoadContent(Content, _graphics, _spriteBatch);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here            
            base.Update(gameTime);
            _currentLesson.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            
            base.Draw(gameTime);
            _currentLesson.Draw(gameTime, _graphics, _spriteBatch);
        }
    }
}
