using System;
using System.Collections.Generic;
using System.Drawing;

namespace TankLib {
    public enum Direction
    {
        FRONT,
        BACK,
        LEFT,
        RIGHT,
        NONE
    }

    public class Bullet {
        public Point Location { set; get; }
        public Direction Dir { set; get; }
        public bool isFalling { set; get; }
        public int Speed { set; get; }
        public int Tick { set; get; }
        public Bullet()
        {
            Location = new Point(0,0);
            Dir = Direction.NONE;
            isFalling = false;
        }
        public Bullet(Point loc,Direction dir,int speed)
        {
            this.Location = loc;
            this.Dir = dir;
            this.Speed = speed;
            isFalling = false;
            Tick = 0;
        }
        public void Shoot() => this.isFalling = true;
        public bool Land(ref List<Tank> tank,int index) {
            if (Rectangle.Intersect(new Rectangle(tank[index].Location, new Size(1, 1)), new Rectangle(this.Location, new Size(5, 5))) != Rectangle.Empty)
            {
                this.isFalling = false;
                return true;
            }
            else {
                return false;
            }
        }
        public void Move()
        {
            Tick += this.Speed;
            switch (this.Dir)
            {
                case Direction.FRONT:
                    this.Location = new Point(this.Location.X, this.Location.Y - this.Speed);
                    break;
                case Direction.BACK:
                    this.Location = new Point(this.Location.X, this.Location.Y + this.Speed);
                    break;
                case Direction.LEFT:
                    this.Location = new Point(this.Location.X - this.Speed, this.Location.Y);
                    break;
                case Direction.RIGHT:
                    this.Location = new Point(this.Location.X + this.Speed, this.Location.Y);
                    break;
                case Direction.NONE:
                    break;
                default:
                    break;
            }
        }


    }

    public class Tank
    {
        public Point Location { set; get; }
        public Point Size { set; get; }
        public Direction Dir { set; get; }
    
        public int Speed { set; get; }
        public int HP { set; get; }
        public string Texture { set; get; }
        public Bullet bullet { set; get; }

        public Tank() { 
            
        }
        public Tank(Point loc, Point size, Direction dir, int speed)
        {
            this.Location = loc;
            this.Size = size;
            this.Dir = dir;
            this.Speed = speed;
    
            this.HP = 100;
            this.Texture = "tank.png";
            this.bullet = new Bullet();
    
        }
    
    
    
        public void Move()
        {
            switch (this.Dir)
            {
                case Direction.FRONT:
                    this.Location = new Point(this.Location.X, this.Location.Y - this.Speed);
                    break;
                case Direction.BACK:
                    this.Location = new Point(this.Location.X, this.Location.Y + this.Speed);
                    break;
                case Direction.LEFT:
                    this.Location = new Point(this.Location.X - this.Speed, this.Location.Y);
                    break;
                case Direction.RIGHT:
                    this.Location = new Point(this.Location.X + this.Speed, this.Location.Y);
                    break;
                case Direction.NONE:
                    break;
                default:
                    break;
            }
        }

        public Rectangle GetRectangle() {
            return new Rectangle(this.Location,new Size(this.Size));
        }

        public override string ToString()
        {
            return this.Location.X+" "+ this.Location.Y+" "+this.HP+" "+this.Dir.ToString();
        }



    }
}