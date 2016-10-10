using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Mesher : MonoBehaviour
{
    List<Vector3> verts;
    List<int> idxs;

    public float scale = 0.1f;

	void Start()
    {
        verts = new List<Vector3>();
        idxs = new List<int>();

        //int[,,] test = new int[,,]
        //{
        //    { { 0,0,0,0 },
        //      { 0,0,0,0 },
        //      { 0,0,0,0 },
        //      { 0,0,0,0 } },

        //    { { 0,0,0,0 },
        //      { 0,1,1,0 },
        //      { 0,1,1,0 },
        //      { 0,0,0,0 } },

        //    { { 0,0,0,0 },
        //      { 0,1,0,0 },
        //      { 0,0,1,0 },
        //      { 0,0,0,0 } },

        //    { { 0,0,0,0 },
        //      { 0,0,0,0 },
        //      { 0,0,0,0 },
        //      { 0,0,0,0 } },
        //};

        //GenMesh(p => test[p.X, p.Y, p.Z], new RangeI[]
        //{
        //    new RangeI(0, 4),
        //    new RangeI(0, 4),
        //    new RangeI(0, 4)
        //});

        RangeI r = new RangeI(0, 20);
        GenMesh(p => DebugSphereGen(new PointI(10, 10, 10), 8, p), new RangeI[] { r, r, r });

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = verts.ToArray();
        mesh.triangles = idxs.ToArray();
        mesh.RecalculateNormals();        
	}	
	
	void Update()
    {
	
	}

    int DebugCubeGen(PointI p)
    {
        for (int i = 0; i < 3; i++)        
            if (p[i] < 1 || p[i] > 3) return 0;        

        return 1;
    }    

    int DebugSphereGen(PointI c, float r, PointI p)
    {
        float dist = Mathf.Sqrt((p.X - c.X) * (p.X - c.X) +
                                (p.Y - c.Y) * (p.Y - c.Y) +
                                (p.Z - c.Z) * (p.Z - c.Z));

        
        return dist <= r ? 1 : 0;
    }

    void GenMesh(Func<PointI, int> gen, RangeI[] ranges)
    {        
        for(int d = 0; d < 3; d++)
        {
            int id, jd;
            switch (d)
            {
                case 0: id = 1; jd = 2; break;
                case 1: id = 2; jd = 0; break;
                case 2: id = 0; jd = 1; break;
                default: throw new InvalidProgramException();
            }

            int ni = ranges[id].Max - ranges[id].Min;
            int nj = ranges[jd].Max - ranges[jd].Min;
            
            for (int dir = 0; dir < 2; dir++)
            {               
                for (int k = ranges[d].Min; k < ranges[d].Max - 1; k++)
                {
                    //Filling slice
                    int[,] slice = new int[ni, nj];
                    int n = 0;
                    for (int i = 0; i < ni; i++)
                    {
                        for (int j = 0; j < nj; j++)
                        {
                            PointI p0 = new PointI(3);
                            p0[d] = k;
                            p0[id] = i + ranges[id].Min;
                            p0[jd] = j + ranges[jd].Min;
                            PointI p1 = p0.Clone();
                            p1[d]++;

                            int v0 = gen(p0);
                            int v1 = gen(p1);

                            if(dir == 0 && v0 > 0 && v1 == 0 ||
                               dir == 1 && v0 == 0 && v1 > 0)
                            {
                                slice[i, j] = 1;
                                n++;
                            }                            
                        }
                    }

                    //Draw quads greedy
                    //int debugI = 0;
                    while(n > 0)
                    {
                        int l = 0, t = 0;

                        //Find left top
                        for (int i = 0; i < ni; i++)                        
                            for(int j = 0; j < nj; j++)                            
                                if(slice[i,j] == 1)
                                {
                                    t = i;
                                    l = j;
                                    goto Exit1;
                                }

                        Exit1:
                        //Find width
                        int w = 1;
                        for (int j = l + 1; j < nj; j++, w++)                        
                            if (slice[t, j] == 0) break;

                        //Find height
                        int h = 1;
                        for (int i = t + 1; i < ni; i++, h++)
                            for (int j = l; j < l + w; j++)
                                if (slice[i, j] == 0) goto Exit2;

                        Exit2:
                        //Erase slice
                        for (int i = t; i < t + h; i++)
                            for (int j = l; j < l + w; j++)
                            {
                                slice[i, j] = 0;
                                n--;
                            }
                        
                        DrawQuad(d, dir, k, new RangeI(t, t + h), new RangeI(l, l + w));

                        //debugI++;
                        //if (debugI > 100) break;
                    }
                }
            }
        }
    }

    Vector3 FillVector3(int d, float v0, float v1, float v2)
    {
        switch(d)
        {
            case 0: return new Vector3(v0, v1, v2);
            case 1: return new Vector3(v2, v0, v1);
            case 2: return new Vector3(v1, v2, v0);
            default: throw new InvalidProgramException();
        }
    }

    private void DrawQuad(int d, int dir, int k, RangeI rangeI1, RangeI rangeI2)
    {
        //Debug.Log("(" + d + "," + dir + "," + k + ") " + rangeI1.Min + " " + rangeI2.Min + " " + rangeI1.Max + " " + rangeI2.Max);

        k++;

        int i = verts.Count;

        verts.Add(FillVector3(d, k, rangeI1.Min, rangeI2.Min) * scale);
        verts.Add(FillVector3(d, k, rangeI1.Max, rangeI2.Min) * scale);
        verts.Add(FillVector3(d, k, rangeI1.Max, rangeI2.Max) * scale);
        verts.Add(FillVector3(d, k, rangeI1.Min, rangeI2.Max) * scale);

        if (dir == 0)
            idxs.AddRange(new[] { i,i+1,i+3,i+3,i+1,i+2 });
        else
            idxs.AddRange(new[] { i,i+3,i+1,i+3,i+2,i+1 });
    }
}
