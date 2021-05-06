using System;
using System.Diagnostics;
using System.Threading;

namespace Ayy
{
    class Program
    {
        public static int Width = 100;
        public static int Height = 40;
        static void Main(string[] args)
        {
            DrawPoint[,] DisplayBuffer = DrawPoint.InitPoint(Width, Height);
            DisplayBuffer = DrawPoint.Const('#', ConsoleColor.White, DisplayBuffer);
            Draw ConsoleDrawer = new Draw(10, Width, Height, DisplayBuffer, "Yeeetumus");
            ConsoleDrawer.Start();
            Thread.Sleep(1000);
            DisplayBuffer = DrawPoint.Const('#', ConsoleColor.DarkGreen, DisplayBuffer);
            ConsoleDrawer.UpdateBuffer(DisplayBuffer);
        }

        
    }
    public class DrawPoint
    {
        public char DisplayChar = ' ';
        public ConsoleColor DisplayColor = ConsoleColor.White;

        public static DrawPoint[,] CopyTo(DrawPoint[,] ObjToCopy, DrawPoint[,] ObjToTransferTo)
        {
            for (int y = 0; y < ObjToCopy.GetLength(1); y++)
            {
                for (int x = 0; x < ObjToCopy.GetLength(0); x++)
                {
                    if (ObjToTransferTo[x, y] == null) { ObjToTransferTo[x, y] = new DrawPoint(); }
                    ObjToTransferTo[x, y].DisplayChar = ObjToCopy[x, y].DisplayChar;
                    ObjToTransferTo[x, y].DisplayColor = ObjToCopy[x, y].DisplayColor;
                }
            }
            return ObjToTransferTo;
        }

        public static DrawPoint[,] InitPoint(int width, int height)
        {
            DrawPoint[,] Out = new DrawPoint[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Out[x, y] = new DrawPoint();
                }
            }
            return Out;
        }

        public static DrawPoint[,] Const(char Char, ConsoleColor Color, DrawPoint[,] Buffer)
        {
            for(int y = 0; y < Buffer.GetLength(1); y++) 
            {
                for(int x = 0; x < Buffer.GetLength(0); x++)
                {
                    if (Buffer[x, y] == null) { Buffer[x, y] = new DrawPoint(); }
                    Buffer[x, y].DisplayChar = Char;
                    Buffer[x, y].DisplayColor = Color;
                }
            }
            return Buffer;
        }
    }

    class Draw
    {
        public bool ForceDraw;
        int Width;
        int Height;
        int TimeBetweenUpdates;
        bool StopThreads;
        DrawPoint[,] CurrDisplay;
        DrawPoint[,] Buffer;
        /// <summary>
        /// Initiates a draw object
        /// </summary>
        /// <param name="TimeBetweenUpdates">Time in miliseconds between each frame update</param>
        /// <param name="width">Width of display</param>
        /// <param name="height">Height of display</param>
        /// <param name="StartBuffer">First buffer screen to display</param>
        /// <param name="Title">Title of console program</param>
        public Draw(int TimeBetweenUpdates, int width, int height, DrawPoint[,] StartBuffer, string Title)
        {
            this.Width = width;
            this.Height = height;
            this.TimeBetweenUpdates = TimeBetweenUpdates;
            this.CurrDisplay = DrawPoint.Const(' ', ConsoleColor.White, new DrawPoint[width, height]);
            this.Buffer = DrawPoint.CopyTo(StartBuffer, new DrawPoint[width, height]);
            if (Console.BufferHeight < Height) { Console.BufferHeight = Height; }
            if (Console.BufferWidth < Width) { Console.BufferWidth = Width; }
            if(Console.WindowHeight < Height) { Console.WindowHeight = Height; }
            if (Console.WindowWidth < Width) { Console.WindowWidth = Width; }
            Console.Title = Title;
            Console.WindowHeight = Height;
            Console.WindowWidth = Width;
        }
        /// <summary>
        /// Updates the display buffer
        /// </summary>
        /// <param name="Buffer">stuff to display on screen</param>
        public void UpdateBuffer (DrawPoint[,] Buffer)
        {
            this.Buffer = Buffer;
        }

        public void Start()
        {
            StopThreads = false;
            Thread BeginDraw = new Thread(new ThreadStart(DrawLoop));
            ForceDraw = true;
            BeginDraw.Start();
        }

        void DrawLoop()
        {
            StopThreads = false;
            while (!StopThreads)
            {
                DrawScreen();
                Thread.Sleep(TimeBetweenUpdates);
            }
        }

        public void Stop()
        {
            StopThreads = true;
        }

        void DrawScreen ()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if ((Buffer[x, y].DisplayChar != CurrDisplay[x, y].DisplayChar) || (Buffer[x, y].DisplayColor != CurrDisplay[x, y].DisplayColor) || ForceDraw)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = Buffer[x, y].DisplayColor;
                        Console.Write(Buffer[x, y].DisplayChar);
                        
                    };
                }
            }
            ForceDraw = false;
            CurrDisplay = DrawPoint.CopyTo(Buffer, CurrDisplay);
        }
    }
}
