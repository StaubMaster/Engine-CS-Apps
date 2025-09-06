
using Engine3D.Abstract2D;
using Engine3D.Graphics;
using Engine3D.Graphics.Display;

using VoidFactory.Production.Data;
using VoidFactory.Production.Transfer;
using VoidFactory.Surface2D;

namespace VoidFactory.Inventory
{
    static class Inventory_Storage
    {
        public static GameSelect.Game3D.GraphicsData Graphic;
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
        public static void Draw(DATA_Recipy recipy)
        {
            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);

            DATA_Buffer found;
            float x, y;

            x = 0.9f;
            y = 0.9f;
            Graphic.Icon_Prog.UniPos(x, y);
            Graphic.Icon_Prog.UniScale(0.025f);
            IO_Port.BodyInn.Draw_Main();
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
            IO_Port.BodyOut.Draw_Main();
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
        }
        public static void Draw(DATA_Thing thing, int off = 0)
        {
            DATA_Buffer found = Find(thing);
            if (found == null) { return; }

            //MainContext.Text_Buff.Insert(0.95f, 0.65f - off * 0.2f, 0xFFFFFF, true, found.Num.ToString());
            Text_Buffer.Insert(
                new UIGridPosition(new Point2D(0.95f, 0.65f - off * 0.2f), new Point2D(0, 0), new Point2D(+0.5f, -0.5f)),
                Text_Buffer.Default_TextSize, UIGridDirection.DiagDR, 0xFFFFFF,
                found.Num.ToString());

            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);
            found.Thing.Draw(Graphic.Icon_Prog, 0.9f, 0.7f - off * 0.2f, 0.5f);
        }
        public static void Draw(DATA_Buffer buffer, int off = 0)
        {
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
        }
        public static void DrawInn(DATA_Cost cost)
        {
            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);

            DATA_Buffer found;
            float x, y;

            x = 0.9f;
            y = 0.9f;
            Graphic.Icon_Prog.UniPos(x, y);
            Graphic.Icon_Prog.UniScale(0.025f);
            IO_Port.BodyInn.Draw_Main();
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
        }
        public static void DrawOut(DATA_Cost cost)
        {
            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);

            DATA_Buffer found;
            float x, y;

            x = 0.7f;
            y = 0.9f;
            Graphic.Icon_Prog.UniPos(x, y);
            Graphic.Icon_Prog.UniScale(0.025f);
            IO_Port.BodyOut.Draw_Main();
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
