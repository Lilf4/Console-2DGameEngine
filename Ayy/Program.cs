using System;
using System.Diagnostics;
using System.Threading;

namespace Ayy
{
    class Program
    {
        public static int Width = 100;
        public static int Height = 100;
        static DrawPoint[,] WhiteSreen = new DrawPoint[Width, Height];
        static void Main(string[] args)
        {
            DrawPoint[,] DisplayBuffer = DrawPoint.InitPoint(Width, Height);
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    TestBuffer[x, y] = new DrawPoint();
                    TestBuffer[x, y].DisplayChar = '#';
                    TestBuffer[x, y].DisplayColor = ConsoleColor.White;
                }
            }
            WhiteSreen = DrawPoint.Copy(TestBuffer);
           
            Draw ConsoleDrawer = new Draw(10, Width, Height, TestBuffer, "Yeeetumus");
            ConsoleDrawer.Start();
            Random Ran = new Random();
            while (true)
            {
                for(int i = 0; i < 100; i++)
                {
                    int x = Ran.Next(0, Width);
                    int y = Ran.Next(0, Height);
                    if (TestBuffer[x, y].DisplayColor == ConsoleColor.Black)
                    { i--; continue; }
                    TestBuffer[x, y].DisplayColor = ConsoleColor.Black;
                    
                }
                ConsoleDrawer.UpdateBuffer(TestBuffer);
                Thread.Sleep(1000);
                TestBuffer = DrawPoint.Copy(WhiteSreen);
            }
        }

        
    }
    public class DrawPoint
    {
        public char DisplayChar = ' ';
        public ConsoleColor DisplayColor = ConsoleColor.White;

        public static DrawPoint[,] Copy(DrawPoint[,] ObjToCopy)
        {
            DrawPoint[,] A = new DrawPoint[ObjToCopy.GetLength(0), ObjToCopy.GetLength(1)];
            
            for (int y = 0; y < ObjToCopy.GetLength(1); y++)
            {
                for (int x = 0; x < ObjToCopy.GetLength(0); x++)
                {
                    A[x, y] = new DrawPoint();
                    A[x, y].DisplayChar = ObjToCopy[x, y].DisplayChar;
                    A[x, y].DisplayColor = ObjToCopy[x, y].DisplayColor;
                }
            }
            return A;
        }

        public DrawPoint[,] InitPoint(int width, int height)
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
    }

    class Draw
    {
        bool ForceDraw;
        int Width;
        int Height;
        int TimeBetweenUpdates;
        bool StopThreads;
        DrawPoint[,] CurrDisplay;
        DrawPoint[,] Buffer;
        /// <summary>
        /// Initiates a draw object
        /// 
        /// </summary>
        /// <param name="TimeBetweenUpdates">Time in miliseconds between each frame update</param>
        public Draw(int TimeBetweenUpdates, int width, int height, DrawPoint[,] StartBuffer, string Title)
        {
            this.Width = width;
            this.Height = height;
            this.TimeBetweenUpdates = TimeBetweenUpdates;
            this.CurrDisplay = StartBuffer;
            this.Buffer = StartBuffer;
            if (Console.BufferHeight < Height) { Console.BufferHeight = Height; }
            if (Console.BufferWidth < Width) { Console.BufferWidth = Width; }
            Console.Title = Title;
        }
        /// <summary>
        /// Updates the display buffer with a new one
        /// This is technically just the same as directly changing the Buffer value itself
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
                    if (Buffer[x, y].DisplayChar != CurrDisplay[x, y].DisplayChar || Buffer[x, y].DisplayColor != CurrDisplay[x, y].DisplayColor || ForceDraw)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = Buffer[x, y].DisplayColor;
                        Console.Write(Buffer[x, y].DisplayChar);
                        
                    };
                }
            }
            ForceDraw = false;
            CurrDisplay = Buffer;
        }
    }
}
