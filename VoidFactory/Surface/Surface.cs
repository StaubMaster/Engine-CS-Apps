using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Abstract;
using Engine3D.Abstract.Simple;
using Engine3D.Abstract.Complex;
using Engine3D.Graphics;
using Engine3D.Noise;

namespace VoidFactory
{
    /*
        What Should the Surface do:
            1. dynamically destructivle
            2. large (1km)
            3. Save/Load from/to File
            4. depict a Landscape

        How 1:
            check where view is looking
            change whats there
            update memory

        How 2:
            hight detail near view
            lower detail farther away

        How 3:
            Folder for MaP
            File per Chunk: (chunk needs to have side length of 2^n)
            1.  average of whole chunk
            2.  differance for each quarter
            repeat 2. until chunk cant be devided anymore
            trim trailing 0

        How 4:
            Perlin
     */
    partial class Layered_Surface
    {
        private static List<(Punkt, double)> Krater = new List<(Punkt, double)>();
        public static void add_Krater(Punkt p, double r)
        {
            Krater.Add((p, r));
        }


        private static int Surf_Y_Pos = 0;
        private static int Surf_C_Pos = 0;
        public static Perlin.Layers perlin = new Perlin.Layers(new (uint, int, int)[]
        {
            //(0x12345678, 6, 1),
            //(0x56781234, 9, 6),
            //(0x87654321, 12, 9),
            //(0x43218765, 15, 12),
        });
        private static uint calc_Height(int y, int c)
        {
            double yy, cc;
            //y = (int)((y + Y_Pos) * Const.Tile_Width);
            //c = (int)((c + C_Pos) * Const.Tile_Width);
            yy = (y + Surf_Y_Pos) * Const.Tile_Width;
            cc = (c + Surf_C_Pos) * Const.Tile_Width;

            double dst = (yy * yy) + (cc * cc);
            if (Math.Sqrt(dst) >= 16384) { return 0; }

            double h;
            h = 1024.0 + perlin.Sum(yy, cc);

            double ry, rc;
            for (int i = 0; i < Krater.Count; i++)
            {
                ry = yy - Krater[i].Item1.Y;
                rc = cc - Krater[i].Item1.C;

                dst = Math.Sqrt((ry * ry) + (rc * rc));
                if (dst < Krater[i].Item2)
                {
                    h -= Math.Cos((dst / Krater[i].Item2) * (Math.PI * 0.5)) * Krater[i].Item2;
                }
            }

            if (h <= 0.0) { h = 0.0; }
            return (uint)h;
        }





        private Layer[] layers;
        private bool layers_recalc;
        private SurfLayerBuffers[] Buffers;

        public Layered_Surface(byte layer_num)
        {
            layers = new Layer[layer_num];
            Buffers = new SurfLayerBuffers[layer_num];
            for (int i = 0; i < layer_num; i++)
            {
                layers[i] = new Layer(i);

                Buffers[i] = new SurfLayerBuffers();
                Buffers[i].Create();
                Buffers[i].Indexe(Const.Tiles_Per_Side);
                Buffers[i].Heights(layers[i].calc_Heights());
            }
            layers_recalc = false;
        }
        ~Layered_Surface()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = null;

                Buffers[i].Delete();
                Buffers[i] = null;
            }
            layers = null;
            Buffers = null;
        }

        public void CheckShift(Punkt p)
        {
            if (layers != null)
            {
                if (layers[0].CheckShift(p))
                    layers_recalc = true;
                if (layers_recalc)
                {
                    for (int i = 0; i < layers.Length; i++)
                        layers[i].calc_Heights();
                    layers_recalc = false;
                }
            }
        }

        public void Draw(SurfLayerProgram program)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].Draw(program);
                Buffers[i].Draw();
            }
        }

        private class Layer
        {
            private uint[] Heights;
            private uint Tile_Width;
            private uint[] cut;

            private int Layer_Y_Off;
            private int Layer_X_Off;
            private int Layer_C_Off;

            private int Zoom;

            public Layer(int zoom)
            {
                Heights = new uint[Const.Corners_Pre_Surface];
                Tile_Width = Const.Tile_Width << zoom;

                Layer_X_Off = 0;
                Zoom = zoom;

                cut = new uint[4];
                if (zoom != 0)
                {
                    cut[0] = Const.Cut_Lo;
                    cut[1] = Const.Cut_Lo;
                    cut[2] = Const.Cut_Hi;
                    cut[3] = Const.Cut_Hi;
                }
            }

            public uint[] calc_Heights()
            {
                Layer_Y_Off = (int)((Const.Noise_Offset * Tile_Width) + (Surf_Y_Pos * Const.Tile_Width));
                Layer_C_Off = (int)((Const.Noise_Offset * Tile_Width) + (Surf_C_Pos * Const.Tile_Width));

                int y, c;
                for (uint i = 0; i < Const.Corners_Pre_Surface; i++)
                {
                    y = (int)(i % Const.Corners_Per_Side);
                    c = (int)(i / Const.Corners_Per_Side);

                    if (y == 4 && c == 4)
                    {
                        Heights[i] = 256;
                    }
                    else
                    {
                        //y = (y + Const.Noise_Offset) << Zoom;
                        //c = (c + Const.Noise_Offset) << Zoom;
                        //Heights[i] = calc_Height(y, c);
                        Heights[i] = 10;
                    }
                }

                return Heights;
            }

            public bool CheckShift(Punkt p)
            {
                int y, c;
                y = (int)(((p.Y - Layer_Y_Off) * 4.0) / (Tile_Width * Const.Tiles_Per_Side));
                c = (int)(((p.C - Layer_C_Off) * 4.0) / (Tile_Width * Const.Tiles_Per_Side));

                int y_off = 0;
                if (y <= 0) { y_off = y - 1; }
                if (y >= 3) { y_off = y - 2; }

                int c_off = 0;
                if (c <= 0) { c_off = c - 1; }
                if (c >= 3) { c_off = c - 2; }

                y_off = (int)(y_off * (Const.Tiles_Per_Side / 4));
                c_off = (int)(c_off * (Const.Tiles_Per_Side / 4));

                if (y_off != 0 || c_off != 0)
                {
                    Surf_Y_Pos += y_off;
                    Surf_C_Pos += c_off;
                    return true;
                }
                return false;
            }

            public void Draw(SurfLayerProgram program)
            {
                program.Use();
                program.UniWidth(Tile_Width);
                program.UniOffset(Layer_Y_Off, Layer_X_Off, Layer_C_Off);
                program.UniCut(cut);

                //Buffer.Draw();
            }


            /*
               0.0             1.0
            0.0[0]-------------[1]0.0
                | \           / |
                |   \       /   |
                |     \   /     |
                |       m       |
                |     /   \     |
                |   /       \   |
                | /           \ |
            1.0[2]-------------[3]1.0
               0.0             1.0

                0.0 to 1.0
                 0  to  3

                0=0     1>0     2>0     3>0
                0<1     1=1     2>1     3>1
                0<2     1<2     2=2     3>2
                0<3     1<3     2<3     3=3

                y c -> 3-y c

                3>0     2>0     1>0     0=0
                        2>1     1=1     0<1
                                1<2     0<2
                                        0<3

                3>0                        
                3>1     2>1                
                3>2     2=2     1<2        
                3=3     2<3     1<3     0<3

                area 1:     area2:  area3:  area4:
                ####           #            #    
                 ##           ##            ##   
                              ##     ##     ##   
                               #    ####    #    
             */
            private Punkt Corner_to_Point(uint y, uint c)
            {
                return new Punkt(
                    (y * Tile_Width) + Layer_Y_Off,
                    Heights[y + c * Const.Corners_Per_Side],
                    (c * Tile_Width) + Layer_C_Off
                    );
            }
            public double Calc_Height_at_Point_unoptimized(Punkt p)
            {
                double y, c;
                y = (((p.Y - (Surf_Y_Pos * Tile_Width)) * 2) / Const.Tiles_Per_Side) + (Const.Tiles_Per_Side / 2);
                c = (((p.C - (Surf_C_Pos * Tile_Width)) * 2) / Const.Tiles_Per_Side) + (Const.Tiles_Per_Side / 2);

                if (y >= Const.Tiles_Per_Side || c >= Const.Tiles_Per_Side) { return double.NaN; }

                cut[0] = (uint)(y + 0);
                cut[1] = (uint)(c + 0);
                cut[2] = (uint)(y + 1);
                cut[3] = (uint)(c + 1);

                Ray ray = new Ray(p, new Punkt(0, 1, 0));

                Punkt[] ecken = new Punkt[4];
                ecken[0b00] = Corner_to_Point(cut[0], cut[1]);
                ecken[0b01] = Corner_to_Point(cut[2], cut[1]);
                ecken[0b10] = Corner_to_Point(cut[0], cut[3]);
                ecken[0b11] = Corner_to_Point(cut[2], cut[3]);

                Punkt mitte = (ecken[0b00] + ecken[0b01] + ecken[0b10] + ecken[0b11]) * 0.25;
                double t;

                t = Rechnen.Dreieck_Schnitt(ray, ecken[0b00], ecken[0b01], mitte);
                if (!double.IsNaN(t)) { return p.X + t; }
                t = Rechnen.Dreieck_Schnitt(ray, ecken[0b01], ecken[0b11], mitte);
                if (!double.IsNaN(t)) { return p.X + t; }
                t = Rechnen.Dreieck_Schnitt(ray, ecken[0b11], ecken[0b10], mitte);
                if (!double.IsNaN(t)) { return p.X + t; }
                t = Rechnen.Dreieck_Schnitt(ray, ecken[0b10], ecken[0b00], mitte);
                if (!double.IsNaN(t)) { return p.X + t; }

                return double.NaN;
            }
            public Punkt Ray_cross_unoptimized(Ray ray)
            {
                double lowest;
                lowest = double.PositiveInfinity;

                Punkt p00, p01, p10, p11, mid;
                double l;
                for (uint c = 0; c < Const.Tiles_Per_Side; c++)
                {
                    for (uint y = 0; y < Const.Tiles_Per_Side; y++)
                    {
                        p00 = Corner_to_Point(y + 0, c + 0);
                        p01 = Corner_to_Point(y + 0, c + 1);
                        p10 = Corner_to_Point(y + 1, c + 0);
                        p11 = Corner_to_Point(y + 1, c + 1);
                        mid = (p00 + p01 + p10 + p11) * 0.25;

                        l = Rechnen.Dreieck_Schnitt(ray, p00, p01, mid);
                        if (l < lowest) { lowest = l; }
                        l = Rechnen.Dreieck_Schnitt(ray, p01, p11, mid);
                        if (l < lowest) { lowest = l; }
                        l = Rechnen.Dreieck_Schnitt(ray, p11, p10, mid);
                        if (l < lowest) { lowest = l; }
                        l = Rechnen.Dreieck_Schnitt(ray, p10, p00, mid);
                        if (l < lowest) { lowest = l; }
                    }
                }
                if (double.IsInfinity(lowest))
                    return null;
                return ray.Scale(lowest);
            }
        }
    }
}
