using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ayy
{
    class PixelArtist
    {

        List<ConsoleKey> PressedKeys = new List<ConsoleKey>();
        Vector2 canvasSize = new Vector2(50, 50);
        public void start(GameSystem gameSys)
        {
            Vector2 ScreenSize = gameSys.Camera.GetSize();
            Object CanvCursor = new Object("Cursor", new Vector2(1, 1), new Vector2(0, 0), 999, Color.Red);
            Object DebugText = new Object("DebugText", new Vector2(1, 1), new Vector2((-ScreenSize.x / 2) + 1, (ScreenSize.y / 2) - 1), 100000, Color.Green);
            Object RedVal = new Object("RGB", new Vector2(1, 1), canvasSize / 2 + 1, 10, Color.White);
            RedVal.TEXT_OBJ = true;
            DebugText.TEXT_OBJ = true;
            gameSys.AddObject(DebugText);
            gameSys.AddObject(CanvCursor);
            gameSys.AddObject(RedVal);
            
            for(int y = (int)(canvasSize.y * .5f); y > -(int)(canvasSize.y * .5f); y--)
            {
                for(int x = -(int)(canvasSize.x * .5f); x < (int)(canvasSize.x * .5f); x++)
                {
                    Object Pixel = new Object("Canvas", new Vector2(1, 1), new Vector2(x, y), 1, Color.White);

                    gameSys.AddObject(Pixel);
                }
            }

            Vector2 AdditionVector;
            Color currColor = Color.Red;
            Vector3 ColorCode = new Vector3(currColor.R, currColor.G, currColor.B);
            int RGB = 0;

            while (true)
            {
                if (KeyDown(ConsoleKey.R))
                {
                    RGB++;
                    if (RGB > 2) { RGB = 0; }
                }
                if (KeyDown(ConsoleKey.OemPlus)) 
                {
                    switch (RGB) 
                    {
                        case 0:
                            ColorCode.x += 10;
                            if(ColorCode.x > 255) { ColorCode.x = 0; }
                            else if (ColorCode.x < 0) { ColorCode.x = 255; }
                            break;
                        case 1:
                            ColorCode.y += 10;
                            if (ColorCode.y > 255) { ColorCode.y = 0; }
                            else if (ColorCode.y < 0) { ColorCode.y = 255; }
                            break;
                        case 2:
                            ColorCode.z += 10;
                            if (ColorCode.z > 255) { ColorCode.z = 0; }
                            else if (ColorCode.z < 0) { ColorCode.z = 255; }
                            break;
                    }
                    currColor = Color.FromArgb((int)ColorCode.x, (int)ColorCode.y, (int)ColorCode.z);
                }

                RedVal.Text = "Currently changing the ";
                switch (RGB)
                {
                    case 0:
                        RedVal.Text += "\""+"R"+"\" ";
                        break;
                    case 1:
                        RedVal.Text += "\"" + "G" + "\" ";
                        break;
                    case 2:
                        RedVal.Text += "\"" + "B" + "\" ";
                        break;
                }
                RedVal.Text += "value";
                RedVal.Text += "\r\nR:" + ColorCode.x + " G:" + ColorCode.y + " B:" + ColorCode.z;
                AdditionVector = Vector2.Copy(Vector2.Zero);
                if (KeyDown(ConsoleKey.DownArrow))
                {
                    AdditionVector.y--;
                }
                if (KeyDown(ConsoleKey.UpArrow))
                {
                    AdditionVector.y++;
                }
                if (KeyDown(ConsoleKey.RightArrow))
                {
                    AdditionVector.x++;
                }
                if (KeyDown(ConsoleKey.LeftArrow))
                {
                    AdditionVector.x--;
                }
                
                if(DateTime.Now.Millisecond / 250 % 2 == 0)
                {
                    CanvCursor.Color = currColor;
                }
                else
                {
                    CanvCursor.Color = Color.Black;
                }

                CanvCursor.RePosition(CanvCursor.GetPos() + AdditionVector);
                UpdateCursor();
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
                //WrapAroundScreen
                Vector2 CurrCanvasPos = CanvCursor.GetPos();
                if (CurrCanvasPos.x < -canvasSize.x * .5f) { CurrCanvasPos.x = canvasSize.x * .5f - 1; }
                if (CurrCanvasPos.x > canvasSize.x * .5f - 1) { CurrCanvasPos.x = -canvasSize.x * .5f; }
                if (CurrCanvasPos.y < -canvasSize.y * .5f + 1) { CurrCanvasPos.y = canvasSize.y * .5f; }
                if (CurrCanvasPos.y > canvasSize.y * .5f) { CurrCanvasPos.y = -canvasSize.y * .5f + 1; }
                CanvCursor.RePosition(CurrCanvasPos);
                gameSys.GetCollision(CanvCursor);
                DebugText.Text = "";
                DebugText.Text += "Position:\r\n" + "X:" + CanvCursor.GetPos().x + "   Y:" + CanvCursor.GetPos().y + "\r\n";
                DebugText.Text += "\r\nR:" + currColor.R + ", G:" + currColor.G + ", B:" + currColor.B + "\r\n";
                DebugText.Text += "Collisions:\r\n";
                foreach (Object collision in CanvCursor.CollidingObjects)
                {
                    DebugText.Text += collision.NAME + " - ";
                    DebugText.Text += collision.ID + "\r\n";
                }

                if (KeyDown(ConsoleKey.Enter))
                {
                    foreach (Object collision in CanvCursor.CollidingObjects)
                    {
                        if(collision.NAME == "Canvas")
                        collision.Color = currColor;
                    }
                }
                gameSys.RenderScreen();
            }

        }

        int convCurser(Vector2 currsor)
        {
            return (int)(currsor.x + (currsor.y * canvasSize.x));
        }
    }
}
