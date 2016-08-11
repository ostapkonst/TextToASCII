﻿using System;
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
    static class Program
    {
        public static void About()
        {
            Console.WriteLine("TextToASCII Version 1.1 Copyright (c) 2016 Konstantinov Ostap");
        }

        public static void InvalidArgument()
        {
            Console.Error.WriteLine("Error: Invalid Argument");
        }

        public static void InvalidCommand()
        {
            Console.Error.WriteLine("Error: Invalid Command");
        }

        public static void Help()
        {
            Console.WriteLine("Usage: TextToASCII <command> [input] [output] [size]\n"
                + "Commands: -h Help\n"
                + "Examples: TextToASCII -h\n"
                + "TextToASCII.exe input.txt ouput.txt 10");
        }

        private static Size GetAsciiSize(string str, int size)
        {
            using (var bitmap = new Bitmap(1, 1))
            using (var graphics = Graphics.FromImage(bitmap))
            using (var font = new Font("Arial", size, FontStyle.Bold))
                return graphics.MeasureString(str, font).ToSize();
        }

        private static int GetLeftBound(Bitmap btmp)
        {
            for (int x = 0; x < btmp.Width; x++)
                for (int y = 0; y < btmp.Height; y++)
                    if (btmp.GetPixel(x, y).A > 0)
                        return x;

            return 0;
        }

        private static int GetTopBound(Bitmap btmp)
        {
            for (int y = 0; y < btmp.Height; y++)
                for (int x = 0; x < btmp.Width; x++)
                    if (btmp.GetPixel(x, y).A > 0)
                        return y;

            return 0;
        }

        private static int GetRightBound(Bitmap btmp)
        {
            for (int x = btmp.Width - 1; x >= 0; x--)
                for (int y = 0; y < btmp.Height; y++)
                    if (btmp.GetPixel(x, y).A > 0)
                        return x;

            return btmp.Width - 1;
        }

        private static int GetBottomBound(Bitmap btmp)
        {
            for (int y = btmp.Height - 1; y >= 0; y--)
                for (int x = 0; x < btmp.Width; x++)
                    if (btmp.GetPixel(x, y).A > 0)
                        return y;

            return btmp.Height - 1;
        }

        private static Rectangle GetBounds(Bitmap btmp)
        {
            int x = GetLeftBound(btmp), y = GetTopBound(btmp);
            int width = GetRightBound(btmp) - x + 1, height = GetBottomBound(btmp) - y + 1;

            return new Rectangle(x, y, width, height);
        }

        public static string GetAscii(string str, int size)
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
            switch (args.Length)
            {
                case 0:
                    About();
                    break;

                case 1:
                    switch (args[0])
                    {
                        case "-h":
                            Help();
                            break;
                        default:
                            InvalidArgument();
                            break;
                    }
                    break;

                case 3:
                    int size;
                    if (File.Exists(args[0]) && Int32.TryParse(args[2], out size))
                    {
                        string str = File.ReadAllText(args[0]);
                        string tmp = GetAscii(str, size);
                        File.WriteAllText(args[1], tmp);
                    }
                    else
                        InvalidCommand();
                    break;

                default:
                    InvalidArgument();
                    break;
            }
        }
    }
}