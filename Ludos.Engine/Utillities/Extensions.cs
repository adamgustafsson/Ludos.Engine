using Microsoft.Xna.Framework;
using System;
using System.Drawing;

namespace Ludos.Engine.Utilities
{
    static class Extensions
    {
        public static PointF CenterP(this RectangleF rec)
        {
            return new PointF(rec.X + rec.Width / 2,
                              rec.Y + rec.Height / 2);
        }

        public static Vector2 Center(this RectangleF rec)
        {
            return new Vector2(rec.X + rec.Width / 2,
                              rec.Y + rec.Height / 2);
        }

        public static int ToInt32(this float value)
        {
            return Convert.ToInt32(value);
        }
    }
}
