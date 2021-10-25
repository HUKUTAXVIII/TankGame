﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TankLib;

namespace TankGame
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D tanktexture;
        private Tank tank;
        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            tank = new Tank(new System.Drawing.Point(64,64),new System.Drawing.Point(32,32), Direction.FRONT,2);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            tanktexture = Content.Load<Texture2D>("tank");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var key = Keyboard.GetState();

            if (key.IsKeyDown(Keys.Up))
            {
                tank.Dir = Direction.FRONT;
                tank.Move();
            }
            else if (key.IsKeyDown(Keys.Down))
            {
                tank.Dir = Direction.BACK;
                tank.Move();
            }
            else if (key.IsKeyDown(Keys.Left))
            {
                tank.Dir = Direction.LEFT;
                tank.Move();
            }
            else if (key.IsKeyDown(Keys.Right)) {
                tank.Dir = Direction.RIGHT;
                tank.Move();
            }

            

            base.Update(gameTime);
        }

        private void drawTank(Tank tank, bool isEnemy) {

            SpriteEffects effect = (tank.Dir == Direction.BACK) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            float side = 0;
            if (tank.Dir == Direction.RIGHT)
            {
                side = (float)1.5708;
            }
            else if (tank.Dir == Direction.LEFT)
            {
                side = (float)4.71239;
            }


            _spriteBatch.Draw(
                tanktexture,
                new Vector2(tank.Location.X, tank.Location.Y),
                null,
                (!isEnemy)?Color.White:Color.Red,
                side,
                new Vector2(20, 25),
                1,
                effect,
                0);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            drawTank(this.tank,false);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
