namespace Ludos.Engine.Utilities
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public static class Utilities
    {
        public static Texture2D CreateTexture2D(GraphicsDevice graphicsDevice, Point size, Color color, float transparency)
        {
            return CreateTexture2D(graphicsDevice, new System.Drawing.Size(size.X, size.Y), color, transparency);
        }

        public static Texture2D CreateTexture2D(GraphicsDevice graphicsDevice, System.Drawing.Size size, Color color, float transparency)
        {
            Texture2D r = new Texture2D(graphicsDevice, size.Width, size.Height);
            Color[] data = new Color[size.Width * size.Height];

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = color * transparency;
            }

            r.SetData(data);
            return r;
        }
    }
}
