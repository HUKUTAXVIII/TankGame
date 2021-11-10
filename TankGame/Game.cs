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
using System.Linq;

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
        private Texture2D walltexture;
        private List<Wall> walls;
       
        public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            tank = new Tank(new System.Drawing.Point(300,300),new System.Drawing.Point(20,20), Direction.FRONT,2);
            client = new Client("127.0.0.1",8000);
            tanks = new List<Tank>();

            walls = new List<Wall>();
            for (int i = 0; i < Window.ClientBounds.Height; i += 32)
            {
                for (int j = 0; j < Window.ClientBounds.Width; j += 32)
                {
                    
                    if (i == 0 || j == 0 || j >= Window.ClientBounds.Width - 32 || i >= Window.ClientBounds.Height - 32)
                    {
                        walls.Add(new Wall(j, i));
                    }
                }
            }


                //walls = new List<List<Wall>>();


                //if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Tanks\map.txt"))
                //{
                //    this.walls = JsonSerializer.Deserialize<List<List<Wall>>>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\TanksGame\map.txt"));
                //    this.tank = JsonSerializer.Deserialize<Tank>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\TanksGame\player.txt"));
                //}
                //else {

                //    for (int i = 0; i < Window.ClientBounds.Height; i += 32)
                //    {
                //        walls.Add(new List<Wall>());
                //        for (int j = 0; j < Window.ClientBounds.Width; j += 32)
                //        {
                //            if (i == 0 || j == 0 || j >= Window.ClientBounds.Width - 32 || i >= Window.ClientBounds.Height - 32)
                //            {
                //                walls[i / 32].Add(new Wall(j, i));
                //            }
                //            //if (i == 32*10 && (j >= Window.ClientBounds.Width * 0.20 && j <= Window.ClientBounds.Width * 0.8 - 32)) {
                //            //    walls[i / 32].Add(new Point(j, i));
                //            //}
                //            //if (i == 32 * 4 && (j >= Window.ClientBounds.Width * 0.20 && j <= Window.ClientBounds.Width * 0.8 - 32))
                //            //{
                //            //    walls[i / 32].Add(new Point(j, i));
                //            //}
                //            //if (j == 32 * 12 && (i >= Window.ClientBounds.Height * 0.20 && i <= Window.ClientBounds.Height * 0.8 - 32)) {
                //            //    walls[i / 32].Add(new Point(j, i));
                //            //}
                //        }
                //    }
                //    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Tanks");
                //    var json = JsonSerializer.Serialize(this.walls);
                //    File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Tanks\map.txt",json);
                //    //File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Tanks\player.txt", JsonSerializer.Serialize(this.tank));


                //}



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
            walltexture = Content.Load<Texture2D>("wall32");
        }

        protected override void Update(GameTime gameTime)
        {

            try
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    //File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Tanks\player.txt", JsonSerializer.Serialize(this.tank));
                    Exit();
                }
                if (this.tank.HP < 0)
                {
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

                    if (CollisionWall(tank))
                    {
                        tank.Dir = Direction.BACK;
                        tank.Move();
                        tank.Dir = Direction.FRONT;
                    }

            }
            else if (key.IsKeyDown(Keys.Down))
            {
                    tank.Dir = Direction.BACK;
                    tank.Move();

                    if (CollisionWall(tank))
                    {
                        tank.Dir = Direction.FRONT;
                        tank.Move();
                        tank.Dir = Direction.BACK;
                    }
                }
            else if (key.IsKeyDown(Keys.Left))
            {
                    tank.Dir = Direction.LEFT;
                    tank.Move();

                    if (CollisionWall(tank))
                    {
                        tank.Dir = Direction.RIGHT;
                        tank.Move();
                        tank.Dir = Direction.LEFT;
                    }
                }
            else if (key.IsKeyDown(Keys.Right)) {
                    tank.Dir = Direction.RIGHT;
                    tank.Move();

                    if (CollisionWall(tank))
                    {
                        tank.Dir = Direction.LEFT;
                        tank.Move();
                        tank.Dir = Direction.RIGHT;
                    }
                }







            if (tank.bullet.isFalling)
            {
                tank.bullet.Move();
                if (tank.bullet.Tick > 700)
                {
                    tank.bullet.isFalling = false;
                }
                    for(int i = 0;i < tanks.Count ;i++)
                    {
                        if (tanks[i].ToString()!=tank.ToString())
                        {
                            if (tank.bullet.Land(ref tanks, i)) {
                                tanks[i].HP -= 25;
                            }
                        }
                    }
            }
            client.Send(Client.FromStringToBytes(JsonSerializer.Serialize<Tank>(this.tank)));
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

        private bool CollisionWall(Tank tank) {
            bool check = false;
            foreach (var item in this.walls) {
                if (item.GetRectangle().IntersectsWith(tank.GetRectangle())) {
                    Window.Title = tank.Location.X+" "+tank.Location.Y + "      "+item.X +" "+item.Y;
                    //check = true;
                    return true;
                }
            }
            return check;
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


            walls.ForEach(wall=> {
                _spriteBatch.Draw(walltexture, new Rectangle(wall.X, wall.Y, 32, 32), Color.Orange);
            });

            try
            {
                foreach (var t in tanks) {
                    drawTank(t,false);
                    if (t.bullet.isFalling)
                    {
                        drawBullet(t);
                    }
                }


            }
            catch (Exception)
            {

               
            }
            _spriteBatch.End();
            

            base.Draw(gameTime);
        }
    }
}
