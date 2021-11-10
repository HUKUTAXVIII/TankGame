using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TankLib
{
    public class Wall
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public Wall()
        {
            X = 0;
            Y = 0;
            Width = 32;
            Height = 32;
        }
        public Wall(int x,int y)
        {
            X = x;
            Y = y;
            Width = 32;
            Height = 32;
        }
        public Rectangle GetRectangle()
        {
            return new Rectangle(X,Y,Width,Height);
        }
    }
}
