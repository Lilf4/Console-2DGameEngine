using System;
using System.Diagnostics;

public class Vector2
{
    public float x = 0;
    public float y = 0;
    static Random ran = new Random();
    public static readonly Vector2 Zero = new Vector2(0, 0);
    public static readonly Vector2 Up = new Vector2(0, 1);
    public static readonly Vector2 Right = new Vector2(1, 0);

    public Vector2(float x, float y) { CREATE(x, y); }
    public Vector2() { CREATE(0, 0); }

    void CREATE(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
    

    public static implicit operator string(Vector2 v) { return $"{v.x}, {v.y}"; }


    public static bool doOverlap(Vector2 l1, Vector2 r1, Vector2 l2, Vector2 r2)
    {
        return !(l1.y <= r2.y || l2.y <= r1.y) && !(l1.x >= r2.x || l2.x >= r1.x);
    }

    public static Vector2 Copy(Vector2 valueToCopy)
    {
        return new Vector2(valueToCopy.x, valueToCopy.y);
    }

    public static Vector2 Clamp(Vector2 vector, float min, float max) { return CLAMP(vector, min, max, min, max); }
    public static Vector2 Clamp(Vector2 vector, float xmin, float xmax, float ymin, float ymax) { return CLAMP(vector, xmin, xmax, ymin, ymax); }
    static Vector2 CLAMP(Vector2 vector, float xmin, float xmax, float ymin, float ymax)
    {
        return new Vector2(Math.Clamp(vector.x, xmin, xmax), Math.Clamp(vector.y, ymin, ymax));
    }
    public static Vector2 ClampWithOverflow(Vector2 val, Vector2 XSize, Vector2 YSize)
    {
        if (val.x > XSize.y)
        {
            val.x = XSize.x;
        }

        if (val.y > YSize.y)
        {
            val.y = YSize.x;
        }

        if (val.x < XSize.x)
        {
            val.x = XSize.y;
        }

        if (val.y < YSize.x)
        {
            val.y = YSize.y;
        }

        return val;
    }

    public static Vector2 Random(int lowX, int highX, int lowY, int highY)
    {
        return new Vector2(ran.Next(lowX, highX), ran.Next(lowY, highY));
    }

    #region ClassOperators
    public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.x + b.x, a.y + b.y); }
    public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.x - b.x, a.y - b.y); }
    public static Vector2 operator -(Vector2 a) { return new Vector2(-a.x, -a.y); }
    public static Vector2 operator /(Vector2 a, Vector2 b) { return new Vector2(a.x / b.x, a.y / b.y); }
    public static Vector2 operator *(Vector2 a, Vector2 b) { return new Vector2(a.x * b.x, a.y * b.y); }
    
    public static bool operator >(Vector2 a, Vector2 b) { return a.x > b.x && a.y > b.y; }
    public static bool operator <(Vector2 a, Vector2 b) { return a.x < b.x && a.y < b.y; }
    public static bool operator ==(Vector2 a, Vector2 b) { return a.x == b.x && a.y == b.y; }
    public static bool operator !=(Vector2 a, Vector2 b) { return a.x != b.x || a.y != b.y; }
    #endregion

    #region DefaultOperators
    //int
    public static Vector2 operator +(Vector2 a, int b) { return new Vector2(a.x + b, a.y + b); }
    public static Vector2 operator -(Vector2 a, int b) { return new Vector2(a.x - b, a.y - b); }
    public static Vector2 operator /(Vector2 a, int b) { return new Vector2(a.x / b, a.y / b); }
    public static Vector2 operator *(Vector2 a, int b) { return new Vector2(a.x * b, a.y * b); }
    public static bool operator >(Vector2 a, int b) { return a.x > b && a.y > b; }
    public static bool operator <(Vector2 a, int b) { return a.x < b && a.y < b; }
    public static bool operator ==(Vector2 a, int b) { return a.x == b && a.y == b; }
    public static bool operator !=(Vector2 a, int b) { return a.x != b || a.y != b; }

    //float
    public static Vector2 operator +(Vector2 a, float b) { return new Vector2(a.x + b, a.y + b); }
    public static Vector2 operator -(Vector2 a, float b) { return new Vector2(a.x - b, a.y - b); }
    public static Vector2 operator /(Vector2 a, float b) { return new Vector2(a.x / b, a.y / b); }
    public static Vector2 operator *(Vector2 a, float b) { return new Vector2(a.x * b, a.y * b); }
    public static bool operator >(Vector2 a, float b) { return a.x > b && a.y > b; }
    public static bool operator <(Vector2 a, float b) { return a.x < b && a.y < b; }
    public static bool operator ==(Vector2 a, float b) { return a.x == b && a.y == b; }
    public static bool operator !=(Vector2 a, float b) { return a.x != b || a.y != b; }
    #endregion
}

public class ExMath
{
    public static double Round(float num)
    {
        return Math.Round(num, 0, MidpointRounding.ToZero);
    }
}