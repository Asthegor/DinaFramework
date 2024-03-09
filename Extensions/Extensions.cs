using Microsoft.Xna.Framework.Graphics;

using System;
using System.Drawing;
using System.Numerics;

namespace DinaFramework.Extensions
{
    public static class ConvertExtensions
    {
        public static Point ToPoint(this Vector2 vector) => new Point(Convert.ToInt32(vector.X), Convert.ToInt32(vector.Y));
        public static Vector2 ToVector2(this Texture2D texture) => new Vector2(texture.Width, texture.Height);
    }
}
