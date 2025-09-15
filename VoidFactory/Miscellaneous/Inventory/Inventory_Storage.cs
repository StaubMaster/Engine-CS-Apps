
using Engine3D.Abstract2D;
using Engine3D.Abstract3D;
using Engine3D.Graphics;
using Engine3D.Graphics.Display;
using Engine3D.Miscellaneous.EntryContainer;
using Engine3D.Graphics.Display2D.UserInterface;

using VoidFactory.Production.Data;
using VoidFactory.Production.Transfer;
using VoidFactory.Surface2D;

namespace VoidFactory.Inventory
{
    class Entry_Array
    {
        private EntryContainerDynamic<UIBody_Data>.Entry[] Data;

        public int Length { get { return Data.Length; } }

        public EntryContainerDynamic<UIBody_Data>.Entry this[int idx]
        {
            get { return Data[idx]; }
            set { Data[idx] = value; }
        }
        public EntryContainerDynamic<UIBody_Data>.Entry this[uint idx]
        {
            get { return Data[idx]; }
            set { Data[idx] = value; }
        }

        public Entry_Array(int len)
        {
            Data = new EntryContainerBase<UIBody_Data>.Entry[len];
        }
        public void Dispose()
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i].Dispose();
            }
            Data = null;
        }
    }

    static class Inventory_Storage
    {
        //public static DisplayContext MainContext;
        public static TextBuffer Text_Buffer;

        private static DATA_Buffer[] AllThings;

        public static void Create(DATA_Thing[] things)
        {
            AllThings = new DATA_Buffer[things.Length];
            for (int i = 0; i < AllThings.Length; i++)
                AllThings[i] = new DATA_Buffer(things[i]);
        }
        public static void Delete()
        {
            AllThings = null;
        }

        private static DATA_Buffer Find(DATA_Thing thing)
        {
            for (int t = 0; t < AllThings.Length; t++)
            {
                if (AllThings[t].Thing == thing)
                    return AllThings[t];
            }
            return null;
        }



        private static UIGridPosition sPos;
        private static UIGridSize sSize;
        private static Angle3D spin;

        private static Entry_Array InstData;
        private static int InstDataIndex;

        private static void Inst_Meta(IO_Port.MetaBodyIndex idx, Point2D offset)
        {
            InstData[InstDataIndex] = Inventory_Interface.Meta_Bodys[(int)idx].Alloc(1);
            InstData[InstDataIndex][0] = new UIBody_Data(sPos.WithOffset(offset), sSize, 0.4f, spin);
            InstDataIndex++;
        }
        private static void Inst_Thing(DATA_Thing thing, Point2D offset)
        {
            InstData[InstDataIndex] = Inventory_Interface.DATA_Thing_Bodys[thing.Idx].Alloc(1);

            UIBody_Data data = new UIBody_Data(sPos.WithOffset(offset), sSize, 0.4f, spin);
            InstData[InstDataIndex][0] = new UIBody_Data(sPos.WithOffset(offset), sSize, 0.4f, spin);

            Point2D anchor = data.Pos.Normal;
            Point2D pixel = UIGrid.PixelRelPos(data.Pos, data.Size);

            DATA_Buffer found = Find(thing);
            string foundStr;
            if (found == null) { foundStr = "NaN"; }
            else { foundStr = found.Num.ToString(); }

            Text_Buffer.Insert(anchor, pixel + new Point2D(  0,   0), UIGridDirection.DiagCC, (0, 0), 0xFFFFFF, Text_Buffer.Default_TextSize, "X");
            Text_Buffer.Insert(anchor, pixel + new Point2D(+25, +25), UIGridDirection.DiagDL, (0, 0), 0xFFFFFF, Text_Buffer.Default_TextSize, "(" + foundStr + ")");

            InstDataIndex++;
        }
        private static void Inst_Buffer(DATA_Buffer buffer, Point2D offset)
        {
            if (buffer.Thing != null)
            {
                InstData[InstDataIndex] = Inventory_Interface.DATA_Thing_Bodys[buffer.Thing.Idx].Alloc(1);
            }
            else
            {
                InstData[InstDataIndex] = Inventory_Interface.Meta_Bodys[(int)IO_Port.MetaBodyIndex.Error].Alloc(1);
            }

            UIBody_Data data = new UIBody_Data(sPos.WithOffset(offset), sSize, 0.4f, spin);
            InstData[InstDataIndex][0] = new UIBody_Data(sPos.WithOffset(offset), sSize, 0.4f, spin);

            Point2D anchor = data.Pos.Normal;
            Point2D pixel = UIGrid.PixelRelPos(data.Pos, data.Size);

            string haveStr = buffer.Num.ToString();

            DATA_Buffer found = Find(buffer.Thing);
            string foundStr;
            if (found == null) { foundStr = "NaN"; }
            else { foundStr = found.Num.ToString(); }

            Text_Buffer.Insert(anchor, pixel + new Point2D(-25, -25), UIGridDirection.DiagUR, (0, 0), 0xFFFFFF, Text_Buffer.Default_TextSize, "[" + haveStr + "]");
            Text_Buffer.Insert(anchor, pixel + new Point2D(0, 0), UIGridDirection.DiagCC, (0, 0), 0xFFFFFF, Text_Buffer.Default_TextSize, "X");
            Text_Buffer.Insert(anchor, pixel + new Point2D(+25, +25), UIGridDirection.DiagDL, (0, 0), 0xFFFFFF, Text_Buffer.Default_TextSize, "(" + foundStr + ")");

            InstDataIndex++;
        }
        private static void Inst_Buffers(DATA_Buffer[] buffers, Point2D offset)
        {
            for (int i = 0; i < buffers.Length; i++)
            {
                Inst_Buffer(buffers[i], new Point2D(offset.X, offset.Y - i));
            }
        }

        public static void Init_Consts()
        {
            sPos = new UIGridPosition(UIAnchor.TR(), new Point2D(0, 0), UICorner.TR());
            sSize = new UIGridSize(new Point2D(100, 100), 25);
            spin = new Angle3D(0, -0.5f, 0);
        }

        public static Entry_Array Alloc_Recipy(DATA_Recipy recipy)
        {
            Init_Consts();

            InstData = new Entry_Array(recipy.RInn.Length + recipy.ROut.Length + 2);
            InstDataIndex = 0;

            Inst_Meta(IO_Port.MetaBodyIndex.Inn, new Point2D(-0, -0));
            Inst_Buffers(recipy.RInn, new Point2D(-0, -1));

            Inst_Meta(IO_Port.MetaBodyIndex.Out, new Point2D(-1, -0));
            Inst_Buffers(recipy.ROut, new Point2D(-1, -1));

            Entry_Array temp = InstData;
            InstData = null;
            return temp;
        }
        public static Entry_Array Alloc_Cost_Inn(DATA_Cost cost)
        {
            Init_Consts();

            InstData = new Entry_Array(cost.Sum.Length + 1);
            InstDataIndex = 0;

            Inst_Meta(IO_Port.MetaBodyIndex.Inn, new Point2D(-0, -0));
            Inst_Buffers(cost.Sum, new Point2D(-0, -1));

            Entry_Array temp = InstData;
            InstData = null;
            return temp;
        }
        public static Entry_Array Alloc_Cost_Out(DATA_Cost cost)
        {
            Init_Consts();

            InstData = new Entry_Array(cost.Sum.Length + 1);
            InstDataIndex = 0;

            Inst_Meta(IO_Port.MetaBodyIndex.Out, new Point2D(-1, -0));
            Inst_Buffers(cost.Sum, new Point2D(-1, -1));

            Entry_Array temp = InstData;
            InstData = null;
            return temp;
        }
        public static Entry_Array Alloc_Thing(DATA_Thing thing, int off = 0)
        {
            Init_Consts();

            InstData = new Entry_Array(1);
            InstDataIndex = 0;

            Inst_Thing(thing, new Point2D(0, -off));

            Entry_Array temp = InstData;
            InstData = null;
            return temp;
        }
        public static Entry_Array Alloc_Buffer(DATA_Buffer buffer, int off = 0)
        {
            Init_Consts();

            InstData = new Entry_Array(1);
            InstDataIndex = 0;

            Inst_Buffer(buffer, new Point2D(0, -off));

            Entry_Array temp = InstData;
            InstData = null;
            return temp;
        }



        public static void Draw(DATA_Recipy recipy)
        {
            /*
            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);

            DATA_Buffer found;
            float x, y;

            x = 0.9f;
            y = 0.9f;
            Graphic.Icon_Prog.UniPos(x, y);
            Graphic.Icon_Prog.UniScale(0.025f);
            //IO_Port.BodyInn.DrawMain();
            IO_Port.Bodys[(int)IO_Port.MetaBodyIndex.Inn].DrawMain();
            for (int i = 0; i < recipy.RInn.Length; i++)
            {
                y -= 0.2f;
                found = Find(recipy.RInn[i].Thing);
                if (found != null)
                {
                    //MainContext.Text_Buff.Insert(x + 0.050f, y + 0.050f, 0xFFFFFF, true, recipy.RInn[i].Num.ToString());
                    //MainContext.Text_Buff.Insert(x + 0.050f, y - 0.050f, 0xFFFFFF, true, found.Num.ToString());

                    Point2D normPos = new Point2D(x, y);
                    Point2D offsetU = new Point2D(+0.050f, +0.050f);
                    Point2D offsetD = new Point2D(-0.050f, -0.050f);

                    Text_Buffer.Insert(
                        new UIGridPosition(normPos, new Point2D(0, 0), UICorner.TL()),
                        Text_Buffer.Default_TextSize, UIGridDirection.DiagDR, 0x0000FF,
                        "X");

                    Text_Buffer.Insert(
                        new UIGridPosition(normPos + offsetU, new Point2D(0, 0), UICorner.TL()),
                        Text_Buffer.Default_TextSize, UIGridDirection.DiagDR, 0x0000FF,
                        recipy.RInn[i].Num.ToString());

                    Text_Buffer.Insert(
                        new UIGridPosition(normPos + offsetD, new Point2D(0, 0), UICorner.TL()),
                        Text_Buffer.Default_TextSize, UIGridDirection.DiagDR, 0x0000FF,
                        found.Num.ToString());

                    found.Thing.Draw(Graphic.Icon_Prog, x, y, 0.5f);
                }
            }

            x = 0.8f;
            y = 0.9f;
            Graphic.Icon_Prog.UniPos(x, y);
            Graphic.Icon_Prog.UniScale(0.025f);
            //IO_Port.BodyOut.DrawMain();
            IO_Port.Bodys[(int)IO_Port.MetaBodyIndex.Out].DrawMain();
            for (int o = 0; o < recipy.ROut.Length; o++)
            {
                y -= 0.2f;
                found = Find(recipy.ROut[o].Thing);
                if (found != null)
                {
                    //MainContext.Text_Buff.Insert(x + 0.050f, y + 0.050f, 0xFFFFFF, true, recipy.ROut[o].Num.ToString());
                    //MainContext.Text_Buff.Insert(x + 0.050f, y - 0.050f, 0xFFFFFF, true, found.Num.ToString());

                    Point2D normPos = new Point2D(x, y);
                    Point2D offsetU = new Point2D(+0.050f, +0.050f);
                    Point2D offsetD = new Point2D(-0.050f, -0.050f);

                    Text_Buffer.Insert(
                        new UIGridPosition(normPos, new Point2D(0, 0), UICorner.TL()),
                        Text_Buffer.Default_TextSize, UIGridDirection.DiagDR, 0xFF0000,
                        "X");

                    Text_Buffer.Insert(
                        new UIGridPosition(normPos + offsetU, new Point2D(0, 0), UICorner.TL()),
                        Text_Buffer.Default_TextSize, UIGridDirection.DiagDR, 0xFF0000,
                        recipy.ROut[o].Num.ToString());

                    Text_Buffer.Insert(
                        new UIGridPosition(normPos + offsetD, new Point2D(0, 0), UICorner.TL()),
                        Text_Buffer.Default_TextSize, UIGridDirection.DiagDR, 0xFF0000,
                        found.Num.ToString());

                    found.Thing.Draw(Graphic.Icon_Prog, x, y, 0.5f);
                }
            }
            */
        }
        public static void Draw(DATA_Thing thing, int off = 0)
        {
            /*
            DATA_Buffer found = Find(thing);
            if (found == null) { return; }

            //MainContext.Text_Buff.Insert(0.95f, 0.65f - off * 0.2f, 0xFFFFFF, true, found.Num.ToString());
            Text_Buffer.Insert(
                new UIGridPosition(new Point2D(0.95f, 0.65f - off * 0.2f), new Point2D(0, 0), new Point2D(+0.5f, -0.5f)),
                Text_Buffer.Default_TextSize, UIGridDirection.DiagDR, 0xFFFFFF,
                found.Num.ToString());

            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);
            found.Thing.Draw(Graphic.Icon_Prog, 0.9f, 0.7f - off * 0.2f, 0.5f);
            */
        }
        public static void Draw(DATA_Buffer buffer, int off = 0)
        {
            /*
            DATA_Buffer found = Find(buffer.Thing);
            if (found == null) { return; }

            //MainContext.Text_Buff.Insert(0.95f, 0.75f - off * 0.2f, 0xFFFFFF, true, buffer.Num.ToString());
            //MainContext.Text_Buff.Insert(0.95f, 0.65f - off * 0.2f, 0xFFFFFF, true, found.Num.ToString());

            Text_Buffer.Insert(new Point2D(0.95f, 0.75f - off * 0.2f), new Point2D(0, 0), UIGridDirection.DiagDL,
                (+0.5f, -0.5f), 0xFFFFFF, Text_Buffer.Default_TextSize,
                buffer.Num.ToString());
            Text_Buffer.Insert(new Point2D(0.95f, 0.65f - off * 0.2f), new Point2D(0, 0), UIGridDirection.DiagDL,
                (+0.5f, -0.5f), 0xFFFFFF, Text_Buffer.Default_TextSize,
                found.Num.ToString());

            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);
            found.Thing.Draw(Graphic.Icon_Prog, 0.9f, 0.7f - off * 0.2f, 0.5f);
            */
        }
        public static void DrawInn(DATA_Cost cost)
        {
            /*
            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);

            DATA_Buffer found;
            float x, y;

            x = 0.9f;
            y = 0.9f;
            Graphic.Icon_Prog.UniPos(x, y);
            Graphic.Icon_Prog.UniScale(0.025f);
            //IO_Port.BodyInn.DrawMain();
            IO_Port.Bodys[(int)IO_Port.MetaBodyIndex.Inn].DrawMain();
            for (int c = 0; c < cost.Sum.Length; c++)
            {
                y -= 0.2f;
                found = Find(cost.Sum[c].Thing);
                if (found != null)
                {
                    //MainContext.Text_Buff.Insert(x + 0.050f, y + 0.050f, 0xFFFFFF, true, cost.Sum[c].Num.ToString());
                    //MainContext.Text_Buff.Insert(x + 0.050f, y - 0.050f, 0xFFFFFF, true, found.Num.ToString());

                    Text_Buffer.Insert(new Point2D(x + 0.050f, y + 0.050f), new Point2D(0, 0), UIGridDirection.DiagDL,
                        (+0.5f, -0.5f), 0xFFFFFF, Text_Buffer.Default_TextSize,
                        cost.Sum[c].Num.ToString());
                    Text_Buffer.Insert(new Point2D(x + 0.050f, y - 0.050f), new Point2D(0, 0), UIGridDirection.DiagDL,
                        (+0.5f, -0.5f), 0xFFFFFF, Text_Buffer.Default_TextSize,
                        found.Num.ToString());

                    found.Thing.Draw(Graphic.Icon_Prog, x, y, 0.5f);
                }
            }
            */
        }
        public static void DrawOut(DATA_Cost cost)
        {
            /*
            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);

            DATA_Buffer found;
            float x, y;

            x = 0.7f;
            y = 0.9f;
            Graphic.Icon_Prog.UniPos(x, y);
            Graphic.Icon_Prog.UniScale(0.025f);
            //IO_Port.BodyOut.DrawMain();
            IO_Port.Bodys[(int)IO_Port.MetaBodyIndex.Out].DrawMain();
            for (int c = 0; c < cost.Sum.Length; c++)
            {
                y -= 0.2f;
                found = Find(cost.Sum[c].Thing);
                if (found != null)
                {
                    //MainContext.Text_Buff.Insert(x + 0.050f, y + 0.050f, 0xFFFFFF, true, cost.Sum[c].Num.ToString());
                    //MainContext.Text_Buff.Insert(x + 0.050f, y - 0.050f, 0xFFFFFF, true, found.Num.ToString());

                    Text_Buffer.Insert(new Point2D(x + 0.050f, y + 0.050f), new Point2D(0, 0), UIGridDirection.DiagDL,
                        (+0.5f, -0.5f), 0xFFFFFF, Text_Buffer.Default_TextSize,
                        cost.Sum[c].Num.ToString());
                    Text_Buffer.Insert(new Point2D(x + 0.050f, y - 0.050f), new Point2D(0, 0), UIGridDirection.DiagDL,
                        (+0.5f, -0.5f), 0xFFFFFF, Text_Buffer.Default_TextSize,
                        found.Num.ToString());

                    found.Thing.Draw(Graphic.Icon_Prog, x, y, 0.5f);
                }
            }
            */
        }


        public static bool CostCanDeduct(DATA_Cost cost)
        {
            return cost.Can_Deduct(AllThings);
        }
        public static bool CostCanRefund(DATA_Cost cost)
        {
            return cost.Can_Refund(AllThings);
        }

        public static void CostDeduct(DATA_Cost cost)
        {
            cost.Deduct(AllThings);
        }
        public static void CostRefund(DATA_Cost cost)
        {
            cost.Refund(AllThings);
        }
    }
}
