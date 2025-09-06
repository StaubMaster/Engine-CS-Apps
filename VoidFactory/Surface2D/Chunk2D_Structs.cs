using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;

namespace VoidFactory.Surface2D
{
    partial class Chunk2D
    {
        public struct TileIndex
        {
            public const int InvalidIdx = -1;

            public class OutOfBoundException : Exception
            {
                public OutOfBoundException() : base("TileIndex was out of Bound.") { }
            }


            public readonly int idx;
            public readonly int y;
            public readonly int c;

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

            public TileIndex Move(int y, int c)
            {
                return new TileIndex(this.y + y, this.c + c);
            }
            public TileIndex Mod()
            {
                return new TileIndex(
                    ((y % Tiles_Per_Side) + Tiles_Per_Side) % Tiles_Per_Side,
                    ((c % Tiles_Per_Side) + Tiles_Per_Side) % Tiles_Per_Side
                    );
            }

            public bool IsValid()
            {
                return
                    (y >= 0 && y < Tiles_Per_Side) &&
                    (c >= 0 && c < Tiles_Per_Side) &&
                    (idx >= 0 && idx < Tiles_Per_Area);
            }
            public bool IsBoarder()
            {
                return (y == -1 || y == Tiles_Per_Side || c == -1 || c == Tiles_Per_Side);
            }
            public byte BitsBoarderExt()
            {
                byte bits = 0;

                if (y == -1 || y == Tiles_Per_Side)
                    bits |= 0b01;

                if (c == -1 || c == Tiles_Per_Side)
                    bits |= 0b10;

                return bits;
            }
            public byte BitsBoarderInt()
            {
                byte bits = 0;

                if (y == 0)
                    bits |= 0b0001;
                if (c == 0)
                    bits |= 0b0010;
                if (y == Tiles_Per_Side - 1)
                    bits |= 0b0100;
                if (c == Tiles_Per_Side - 1)
                    bits |= 0b1000;

                return bits;

            }
            public byte BitsNeighbour()
            {
                byte bits = 0;

                if (y < 0)
                    bits |= 0b0001;
                if (c < 0)
                    bits |= 0b0010;
                if (y >= Tiles_Per_Side)
                    bits |= 0b0100;
                if (c >= Tiles_Per_Side)
                    bits |= 0b1000;

                return bits;
            }

            public override string ToString()
            {
                return "Chunk y:c[idx]" + y.ToString("00") + ":" + c.ToString("00") + "[" + idx.ToString("000") + "]" + "\n";
            }
            public string ToLine()
            {
                return "y:c[idx]" + y.ToString("00") + ":" + c.ToString("00") + "[" + idx.ToString("000") + "]";
            }
        }
        public struct LayerIndex
        {
            public const int InvalidIdx = -1;

            public readonly int idx;

            public LayerIndex(int idx)
            {
                this.idx = idx;
            }

            public bool IsValid()
            {
                return (idx >= 0 && idx < LayerGen.Length);
            }
            public override string ToString()
            {
                return "Layer idx " + idx.ToString() + "\n";
            }
        }
        public struct ChunkIndex
        {
            public const int InvalidIdx = -1;

            public readonly int y;
            public readonly int c;
            public readonly int idx;

            public ChunkIndex(int y, int c, int idx)
            {
                this.y = y;
                this.c = c;
                this.idx = idx;
            }
            public ChunkIndex(int y, int c)
            {
                this.y = y;
                this.c = c;
                this.idx = InvalidIdx;
            }

            public ChunkIndex Neighbour(TileIndex tileIdx)
            {
                int _y = y, _c = c;

                byte bits = tileIdx.BitsNeighbour();
                if ((bits & 0b0001) == 0b0001) { _y--; }
                if ((bits & 0b0010) == 0b0010) { _c--; }
                if ((bits & 0b0100) == 0b0100) { _y++; }
                if ((bits & 0b1000) == 0b1000) { _c++; }

                return new ChunkIndex(_y, _c);
            }

            public bool IsValid()
            {
                return (idx != InvalidIdx);
            }
            public override string ToString()
            {
                return "Tile y:c[idx] " + y.ToString("+0;-0; 0") + ":" + c.ToString("+0;-0; 0") + "[" + idx.ToString() + "]" + "\n";
            }
        }



        public struct TileHit
        {
            public bool Valid;
            public double Dist;
            public Point3D Cross;
            public Point3D Center;

            public void Reset()
            {
                Valid = false;
                Dist = double.NaN;
                //Cross = null;
                //Center = null;
            }

            public bool IsValid()
            {
                return Valid;
            }
            public override string ToString()
            {
                return "Dist " + Dist + "\n";
                /*string str = "";

                str += "Dist ";
                if (Valid) { str += Dist; }
                str += "\n";

                return str;*/
            }
        }
        public struct TileLayerHit
        {
            public TileHit Tile_Hit;
            public LayerIndex Layer_Idx;

            public void Reset()
            {
                Tile_Hit.Reset();
                Layer_Idx = new LayerIndex();
            }

            public bool IsValid()
            {
                return Tile_Hit.IsValid();
            }
            public override string ToString()
            {
                string str = "";

                str += Layer_Idx;
                str += Tile_Hit;

                return str;
            }
        }
        public struct ChunkTileHit
        {
            public TileLayerHit TileLayer_Hit;
            public TileIndex Tile_Idx;

            public void Reset()
            {
                TileLayer_Hit.Reset();
                Tile_Idx = new TileIndex();
            }

            public bool IsValid()
            {
                return TileLayer_Hit.IsValid();
            }
            public override string ToString()
            {
                string str = "";

                str += Tile_Idx;
                str += TileLayer_Hit;

                return str;
            }
        }
        public struct SurfaceHit
        {
            public ChunkTileHit ChunkTile_Hit;
            public ChunkIndex Chunk_Idx;

            public void Reset()
            {
                ChunkTile_Hit.Reset();
                Chunk_Idx = new ChunkIndex();
            }

            public bool IsValid()
            {
                return ChunkTile_Hit.IsValid();
            }
            public override string ToString()
            {
                string str = "";

                str += Chunk_Idx;
                str += ChunkTile_Hit;

                return str;
            }

            public TileIndex ToTileIndex()
            {
                return ChunkTile_Hit.Tile_Idx;
            }
            public LayerIndex ToLayerIndex()
            {
                return ChunkTile_Hit.TileLayer_Hit.Layer_Idx;
            }
            public ChunkIndex ToChunkIndex()
            {
                return Chunk_Idx;
            }
        }



    }
}
