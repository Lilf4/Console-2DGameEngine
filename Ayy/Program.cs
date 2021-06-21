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
        static Random Ran = new Random();


        static Object TEXTOBJECT = new Object("TEXT_VIS", new Vector2(1, 1), new Vector2((-Width / 2) + 1, (Height / 2) - 1), Color.Green);
        static Object MenuText = new Object("DEATH_TEXT", new Vector2(1, 1), new Vector2(-10, 0),9999 ,Color.Red);
        static void Main(string[] args)
        {
            //OBJECT USED FOR DEBUGGING
            TEXTOBJECT.TEXT_OBJ = true;
            Game.AddObject(TEXTOBJECT);
            MenuText.Visible = true;
            MenuText.TEXT_OBJ = true;
            MenuText.Text = "You have lost\r\nPress 'R' to play again";
            Game.AddObject(MenuText);
            //MAIN GAME CODE GOES UNDER HERE
            int GameXSize = 50;
            int GameYSize = 50;

            Object Player = new Object("PHead", new Vector2(1, 1), new Vector2(0, 0), 1, Color.Green);
            Object APPLE = new Object("APPLE", new Vector2(1, 1), new Vector2(Ran.Next((int)ExMath.Round(-(GameXSize * .5f) + 1), (int)ExMath.Round(GameXSize * .5f - 1)), Ran.Next((int)ExMath.Round(-(GameYSize * .5f) + 1), (int)ExMath.Round(GameYSize * .5f - 1))), Color.Red);
            Game.AddObject(APPLE);
            Game.AddObject(Player);
            Game.AddObject(new Object("LWall", new Vector2(1, GameYSize), new Vector2(-(GameXSize * .5f), 0), Color.DarkGray));
            Game.AddObject(new Object("RWall", new Vector2(1, GameYSize), new Vector2(GameXSize * .5f, 0), Color.DarkGray));
            Game.AddObject(new Object("UWall", new Vector2(GameXSize, 1), new Vector2(0, GameYSize * .5f), Color.DarkGray));
            Game.AddObject(new Object("DWall", new Vector2(GameXSize, 1), new Vector2(0, -(GameYSize * .5f)), Color.DarkGray));

            Vector2 Dir = new Vector2(0, 0);
            float TimeBetweenMoves = 10;
            DateTime NextMove = DateTime.MinValue;
            Vector2 DirMove = Dir;
            bool Alive = true;
            int length = 1;
            List<Object> Body = new List<Object>();
            foreach(Object obj in Game.GetAllObjects())
            {
                //Debug.WriteLine(obj.LAYER + " : " + obj.NAME);
            }
            while (Alive)
            {
                if (Keyboard.IsKeyPressed(ConsoleKey.W) && Dir != -Vector2.Up)
                {
                    DirMove = Vector2.Up;
                }
                else if (Keyboard.IsKeyPressed(ConsoleKey.S) && Dir != Vector2.Up)
                {
                    DirMove = -Vector2.Up;
                }
                if (Keyboard.IsKeyPressed(ConsoleKey.A) && Dir != Vector2.Right)
                {
                    DirMove = -Vector2.Right;
                }
                else if (Keyboard.IsKeyPressed(ConsoleKey.D) && Dir != -Vector2.Right)
                {
                    DirMove = Vector2.Right;
                }
                if (DateTime.Now > NextMove)
                {
                    Dir = DirMove;
                    NextMove = DateTime.Now.AddMilliseconds(TimeBetweenMoves);
                    if (length > 1)
                    {
                        Body.Add(new Object("BODY", new Vector2(1, 1), Vector2.Copy(Player.GetPos()), Color.DarkOliveGreen));
                        Game.AddObject(Body[Body.Count - 1]);
                    }
                    Player.RePosition(Player.GetPos() + Dir);
                    Game.GetCollision(Player);
                    if (Body.Count > length - 1)
                    {
                        Game.RemoveObject(Body[0]);
                        Body.RemoveAt(0);
                    }

                    foreach (Object coll in Player.CollidingObjects)
                    {
                        if (coll.NAME.Contains("Wall") || coll.NAME.Contains("BODY"))
                        {
                            Alive = false;
                            return;
                        }
                        if (coll.NAME.Contains("APPLE"))
                        {
                            APPLE.RePosition(new Vector2(Ran.Next((int)ExMath.Round(-(GameXSize * .5f) + 1), (int)ExMath.Round(GameXSize * .5f - 1)), Ran.Next((int)ExMath.Round(-(GameYSize * .5f) + 1), (int)ExMath.Round(GameYSize * .5f - 1))));
                            length++;
                        }
                    }
                }
                TEXTOBJECT.Text = $"FPS average - {Game.Renderer.AvgFramesPerSec}\r\nCurr move direction : {Dir.x}, {Dir.y}\r\nFuture move direction : {DirMove.x}, {DirMove.y}\r\nLength - {length}\r\nBody length - {Body.Count}\r\nHead pos : : {Player.GetPos().x}, {Player.GetPos().y}";
                while (!Alive) { }
                Game.RenderScreen();
            }

        }
    }
}

/// <summary>
/// legit just straight yoinked this part from u/flying20wedge
/// </summary>
public static class Keyboard
{
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetKeyboardState(byte[] lpKeyState);

    public static int GetKeyState()
    {


        byte[] keys = new byte[256];

        //Get pressed keys
        if (!GetKeyboardState(keys))
        {
            int err = Marshal.GetLastWin32Error();
            throw new Win32Exception(err);
        }

        for (int i = 0; i < 256; i++)
        {

            byte key = keys[i];

            //Logical 'and' so we can drop the low-order bit for toggled keys, else that key will appear with the value 1!
            if ((key & 0x80) != 0)
            {

                //This is just for a short demo, you may want this to return
                //multiple keys!
                return (int)key;
            }
        }
        return -1;
    }

    [DllImport("user32.dll")]
    static extern short GetKeyState(ConsoleKey nVirtKey);

    public static bool IsKeyPressed(ConsoleKey testKey)
    {
        bool keyPressed = false;
        short result = GetKeyState(testKey);

        switch (result)
        {
            case 0:
                // Not pressed and not toggled on.
                keyPressed = false;
                break;

            case 1:
                // Not pressed, but toggled on
                keyPressed = false;
                break;

            default:
                // Pressed (and may be toggled on)
                keyPressed = true;
                break;
        }

        return keyPressed;
    }



    private const uint MAPVK_VK_TO_CHAR = 2;

    [DllImport("user32.dll")]
    static extern uint MapVirtualKeyW(uint uCode, uint uMapType);

    public static char KeyToChar(ConsoleKey key)
    {
        return unchecked((char)MapVirtualKeyW((uint)key, MAPVK_VK_TO_CHAR)); // Ignore high word.  
    }
}
