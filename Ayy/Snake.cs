using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Ayy
{
    class Snake
    {
        static Object MenuText = new Object("DEATH_TEXT", new Vector2(1, 1), new Vector2(-10, 0), 9999, Color.Red);
        public void Start(GameSystem gameSys)
        {
            MenuText.Visible = false;
            MenuText.TEXT_OBJ = true;
            MenuText.Text = "You have lost\r\nPress 'R' to play again";
            gameSys.AddObject(MenuText);
            //MAIN GAME CODE GOES UNDER HERE
            int GameXSize = 50;
            int GameYSize = 50;

            Object Player = new Object("PHead", new Vector2(1, 1), new Vector2(0, 0), 1, Color.Green);
            Object APPLE = new Object("APPLE", new Vector2(1, 1), new Vector2(gameSys.Ran.Next((int)ExMath.Round(-(GameXSize * .5f) + 1), (int)ExMath.Round(GameXSize * .5f - 1)), gameSys.Ran.Next((int)ExMath.Round(-(GameYSize * .5f) + 1), (int)ExMath.Round(GameYSize * .5f - 1))), -1, Color.Red);
            gameSys.AddObject(APPLE);
            gameSys.AddObject(Player);
            gameSys.AddObject(new Object("LWall", new Vector2(1, GameYSize), new Vector2(-(GameXSize * .5f), 0), Color.DarkGray));
            gameSys.AddObject(new Object("RWall", new Vector2(1, GameYSize), new Vector2(GameXSize * .5f, 0), Color.DarkGray));
            gameSys.AddObject(new Object("UWall", new Vector2(GameXSize, 1), new Vector2(0, GameYSize * .5f), Color.DarkGray));
            gameSys.AddObject(new Object("DWall", new Vector2(GameXSize, 1), new Vector2(0, -(GameYSize * .5f)), Color.DarkGray));

            Vector2 Dir = new Vector2(0, 0);
            float TimeBetweenMoves = 100;
            DateTime NextMove = DateTime.MinValue;
            Vector2 DirMove = Dir;
            bool Alive = true;
            int length = 1;
            List<Object> Body = new List<Object>();
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
                    if (length > 1 && length > Body.Count + 1)
                    {
                        Body.Add(new Object("BODY", new Vector2(1, 1), Vector2.Copy(Player.GetPos()), Color.Aqua));
                        gameSys.AddObject(Body[Body.Count - 1]);
                    }
                    if (length > 1)
                    {
                        for (int i = Body.Count - 1; i > 0; i--)
                        {
                            Body[i].RePosition(Body[i - 1].GetPos());
                        }
                        Body[0].RePosition(Player.GetPos());
                    }
                    Player.RePosition(Player.GetPos() + Dir);
                    gameSys.GetCollision(Player);

                    foreach (Object coll in Player.CollidingObjects)
                    {
                        if (coll.NAME.Contains("Wall") || coll.NAME.Contains("BODY"))
                        {
                            Alive = false;
                            return;
                        }
                        if (coll.NAME.Contains("APPLE"))
                        {
                            bool Usable = false;
                            while (!Usable)
                            {
                                APPLE.RePosition(new Vector2(gameSys.Ran.Next((int)ExMath.Round(-(GameXSize * .5f) + 1), (int)ExMath.Round(GameXSize * .5f - 1)), gameSys.Ran.Next((int)ExMath.Round(-(GameYSize * .5f) + 1), (int)ExMath.Round(GameYSize * .5f - 1))));
                                gameSys.GetCollision(APPLE);
                                Usable = true;
                                foreach (Object aColl in APPLE.CollidingObjects)
                                {
                                    if (aColl.NAME.Contains("BODY"))
                                    { Usable = false; return; }

                                }

                            }


                            length++;
                        }
                    }
                }
                while (!Alive) { MenuText.Visible = true; }
                gameSys.RenderScreen();
            }
        }
    }
}
