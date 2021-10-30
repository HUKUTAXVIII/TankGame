using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TankLib;
using ClientServer;
using System.Text.Json;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System;

namespace TankGame
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D tanktexture;
        private Texture2D bullettexture;
        private Tank tank;
        private Client client;
        private List<Tank> tanks;
       
        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            tank = new Tank(new System.Drawing.Point(64,64),new System.Drawing.Point(32,32), Direction.FRONT,2);
            client = new Client("127.0.0.1",8000);
            tanks = new List<Tank>();

            while (!client.socket.Connected)
            {
                try
                {
                    client.Connect();
                }
                catch (System.Exception)
                {
                    Thread.Sleep(100);
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            tanktexture = Content.Load<Texture2D>("tank");
            bullettexture = Content.Load<Texture2D>("bullet");
        }

        protected override void Update(GameTime gameTime)
        {

            Window.Title = tank.bullet.isFalling.ToString();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (this.tank.HP < 0) {
                Exit();
            }
         


            var key = Keyboard.GetState();
            if (key.IsKeyDown(Keys.Space) && !tank.bullet.isFalling) {
                tank.bullet = new Bullet(tank.Location,tank.Dir,tank.Speed*2);
                tank.bullet.Shoot();
            }

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

            if (tank.bullet.isFalling)
            {
                tank.bullet.Move();
                if (tank.bullet.Tick > 200)
                {
                    tank.bullet.isFalling = false;
                }
                //if (tank.bullet.Location.X >= 0 && tank.bullet.Location.X <= Window.ClientBounds.Size.X)
                //{
                //    tank.bullet.isFalling = true;
                //}
                //else {
                //    
                //}
                foreach (var item in tanks)
                {
                    if (!item.Equals(tank))
                    {
                        tank.bullet.Land(item);
                    }
                }
            }
            client.Send(Client.FromStringToBytes(JsonSerializer.Serialize<Tank>(this.tank)));


            try
            {
                //tanks.Clear();

                var item = Client.FromBytesToString(client.Get());
                var tankarr = JsonSerializer.Deserialize<List<Tank>>(item);

                if (tankarr.Count == tanks.Count)
                {
                    for (int i = 0; i < tanks.Count; i++)
                    {
                        tanks[i].Location = new System.Drawing.Point(tankarr[i].Location.X, tankarr[i].Location.Y);
                        tanks[i].Dir = tankarr[i].Dir;
                        tanks[i].HP = tankarr[i].HP;
                    }
                }
                else{
                    tanks.Clear();
                    tanks.AddRange(tankarr);
                }

                tanks = JsonSerializer.Deserialize<List<Tank>>(item);

                GC.Collect(GC.GetGeneration(item));





            }
            catch (System.Exception)
            {
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
            else if (tank.Dir == Direction.BACK)
            {
                side = (float)4.71239- (float)1.5708;
            }


            _spriteBatch.Draw(
                tanktexture,
                new Vector2(tank.Location.X, tank.Location.Y),
                null,
                (!isEnemy)?Color.White:Color.Red,
                side,
                new Vector2(20, 25),
                1,
                SpriteEffects.None,
                0);
            GC.Collect(GC.GetGeneration(side));
            GC.Collect(GC.GetGeneration(effect));
        }
        private void drawBullet(Tank tank)
        {

            SpriteEffects effect = (tank.Dir == Direction.BACK) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            float side = 0;
            if (tank.bullet.Dir == Direction.RIGHT)
            {
                side = (float)1.5708;
            }
            else if (tank.bullet.Dir == Direction.LEFT)
            {
                side = (float)4.71239;
            }
            else if (tank.bullet.Dir == Direction.BACK)
            {
                side = (float)4.71239 - (float)1.5708;
            }

           
            _spriteBatch.Draw(
                bullettexture,
                new Vector2(tank.bullet.Location.X, tank.bullet.Location.Y),
                null,
                Color.White,
                side,
                new Vector2(5, 15),
                1,
                SpriteEffects.None,
                0);
            GC.Collect(GC.GetGeneration(side));
            GC.Collect(GC.GetGeneration(effect));
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            //drawTank(this.tank,false);
          
                foreach (var t in tanks) {
                    drawTank(t,false);
                    if (t.bullet.isFalling)
                    {
                        drawBullet(t);
                    }
                }

            _spriteBatch.End();
            

            base.Draw(gameTime);
        }
    }
}
