using System;
using System.Diagnostics;

public class Vector2
{
    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2()
    {
        this.x = 0;
        this.y = 0;
    }

    public static readonly Vector2 Zero = new Vector2(0, 0);
    public static readonly Vector2 Up = new Vector2(0, 1);
    public static readonly Vector2 Down = new Vector2(0, -1);
    public static readonly Vector2 Left = new Vector2(-1, 0);
    public static readonly Vector2 Right = new Vector2(1, 0);
    public float x = 0;
    public float y = 0;

    public static implicit operator string(Vector2 v) { return $"{v.x}, {v.y}"; }


    public static bool doOverlap(Vector2 l1, Vector2 r1, Vector2 l2, Vector2 r2)
    {

        // If one rectangle is on left side of other
        if (l1.x >= r2.x || l2.x >= r1.x)
            return false;

        // If one rectangle is above other
        if (l1.y <= r2.y || l2.y <= r1.y)
            return false;

        return true;
    }

    public static Vector2 Copy(Vector2 valueToCopy)
    {
        return new Vector2(valueToCopy.x, valueToCopy.y);
    }

    public static Vector2 Clamp(Vector2 vector, float min, float max)
    {
        return new Vector2(Math.Clamp(vector.x, min, max), Math.Clamp(vector.y, min, max));
    }
    public static Vector2 Clamp(Vector2 vector, float xmin, float xmax, float ymin, float ymax)
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
    public static Vector2 Random(float lowX, float highX, float lowY, float highY)
    {
        Random ran = new Random();
        return new Vector2(ran.Next((int)lowX, (int)highX), ran.Next((int)lowY, (int)highY));
    }

    #region ClassOperators
    public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.x + b.x, a.y + b.y); }
    public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.x - b.x, a.y - b.y); }
    public static Vector2 operator /(Vector2 a, Vector2 b) { return new Vector2(a.x / b.x, a.y / b.y); }
    public static Vector2 operator *(Vector2 a, Vector2 b) { return new Vector2(a.x * b.x, a.y * b.y); }
    public static bool operator >(Vector2 a, Vector2 b) { if (a.x > b.x && a.y > b.y) { return true; } return false; }
    public static bool operator <(Vector2 a, Vector2 b) { if (a.x < b.x && a.y < b.y) { return true; } return false; }
    public static bool operator ==(Vector2 a, Vector2 b) { if (a.x == b.x && a.y == b.y) { return true; } return false; }
    public static bool operator !=(Vector2 a, Vector2 b) { if (a.x != b.x || a.y != b.y) { return true; } return false; }
    #endregion

    #region DefaultOperators
    //int
    public static Vector2 operator +(Vector2 a, int b) { return new Vector2(a.x + b, a.y + b); }
    public static Vector2 operator -(Vector2 a, int b) { return new Vector2(a.x - b, a.y - b); }
    public static Vector2 operator /(Vector2 a, int b) { return new Vector2(a.x / b, a.y / b); }
    public static Vector2 operator *(Vector2 a, int b) { return new Vector2(a.x * b, a.y * b); }
    public static bool operator >(Vector2 a, int b) { if (a.x > b && a.y > b) { return true; } return false; }
    public static bool operator <(Vector2 a, int b) { if (a.x < b && a.y < b) { return true; } return false; }
    public static bool operator ==(Vector2 a, int b) { if (a.x == b && a.y == b) { return true; } return false; }
    public static bool operator !=(Vector2 a, int b) { if (a.x != b || a.y != b) { return true; } return false; }

    //float
    public static Vector2 operator +(Vector2 a, float b) { return new Vector2(a.x + b, a.y + b); }
    public static Vector2 operator -(Vector2 a, float b) { return new Vector2(a.x - b, a.y - b); }
    public static Vector2 operator /(Vector2 a, float b) { return new Vector2(a.x / b, a.y / b); }
    public static Vector2 operator *(Vector2 a, float b) { return new Vector2(a.x * b, a.y * b); }
    public static bool operator >(Vector2 a, float b) { if (a.x > b && a.y > b) { return true; } return false; }
    public static bool operator <(Vector2 a, float b) { if (a.x < b && a.y < b) { return true; } return false; }
    public static bool operator ==(Vector2 a, float b) { if (a.x == b && a.y == b) { return true; } return false; }
    public static bool operator !=(Vector2 a, float b) { if (a.x != b || a.y != b) { return true; } return false; }
    #endregion
}

public class ExMath
{
    public static int Round(float num)
    {
        var NumTruned = Math.Truncate(num);
        var x = num - NumTruned;
        if(x < 0) 
        { 
            if(x < -.5)
            {
                return (int)(num - x);
            }
            else
            {
                return (int)(num - x) + 1;
            }
        }
        else
        {
            if (x < .5)
            {
                return (int)(num - x);
            }
            else
            {
                return (int)(num - x) + 1;
            }
        }
        
    }
}