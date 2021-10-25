using System;
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
    public class Tank
    {
        public Point Location { set; get; }
        public Point Size { set; get; }
        public Direction Dir { set; get; }
    
        public int Speed { set; get; }
        public int HP { set; get; }
        public string Texture { set; get; }
    
        public Tank(Point loc, Point size, Direction dir, int speed)
        {
            this.Location = loc;
            this.Size = size;
            this.Dir = dir;
            this.Speed = speed;
    
            this.HP = 100;
            this.Texture = "tank.png";
    
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
    
    
    
    
    
    }
}