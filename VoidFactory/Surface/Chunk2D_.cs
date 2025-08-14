using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Abstract;
using Engine3D.Abstract.Simple;
using Engine3D.Abstract.Complex;
using Engine3D.Graphics;
using Engine3D.Noise;

namespace VoidFactory.Surface
{
    partial class Chunk2D_
    {
        private const int Tile_Size = 8;
        private const int Tiles_Per_Side = 1 << 4;
        private const int Tiles_Per_Area = Tiles_Per_Side * Tiles_Per_Side;
        private const int Chunk_Size = Tile_Size * Tiles_Per_Side;

        private static readonly uint[] Chunk_Indexe;
        static Chunk2D_()
        {
            Chunk_Indexe = new uint[Tiles_Per_Area * 4];

            uint indexe_idx = 0xFFFFFFFF;
            uint tile_idx = 0;

            for (int y = 0; y < Tiles_Per_Side; y++)
            {
                for (int x = 0; x < Tiles_Per_Side; x++)
                {
                    Chunk_Indexe[++indexe_idx] = tile_idx + 0;
                    Chunk_Indexe[++indexe_idx] = tile_idx + 1;
                    Chunk_Indexe[++indexe_idx] = tile_idx + 0 + Tiles_Per_Side;
                    Chunk_Indexe[++indexe_idx] = tile_idx + 1 + Tiles_Per_Side;
                    tile_idx++;
                }
            }
        }

        public static SurfChunkProgram Program;

        private static Perlin.Layers perlin = new Perlin.Layers(new Perlin[]
        {
            new Perlin(0x12345678, 4, 5),

            //new Perlin(0x12345678, 6, 6),
            //new Perlin(0x56781234, 9, 6),
            //new Perlin(0x87654321, 12, 9),
            //new Perlin(0x43218765, 15, 12),
        });
        private static int calcHeight(int y, int c)
        {
            return (int)perlin.Sum(y, c);
        }


        public class TileIndex
        {
            public int idx;
            public int y;
            public int c;

            public TileIndex(int idx)
            {
                this.idx = idx;
                y = idx % Tiles_Per_Side;
                c = idx / Tiles_Per_Side;
            }
            public TileIndex(int y, int c)
            {
                idx = y + c * Tiles_Per_Side;
                this.y = y;
                this.c = c;
            }

            public bool isValid()
            {
                return (idx >= 0 && idx < Tiles_Per_Area);
            }

            public override string ToString()
            {
                return y.ToString("00") + ":" + c.ToString("00") + "[" + idx.ToString("000") + "]";
            }
        }
        public class ChunkIndex
        {
            public int idx;
            public int y;
            public int c;

            public ChunkIndex(int y, int c, int idx)
            {
                this.idx = idx;
                this.y = y;
                this.c = c;
            }

            public override string ToString()
            {
                return y.ToString("+0;-0; 0") + ":" + c.ToString("+0;-0; 0") + "[" + idx + "]";
            }
        }


        public struct TileData
        {
            public int Height;
            public int Size;
            public uint Color;
        }

        private int[] Heights;
        private int[] Sizes;
        private uint[] Colors;

        private SurfChunkBuffers Buffer;
        private int Pos_Y;
        private int Pos_C;
        private int Tile_Off_Y;
        private int Tile_Off_C;
        private Chunk2D_ Side_YP;
        private Chunk2D_ Side_YM;
        private Chunk2D_ Side_CP;
        private Chunk2D_ Side_CM;

        public int this[int idx]
        {
            get
            {
                return Heights[idx];
            }
        }
        public int this[int y, int c]
        {
            get
            {
                return Heights[y + c * Tiles_Per_Side];
            }
        }
        public int this[TileIndex tile]
        {
            get
            {
                return Heights[tile.idx];
            }
            set
            {
                Heights[tile.idx] = value;
                Buffer.Heights(Heights);
                CalcSize(tile.y, tile.c);
                CalcSize(tile.y + 1, tile.c);
                CalcSize(tile.y, tile.c + 1);
                CalcSize(tile.y - 1, tile.c);
                CalcSize(tile.y, tile.c - 1);
                Buffer.Sizes(Sizes);
            }
        }


        private Chunk2D_(int y, int c)
        {
            Heights = new int[Tiles_Per_Area];
            Sizes = new int[Tiles_Per_Area];
            Colors = new uint[Tiles_Per_Area];
            CalcColor();

            Buffer = new SurfChunkBuffers();
            Buffer.Create();
            Buffer.Indexe(Chunk_Indexe);
            Buffer.Colors(Colors);

            Pos_Y = y;
            Pos_C = c;
            Tile_Off_Y = Pos_Y * Tiles_Per_Side;
            Tile_Off_C = Pos_C * Tiles_Per_Side;

            Side_YP = null;
            Side_YM = null;
            Side_CP = null;
            Side_CM = null;
        }
        /*private Chunk2D(int y, int c, int[] heights)
        {
            Heights = heights;
            Colors = new uint[Tiles_Per_Area];

            Buffer = new SurfChunkBuffers();
            Buffer.Create();
            Buffer.Indexe(Chunk_Indexe);
            Buffer.Heights(Heights);

            Pos_Y = y;
            Pos_C = c;
            Tile_Off_Y = Pos_Y * Tiles_Per_Side;
            Tile_Off_C = Pos_C * Tiles_Per_Side;

            Side_YP = null;
            Side_YM = null;
            Side_CP = null;
            Side_CM = null;
        }*/
        ~Chunk2D_()
        {
            if (Buffer != null)
            {
                Buffer.Delete();
                Buffer = null;
            }
        }

        public static Chunk2D_ Generate(int y, int c)
        {
            Chunk2D_ chunk = new Chunk2D_(y, c);

            int i = 0;
            for (int _c = 0; _c < Tiles_Per_Side; _c++)
            {
                for (int _y = 0; _y < Tiles_Per_Side; _y++)
                {
                    chunk.Heights[i] = calcHeight(_y + chunk.Tile_Off_Y, _c + chunk.Tile_Off_C);

                    //if ((((y % 2) + 2) % 2) == (((c % 2) + 2) % 2))
                    //    chunk.Heights[i] = +1;
                    //else
                    //    chunk.Heights[i] = -1;

                    //chunk.Heights[i] = 0;
                    i++;
                }
            }
            chunk.Buffer.Heights(chunk.Heights);
            chunk.CalcSize();
            chunk.Buffer.Sizes(chunk.Sizes);

            return chunk;
        }

        private void CalcSize(int y, int c)
        {
            if (y < 0 || y >= Tiles_Per_Side || c < 0 || c >= Tiles_Per_Side)
                return;

            int min = this[y, c];

            if (y >= Tiles_Per_Side - 1 && Side_YP != null)
                min = Math.Min(min, Side_YP[0, c]);
            else if (y < Tiles_Per_Side - 1)
                min = Math.Min(min, this[y + 1, c]);

            if (y <= 0 && Side_YM != null)
                min = Math.Min(min, Side_YM[Tiles_Per_Side - 1, c]);
            else if (y > 0)
                min = Math.Min(min, this[y - 1, c]);


            if (c >= Tiles_Per_Side - 1 && Side_CP != null)
                min = Math.Min(min, Side_CP[y, 0]);
            else if (c < Tiles_Per_Side - 1)
                min = Math.Min(min, this[y, c + 1]);

            if (c <= 0 && Side_CM != null)
                min = Math.Min(min, Side_CM[y, Tiles_Per_Side - 1]);
            else if(c > 0)
                min = Math.Min(min, this[y, c - 1]);

            Sizes[y + c * Tiles_Per_Side] = min;
        }
        private void CalcSize()
        {
            for (int c = 0; c < Tiles_Per_Side; c++)
            {
                for (int y = 0; y < Tiles_Per_Side; y++)
                {
                    CalcSize(y, c);
                }
            }
        }

        private static uint CalcColor(int y, int c)
        {
            if ((y % 2) == (c % 2))
                return 0x9F9F9F;
            else
                return 0x5F5F5F;
        }
        private void CalcColor()
        {
            for (int c = 0; c < Tiles_Per_Side; c++)
            {
                for (int y = 0; y < Tiles_Per_Side; y++)
                {
                    Colors[y + c * Tiles_Per_Side] = CalcColor(y, c);
                }
            }
        }

        public void Draw()
        {
            Program.UniPos(Pos_Y, 0, Pos_C);
            Buffer.Draw();
        }
        public void Log()
        {
            string str = "";
            for (int i = 0; i < Tiles_Per_Area; i++)
            {
                if ((i % Tiles_Per_Side) == 0)
                    str += "\n";
                str += " " + Heights[i].ToString("+00;-00; 00");
            }
            ConsoleLog.Log(str);
        }

        private Punkt Extern_Intern_Scaled(Punkt p)
        {
            return new Punkt(
                    (p.Y / Tile_Size) - Tile_Off_Y,
                    (p.X / Tile_Size),
                    (p.C / Tile_Size) - Tile_Off_C
                );
        }
        private Punkt Intern_Extern_Scaled(Punkt p)
        {
            return new Punkt(
                    (p.Y + Tile_Off_Y) * Tile_Size,
                    (p.X * Tile_Size),
                    (p.C + Tile_Off_C) * Tile_Size
                );
        }
        private Punkt Extern_Intern(Punkt p)
        {
            return new Punkt(
                    (p.Y / Tile_Size) - Tile_Off_Y,
                    (p.X),
                    (p.C / Tile_Size) - Tile_Off_C
                );
        }
        private Punkt Intern_Extern(Punkt p)
        {
            return new Punkt(
                    (p.Y + Tile_Off_Y) * Tile_Size,
                    (p.X),
                    (p.C + Tile_Off_C) * Tile_Size
                );
        }

        public Punkt TileCenter(TileIndex tile)
        {
            return Intern_Extern(new Punkt(tile.y + 0.5, this[tile.idx], tile.c + 0.5));
        }

        private Punkt Cross(Ray ray, int y, int c)
        {
            double height = this[y, c] * (1.0 / Tile_Size);

            Punkt p00, p01, p10, p11, m;
            p00 = new Punkt((y + 0), height, (c + 0));
            p01 = new Punkt((y + 1), height, (c + 0));
            p10 = new Punkt((y + 0), height, (c + 1));
            p11 = new Punkt((y + 1), height, (c + 1));

            m = new Punkt((y + 0.5), height, (c + 0.5));

            double[] h = new double[4];
            h[0b00] = Rechnen.Dreieck_Schnitt(ray, p00, p01, m);
            h[0b01] = Rechnen.Dreieck_Schnitt(ray, p01, p11, m);
            h[0b10] = Rechnen.Dreieck_Schnitt(ray, p11, p10, m);
            h[0b11] = Rechnen.Dreieck_Schnitt(ray, p10, p00, m);

            double t;
            t = Ray.FindMin(h, out _);

            return ray.Scale(t);
        }
        public Punkt Cross(Ray ray, out TileIndex tile_idx_ext)
        {
            Punkt p = null;
            TileIndex tile_idx_int = null;

            ray.Pos = Extern_Intern_Scaled(ray.Pos);
            bool TileFunc(int y, int c, double t)
            {
                if (y < 0 || y >= Tiles_Per_Side || c < 0 || c >= Tiles_Per_Side)
                {
                    return (y == -1 || y == Tiles_Per_Side || c == -1 || c == Tiles_Per_Side);
                }

                if (Cross_Tile_Log != null)
                {
                    if (Cross_Tile_Idx < Cross_Tile_Log.Length)
                    {
                        Cross_Tile_Log[Cross_Tile_Idx].Trans.Pos = Intern_Extern_Scaled(new Punkt(y + 0.5, this[y, c] * (1.0 / Tile_Size), c + 0.5));
                        Cross_Tile_Idx++;
                    }
                }

                p = Cross(ray, y, c);
                tile_idx_int = new TileIndex(y, c);
                return p == null;
            }
            ray.LineFunc2D(TileFunc, Tiles_Per_Area);

            if (p != null)
            {
                tile_idx_ext = tile_idx_int;
                return Intern_Extern_Scaled(p);
            }
            tile_idx_ext = null;
            return null;
        }


        public static Engine3D.Entity.TransBody[] Cross_Tile_Log;
        public static Engine3D.Entity.TransBody[] Cross_Chunk_Log;
        public static int Cross_Tile_Idx;
        public static int Cross_Chunk_Idx;


        public struct Tile_Hit
        {
            public bool Valid;

            public TileIndex TileIdx;
            public ChunkIndex ChunkIdx;
            public double Ray_Dist;

            public Punkt Point;

            public void Reset()
            {
                Valid = false;

                TileIdx = null;
                ChunkIdx = null;
                Ray_Dist = double.NaN;

                Point = null;
            }

            public override string ToString()
            {
                string str = "";

                str += "Chunk y:c[idx]" + ChunkIdx + "\n";
                str += "Tile  y:c[idx]" + TileIdx + "\n";
                str += "Dist " + Ray_Dist;

                return str;
            }
        }


        public class Collection
        {
            private Chunk2D_[] Chunks;

            public void Create(int numY, int numC)
            {
                Program.UniTiles(Tile_Size, Tiles_Per_Side);

                int offY, offC;
                offY = numY / 2;
                offC = numC / 2;

                Chunks = new Chunk2D_[numY * numC];
                {
                    int i = 0;
                    for (int c = 0; c < numC; c++)
                    {
                        for (int y = 0; y < numY; y++)
                        {
                            Chunks[i] = Generate(y - offY, c - offC);
                            i++;
                        }
                    }
                }

                {
                    int y, c;
                    for (int i = 0; i < Chunks.Length; i++)
                    {
                        y = Chunks[i].Pos_Y;
                        c = Chunks[i].Pos_C;
                        Chunks[i].Side_YP = FindChunk(y + 1, c);
                        Chunks[i].Side_YM = FindChunk(y - 1, c);
                        Chunks[i].Side_CP = FindChunk(y, c + 1);
                        Chunks[i].Side_CM = FindChunk(y, c - 1);
                    }
                }

                //Cross_Chunk_Log = new TransBody[16];
                //for (int i = 0; i < Chunk.Cross_Chunk_Log.Length; i++)
                //    Chunk.Cross_Chunk_Log[i] = new TransBody(Static_Body_Cache.Find("trans"), false);
                //
                //Cross_Tile_Log = new TransBody[128];
                //for (int i = 0; i < Chunk.Cross_Tile_Log.Length; i++)
                //    Chunk.Cross_Tile_Log[i] = new TransBody(Static_Body_Cache.Find("out_box"), false);
            }
            public void Delete()
            {
                for (int i = 0; i < Chunks.Length; i++)
                {
                    Chunks[i].Buffer.Delete();
                    Chunks[i].Buffer = null;
                }
                Chunks = null;

                Cross_Chunk_Log = null;
                Cross_Tile_Log = null;
            }

            public void Draw()
            {
                Program.Use();
                for (int i = 0; i < Chunks.Length; i++)
                    Chunks[i].Draw();

                /*
                Graphic.Trans_Normal.Use();
                for (int i = 0; i < Cross_Chunk_Idx; i++)
                    Cross_Chunk_Log[i].Draw(Graphic.Trans_Normal);
                for (int i = 0; i < Cross_Tile_Idx; i++)
                    Cross_Tile_Log[i].Draw(Graphic.Trans_Normal);
                */
            }

            private Chunk2D_ FindChunk(int y, int c)
            {
                for (int i = 0; i < Chunks.Length; i++)
                {
                    if (Chunks[i].Pos_Y == y && Chunks[i].Pos_C == c)
                        return Chunks[i];
                }
                return null;
            }
            private ChunkIndex FindChunkIdx(int y, int c)
            {
                for (int i = 0; i < Chunks.Length; i++)
                {
                    if (Chunks[i].Pos_Y == y && Chunks[i].Pos_C == c)
                        return new ChunkIndex(y, c, i);
                }
                return null;
            }
            private Punkt Cross(Ray ray, out ChunkIndex chunk_idx_ext, out TileIndex tile_idx_ext)
            {
                Punkt p = null;
                ChunkIndex chunk_idx_int = null;
                TileIndex tile_idx_int = null;

                Ray chunk_ray;
                chunk_ray = new Ray(ray.Pos * (1.0 / Chunk_Size), ray.Dir * (1.0));

                Ray tile_ray;
                tile_ray = new Ray(+ray.Pos, +ray.Dir);

                bool ChunkFunc(int y, int c, double t)
                {
                    chunk_idx_int = FindChunkIdx(y, c);
                    if (chunk_idx_int == null)
                        return false;

                    tile_ray.Pos = chunk_ray.Scale(t) * Chunk_Size;

                    if (Cross_Chunk_Log != null)
                    {
                        if (Cross_Chunk_Idx < Cross_Chunk_Log.Length)
                        {
                            Cross_Chunk_Log[Cross_Chunk_Idx].Trans = tile_ray.ToTrans();
                            Cross_Chunk_Idx++;
                        }
                    }

                    p = Chunks[chunk_idx_int.idx].Cross(tile_ray, out tile_idx_int);
                    return p == null;
                }
                chunk_ray.LineFunc2D(ChunkFunc, 16);

                if (p != null)
                {
                    chunk_idx_ext = chunk_idx_int;
                    tile_idx_ext = tile_idx_int;
                    return p;
                }
                chunk_idx_ext = null;
                tile_idx_ext = null;
                return null;
            }
            public Tile_Hit Ray_Hit(Ray ray)
            {
                Tile_Hit hit = new Tile_Hit();

                hit.Point = Cross(ray, out hit.ChunkIdx, out hit.TileIdx);

                if (hit.Point != null)
                {
                    hit.Valid = true;
                    hit.Ray_Dist = (ray.Pos - hit.Point).Len;
                }
                else
                {
                    hit.Reset();
                }

                return hit;
            }

            public Punkt TileCenter(Tile_Hit hit)
            {
                if (hit.Valid)
                    return Chunks[hit.ChunkIdx.idx].TileCenter(hit.TileIdx);
                return null;
            }

            public void Inc(Tile_Hit hit)
            {
                if (hit.Valid)
                {
                    Chunks[hit.ChunkIdx.idx][hit.TileIdx]++;
                }
            }
            public void Dec(Tile_Hit hit)
            {
                if (hit.Valid)
                {
                    Chunks[hit.ChunkIdx.idx][hit.TileIdx]--;
                }
            }
        }


        /*
        public static class File
        {
            private static int Average_Segment(int[] heights, int seg_y, int seg_c, int size)
            {
                if (size == 1)
                {
                    return heights[seg_y + seg_c * Tiles_Per_Side];
                }

                long sum = 0;
                int y, c;
                for (int rel_c = 0; rel_c < size; rel_c++)
                {
                    c = (rel_c + seg_c) * Tiles_Per_Side;
                    for (int rel_y = 0; rel_y < size; rel_y++)
                    {
                        y = rel_y + seg_y;
                        sum += heights[y + c];
                    }
                }
                return (int)(sum / (size * size));
            }
            private static void Adjust_Segment(int[] heights, int seg_y, int seg_c, int size, int avg)
            {
                if (size == 1)
                {
                    heights[seg_y + seg_c * Tiles_Per_Side] -= avg;
                    return;
                }

                int y, c;
                for (int rel_c = 0; rel_c < size; rel_c++)
                {
                    c = (rel_c + seg_c) * Tiles_Per_Side;
                    for (int rel_y = 0; rel_y < size; rel_y++)
                    {
                        y = rel_y + seg_y;
                        heights[y + c] -= avg;
                    }
                }
            }

            private static void Save_Text_Segment(int[] heights, int size, ref string str)
            {
                int avg;
                for (int seg_c = 0; seg_c < Tiles_Per_Side; seg_c += size)
                {
                    str += "\n";
                    for (int seg_y = 0; seg_y < Tiles_Per_Side; seg_y += size)
                    {
                        avg = Average_Segment(heights, seg_y, seg_c, size);
                        str += " " + avg.ToString("+00;-00; 00");
                        Adjust_Segment(heights, seg_y, seg_c, size, avg);
                    }
                }

                if (size > 1)
                {
                    Save_Text_Segment(heights, size >> 1, ref str);
                }
            }
            public static void Save_Text(string file, in Chunk2D chunk)
            {
                ConsoleLog.Log("<<<< Chunk Save >>>>");

                int[] heights = new int[Tiles_Per_Area];
                for (int i = 0; i < heights.Length; i++)
                    heights[i] = chunk[i];

                string str = "";
                Save_Text_Segment(heights, Tiles_Per_Side, ref str);

                System.IO.File.WriteAllText(file, str);
            }

            private static void Load_Text_Segment(int[] heights, int size, string[] nums, ref int idx)
            {
                int avg;
                for (int seg_c = 0; seg_c < Tiles_Per_Side; seg_c += size)
                {
                    for (int seg_y = 0; seg_y < Tiles_Per_Side; seg_y += size)
                    {
                        avg = int.Parse(nums[idx]);
                        Adjust_Segment(heights, seg_y, seg_c, size, -avg);
                        idx++;
                    }
                }

                if (size > 1)
                {
                    Load_Text_Segment(heights, size >> 1, nums, ref idx);
                }
            }
            public static void Load_Text(string file, out Chunk2D chunk)
            {
                ConsoleLog.Log(">>>> Chunk Load <<<<");

                string str = System.IO.File.ReadAllText(file);

                int[] heights = new int[Tiles_Per_Area];

                string[] nums;
                nums = str.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                int idx = 0;
                Load_Text_Segment(heights, Tiles_Per_Side, nums, ref idx);

                chunk = new Chunk2D(0, 0, heights);
            }
        }
        */
    }
}
