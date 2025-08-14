using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.Noise;
using Engine3D.GraphicsOld;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;

using VoidFactory.Production.Data;

namespace VoidFactory.Surface2D
{
    partial class Chunk2D
    {
        public class Collection
        {
            private Chunk2D[] Chunks;

            private Chunk2D this[ChunkIndex chunkIdx]
            {
                get { return Chunks[chunkIdx.idx]; }
                set { Chunks[chunkIdx.idx] = value; }
            }

            public void Create()
            {
                int len1 = 16, len2 = len1 / 2;
                Chunks = new Chunk2D[len1 * len1];

                {
                    int i = 0;
                    for (int c = -len2; c < +len2; c++)
                    {
                        for (int y = -len2; y < +len2; y++)
                        {
                            Chunks[i] = new Chunk2D(y, c);
                            Chunks[i].GenerateLayers();
                            i++;
                        }
                    }
                }

                {
                    int y, c;
                    for (int i = 0; i < Chunks.Length; i++)
                    {
                        y = Chunks[i].Chunk_Idx_Y;
                        c = Chunks[i].Chunk_Idx_C;

                        Chunks[i].Neighbour[0] = FindChunk(y - 1, c);
                        Chunks[i].Neighbour[1] = FindChunk(y - 1, c - 1);
                        Chunks[i].Neighbour[2] = FindChunk(y, c - 1);
                        Chunks[i].Neighbour[3] = FindChunk(y + 1, c - 1);
                        Chunks[i].Neighbour[4] = FindChunk(y + 1, c);
                        Chunks[i].Neighbour[5] = FindChunk(y + 1, c + 1);
                        Chunks[i].Neighbour[6] = FindChunk(y, c + 1);
                        Chunks[i].Neighbour[7] = FindChunk(y - 1, c + 1);
                    }
                }

                for (int i = 0; i < Chunks.Length; i++)
                {
                    for (int l = 0; l < Chunks[i].Layers.Length; l++)
                    {
                        Chunks[i].Corn_Update_All(l);
                        Chunks[i].Layers[l].BufferCreate();
                        Chunks[i].Layers[l].BufferUpdate();
                    }
                    Chunks[i].GenerateFeatures();
                }
            }
            public void Delete()
            {
                for (int i = 0; i < Chunks.Length; i++)
                {
                    for (int l = 0; l < Chunks[i].Layers.Length; l++)
                    {
                        Chunks[i].Layers[l].BufferDelete();
                    }
                }
                Chunks = null;
            }

            public void Draw(Chunk2DProgram program)
            {
                for (int i = 0; i < Chunks.Length; i++)
                {
                    Chunks[i].Draw(program);
                }
            }
            public void Draw(Graphics.Chunk2D_Shader program)
            {
                for (int i = 0; i < Chunks.Length; i++)
                {
                    Chunks[i].Draw(program);
                }
            }
            public void Draw(TransUniProgram program)
            {
                Chunk2D chunk;
                SURF_Object thing;
                for (int c = 0; c < Chunks.Length; c++)
                {
                    chunk = Chunks[c];
                    for (int t = 0; t < Tiles_Per_Area; t++)
                    {
                        thing = chunk.Things[t];
                        if (thing != null)
                            thing.ToDraw = true;
                    }

                    for (int t = 0; t < Tiles_Per_Area; t++)
                    {
                        thing = chunk.Things[t];
                        if (thing != null && thing.ToDraw)
                        {
                            thing.Draw(program);
                            thing.ToDraw = false;
                        }
                    }
                }
            }
            public void Draw(CShaderTransformation program)
            {
                Chunk2D chunk;
                SURF_Object thing;
                for (int c = 0; c < Chunks.Length; c++)
                {
                    chunk = Chunks[c];
                    for (int t = 0; t < Tiles_Per_Area; t++)
                    {
                        thing = chunk.Things[t];
                        if (thing != null)
                            thing.ToDraw = true;
                    }

                    for (int t = 0; t < Tiles_Per_Area; t++)
                    {
                        thing = chunk.Things[t];
                        if (thing != null && thing.ToDraw)
                        {
                            thing.Draw(program);
                            thing.ToDraw = false;
                        }
                    }
                }
            }
            public void Draw(BodyElemUniShader program)
            {
                Chunk2D chunk;
                SURF_Object thing;
                for (int c = 0; c < Chunks.Length; c++)
                {
                    chunk = Chunks[c];
                    for (int t = 0; t < Tiles_Per_Area; t++)
                    {
                        thing = chunk.Things[t];
                        if (thing != null)
                        {
                            thing.ToDraw = true;
                        }
                    }

                    for (int t = 0; t < Tiles_Per_Area; t++)
                    {
                        thing = chunk.Things[t];
                        if (thing != null && thing.ToDraw)
                        {
                            thing.Draw(program);
                            thing.ToDraw = false;
                        }
                    }
                }
            }

            public Chunk2D FindChunk(int y, int c)
            {
                for (int i = 0; i < Chunks.Length; i++)
                {
                    if (Chunks[i].Chunk_Idx_Y == y && Chunks[i].Chunk_Idx_C == c)
                    {
                        return Chunks[i];
                    }
                }
                return null;
            }
            public ChunkIndex? FindChunkIdx(int y, int c)
            {
                for (int i = 0; i < Chunks.Length; i++)
                {
                    if (Chunks[i].Chunk_Idx_Y == y && Chunks[i].Chunk_Idx_C == c)
                    {
                        return new ChunkIndex(y, c, i);
                    }
                }
                return null;
            }
            public ChunkIndex? FindChunk(TileIndex tileIdx)
            {
                int y = tileIdx.y;
                y = y % Tiles_Per_Side;
                y = y + Tiles_Per_Side;
                y = y % Tiles_Per_Side;
                y = tileIdx.y - Tiles_Per_Side * y;

                int c = tileIdx.c;
                c = c % Tiles_Per_Side;
                c = c + Tiles_Per_Side;
                c = c % Tiles_Per_Side;
                c = tileIdx.c - Tiles_Per_Side * c;

                return FindChunkIdx(y, c);
            }
            public SurfaceHit CrossSurface(Ray3D ray)
            {
                SurfaceHit hit = new SurfaceHit();
                hit.Reset();

                Ray3D chunkRay = new Ray3D(ray.Pos * (1.0 / Chunk_Size_Per_Side), ray.Dir * 1.0);
                Ray3D tileRay = new Ray3D(+ray.Pos, +ray.Dir);

                ChunkIndex? chunkIdx;
                bool ChunkFunc(int y, int c, double t)
                {
                    chunkIdx = FindChunkIdx(y, c);
                    if (chunkIdx == null)
                        return false;

                    tileRay.Pos = chunkRay.Scale(t) * Chunk_Size_Per_Side;

                    hit.ChunkTile_Hit = Chunks[chunkIdx.Value.idx].Cross_Chunk(tileRay);
                    hit.Chunk_Idx = chunkIdx.Value;

                    return (!hit.IsValid());
                }
                chunkRay.LineFunc2D(ChunkFunc, 16);

                if (!hit.IsValid())
                    hit.Reset();
                return hit;
            }

            public AxisBox3D TileBox(SurfaceHit hit)
            {
                if (hit.IsValid())
                    return Chunks[hit.Chunk_Idx.idx].TileBox(hit.ToTileIndex(), hit.ToLayerIndex());
                return null;
            }

            public void Inc(SurfaceHit hit)
            {
                if (hit.IsValid())
                {
                    Chunk2D chunk = this[hit.Chunk_Idx];
                    TileData tile = chunk[hit.ToTileIndex(), hit.ToLayerIndex()];

                    tile.Height_Mid++;
                    chunk[hit.ToTileIndex(), hit.ToLayerIndex()] = tile;
                }
            }
            public void Dec(SurfaceHit hit)
            {
                if (hit.IsValid())
                {
                    Chunk2D chunk = this[hit.Chunk_Idx];
                    TileData tile = chunk[hit.ToTileIndex(), hit.ToLayerIndex()];

                    tile.Height_Mid--;
                    chunk[hit.ToTileIndex(), hit.ToLayerIndex()] = tile;
                }
            }

            public void AddThing(SurfaceHit hit, SURF_Object thing)
            {
                if (hit.IsValid())
                {
                    this[hit.Chunk_Idx].ObjectAdd(hit.ToTileIndex(), thing);
                }
            }
            public SURF_Object FindThing(SurfaceHit hit)
            {
                if (hit.IsValid())
                    return Chunks[hit.Chunk_Idx.idx].ObjectIdx(hit.ToTileIndex());
                return null;
            }
            public DATA_Cost SubThing(SurfaceHit hit)
            {
                if (hit.IsValid())
                    return Chunks[hit.Chunk_Idx.idx].ObjectSub(hit.ToTileIndex());
                return null;
            }
            public DATA_Cost SubThing(ChunkIndex chunkIdx, TileIndex tileIdx)
            {
                chunkIdx = chunkIdx.Neighbour(tileIdx);
                tileIdx = tileIdx.Mod();

                Chunk2D chunk = FindChunk(chunkIdx.y, chunkIdx.c);
                if (chunk != null)
                    return chunk.ObjectSub(tileIdx);
                return null;
            }



            private void MultiChunkRadFunc(SurfaceHit hit, double rad, Func<int, double, double, double, int> func)
            {
                Chunk2D[] toUpdate;
                {
                    int toUpdatePerHalf = (int)(rad / Chunk_Size_Per_Side) + 1;
                    int toUpdatePerSide = toUpdatePerHalf * 2 + 1;
                    toUpdate = new Chunk2D[toUpdatePerSide * toUpdatePerSide];

                    int MinY = hit.Chunk_Idx.y - toUpdatePerHalf;
                    int MaxY = hit.Chunk_Idx.y + toUpdatePerHalf;
                    int MinC = hit.Chunk_Idx.c - toUpdatePerHalf;
                    int MaxC = hit.Chunk_Idx.c + toUpdatePerHalf;

                    int idx = 0;
                    for (int c = MinC; c <= MaxC; c++)
                    {
                        for (int y = MinY; y <= MaxY; y++)
                        {
                            toUpdate[idx] = FindChunk(y, c);
                            idx++;
                        }
                    }
                }

                double chunk_y, chunk_c;
                LayerData[] layers;

                Point3D mid, abs;
                mid = +hit.ChunkTile_Hit.TileLayer_Hit.Tile_Hit.Cross;
                abs = new Point3D(0, mid.X, 0);

                double dist;
                for (int i = 0; i < toUpdate.Length; i++)
                {
                    if (toUpdate[i] != null)
                    {
                        chunk_y = toUpdate[i].Chunk_Tile_Idx_Y * Tile_Size;
                        chunk_c = toUpdate[i].Chunk_Tile_Idx_C * Tile_Size;
                        layers = toUpdate[i].Layers;

                        TileIndex tileIdx;
                        for (int t = 0; t < Tiles_Per_Area; t++)
                        {
                            tileIdx = new TileIndex(t);

                            abs.Y = chunk_y + (tileIdx.y * Tile_Size);
                            abs.C = chunk_c + (tileIdx.c * Tile_Size);

                            dist = (mid - abs).Len;
                            if (dist <= rad)
                            {
                                layers[hit.ToLayerIndex().idx].Tiles[tileIdx.idx].Height_Mid
                                    = func(layers[hit.ToLayerIndex().idx].Tiles[tileIdx.idx].Height_Mid,
                                    mid.X, rad, dist);
                            }
                        }
                    }
                }
                for (int i = 0; i < toUpdate.Length; i++)
                {
                    if (toUpdate[i] != null)
                    {
                        toUpdate[i].Corn_Update_All(hit.ToLayerIndex().idx);
                        toUpdate[i].Layers[hit.ToLayerIndex().idx].BufferUpdate();
                    }
                }
            }
            private static int RadIncFunc(int curr, double h, double rad, double dist)
            {
                double height = (rad - dist) + h;
                if (curr < height)
                    return (int)height;
                return curr;
            }
            private static int RadDecFunc(int curr, double h, double rad, double dist)
            {
                double height = (dist - rad) + h;
                if (curr > height)
                    return (int)height;
                return curr;
            }
            public void RadInc(SurfaceHit hit, double rad)
            {
                MultiChunkRadFunc(hit, rad, RadIncFunc);
            }
            public void RadDec(SurfaceHit hit, double rad)
            {
                MultiChunkRadFunc(hit, rad, RadDecFunc);
            }
        }
    }
}
