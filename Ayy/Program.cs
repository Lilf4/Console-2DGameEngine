using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace Ayy
{
	class Program
	{
		public static int Width = 200;
		public static int Height = 100;
		static GameSystem Game = new GameSystem(Width, Height, "Yeet");
		public static Vector2 MoveVel = Vector2.Zero;
		static Object Player = new Object("PLAYER", new Vector2(5, 5), new Vector2(0, 0), Color.DarkBlue);
		static Object PGroundCol = new Object("AAA", new Vector2(5, 1), new Vector2(10, 0), Color.Green);
		static Object TEXTOBJECT = new Object("TEXT_VIS", new Vector2(1, 1), new Vector2((-Width / 2) + 1, (Height / 2) - 1), Color.Green);
		static void Main(string[] args)
		{
			TEXTOBJECT.TEXT_OBJ = true;
			//Player.AddObjectAsChild(PlayerGroundCollision);

			PGroundCol.Visible = false;
			Game.ConsoleDrawer.LTRTTB_TTBLTR = true;
            Game.AddObject(new Object("GROUND", new Vector2(Width + 1, 10), new Vector2(0, -MathF.Round(Height / 2)), Color.Gray));

			//Thread DEBUG = new Thread(new ThreadStart(DebugS));
			//DEBUG.Start();
			Game.AddObject(PGroundCol);
			Game.AddObject(Player);
			Game.AddObject(TEXTOBJECT);
			Game.RenderScreen();
            float TimeBetween = DateTime.Now.Millisecond;
            Vector2 MoveVectors = new Vector2(1, 1);
			Vector2 MoveSpeed = new Vector2(5, 5);
			Vector2 Gravity = new Vector2(0, -5.7f);

			List<Object> SINEWAVE = new List<Object>();

			/*for(float i = 0; i < 200; i += 1f)
            {
				SINEWAVE.Add(new Object("SINE", new Vector2(1, 1), new Vector2(i - 100, 0)));
            }
			float Volume = 20f;
			float Frequency = 5f; 
			float WAVEXPOS = 0f;
			foreach (Object obj in SINEWAVE)
            {
				Game.AddObject(obj);
				obj.RePosition(new Vector2(obj.GetPos().x, MathF.Sin(WAVEXPOS * Frequency) * Volume));
				WAVEXPOS += 0.001f;
			}*/
			bool KeyWasPressed;

			float TIME = 0;
			while (true)
            {
                TimeBetween = (DateTime.Now.Millisecond - TimeBetween) / 1000;
                if(TimeBetween < 0) { TimeBetween = 0; }

				/*
				TIME += TimeBetween;
				WAVEXPOS = 0f;
				foreach (Object obj in SINEWAVE)
				{
					obj.RePosition(obj.GetPos().x, MathF.Sin((WAVEXPOS * Frequency) + TIME) * Volume);
					WAVEXPOS += 0.01f;
				}*/



				PGroundCol.RePosition(Player.GetPos() + Vector2.Down * (Player.GetSize().y * 0.5f));
				Player.RePosition(Player.GetPos() + (MoveVel * (MoveVectors * MoveSpeed * TimeBetween)));
				KeyWasPressed = false;
				if (Keyboard.IsKeyPressed(ConsoleKey.A))
				{
					MoveVel += Vector2.Left * 3;
					KeyWasPressed = true;
				}
				if (Keyboard.IsKeyPressed(ConsoleKey.D))
				{
					MoveVel += Vector2.Right * 3;
					KeyWasPressed = true;
				}
                if (!KeyWasPressed)
                {
					MoveVel.x = 0;
                }
				MoveVel += Gravity * TimeBetween;

				TEXTOBJECT.txt = "";
				for(int i = 0; i < PGroundCol.CollidingObjects.Count; i++)
                {
					TEXTOBJECT.txt += PGroundCol.CollidingObjects[i].NAME + "\r\n";
				}

				TEXTOBJECT.txt = TEXTOBJECT.txt.Remove(TEXTOBJECT.txt.LastIndexOf("\r\n"));
				foreach (Object a in PGroundCol.CollidingObjects)
				{
					if(a.NAME == "GROUND")
                    {
						if (Keyboard.IsKeyPressed(ConsoleKey.W))
						{
							MoveVel.y += 10;
						}
						else
						{
							MoveVel.y = 0;
							Player.RePosition(new Vector2(Player.GetPos().x, a.GetPos().y + (a.GetSize().y / 2) + (Player.GetSize().y / 2)));
							break;
						}
					}
				}
				MoveVel = Vector2.Clamp(MoveVel, -3f, 3f, -5f, 5f);
				
				TimeBetween = DateTime.Now.Millisecond;
                Game.RenderScreen();
            }
        }


		static void DebugS()
        {
            while (true)
            {
				Debug.WriteLine(Game.Camera.GetPos());
				Debug.WriteLine(Player.GetPos());
				Thread.Sleep(2000);
			}
        }
	}

		/// <summary>
		/// yoinked from u/flying20wedge
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
	
}
