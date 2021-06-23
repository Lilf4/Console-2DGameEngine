using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Ayy
{
    class Program
    {
        public static int Width = 200;
        public static int Height = 100;
        static GameSystem Game = new GameSystem(Width, Height, "Yeet", true);


        static Object TEXTOBJECT = new Object("TEXT_VIS", new Vector2(1, 1), new Vector2((-Width / 2) + 1, (Height / 2) - 1), Color.Green);
        static void Main(string[] args)
        {
            //OBJECT USED FOR DEBUGGING
            TEXTOBJECT.TEXT_OBJ = true;
            Game.AddObject(TEXTOBJECT);
            PixelArtist P = new PixelArtist();
            P.start(Game);
            //-------------------------

        }
    }
}

