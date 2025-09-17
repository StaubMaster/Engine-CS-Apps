
using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;
using Engine3D.Graphics.Display3D;
using Engine3D.Miscellaneous.EntryContainer;

using VoidFactory.Production.Data;
using VoidFactory.Inventory;

namespace VoidFactory.Production.Transfer
{
    partial class IO_TransPorter
    {
        public static GameSelect.Game3D game;

        private const uint Thing_Shift = 3;
        private const uint Thing_Limit = 1 << (int)Thing_Shift;
        private const uint Thing_Tick_Limit = 256;
        private const uint Thing_Tick_Dist = (int)Thing_Tick_Limit >> (int)Thing_Shift;



        public bool ToRemove;

        private readonly IO_Port Inn;
        private readonly IO_Port Out;

        private DATA_Thing[] Thing;
        private EntryContainerBase<PolyHedraInstance_3D_Data>.Entry[] ThingInstData;
        private uint[] Tick;
        private uint Count;

        public IO_TransPorter(IO_Port Inn, IO_Port Out)
        {
            ToRemove = false;

            this.Inn = Inn;
            this.Out = Out;
            Inn.TransPorter = this;
            Out.TransPorter = this;

            Thing = new DATA_Thing[Thing_Limit];
            ThingInstData = new EntryContainerBase<PolyHedraInstance_3D_Data>.Entry[Thing_Limit];
            Tick = new uint[Thing_Limit];
            Count = 0;
        }
        public void Remove()
        {
            Inn.TransPorter = null;
            Out.TransPorter = null;

            DATA_Cost cost = new DATA_Cost();
            for (int i = 0; i < Count; i++)
            {
                cost.Accumulate(new DATA_Buffer(Thing[i], 1));
                ThingInstData[i].Dispose();
                ThingInstData[i] = null;
            }
            Inventory_Storage.CostRefund(cost);

            ToRemove = true;
        }

        private bool canInn()
        {
            return (Count < Thing_Limit) && (Count == 0 || Tick[Count - 1] >= Thing_Tick_Dist);
        }
        private bool canOut()
        {
            return (Count > 0) && (Tick[0] == Thing_Tick_Limit);
        }

        private void tryInn(ref DATA_Thing thing)
        {
            if (thing == null) { return; }

            Thing[Count] = thing;
            ThingInstData[Count] = game.PH_3D[thing.Idx].Alloc(1);
            ThingInstData[Count][0] = new PolyHedraInstance_3D_Data(Transformation3D.Default());
            Tick[Count] = 0;

            thing = null;
            Count++;
        }
        private void tryOut(ref DATA_Thing thing)
        {
            if (thing != null) { return; }

            thing = Thing[0];

            Count--;
            for (uint i = 0; i < Count; i++)
            {
                Thing[i] = Thing[i + 1];
                Tick[i] = Tick[i + 1];
            }
            Thing[Count] = null;
            ThingInstData[Count].Dispose();
            ThingInstData[Count] = null;
            Tick[Count] = 0;
        }

        public void Update()
        {
            if (canInn() && Inn.canOut())
            {
                DATA_Thing thing = null;
                Inn.tryOutS(ref thing);
                tryInn(ref thing);
            }

            {
                if (Tick[0] < Thing_Tick_Limit)
                {
                    Tick[0]++;
                }
                for (uint i = 1; i < Count; i++)
                {
                    if (Tick[i] + Thing_Tick_Dist < Tick[i - 1])
                    {
                        Tick[i]++;
                    }
                }
            }

            if (canOut() && Out.canInn())
            {
                DATA_Thing thing = null;
                tryOut(ref thing);
                Out.tryInnS(ref thing);
            }
        }

        public void Draw()
        {
            Ray3D ray = new Ray3D(Inn.Pos, Out.Pos - Inn.Pos);
            Transformation3D trans = Transformation3D.Default();
            PolyHedraInstance_3D_Data data;

            for (int i = 0; i < Count; i++)
            {
                if (Thing[i] == null) { break; }
                trans.Pos = ray.Scale((1.0 * Tick[i]) / Thing_Tick_Limit);

                data = ThingInstData[i][0];
                data.Trans = trans;
                ThingInstData[i][0] = data;
            }
        }

        public override string ToString()
        {
            string str = "";

            str += Count + "/" + Thing_Limit + "\n";
            for (int i = 0; i < Count; i++)
            {
                if (Thing[i] == null) { break; }
                str += "[" + i + "]" + Thing[i].Idx + ":" + Tick[i].ToString("000") + "\n";
            }

            return str;
        }
    }
}
