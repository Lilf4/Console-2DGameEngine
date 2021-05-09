using System;
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
		static Object Player = new Object("PLAYER", new Vector2(3, 3), new Vector2(0, 0), Color.DarkBlue);

		static void Main(string[] args)
		{
			Game.ConsoleDrawer.LTRTTB_TTBLTR = true;
            Game.AddObject(new Object("GROUND", new Vector2(Width + 1, 10), new Vector2(0, -MathF.Round(Height / 2)), Color.Gray));
			Thread DEBUG = new Thread(new ThreadStart(DebugS));
			//DEBUG.Start();
            Game.AddObject(Player);
            Game.RenderScreen();
            float TimeBetween = DateTime.Now.Millisecond;
            Vector2 MoveVectors = new Vector2(1, 1);
			Vector2 MoveSpeed = new Vector2(5, 5);
			Vector2 Gravity = new Vector2(0, -5.7f);
			bool KeyWasPressed;

			while (true)
            {
                TimeBetween = (DateTime.Now.Millisecond - TimeBetween) / 1000;
                if(TimeBetween < 0) { TimeBetween = 0; }
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
				foreach (Object a in Player.CollidingObjects)
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
						}
					}
				}

				MoveVel = Vector2.Clamp(MoveVel, -3f, 3f, -5f, 5f);

				TimeBetween = DateTime.Now.Millisecond;
                Game.RenderScreen();
            }
        }
		static float lerp(float v0, float v1, float t)
		{
			return v0 + t * (v1 - v0);
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
