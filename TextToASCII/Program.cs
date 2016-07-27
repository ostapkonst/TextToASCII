using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace TextToASCII
{
    class Program
    {
        static Size GetAsciiSize(string str, int size)
        {
            using (Bitmap bitmap = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(bitmap))
            using (Font font = new Font("Arial", size, FontStyle.Bold))
                return graphics.MeasureString(str, font).ToSize();
        }

        static int GetLeftBound(Bitmap btmp)
        {
            for (int x = 0; x < btmp.Width; x++)
                for (int y = 0; y < btmp.Height; y++)
                    if (btmp.GetPixel(x, y).A > 0)
                        return x;

            return 0;
        }

        static int GetTopBound(Bitmap btmp)
        {
            for (int y = 0; y < btmp.Height; y++)
                for (int x = 0; x < btmp.Width; x++)
                    if (btmp.GetPixel(x, y).A > 0)
                        return y;

            return 0;
        }

        static int GetRightBound(Bitmap btmp)
        {
            for (int x = btmp.Width - 1; x >= 0; x--)
                for (int y = 0; y < btmp.Height; y++)
                    if (btmp.GetPixel(x, y).A > 0)
                        return x;

            return btmp.Width - 1;
        }

        static int GetBottomBound(Bitmap btmp)
        {
            for (int y = btmp.Height - 1; y >= 0; y--)
                for (int x = 0; x < btmp.Width; x++)
                    if (btmp.GetPixel(x, y).A > 0)
                        return y;

            return btmp.Height - 1;
        }

        static Rectangle GetBounds(Bitmap btmp)
        {
            int x = GetLeftBound(btmp), y = GetTopBound(btmp);
            int width = GetRightBound(btmp) - x + 1, height = GetBottomBound(btmp) - y + 1;

            return new Rectangle(x, y, width, height);
        }

        static string GetASCII(string str, int size)
        {
            Size fsize = GetAsciiSize(str, size);

            using (Bitmap bitmap = new Bitmap(fsize.Width, fsize.Height))
            {
                using (Graphics graph = Graphics.FromImage(bitmap))
                using (Font font = new Font("Arial", size, FontStyle.Bold))
                {
                    graph.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    graph.DrawString(str, font, Brushes.Black, 0f, 0f);
                }

                Rectangle rect = GetBounds(bitmap);
                StringBuilder text = new StringBuilder();

                for (int y = 0; y < rect.Height; y++)
                {
                    if (text.Length > 0) text.AppendLine();

                    for (int x = 0; x < rect.Width; x++)
                        text.Append(bitmap.GetPixel(rect.X + x, rect.Y + y).A > 0 ? "*" : " ");
                }

                return text.ToString();
            }
        }

        static void Main(string[] args)
        {
            int size;
            string str;

            using (StreamReader sr = new StreamReader("input.txt"))
            {
                size = Int32.Parse(sr.ReadLine());
                str = sr.ReadToEnd();
            }

            string res = GetASCII(str, size);

            using (StreamWriter sw = new StreamWriter("output.txt"))
                sw.Write(res);
        }
    }
}