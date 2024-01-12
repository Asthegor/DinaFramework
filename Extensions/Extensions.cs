using System;
using System.Drawing;
using System.Numerics;

namespace DinaFramework.Extensions
{
    public static class ConvertExtensions
    {
        public static Point ToPoint(this Vector2 vector) => new Point(Convert.ToInt32(vector.X), Convert.ToInt32(vector.Y));
    }
}
