using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

public class GameSystem
{
    public Color Background = Color.White;
    public char BackgroundChar = ' ';
    DrawPoint[,] DisplayBuffer;
    public Draw ConsoleDrawer;

    Random Ran = new Random();
    List<Object> Objects = new List<Object>();
    List<Object> ObjectCollision = new List<Object>();
    List<Object> ObjectsToCheck = new List<Object>();
    public Object Camera;
    public GameSystem(int ScreenWidth, int ScreenHeight, string GameName)
    {
        Camera = new Object("CAMERA", new Vector2(ScreenWidth, ScreenHeight), new Vector2(0, 0));
        DisplayBuffer = DrawPoint.InitPoint(ScreenWidth, ScreenHeight);
        DisplayBuffer = DrawPoint.Const(BackgroundChar, Background, DisplayBuffer);
        ConsoleDrawer = new Draw(ScreenWidth, ScreenHeight, DisplayBuffer, GameName);
        ConsoleDrawer.Start();
        AddObject(Camera);
    }

    public void AddObject(Object obj)
    {
        while (!isValidID(Ran.Next(9999999).ToString())) { }

        Objects.Add(obj);
        ObjectCollision.Add(obj);

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

    Vector2 localPos;
    Vector2 AdditionVector = Vector2.Copy(Vector2.Zero);
    Vector2 LastPos = Vector2.Copy(Vector2.Zero);


    //NOTE: STUFF WORKS BUT IT NEEDS TO BE SMOOTHED IN SOME WAY AS RIGHT NOW MOVING THINGS ARE PRETTY JANKY
    public void RenderScreen()
    {
        DoCollisionChecks();
        DisplayBuffer = DrawPoint.Const(BackgroundChar, Background, DisplayBuffer);
        for (int i = 0; i < Camera.CollidingObjects.Count; i++)
        {
            if (Camera.CollidingObjects[i].Visible && Camera.CollidingObjects[i].NAME != Camera.NAME)
            {
                LastPos = Vector2.Copy(Vector2.Zero);
                localPos = Camera.LocalizePos(Camera.CollidingObjects[i]);
                localPos -= Camera.CollidingObjects[i].GetSize() / 2;
                for (int y = 0; y < Camera.CollidingObjects[i].GetSize().y; y++)
                {
                    AdditionVector = Vector2.Copy(localPos); 
                    if(LastPos.y == ExMath.Round(y + localPos.y)) { AdditionVector.y++; }
                    
                    for (int x = 0; x < Camera.CollidingObjects[i].GetSize().x; x++)
                    {
                        if(LastPos.x == ExMath.Round(x + localPos.x)) { AdditionVector.x++; }
                        DisplayBuffer = DrawPoint.InsertAsMiddle(DisplayBuffer, ExMath.Round(x + AdditionVector.x), ExMath.Round(y + AdditionVector.y), '#', Camera.CollidingObjects[i].Color);
                        
                        LastPos.x = ExMath.Round(x + localPos.x);
                    }
                    LastPos.y = ExMath.Round(y + localPos.y);
                }
            }
        }
        ConsoleDrawer.UpdateBuffer(DisplayBuffer);
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

    public void DoCollisionChecks()
    {
        ObjectsToCheck.AddRange(ObjectCollision);
        for (int i = 0; i < ObjectCollision.Count; i++)
        {
            if (ObjectsToCheck.Contains(ObjectCollision[i]))
            {
                ObjectCollision[i].CollidingObjects.Clear();
                for (int x = 0; x < ObjectCollision.Count; x++)
                {
                    if (ObjectCollision[i].CheckIfColliding(ObjectCollision[x]))
                    {
                        ObjectCollision[i].CollidingObjects.Add(ObjectCollision[x]);
                        ObjectCollision[x].CollidingObjects.Add(ObjectCollision[i]);
                        if(ObjectCollision[i].NAME != "CAMERA")
                        {
                            ObjectsToCheck.Remove(ObjectCollision[x]);
                        }
                    }
                }
            }
        }
    }

    public void RemoveObject(Object obj)
    {
        Objects.Remove(obj);
        ObjectCollision.Remove(obj);
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
        Vector2 A = new Vector2(x + array.GetLength(0) / 2, -y + array.GetLength(1) / 2);
        if (A.x > -1 && A.x < array.GetLength(0) && A.y > -1 && A.y < array.GetLength(1))
        {
            array[(int)MathF.Round(A.x), (int)MathF.Round(A.y)].DisplayChar = Char;
            array[(int)MathF.Round(A.x), (int)MathF.Round(A.y)].DisplayColor = Color;
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
    /// <para>Bottleneck frame rate True/False</para>
    /// </summary>
    public bool BTLNCKFR = false;
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
    public int BttlFrm = 5;

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
    public void UpdateBuffer(DrawPoint[,] Buffer)
    {
        this.SafeGaurdBuffer = DrawPoint.CopyTo(Buffer, this.SafeGaurdBuffer);
    }

    public void Start()
    {
        var handle = GetStdHandle(-11);
        int mode;
        GetConsoleMode(handle, out mode);
        SetConsoleMode(handle, mode | 0x4);

        StopThreads = false;
        Thread BeginDraw = new Thread(new ThreadStart(DrawLoop));
        ForceDraw = true;
        BeginDraw.Start();
    }

    DateTime BottleNeckTimer = DateTime.Now;
    DateTime FrameTimer;
    int Frames = 0;
    void DrawLoop()
    {
        StopThreads = false;
        while (!StopThreads)
        {
            DrawScreen();
            switch (BTLNCKFR)
            {
                case true:
                    if (BottleNeckTimer == null || DateTime.Now > BottleNeckTimer)
                    {
                        BottleNeckTimer = DateTime.Now.AddMilliseconds(1000 / BttlFrm);
                        Thread.Sleep(1000 / BttlFrm);
                    }
                    break;
            }
        }
        if (FrameTimer == null || FrameTimer <= DateTime.Now)
        {
            FrameTimer = DateTime.Now.AddSeconds(1);
            AvgFramesPerSec = Frames;
            Frames = 0;
        }
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


        

        void DrawToPoint(int x, int y)
        {
            Console.SetCursorPosition(x, y);
            DisColor = Buffer[x, y].DisplayColor;
            Console.Write($"\x1b[38;2;{DisColor.R};{DisColor.G};{DisColor.B}m{Buffer[x, y].DisplayChar}");
        }

        Console.SetCursorPosition(0, 0);
        ForceDraw = false;
        CurrDisplay = DrawPoint.CopyTo(Buffer, CurrDisplay);
        Frames++;
    }

}

public class Object
{
    public string ID;
    public string NAME;
    public bool Visible = true;
    public Color Color = Color.White;
    Vector2 TopLeft;
    Vector2 BotRight;
    Vector2 Size;
    Vector2 Position;
    public List<Object> CollidingObjects = new List<Object>();
    public List<Object> Children = new List<Object>();
    public Object Parent;

    public Object(string NAME, Vector2 Size, Vector2 Position)
    {
        Random ran = new Random();
        this.NAME = NAME;
        this.Size = Size;
        this.Position = Position;
        CalcNewSizePos();
    }

    public Object(string NAME, Vector2 Size, Vector2 Position, Color Color)
    {
        Random ran = new Random();
        this.NAME = NAME;
        this.Size = Size;
        this.Position = Position;
        this.Color = Color;
        CalcNewSizePos();
    }


    public Vector2 LocalizePos(Object obj)
    {
        return obj.Position - Position;
    }

    void RecalcParentPos()
    {
        this.Position = Position + Parent.GetPos();
    }

    public void Resize(Vector2 Size)
    {
        this.Size = Size;
        CalcNewSizePos();
    }

    public void RePosition(Vector2 Position)
    {
        this.Position = Position;
        CalcNewSizePos();
    }

    public void RePosition(Vector2 Position, bool doParent)
    {
        switch (doParent)
        {
            case true:
                RecalcParentPos();
                break;
            case false:
                this.Position = Position;
                break;
        }
        CalcNewSizePos();
    }

    public Vector2 GetPos()
    {
        return Position;
    }

    public Vector2 GetSize()
    {
        return Size;
    }

    public bool CheckIfColliding(Object Obj)
    {
        return Vector2.doOverlap(this.TopLeft, this.BotRight, Obj.TopLeft, Obj.BotRight);
    }

    void CalcNewSizePos()
    {
        TopLeft = new Vector2(-Size.x / 2, Size.y / 2) + Position;
        BotRight = new Vector2(Size.x / 2, -Size.y / 2) + Position;
    }


}

