using Engine3D.Abstract3D;
using Engine3D.Abstract2D;
using Engine3D.Graphics;
using Engine3D.Graphics.PolyHedraInstance.PH_UI;
using Engine3D.Miscellaneous.EntryContainer;

using VoidFactory.Production.Data;
using VoidFactory.Production.Buildings;
using VoidFactory.Production.Transfer;

namespace VoidFactory.Inventory
{
    abstract class UI_3D_Base
    {
        public UIGridPosition Pos;
        public UIGridSize Size;

        protected UI_3D_Base(UIGridPosition pos, UIGridSize size)
        {
            Pos = pos;
            Size = size;
        }

        public AxisBox2D PixelBoxNoPadding(Point2D pixelSize)
        {
            return UIGrid.PixelBoxNoPadding(Pos, Size, pixelSize);
        }

        public abstract void Update();
        public abstract void Dispose();
    }
    class UI_Meta : UI_3D_Base
    {
        private EntryContainerDynamic<UIBody_Data>.Entry Inst;
        public UI_Meta(UIGridPosition pos, UIGridSize size, IO_Port.MetaBodyIndex idx) : base(pos, size)
        {
            Inst = Inventory_Interface.game.PH_UI[(int)idx].Alloc(1);
            Inst[0] = new UIBody_Data(pos, size, 0.2f, new Angle3D(0, -0.5f, 0));
        }

        public override void Update()
        {
            UIBody_Data data;
            data = Inst[0];
            data.Trans.Rot.A += 0.01f;
            Inst[0] = data;
        }
        public override void Dispose()
        {
            Inst.Dispose();
        }
    }
    class UI_Building : UI_3D_Base
    {
        private EntryContainerDynamic<UIBody_Data>.Entry Inst;
        public UI_Building(UIGridPosition pos, UIGridSize size, BLD_Base.Template_Base template) : base(pos, size)
        {
            Inst = Inventory_Interface.game.PH_UI[template.Idx].Alloc(1);
            Inst[0] = new UIBody_Data(pos, size, 0.1f, new Angle3D(0, -0.5f, 0));
        }

        public override void Update()
        {
            UIBody_Data data;
            data = Inst[0];
            data.Trans.Rot.A += 0.01f;
            Inst[0] = data;
        }
        public override void Dispose()
        {
            Inst.Dispose();
        }
    }
    class UI_Thing : UI_3D_Base
    {
        private EntryContainerDynamic<UIBody_Data>.Entry Inst;
        public UI_Thing(UIGridPosition pos, UIGridSize size, DATA_Thing thing) : base(pos, size)
        {
            Inst = Inventory_Interface.game.PH_UI[thing.Idx].Alloc(1);
            Inst[0] = new UIBody_Data(pos, size, 0.4f, new Angle3D(0, -0.5f, 0));
        }

        public override void Update()
        {
            UIBody_Data data;
            data = Inst[0];
            data.Trans.Rot.A += 0.01f;
            Inst[0] = data;
        }
        public override void Dispose()
        {
            Inst.Dispose();
        }
    }
    class UI_Recipy : UI_3D_Base
    {
        private EntryContainerDynamic<UIBody_Data>.Entry[] Inn;
        private EntryContainerDynamic<UIBody_Data>.Entry[] Out;

        public UI_Recipy(UIGridPosition pos, UIGridSize size, DATA_Recipy recipy) : base(pos, size)
        {
            int total_sum;
            int total_idx;
            UIBody_Data data;

            Inn = new EntryContainerBase<UIBody_Data>.Entry[recipy.RInn.Length];
            total_sum = 0;
            for (int idx = 0; idx < recipy.RInn.Length; idx++)
            {
                DATA_Buffer buf = recipy.RInn[idx];
                if (buf.Thing != null)
                {
                    Inn[idx] = Inventory_Interface.game.PH_UI[buf.Thing.Idx].Alloc((int)buf.Num);
                }
                else
                {
                    Inn[idx] = Inventory_Interface.game.PH_UI[(int)IO_Port.MetaBodyIndex.Error].Alloc((int)buf.Num);
                }
                for (uint off = 0; off < buf.Num; off++)
                {
                    Transformation3D trans = Transformation3D.Default();
                    trans.Pos.C = 1.0f;
                    trans.Rot.S = -0.5f;
                    Inn[idx][(int)off] = new UIBody_Data(pos, size, 0.1f, trans);
                    total_sum++;
                }
            }
            total_idx = 0;
            for (int idx = 0; idx < Inn.Length; idx++)
            {
                if (Inn[idx] != null)
                {
                    for (int off = 0; off < Inn[idx].Length; off++)
                    {
                        data = Inn[idx][off];
                        data.Trans.Rot.A = (Angle3D.Deg360 * total_idx) / total_sum;
                        Inn[idx][off] = data;
                        total_idx++;
                    }
                }
            }

            Out = new EntryContainerBase<UIBody_Data>.Entry[recipy.ROut.Length];
            total_sum = 0;
            for (int idx = 0; idx < recipy.ROut.Length; idx++)
            {
                DATA_Buffer buf = recipy.ROut[idx];
                if (buf.Thing != null)
                {
                    Out[idx] = Inventory_Interface.game.PH_UI[buf.Thing.Idx].Alloc((int)buf.Num);
                }
                else
                {
                    Out[idx] = Inventory_Interface.game.PH_UI[(int)IO_Port.MetaBodyIndex.Error].Alloc((int)buf.Num);
                }
                for (uint off = 0; off < buf.Num; off++)
                {
                    Transformation3D trans = Transformation3D.Default();
                    trans.Pos.C = 0.5f;
                    trans.Rot.S = -0.5f;
                    Out[idx][(int)off] = new UIBody_Data(pos, size, 0.1f, trans);
                    total_sum++;
                }
            }
            total_idx = 0;
            for (int idx = 0; idx < Out.Length; idx++)
            {
                if (Out[idx] != null)
                {
                    for (int off = 0; off < Out[idx].Length; off++)
                    {
                        data = Out[idx][off];
                        data.Trans.Rot.A = (Angle3D.Deg360 * total_idx) / total_sum;
                        Out[idx][off] = data;
                        total_idx++;
                    }
                }
            }
        }
        public override void Update()
        {
            UIBody_Data data;

            for (int idx = 0; idx < Inn.Length; idx++)
            {
                if (Inn[idx] != null)
                {
                    for (int off = 0; off < Inn[idx].Length; off++)
                    {
                        data = Inn[idx][off];
                        data.Trans.Rot.A += 0.02f;
                        Inn[idx][off] = data;
                    }
                }
            }

            for (int idx = 0; idx < Out.Length; idx++)
            {
                if (Out[idx] != null)
                {
                    for (int off = 0; off < Out[idx].Length; off++)
                    {
                        data = Out[idx][off];
                        data.Trans.Rot.A -= 0.01f;
                        Out[idx][off] = data;
                    }
                }
            }
        }
        public override void Dispose()
        {
            for (int i = 0; i < Inn.Length; i++)
            {
                Inn[i]?.Dispose();
                Inn[i] = null;
            }

            for (int o = 0; o < Out.Length; o++)
            {
                Out[o]?.Dispose();
                Out[o] = null;
            }
        }
    }
}
