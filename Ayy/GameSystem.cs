using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

public class GameSystem
{
    public Color Background = Color.White;
    public char BackgroundChar = ' ';
    DrawPoint[,] DisplayBuffer;
    public Draw Renderer;

    public Random Ran = new Random();
    List<Object> Objects = new List<Object>();
    public Object Camera;

    public GameSystem(int ScreenWidth, int ScreenHeight)
    {
        SETUP(ScreenWidth, ScreenHeight, "", false);
    }
    public GameSystem(int ScreenWidth, int ScreenHeight, bool LTRTTB_TTBLTR)
    {
        SETUP(ScreenWidth, ScreenHeight, "", LTRTTB_TTBLTR);
    }
    public GameSystem(int ScreenWidth, int ScreenHeight, string GameName)
    {
        SETUP(ScreenWidth, ScreenHeight, GameName, false);
    }
    public GameSystem(int ScreenWidth, int ScreenHeight, string GameName, bool LTRTTB_TTBLTR)
    {
        SETUP(ScreenWidth, ScreenHeight, GameName, LTRTTB_TTBLTR);
    }

    //Quick fix - should prb look into changing this later
    Dictionary<int, List<Object>> QuickSortDic = new Dictionary<int, List<Object>>();
    public void ChangeLayer(Object obj, int layer)
    {
        if(Objects.Count > 0)
        {
            List<int> Indexes = new List<int>();

            Objects.Remove(obj);
            obj.LAYER = layer;
            QuickSortDic.Clear();
            List<Object> ObjectsList;
            for(int i = 0; i < Objects.Count; i++)
            {
                QuickSortDic.TryGetValue(Objects[i].LAYER, out ObjectsList);
                if(ObjectsList == null)
                {
                    QuickSortDic.Add(Objects[i].LAYER, new List<Object>());
                    QuickSortDic.TryGetValue(Objects[i].LAYER, out ObjectsList);
                    Indexes.Add(Objects[i].LAYER);
                }
                ObjectsList.Add(Objects[i]);
            }
            QuickSortDic.TryGetValue(obj.LAYER, out ObjectsList);
            if (ObjectsList == null)
            {
                QuickSortDic.Add(obj.LAYER, new List<Object>());
                QuickSortDic.TryGetValue(obj.LAYER, out ObjectsList);
                Indexes.Add(obj.LAYER);
            }
            ObjectsList.Add(obj);
            Indexes.Sort();
            Objects.Clear();
            foreach(int index in Indexes)
            {
                QuickSortDic.TryGetValue(index, out ObjectsList);
                foreach(Object _obj in ObjectsList)
                {
                    Objects.Add(_obj);
                }
            }
            QuickSortDic.Clear();
        }
        else
        {
            Objects.Add(obj);
        }
    }

    

    void SETUP(int ScreenWidth, int ScreenHeight, string GameName, bool LTRTTB_TTBLTR)
    {
        Camera = new Object("CAMERA", new Vector2(ScreenWidth, ScreenHeight), new Vector2(0, 0));
        DisplayBuffer = DrawPoint.InitPoint(ScreenWidth, ScreenHeight);
        DisplayBuffer = DrawPoint.Const(BackgroundChar, Background, DisplayBuffer);
        Renderer = new Draw(ScreenWidth, ScreenHeight, DisplayBuffer, GameName);
        Renderer.LTRTTB_TTBLTR = LTRTTB_TTBLTR;
        Renderer.Start();
        AddObject(Camera);
    }


    public void AddObject(Object obj)
    {
        while (!isValidID(Ran.Next(9999999).ToString())) { }

        ChangeLayer(obj, obj.LAYER);
        

        bool isValidID(string ID)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                if (ID == Objects[i].ID)
                {
                    return false;
                }
            }
            obj.ID = ID;
            return true;
        }
    }

    

    public void GetCollision(Object obj)
    {
        obj.CollidingObjects.Clear();
        for (int x = 0; x < Objects.Count; x++)
        {
            if (obj.NAME != Objects[x].NAME && obj.CheckIfColliding(Objects[x]))
            {
                obj.CollidingObjects.Add(Objects[x]);
            }
        }
    }
    public void GetCollision(Object[] obj)
    {
        foreach(Object Iobj in obj)
        {
            Iobj.CollidingObjects.Clear();
            for (int x = 0; x < Objects.Count; x++)
            {
                if (Iobj.NAME != Objects[x].NAME && Iobj.CheckIfColliding(Objects[x]))
                {
                    Iobj.CollidingObjects.Add(Objects[x]);
                }
            }
        }
        
    }

    //NOTE: there are still some weird artifacts going on as if something is being rounded the wrong way --- no idea if this is still the case i hafta look into it - TODO
    //yes there are indeed still some artifacts around the 0 coordinate but that will hafta wait
    public void RenderScreen()
    {
        Vector2 localPos;
        Vector2 AdditionVector;
        Vector2 LastPos;

        GetCollision(Camera);
        DisplayBuffer = DrawPoint.Const(BackgroundChar, Background, DisplayBuffer);
        for (int i = 0; i < Camera.CollidingObjects.Count; i++)
        {
            if (Camera.CollidingObjects[i].Visible)
            {
                if (Camera.CollidingObjects[i].NAME != Camera.NAME && !Camera.CollidingObjects[i].TEXT_OBJ)
                {
                    LastPos = Camera.CollidingObjects[i].GetPos() - 1;
                    localPos = Camera.LocalizePos(Camera.CollidingObjects[i]);
                    localPos -= Camera.CollidingObjects[i].GetSize() / 2;
                    for (int y = 0; y < Camera.CollidingObjects[i].GetSize().y; y++)
                    {

                        AdditionVector = Vector2.Copy(localPos);
                        LastPos.y = y + localPos.y - 1;
                        if (LastPos.y == ExMath.Round(y + localPos.y)) { AdditionVector.y++; }
                        for (int x = 0; x < Camera.CollidingObjects[i].GetSize().x; x++)
                        {
                            LastPos.x = x + localPos.x - 1;
                            if (LastPos.x == ExMath.Round(x + localPos.x)) { AdditionVector.x++; }

                            DisplayBuffer = DrawPoint.InsertAsMiddle(DisplayBuffer, (int)ExMath.Round(x + AdditionVector.x), (int)ExMath.Round(y + AdditionVector.y), '#', Camera.CollidingObjects[i].Color);


                        }

                    }
                }
                else if (Camera.CollidingObjects[i].TEXT_OBJ)
                {
                    localPos = Camera.LocalizePos(Camera.CollidingObjects[i]);
                    string[] TEXT = Camera.CollidingObjects[i].Text.Split("\r\n");
                    for (int y = 0; y < TEXT.Length; y++)
                    {
                        for (int x = 0; x < TEXT[y].Length; x++)
                        {
                            //Debug.WriteLine(TEXT[y] + " : " + y);
                            DisplayBuffer = DrawPoint.InsertAsMiddle(DisplayBuffer, (int)ExMath.Round(x + localPos.x), (int)ExMath.Round(localPos.y - y), TEXT[y][x], Camera.CollidingObjects[i].Color);
                        }
                    }
                }
            }
        }
        Renderer.UpdateBuffer(DisplayBuffer);
    }

    public Object[] GetObjectByName(string Name)
    {
        List<Object> Objects = new List<Object>();
        for (int i = 0; i < this.Objects.Count; i++)
        {
            if (this.Objects[i].NAME == Name)
            {
                Objects.Add(this.Objects[i]);
            }
        }
        return Objects.ToArray();
    }
    public Object[] GetAllObjects()
    {
        return Objects.ToArray();
    }
    public Object GetObjectByID(string ID)
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            if (Objects[i].ID == ID)
            {
                return Objects[i];
            }
        }
        return null;
    }

    public void RemoveObject(Object obj)
    {
        Objects.Remove(obj);
    }
}
public class DrawPoint
{
    public char DisplayChar = ' ';
    public Color DisplayColor = Color.White;

    //FUNCTIONS
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
    public static DrawPoint[,] InsertAsMiddle(DrawPoint[,] array, int x, int y, char Char, Color Color)
    {
        Vector2 A = new Vector2(x + (array.GetLength(0) * .5f), -y + (array.GetLength(1) * .5f));
        if (A.x > -1 && A.x < array.GetLength(0) && A.y > -1 && A.y < array.GetLength(1))
        {
            array[(int)A.x, (int)A.y].DisplayChar = Char;
            array[(int)A.x, (int)A.y].DisplayColor = Color;
        }
        return array;
    }
    public static DrawPoint[,] Const(char Char, Color Color, DrawPoint[,] Buffer)
    {
        for (int y = 0; y < Buffer.GetLength(1); y++)
        {
            for (int x = 0; x < Buffer.GetLength(0); x++)
            {
                if (Buffer[x, y] == null) { Buffer[x, y] = new DrawPoint(); }
                Buffer[x, y].DisplayChar = Char;
                Buffer[x, y].DisplayColor = Color;
            }
        }
        return Buffer;
    }
}
public class Draw
{
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetConsoleMode(IntPtr handle, out int mode);
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int handle);

    public bool ForceDraw;
    /// <summary>
    /// <para>false = Draw screen Left To Right Top to Bottom</para>
    /// <para>true = Draw screen Top to Bottom Left To Right</para>
    /// </summary>
    public bool LTRTTB_TTBLTR;
    int Width;
    int Height;
    /// <summary>
    /// <para>Only used if bottlenecking</para>
    /// <para>Target frames per sec</para>
    /// </summary>
    public int AvgFramesPerSec;
    bool StopThreads;
    DrawPoint[,] CurrDisplay;
    DrawPoint[,] Buffer;
    DrawPoint[,] SafeGaurdBuffer;
    /// <summary>
    /// Initiates a draw object
    /// </summary>
    /// <param name="width">Width of display</param>
    /// <param name="height">Height of display</param>
    /// <param name="StartBuffer">First buffer screen to display</param>
    /// <param name="Title">Title of console program</param>
    public Draw(int width, int height, DrawPoint[,] StartBuffer, string Title)
    {
        Console.CursorVisible = false;
        this.Width = width;
        this.Height = height;
        this.CurrDisplay = DrawPoint.Const(' ', Color.White, new DrawPoint[width, height]);
        this.Buffer = DrawPoint.CopyTo(StartBuffer, new DrawPoint[width, height]);
        this.SafeGaurdBuffer = DrawPoint.CopyTo(StartBuffer, new DrawPoint[width, height]);
        if (Console.BufferHeight < Height) { Console.BufferHeight = Height; }
        if (Console.BufferWidth < Width) { Console.BufferWidth = Width; }
        if (Console.WindowHeight < Height) { Console.WindowHeight = Height; }
        if (Console.WindowWidth < Width) { Console.WindowWidth = Width; }
        Console.Title = Title;
    }
    /// <summary>
    /// Updates the display buffer
    /// </summary>
    /// <param name="Buffer">stuff to display on screen</param>
    
    int CurrFrames;
    public void UpdateBuffer(DrawPoint[,] Buffer)
    {
        if(SafeGaurdBuffer != Buffer)
        {
            this.SafeGaurdBuffer = DrawPoint.CopyTo(Buffer, this.SafeGaurdBuffer);
            DrawScreen();
        }

        CurrFrames++;
    }

    void FrameLoop()
    {
        while (!StopThreads)
        {
            Thread.Sleep(1000);
            AvgFramesPerSec = CurrFrames;
            CurrFrames = 0;
        }
    }

    public void Start()
    {
        var handle = GetStdHandle(-11);
        int mode;
        GetConsoleMode(handle, out mode);
        SetConsoleMode(handle, mode | 0x4);

        StopThreads = false;
        Thread BeginFPSC = new Thread(new ThreadStart(FrameLoop));
        ForceDraw = true;
        DrawScreen();
        BeginFPSC.Start();

    }
    public void Stop()
    {
        StopThreads = true;
    }
    Color DisColor;
    void DrawScreen()
    {
        Buffer = DrawPoint.CopyTo(SafeGaurdBuffer, Buffer);
        switch (LTRTTB_TTBLTR)
        {
            case false:
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if ((Buffer[x, y].DisplayChar != CurrDisplay[x, y].DisplayChar) || (Buffer[x, y].DisplayColor != CurrDisplay[x, y].DisplayColor) || ForceDraw)
                        {
                            DrawToPoint(x, y);
                        };
                    }
                }
                break;
            case true:
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if ((Buffer[x, y].DisplayChar != CurrDisplay[x, y].DisplayChar) || (Buffer[x, y].DisplayColor != CurrDisplay[x, y].DisplayColor) || ForceDraw)
                        {
                            DrawToPoint(x, y);
                        }
                    }
                }
                break;
        }

        Console.SetCursorPosition(0, 0);
        ForceDraw = false;
        CurrDisplay = DrawPoint.CopyTo(Buffer, CurrDisplay);
    }

    void DrawToPoint(int x, int y)
    {
        Console.SetCursorPosition(x, y);
        DisColor = Buffer[x, y].DisplayColor;
        Console.Write($"\x1b[38;2;{DisColor.R};{DisColor.G};{DisColor.B}m{Buffer[x, y].DisplayChar}");
    }
}

public class Object
{
    public string ID;
    public string NAME;
    public int LAYER;
    public bool Visible = true;
    public bool Collision = true;
    public bool TEXT_OBJ;
    public string Text = "";
    public Color Color = Color.White;
    Vector2 TopLeft;
    Vector2 BotRight;
    Vector2 Size;
    Vector2 Position;
    public Vector2 CentParPos;
    public List<Object> CollidingObjects = new List<Object>();
    public List<Object> Children = new List<Object>();
    public Object Parent;

    public Object(string NAME, Vector2 Size, Vector2 Position) { CREATEOBJECT(NAME, Size, Position, Color.White, 0); }
    public Object(string NAME, Vector2 Size, Vector2 Position, Color Color) { CREATEOBJECT(NAME, Size, Position, Color, 0); }
    public Object(string NAME, Vector2 Size, Vector2 Position, int Layer) { CREATEOBJECT(NAME, Size, Position, Color.White, Layer); }

    public Object(string NAME, Vector2 Size, Vector2 Position, int Layer, Color Color) { CREATEOBJECT(NAME, Size, Position, Color, Layer); }
    void CREATEOBJECT(string NAME, Vector2 Size, Vector2 Position, Color Color, int Layer)
    {
        this.NAME = NAME;
        this.Size = Size;
        this.Position = Position;
        this.CentParPos = Position;
        this.Color = Color;
        this.LAYER = Layer;
        CalcNewSizePos();
    }


    public Vector2 LocalizePos(Object obj) { return obj.Position - Position; }
    void RecalcParentPos()
    {
        this.Position = CentParPos + Parent.GetPos();
        CalcNewSizePos();
    }
    public void Resize(Vector2 Size)
    {
        this.Size = Size;
        CalcNewSizePos();
    }
    void ReposChildren()
    {
        if (Children.Count > 0)
        {
            foreach (Object child in Children)
            {
                child.RecalcParentPos();
            }
        }
    }
    public void RePosition(Vector2 Position) { REPOS(Position.x, Position.y, false); }
    public void RePosition(float x, float y) { REPOS(x, y, false); }
    public void RePosition(float x, float y, bool doParent) { REPOS(x, y, doParent); }
    public void RePosition(Vector2 Position, bool doParent) { REPOS(Position.x, Position.y, doParent); }
    void REPOS(float x, float y, bool doParent)
    {
        switch (doParent)
        {
            case true:
                CentParPos.x = x;
                CentParPos.y = y;
                RecalcParentPos();
                break;
            case false:
                this.Position.x = x;
                this.Position.y = y;
                break;
        }
        ReposChildren();
        CalcNewSizePos();
    }
    public void AddObjectAsChild(Object Child)
    {
        this.Children.Add(Child);
        Child.Parent = this;
    }
    public Vector2 GetPos() { return Position; }
    public Vector2 GetSize() { return Size; }
    public bool CheckIfColliding(Object Obj) { return Vector2.doOverlap(this.TopLeft, this.BotRight, Obj.TopLeft, Obj.BotRight); }
    void CalcNewSizePos()
    {
        TopLeft = new Vector2(-Size.x / 2, Size.y / 2) + Position;
        BotRight = new Vector2(Size.x / 2, -Size.y / 2) + Position;
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