
using Engine3D.Abstract2D;
using Engine3D.Abstract3D;
using Engine3D.Graphics;
using Engine3D.Graphics.PolyHedraInstance.PH_UI;
using Engine3D.Miscellaneous.EntryContainer;

using VoidFactory.Production.Data;
using VoidFactory.Production.Transfer;
using VoidFactory.Surface2D;

namespace VoidFactory.Inventory
{
    class UI_Entry_Array
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

        public UI_Entry_Array(int len)
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

        private static UI_Entry_Array InstData;
        private static int InstDataIndex;

        private static void Inst_Meta(IO_Port.MetaBodyIndex idx, Point2D offset)
        {
            InstData[InstDataIndex] = Inventory_Interface.game.PH_UI[(int)idx].Alloc(1);
            InstData[InstDataIndex][0] = new UIBody_Data(sPos.WithOffset(offset), sSize, 0.4f, spin);
            InstDataIndex++;
        }
        private static void Inst_Thing(DATA_Thing thing, Point2D offset)
        {
            InstData[InstDataIndex] = Inventory_Interface.game.PH_UI[thing.Idx].Alloc(1);

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
                InstData[InstDataIndex] = Inventory_Interface.game.PH_UI[buffer.Thing.Idx].Alloc(1);
            }
            else
            {
                InstData[InstDataIndex] = Inventory_Interface.game.PH_UI[(int)IO_Port.MetaBodyIndex.Error].Alloc(1);
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

        public static UI_Entry_Array Alloc_Recipy(DATA_Recipy recipy)
        {
            Init_Consts();

            InstData = new UI_Entry_Array(recipy.RInn.Length + recipy.ROut.Length + 2);
            InstDataIndex = 0;

            Inst_Meta(IO_Port.MetaBodyIndex.Inn, new Point2D(-0, -0));
            Inst_Buffers(recipy.RInn, new Point2D(-0, -1));

            Inst_Meta(IO_Port.MetaBodyIndex.Out, new Point2D(-1, -0));
            Inst_Buffers(recipy.ROut, new Point2D(-1, -1));

            UI_Entry_Array temp = InstData;
            InstData = null;
            return temp;
        }
        public static UI_Entry_Array Alloc_Cost_Inn(DATA_Cost cost)
        {
            Init_Consts();

            InstData = new UI_Entry_Array(cost.Sum.Length + 1);
            InstDataIndex = 0;

            Inst_Meta(IO_Port.MetaBodyIndex.Inn, new Point2D(-0, -0));
            Inst_Buffers(cost.Sum, new Point2D(-0, -1));

            UI_Entry_Array temp = InstData;
            InstData = null;
            return temp;
        }
        public static UI_Entry_Array Alloc_Cost_Out(DATA_Cost cost)
        {
            Init_Consts();

            InstData = new UI_Entry_Array(cost.Sum.Length + 1);
            InstDataIndex = 0;

            Inst_Meta(IO_Port.MetaBodyIndex.Out, new Point2D(-1, -0));
            Inst_Buffers(cost.Sum, new Point2D(-1, -1));

            UI_Entry_Array temp = InstData;
            InstData = null;
            return temp;
        }
        public static UI_Entry_Array Alloc_Thing(DATA_Thing thing, int off = 0)
        {
            Init_Consts();

            InstData = new UI_Entry_Array(1);
            InstDataIndex = 0;

            Inst_Thing(thing, new Point2D(0, -off));

            UI_Entry_Array temp = InstData;
            InstData = null;
            return temp;
        }
        public static UI_Entry_Array Alloc_Buffer(DATA_Buffer buffer, int off = 0)
        {
            Init_Consts();

            InstData = new UI_Entry_Array(1);
            InstDataIndex = 0;

            Inst_Buffer(buffer, new Point2D(0, -off));

            UI_Entry_Array temp = InstData;
            InstData = null;
            return temp;
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
