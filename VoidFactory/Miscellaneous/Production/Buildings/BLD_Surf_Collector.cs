using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;
using VoidFactory.Production.Data;
using VoidFactory.Surface2D;

namespace VoidFactory.Production.Buildings
{
    class BLD_Surf_Collector : BLD_Base
    {
        public static Chunk2D.Collection Chunks;

        private Chunk2D.ChunkIndex chunkMid;
        private Chunk2D.TileIndex tileMin;
        private Chunk2D.TileIndex tileMax;

        private Chunk2D.ChunkIndex chunkIdx;
        private int tileIdxY;
        private int tileIdxC;

        private uint Tick;
        private DATA_Cost Storage;

        public BLD_Surf_Collector(Template_Base temp, Transformation3D trans, Chunk2D.ChunkIndex chunkIdx, Chunk2D.TileIndex tileIdx, int rad)
            : base(temp, trans)
        {
            chunkMid = chunkIdx;
            tileMin = tileIdx.Move(-rad, -rad);
            tileMax = tileIdx.Move(+rad, +rad);

            this.chunkIdx = chunkIdx;
            tileIdxY = tileIdx.y;
            tileIdxC = tileIdx.c;

            Out[0].Limit = 4;

            Tick = 0;
        }

        public override void Update()
        {
            if (Storage == null || Storage.Sum.Length == 0)
            {
                Storage = Chunks.SubThing(chunkIdx, new Chunk2D.TileIndex(tileIdxY, tileIdxC));

                if (tileIdxY != tileMax.y)
                {
                    tileIdxY++;
                }
                else
                {
                    tileIdxY = tileMin.y;
                    if (tileIdxC != tileMax.c)
                    {
                        tileIdxC++;
                    }
                    else
                    {
                        tileIdxC = tileMin.c;
                    }
                }
            }
            else
            {
                DATA_Buffer temp = Storage.Deccumulate();
                if (Out[0].canInn())
                    Out[0].tryInnA(temp);
                else
                    Storage.Accumulate(temp);
            }
        }



        public class Template : Template_Base
        {
            public BLD_Surf_Collector ToInstance(Transformation3D trans, Chunk2D.ChunkIndex chunkIdx, Chunk2D.TileIndex tileIdx, int rad)
            {
                return new BLD_Surf_Collector(this, trans, chunkIdx, tileIdx, rad);
            }
        }
    }
}
