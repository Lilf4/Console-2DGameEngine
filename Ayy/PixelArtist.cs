using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Ayy
{
    class PixelArtist
    {

        List<ConsoleKey> PressedKeys = new List<ConsoleKey>();
        Vector2 canvasSize = new Vector2(50, 50);
        public void start(GameSystem gameSys)
        {
            List<Object> Pixels = new List<Object>();
            Vector2 CurrCanvasPos = new Vector2(11, 1);
            Vector2 LastPos = CurrCanvasPos;
            for(int y = (int)(canvasSize.y * .5f); y > -(int)(canvasSize.y * .5f); y--)
            {
                for(int x = -(int)(canvasSize.x * .5f); x < (int)(canvasSize.x * .5f); x++)
                {
                    Object Pixel = new Object("Canvas", new Vector2(1, 1), new Vector2(x, y), 1, Color.White);

                    Pixels.Add(Pixel);
                    gameSys.AddObject(Pixel);
                }
            }
            Color LastCol = Pixels[convCurser(CurrCanvasPos)].Color;
            while (true)
            {
                bool change = false;
                if (KeyDown(ConsoleKey.DownArrow))
                {
                    CurrCanvasPos.y++;
                    change = true;
                }
                if (KeyDown(ConsoleKey.UpArrow))
                {
                    CurrCanvasPos.y--;
                    change = true;
                }
                if (KeyDown(ConsoleKey.RightArrow))
                {
                    CurrCanvasPos.x++;
                    change = true;
                }
                if (KeyDown(ConsoleKey.LeftArrow))
                {
                    CurrCanvasPos.x--;
                    change = true;
                }

                if (change)
                {
                    UpdateCursor();
                }
            }


            bool KeyDown(ConsoleKey key)
            {
                if (PressedKeys.Contains(key))
                {
                    if (!Keyboard.IsKeyPressed(key))
                    {
                        PressedKeys.Remove(key);
                        return false;
                    }
                }
                else
                {
                    if (Keyboard.IsKeyPressed(key))
                    {
                        PressedKeys.Add(key);
                        return true;
                    }
                }
                return false;
            }

            void UpdateCursor()
            {
                if (CurrCanvasPos.x < 0) { CurrCanvasPos.x = canvasSize.x - 1; }
                if (CurrCanvasPos.x > canvasSize.x - 1) { CurrCanvasPos.x = 0; }
                if (CurrCanvasPos.y < 0) { CurrCanvasPos.y = canvasSize.y - 1; }
                if (CurrCanvasPos.y > canvasSize.y - 1) { CurrCanvasPos.y = 0; }
                Pixels[convCurser(LastPos)].Color = LastCol;
                LastCol = Pixels[convCurser(CurrCanvasPos)].Color;
                LastPos = Vector2.Copy(CurrCanvasPos);

                Pixels[convCurser(CurrCanvasPos)].Color = Color.Red;
                gameSys.RenderScreen();
            }

        }

        int convCurser(Vector2 currsor)
        {
            return (int)(currsor.x + (currsor.y * canvasSize.x));
        }
    }
}
