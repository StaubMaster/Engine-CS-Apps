
using System.Collections.Generic;

using Engine3D.Abstract3D;
using Engine3D.Noise;

using VoidFactory.Production.Data;

namespace VoidFactory.Surface2D
{
    /*
        ================================
        Structure

            Surface
                Chunk_s
                    Layer_s
                        Tile_s
                    Object_s

        ================================
        Neighbours

            Index   RelY    RelC    Bits
              0      -1       0     0001
              1      -1      -1     0011
              2       0      -1     0010
              3      +1      -1     0110
              4      +1       0     0100
              5      +1      +1     1100
              6       0      +1     1000
              7      -1      +1     1001

        ================================
        Structs

            TileIndex (Index of Tile in Layer)
            LayerIndex (Index of Layer in Chunk)
            ChunkIndex (Index of Chunk in Surface)

            TileHit (Point and Valie)
            LayerHit (TileIndex and TileHit)
            ChunkHit (LayerIndex and LayerHit)
            SurfaceHit (ChunkHit and ChunkIndex)

        ================================
        RayHit()

            SurfaceHit Cross(Ray)
                ChunkIndex in LineYC
                ChunkHit = Cross_Chunk(Ray, ChunkIndex) in LineYC

            ChunkHit Cross_Chunk(Ray, ChunkIndex)
                TileIndex LineYC
                LayerHit = Cross_Layers(Ray, TileIndex) LineYC

            LayerHit Cross_Layers(Ray, TileIndex)
                LayerIndex in LineX
                Cross_Tile(Ray, LayerIndex) in Layer_s

            TileHit Cross_Tile(Ray, LayerIndex)

        ================================
        Object_Collect

            Surface_Object_Collect(ChunkIndex, LayerIndex, TileInderx)
            Chunk_Object_Collect(LayerIndex, TileInderx)
            Layer_Object_Collect(TileInderx)
            Tile_Object_Collect()

        ================================
     */
    partial class Chunk2D
    {
        public const int Tiles_Per_Side = 1 << 4;
        private const int Tiles_Per_Area = Tiles_Per_Side * Tiles_Per_Side;

        public const int Tile_Size = 8;
        private const int Chunk_Size_Per_Side = Tile_Size * Tiles_Per_Side;
        private const double Tile_Size_Inv = 1.0 / Tile_Size;

        public struct LayerGenerationData
        {
            public uint Color1;
            public uint Color2;

            public DATA_Thing Thing;
            public NoiseSum Noise;
        }
        public static LayerGenerationData[] LayerGen;
        public static SURF_Object.Template[] SurfThingTemplates;

        public struct TileData
        {
            public const int Size_Color       = 0;
            public const int Size_Height_Mid  = Size_Color       + sizeof(uint);
            public const int Size_Height_Corn = Size_Height_Mid  + sizeof(int);
            public const int Size             = Size_Height_Corn + sizeof(int) * 4;

            public uint Color;
            public int Height_Mid;
            public int Height_MnsMns;
            public int Height_PlsMns;
            public int Height_PlsPls;
            public int Height_MnsPls;

            public static TileData Max(TileData tile, TileData? tile1, TileData? tile2, TileData? tile3)
            {
                if (tile1 != null && tile1.Value.Height_Mid > tile.Height_Mid)
                    tile = tile1.Value;
                if (tile2 != null && tile2.Value.Height_Mid > tile.Height_Mid)
                    tile = tile2.Value;
                if (tile3 != null && tile3.Value.Height_Mid > tile.Height_Mid)
                    tile = tile3.Value;
                return tile;
            }
            public static TileData Min(TileData tile, TileData? tile1, TileData? tile2, TileData? tile3)
            {
                if (tile1 != null && tile1.Value.Height_Mid < tile.Height_Mid)
                    tile = tile1.Value;
                if (tile2 != null && tile2.Value.Height_Mid < tile.Height_Mid)
                    tile = tile2.Value;
                if (tile3 != null && tile3.Value.Height_Mid < tile.Height_Mid)
                    tile = tile3.Value;
                return tile;
            }
            public static int Avg(TileData tile, TileData? tile1, TileData? tile2, TileData? tile3)
            {
                int num = 1;
                if (tile1 != null)
                    num++;
                if (tile2 != null)
                    num++;
                if (tile3 != null)
                    num++;

                int avg = tile.Height_Mid;

                if (tile1 != null)
                    avg += tile1.Value.Height_Mid;
                if (tile2 != null)
                    avg += tile2.Value.Height_Mid;
                if (tile3 != null)
                    avg += tile3.Value.Height_Mid;

                return avg / num;
            }
        }
        public struct LayerData
        {
            public TileData[] Tiles;
            //public Chunk2DBuffers Buffer;
            public Graphics.Chunk2D_Buffer Buffer;

            public TileData this[TileIndex tileIdx]
            {
                get { return Tiles[tileIdx.idx]; }
            }

            public void BufferCreate()
            {
                //Buffer = new Chunk2DBuffers();
                //Buffer.Create();
                Buffer = new Graphics.Chunk2D_Buffer();
            }
            public void BufferDelete()
            {
                //Buffer.Delete();
                Buffer = null;
            }
            public void BufferUpdate()
            {
                //Buffer.Tiles(Tiles);
                Buffer.Bind(Tiles);
            }

            public void CalcColors(uint col1, uint col2)
            {
                TileIndex tileIdx;

                uint col;
                for (int i = 0; i < Tiles_Per_Area; i++)
                {
                    tileIdx = new TileIndex(i);

                    if ((tileIdx.y % 2) == (tileIdx.c % 2))
                        col = col1;
                    else
                        col = col2;

                    Tiles[tileIdx.idx].Color = col;
                }
            }
            public void CalcHeights(NoiseSum perlin, int chunk_y, int chunk_c)
            {
                TileIndex tileIdx;

                int h;
                for (int i = 0; i < Tiles_Per_Area; i++)
                {
                    tileIdx = new TileIndex(i);

                    h = (int)perlin.Sum(tileIdx.y + chunk_y, tileIdx.c + chunk_c);

                    Tiles[tileIdx.idx].Height_Mid = h;
                    Tiles[tileIdx.idx].Height_MnsMns = h;
                    Tiles[tileIdx.idx].Height_MnsPls = h;
                    Tiles[tileIdx.idx].Height_PlsPls = h;
                    Tiles[tileIdx.idx].Height_PlsMns = h;
                }
            }
            public void CalcStaticTest(int chunk_y, int chunk_c)
            {
                int h = 100;

                TileIndex tileIdx;
                uint col;
                for (int i = 0; i < Tiles_Per_Area; i++)
                {
                    tileIdx = new TileIndex(i).Move(chunk_y, chunk_c);

                    col = 0;
                    for (int t = 0; t < SurfThingTemplates.Length; t++)
                    {
                        if (SurfThingTemplates[t].Gen.generateBool(tileIdx.y, tileIdx.c))
                        {
                            col = SurfThingTemplates[t].Color;
                            break;
                        }
                    }

                    Tiles[tileIdx.idx].Color = col;
                    Tiles[tileIdx.idx].Height_Mid = h;
                    Tiles[tileIdx.idx].Height_MnsMns = h;
                    Tiles[tileIdx.idx].Height_MnsPls = h;
                    Tiles[tileIdx.idx].Height_PlsPls = h;
                    Tiles[tileIdx.idx].Height_PlsMns = h;
                }
            }

            private static TileData? GetTile(TileIndex tileIdx, LayerData curr, LayerData? sideY, LayerData? sideC, LayerData? sideYC)
            {
                if (tileIdx.IsValid())
                {
                    return curr.Tiles[tileIdx.idx];
                }
                else
                {
                    byte bits = tileIdx.BitsBoarderExt();
                    tileIdx = tileIdx.Mod();

                    if (bits == 0b11 && sideYC != null)
                        return sideYC.Value.Tiles[tileIdx.idx];
                    else if (bits == 0b01 && sideY != null)
                        return sideY.Value.Tiles[tileIdx.idx];
                    else if (bits == 0b10 && sideC != null)
                        return sideC.Value.Tiles[tileIdx.idx];
                }
                return null;
            }
            public void Tile_Render_Update(TileIndex tileIdx, LayerData?[] neigh)
            {
                TileData tile = Tiles[tileIdx.idx];

                TileData? m_, mm, _m, mp, p_, pp, _p, pm;
                m_ = GetTile(tileIdx.Move(-1, 0), this, neigh[0], null, null);
                _m = GetTile(tileIdx.Move(0, -1), this, null, neigh[2], null);
                p_ = GetTile(tileIdx.Move(+1, 0), this, neigh[4], null, null);
                _p = GetTile(tileIdx.Move(0, +1), this, null, neigh[6], null);

                mm = GetTile(tileIdx.Move(-1, -1), this, neigh[0], neigh[2], neigh[1]);
                pm = GetTile(tileIdx.Move(+1, -1), this, neigh[4], neigh[2], neigh[3]);
                pp = GetTile(tileIdx.Move(+1, +1), this, neigh[4], neigh[6], neigh[5]);
                mp = GetTile(tileIdx.Move(-1, +1), this, neigh[0], neigh[6], neigh[7]);

                tile.Height_MnsMns = TileData.Avg(tile, m_, _m, mm);
                tile.Height_MnsPls = TileData.Avg(tile, m_, _p, mp);
                tile.Height_PlsPls = TileData.Avg(tile, p_, _p, pp);
                tile.Height_PlsMns = TileData.Avg(tile, p_, _m, pm);

                Tiles[tileIdx.idx] = tile;
            }
        }



        private LayerData[] Layers;
        private SURF_Object[] Things;

        private readonly int Chunk_Idx_Y;
        private readonly int Chunk_Idx_C;
        private readonly int Chunk_Tile_Idx_Y;
        private readonly int Chunk_Tile_Idx_C;

        private Chunk2D[] Neighbour;

        private Chunk2D(int y, int c)
        {
            Layers = new LayerData[LayerGen.Length];
            for (int l = 0; l < Layers.Length; l++)
                Layers[l].Tiles = new TileData[Tiles_Per_Area];
            Things = new SURF_Object[Tiles_Per_Area];

            Chunk_Idx_Y = y;
            Chunk_Idx_C = c;
            Chunk_Tile_Idx_Y = Chunk_Idx_Y * Tiles_Per_Side;
            Chunk_Tile_Idx_C = Chunk_Idx_C * Tiles_Per_Side;

            Neighbour = new Chunk2D[8];

            Engine3D.ConsoleLog.Log("++++ Chunk2D " + "[ " + Chunk_Idx_Y + " , " + Chunk_Idx_C + " ]");
        }
        ~Chunk2D()
        {
            Engine3D.ConsoleLog.Log("---- Chunk2D " + "[ " + Chunk_Idx_Y + " , " + Chunk_Idx_C + " ]");
        }

        private Chunk2D NeighbourFromBits(byte bits)
        {
            if (bits == 0) { return this; }
            else if (bits == 0b0001) { return Neighbour[0]; }
            else if (bits == 0b0011) { return Neighbour[1]; }
            else if (bits == 0b0010) { return Neighbour[2]; }
            else if (bits == 0b0110) { return Neighbour[3]; }
            else if (bits == 0b0100) { return Neighbour[4]; }
            else if (bits == 0b1100) { return Neighbour[5]; }
            else if (bits == 0b1000) { return Neighbour[6]; }
            else if (bits == 0b1001) { return Neighbour[7]; }
            return null;
        }
        private TileData this[TileIndex tileIdx, LayerIndex layerIdx]
        {
            get { return Layers[layerIdx.idx].Tiles[tileIdx.idx]; }
            set { Layers[layerIdx.idx].Tiles[tileIdx.idx] = value; Corn_Update_Neigh(tileIdx, layerIdx); }
        }

        private TileData TileHighest(TileIndex tileIdx)
        {
            TileData tile = Layers[0][tileIdx];
            for (int l = 1; l < Layers.Length; l++)
            {
                if (Layers[l][tileIdx].Height_Mid > tile.Height_Mid)
                    tile = Layers[l][tileIdx];
            }
            return tile;
        }

        public string ToChunkInfo()
        {
            string str = "";

            {
                int thingCount = 0;
                for (int i = 0; i < Tiles_Per_Area; i++)
                {
                    if (Things[i] != null) { thingCount++; }
                }
                str += "Thins: " + thingCount + "\n";
            }

            return str;
        }

        public void ObjectAdd(TileIndex tileIdx, SURF_Object thing)
        {
            if (thing == null) { return; }
            if (Things[tileIdx.idx] == null)
            {
                Things[tileIdx.idx] = thing;
                Things[tileIdx.idx].Place();
            }
            else if (thing.Content.Num > Things[tileIdx.idx].Content.Num)
            {
                Things[tileIdx.idx].Remove();
                Things[tileIdx.idx] = thing;
                Things[tileIdx.idx].Place();
            }
        }
        public SURF_Object ObjectIdx(TileIndex tileIdx)
        {
            return Things[tileIdx.idx];
        }
        public DATA_Cost ObjectSub(TileIndex tileIdx)
        {
            if (Things[tileIdx.idx] != null)
            {
                //Engine3D.ConsoleLog.Log("ObjectSub() " + tileIdx.idx);
                DATA_Cost cost = Things[tileIdx.idx].Collect();
                Inventory.Inventory_Storage.CostCanRefund(cost);

                if (Things[tileIdx.idx].ToRemove)
                {
                    //Engine3D.ConsoleLog.Log("Things[" + tileIdx.idx + "] = null");
                    Things[tileIdx.idx] = null;
                }

                return cost;
            }
            Things[tileIdx.idx] = null;
            return null;
        }

        private void FeaturePlaceRef(int y, int c, double dist, double perc, SURF_Object obj)
        {
            if (perc < 0 || perc > 1)
            {
                return;
            }

            TileIndex tileIdx = new TileIndex(y - Chunk_Tile_Idx_Y, c - Chunk_Tile_Idx_C);
            Chunk2D chunk = NeighbourFromBits(tileIdx.BitsNeighbour());
            if (chunk != null)
            {
                chunk.ObjectAdd(tileIdx.Mod(), obj);
            }
        }
        private void FeaturesPlaceRing(int y, int c, double dist, double perc, SURF_Object.Template temp)
        {
            if (perc < 0 || perc > 1) { return; }

            double h = 0.0;
            TileIndex tileIdx = new TileIndex(y - Chunk_Tile_Idx_Y, c - Chunk_Tile_Idx_C);
            Chunk2D chunk = NeighbourFromBits(tileIdx.BitsNeighbour());
            if (chunk != null) { h = chunk.TileHighest(tileIdx.Mod()).Height_Mid; }

            Transformation3D trans = new Transformation3D(
                new Point3D((y + 0.5) * Tile_Size, h, (c + 0.5) * Tile_Size),
                new Angle3D(y * 128, c * 128, (y + c) * 128)
            );

            FeaturePlaceRef(y, c, dist, perc, temp.ToInstance1(trans, perc, tileIdx));
        }
        private void FeaturesPlaceCenter(TileIndex tileIdx, SURF_Object.Template temp)
        {
            double h = double.NaN;
            void findHighest(int y, int c, double dist, double perc)
            {
                TileIndex tileIdx2 = new TileIndex(y - Chunk_Tile_Idx_Y, c - Chunk_Tile_Idx_C);
                Chunk2D chunk = NeighbourFromBits(tileIdx2.BitsNeighbour());
                TileData tile;
                if (chunk != null)
                {
                    tile = chunk.TileHighest(tileIdx2.Mod());
                    if (!(h > tile.Height_Mid))
                        h = tile.Height_Mid;
                }
            }
            Grid2D.FuncCircle(tileIdx.y, tileIdx.c, temp.MinDist, findHighest);

            SURF_Object thing;
            thing = temp.ToInstance2(new Transformation3D(new Point3D((tileIdx.y + 0.5) * Tile_Size, h, (tileIdx.c + 0.5) * Tile_Size)), 1.0, tileIdx);
            Grid2D.FuncCircle(tileIdx.y, tileIdx.c, temp.MinDist, FeaturePlaceRef, thing);
        }
        private void GenerateFeatureTemplate(SURF_Object.Template temp)
        {
            for (int i = 0; i < Tiles_Per_Area; i++)
            {
                TileIndex tileIdx;
                tileIdx = new TileIndex(i).Move(Chunk_Tile_Idx_Y, Chunk_Tile_Idx_C);

                if (temp.Gen.generateBool(tileIdx.y, tileIdx.c))
                {
                    FeaturesPlaceCenter(tileIdx, temp);
                    Grid2D.FuncCircle(tileIdx.y, tileIdx.c, temp.MinDist, temp.MaxDist, FeaturesPlaceRing, temp);
                }
            }
        }


        private void GenerateFeatures()
        {
            for (int t = 0; t < SurfThingTemplates.Length; t++)
            {
                GenerateFeatureTemplate(SurfThingTemplates[t]);
            }
        }
        private void GenerateLayers()
        {
            for (int l = 0; l < LayerGen.Length; l++)
            {
                Layers[l].CalcHeights(LayerGen[l].Noise, Chunk_Tile_Idx_Y, Chunk_Tile_Idx_C);
                Layers[l].CalcColors(LayerGen[l].Color1, LayerGen[l].Color2);
            }
            //Layers[LayerGen.Length].CalcStaticTest(Chunk_Tile_Idx_Y, Chunk_Tile_Idx_C);
        }


        private static void Corn_Update_Neigh(TileIndex tileIdx, LayerIndex layerIdx, Chunk2D curr, Chunk2D sideY, Chunk2D sideC, Chunk2D sideYC)
        {
            if (tileIdx.IsValid())
            {
                curr.Corn_Update_This(tileIdx, layerIdx);
            }
            else
            {
                byte bits = tileIdx.BitsBoarderExt();
                tileIdx = tileIdx.Mod();

                if (bits == 0b11 && sideYC != null)
                    sideYC.Corn_Update_This(tileIdx, layerIdx);
                else if (bits == 0b01 && sideY != null)
                    sideY.Corn_Update_This(tileIdx, layerIdx);
                else if (bits == 0b10 && sideC != null)
                    sideC.Corn_Update_This(tileIdx, layerIdx);
            }
        }
        private void Corn_Update_Neigh(TileIndex tileIdx, LayerIndex layerIdx)
        {
            Corn_Update_This(tileIdx, layerIdx);

            Corn_Update_Neigh(tileIdx.Move(-1,  0), layerIdx, this, Neighbour[0], null, null);
            Corn_Update_Neigh(tileIdx.Move( 0, -1), layerIdx, this, null, Neighbour[2], null);
            Corn_Update_Neigh(tileIdx.Move(+1,  0), layerIdx, this, Neighbour[4], null, null);
            Corn_Update_Neigh(tileIdx.Move( 0, +1), layerIdx, this, null, Neighbour[6], null);

            Corn_Update_Neigh(tileIdx.Move(-1, -1), layerIdx, this, Neighbour[0], Neighbour[2], Neighbour[1]);
            Corn_Update_Neigh(tileIdx.Move(+1, -1), layerIdx, this, Neighbour[4], Neighbour[2], Neighbour[3]);
            Corn_Update_Neigh(tileIdx.Move(+1, +1), layerIdx, this, Neighbour[4], Neighbour[6], Neighbour[5]);
            Corn_Update_Neigh(tileIdx.Move(-1, +1), layerIdx, this, Neighbour[0], Neighbour[6], Neighbour[7]);

            Layers[layerIdx.idx].BufferUpdate();
            byte bits = tileIdx.BitsBoarderInt();
            if ((bits & 0b0001) == 0b0001 && Neighbour[0] != null) { Neighbour[0].Layers[layerIdx.idx].BufferUpdate(); }
            if ((bits & 0b0011) == 0b0011 && Neighbour[1] != null) { Neighbour[1].Layers[layerIdx.idx].BufferUpdate(); }
            if ((bits & 0b0010) == 0b0010 && Neighbour[2] != null) { Neighbour[2].Layers[layerIdx.idx].BufferUpdate(); }
            if ((bits & 0b0110) == 0b0110 && Neighbour[3] != null) { Neighbour[3].Layers[layerIdx.idx].BufferUpdate(); }
            if ((bits & 0b0100) == 0b0100 && Neighbour[4] != null) { Neighbour[4].Layers[layerIdx.idx].BufferUpdate(); }
            if ((bits & 0b1100) == 0b1100 && Neighbour[5] != null) { Neighbour[5].Layers[layerIdx.idx].BufferUpdate(); }
            if ((bits & 0b1000) == 0b1000 && Neighbour[6] != null) { Neighbour[6].Layers[layerIdx.idx].BufferUpdate(); }
            if ((bits & 0b1001) == 0b1001 && Neighbour[7] != null) { Neighbour[7].Layers[layerIdx.idx].BufferUpdate(); }
        }
        private void Corn_Update_This(TileIndex tileIdx, LayerIndex layerIdx)
        {
            LayerData?[] neigh = new LayerData?[Neighbour.Length];
            for (int n = 0; n < neigh.Length; n++)
            {
                if (Neighbour[n] != null)
                    neigh[n] = Neighbour[n].Layers[layerIdx.idx];
            }

            Layers[layerIdx.idx].Tile_Render_Update(tileIdx, neigh);
        }
        private void Corn_Update_All(int layerIdx)
        {
            LayerData?[] neigh = new LayerData?[Neighbour.Length];
            for (int n = 0; n < neigh.Length; n++)
            {
                if (Neighbour[n] != null)
                    neigh[n] = Neighbour[n].Layers[layerIdx];
            }

            TileIndex tileIdx;
            for (int i = 0; i < Tiles_Per_Area; i++)
            {
                tileIdx = new TileIndex(i);
                Layers[layerIdx].Tile_Render_Update(tileIdx, neigh);
            }
        }


        public static void UniformConst(Chunk2DProgram program)
        {
            program.UniTileSize(Tile_Size, Tiles_Per_Side);
        }
        /*private void Draw(Chunk2DProgram program)
        {
            program.UniChunkIdx(Chunk_Idx_Y, 0, Chunk_Idx_C);
            for (int l = 0; l < Layers.Length; l++)
            {
                Layers[l].Buffer.Draw();
            }
        }*/
        private void Draw(Graphics.Chunk2D_Shader program)
        {
            program.ChunkPos.Set(new int[] { Chunk_Idx_Y, 0, Chunk_Idx_C });
            for (int l = 0; l < Layers.Length; l++)
            {
                Layers[l].Buffer.Draw();
            }
        }

        private Point3D Extern_Intern_Scaled(Point3D p)
        {
            return new Point3D(
                    (p.Y / Tile_Size) - Chunk_Tile_Idx_Y,
                    (p.X / Tile_Size),
                    (p.C / Tile_Size) - Chunk_Tile_Idx_C
                );
        }
        private Point3D Intern_Extern_Scaled(Point3D p)
        {
            return new Point3D(
                    (p.Y + Chunk_Tile_Idx_Y) * Tile_Size,
                    (p.X * Tile_Size),
                    (p.C + Chunk_Tile_Idx_C) * Tile_Size
                );
        }
        private Point3D Extern_Intern(Point3D p)
        {
            return new Point3D(
                    (p.Y / Tile_Size) - Chunk_Tile_Idx_Y,
                    (p.X),
                    (p.C / Tile_Size) - Chunk_Tile_Idx_C
                );
        }
        private Point3D Intern_Extern(Point3D p)
        {
            return new Point3D(
                    (p.Y + Chunk_Tile_Idx_Y) * Tile_Size,
                    (p.X),
                    (p.C + Chunk_Tile_Idx_C) * Tile_Size
                );
        }

        public static Point3D PunktTileCenter(TileIndex tileIdx, double h)
        {
            return new Point3D(tileIdx.y + 0.5, h, tileIdx.c + 0.5);
        }
        public static Point3D[] PunktTileConers(TileIndex tileIdx, double h00, double h01, double h11, double h10)
        {
            return new Point3D[4]
            {
                new Point3D(tileIdx.y + 0, h00, tileIdx.c + 0),
                new Point3D(tileIdx.y + 0, h01, tileIdx.c + 1),
                new Point3D(tileIdx.y + 1, h11, tileIdx.c + 1),
                new Point3D(tileIdx.y + 1, h10, tileIdx.c + 0),
            };
        }

        private double Cross_TileLayer(Ray3D ray, TileIndex tileIdx, TileData tile)
        {
            Point3D[] corn = PunktTileConers(tileIdx,
                tile.Height_MnsMns,
                tile.Height_MnsPls,
                tile.Height_PlsPls,
                tile.Height_PlsMns);
            Point3D m = PunktTileCenter(tileIdx, tile.Height_Mid);

            corn[0].X = (float)(corn[0].X * Tile_Size_Inv);
            corn[1].X = (float)(corn[1].X * Tile_Size_Inv);
            corn[2].X = (float)(corn[2].X * Tile_Size_Inv);
            corn[3].X = (float)(corn[3].X * Tile_Size_Inv);
            m.X = (float)(m.X * Tile_Size_Inv);

            double[] h = new double[4]
            {
                ray.Dreieck_Schnitt_Interval(corn[0], corn[1], m),
                ray.Dreieck_Schnitt_Interval(corn[1], corn[2], m),
                ray.Dreieck_Schnitt_Interval(corn[2], corn[3], m),
                ray.Dreieck_Schnitt_Interval(corn[3], corn[0], m),
            };

            return Ray3D.FindMin(h, out _);
        }
        private TileLayerHit Cross_ChunkTile(Ray3D ray, TileIndex tileIdx)
        {
            TileLayerHit hit = new TileLayerHit();
            hit.Reset();

            double[] h = new double[Layers.Length];
            for (int l = 0; l < Layers.Length; l++)
                h[l] = Cross_TileLayer(ray, tileIdx, Layers[l].Tiles[tileIdx.idx]);

            Point3D cross = ray.Scale(Ray3D.FindMin(h, out int layer));
            if (cross.Is())
            {
                hit.Tile_Hit.Valid = true;
                hit.Tile_Hit.Cross = Intern_Extern_Scaled(cross);
                hit.Tile_Hit.Dist = (hit.Tile_Hit.Cross - ray.Pos).Len;
                hit.Tile_Hit.Center = Intern_Extern(PunktTileCenter(tileIdx, Layers[layer].Tiles[tileIdx.idx].Height_Mid));
                hit.Layer_Idx = new LayerIndex(layer);
            }

            return hit;
        }
        private ChunkTileHit Cross_Chunk(Ray3D ray)
        {
            ChunkTileHit hit = new ChunkTileHit();
            hit.Reset();

            ray.Pos = Extern_Intern_Scaled(ray.Pos);

            TileIndex tileIdx;
            bool TileFunc(int y, int c, double t)
            {
                tileIdx = new TileIndex(y, c);
                if (!tileIdx.IsValid())
                    return tileIdx.IsBoarder();

                hit.Tile_Idx = tileIdx;
                hit.TileLayer_Hit = Cross_ChunkTile(ray, tileIdx);

                return (!hit.IsValid());
            }
            ray.LineFunc2D(TileFunc, Tiles_Per_Side * 2);

            return hit;
        }


        private AxisBox3D TileBox(TileIndex tileIdx, LayerIndex layerIdx)
        {
            Point3D[] corn = new Point3D[5];
            TileData tile = Layers[layerIdx.idx].Tiles[tileIdx.idx];

            corn[0] = Intern_Extern(new Point3D(tileIdx.y + 0.5, tile.Height_Mid, tileIdx.c + 0.5));
            corn[1] = Intern_Extern(new Point3D(tileIdx.y + 0, tile.Height_MnsMns, tileIdx.c + 0));
            corn[2] = Intern_Extern(new Point3D(tileIdx.y + 0, tile.Height_MnsPls, tileIdx.c + 1));
            corn[3] = Intern_Extern(new Point3D(tileIdx.y + 1, tile.Height_PlsPls, tileIdx.c + 1));
            corn[4] = Intern_Extern(new Point3D(tileIdx.y + 1, tile.Height_PlsMns, tileIdx.c + 0));

            return AxisBox3D.MinMax(corn);
        }
    }
}
