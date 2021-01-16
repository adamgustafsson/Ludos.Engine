using Microsoft.Xna.Framework;
using System;

namespace Ludos.Engine.Utilities
{
    static class Extensions
    {
        public static System.Drawing.PointF CenterP(this System.Drawing.RectangleF rec)
        {
            return new System.Drawing.PointF(rec.X + rec.Width / 2,
                              rec.Y + rec.Height / 2);
        }

        public static Vector2 Center(this System.Drawing.RectangleF rec)
        {
            return new Vector2(rec.X + rec.Width / 2,
                              rec.Y + rec.Height / 2);
        }

        public static int ToInt32(this float value)
        {
            return Convert.ToInt32(value);
        }

        public static Rectangle AdjustLocation(this Rectangle rec, int x, int y)
        {
            rec.Offset(x, y);
            return rec;
        }

        public static Color FromRgb(this Color color, int r, int g, int b)
        {
            return new Color(r, b, g, 255);
        }
    }
}
