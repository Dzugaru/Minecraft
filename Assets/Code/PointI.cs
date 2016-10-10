using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public struct PointI
{
    int[] v;

    public PointI(int ndim)
    {
        v = new int[ndim];
    }

    public PointI(int[] v)
    {
        this.v = v;
    }

    public PointI(int x, int y)
    {
        v = new int[2];
        v[0] = x;
        v[1] = y;
    }

    public PointI(int x, int y, int z)
    {
        v = new int[3];
        v[0] = x;
        v[1] = y;
        v[2] = z;
    }

    public PointI Clone()
    {        
        return new PointI((int[])v.Clone());
    }

    public int X { get { return v[0]; } set { v[0] = value; } }
    public int Y { get { return v[1]; } set { v[1] = value; } }
    public int Z { get { return v[2]; } set { v[2] = value; } }

    public int this[int i]
    {
        get
        {
            return v[i];
        }
        set
        {
            v[i] = value;
        }
    }
}

