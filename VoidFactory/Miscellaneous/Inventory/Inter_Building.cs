
using Engine3D.GraphicsOld;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;

using VoidFactory.Production.Data;
using VoidFactory.Production.Transfer;
using VoidFactory.Production.Buildings;

namespace VoidFactory.Inventory
{
    abstract class Inter_Building : Interaction
    {
        public static BLD_Base.Collection Buildings;

        protected BLD_Base.Select_Building Hover;

        public override void Init()
        {
            Graphic.Draw_Gray = true;
            Graphic.Draw_Ports = false;
        }

        public override void Update()
        {
            Hover = Buildings.Select(Graphic.View_Ray);
            Graphic.Draw_Gray_Exclude_Idx = Hover.Building_Idx;
        }
        public override void Draw()
        {
            if (Hover.Valid)
            {
                //Graphic.Trans_Direct.UniTrans(new RenderTrans(Hover.Pos));
                BodyUni_Shader.Trans.Value(new Engine3D.Abstract3D.Transformation3D(Hover.Pos));
                //IO_Port.BodyOct.DrawMain();
                IO_Port.Bodys[(int)IO_Port.MetaBodyIndex.Out].DrawMain();
            }

            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.HoriL, 0, -9, 0xFFFFFF,
            //    "Hover Building:\n" + Hover);
            //MainContext.Text_Buff.InsertTR(
            //    (-0.5f, -0.5f - 9), 0xFFFFFF, 20f, 2f,
            //    "Hover Building:\n" + Hover);

            string str = "";

            str += "Hover Building:\n" + Hover;

            if (Hover.Valid)
            {
                str += "\n\n";
                //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.HoriL, 0, -5, 0xFFFFFF,
                //    "Building:\n" + Buildings.StringOf(Hover.Building_Idx));
                //MainContext.Text_Buff.InsertTR(
                //    (-0.5f, -0.5f - 5), 0xFFFFFF, 20f, 2f,
                //    "Building:\n" + Buildings.StringOf(Hover.Building_Idx));
                str += "Building:\n" + Buildings.StringOf(Hover.Building_Idx);
            }

            Text_Buffer.InsertTL(
                (0, -3), Text_Buffer.Default_TextSize, 0xFFFFFF,
                str);
        }
    }
    class Inter_Remover : Inter_Building
    {
        public override void Draw_Icon_Alloc(UIGridPosition pos, UIGridSize size)
        {
            InstRef = new UI_Meta(pos, size, IO_Port.MetaBodyIndex.Error);
        }

        public override void Draw()
        {
            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.BR, 0, 0, 0xFFFFFF, "Remover");
            //MainContext.Text_Buff.InsertBR(
            //    (+0.5f, -0.5f - 1), 0xFFFFFF, 20f, 2f,
            //    "Remover");
            Text_Type("Remover");

            if (Hover.Valid)
            {
                Buildings.DrawCost(Hover);
            }
        }

        public override void Func1()
        {
            Buildings.Sub(Hover);
            Hover.Reset();
        }
    }
    class Inter_Recipy : Inter_Building
    {
        private DATA_Recipy Recipy;
        private uint Tick;
        private bool Crafting;
        private DATA_Cost CostInn;
        private DATA_Cost CostOut;

        public Inter_Recipy(DATA_Recipy recipy)
        {
            Recipy = recipy;
            CostInn = new DATA_Cost(Recipy.RInn);
            CostOut = new DATA_Cost(Recipy.ROut);
        }

        public override void Init()
        {
            base.Init();
            Tick = 0;
            Crafting = false;
        }

        public override void Draw_Icon_Alloc(UIGridPosition pos, UIGridSize size)
        {
            InstRef = new UI_Recipy(pos, size, Recipy);
        }

        public override void Draw_Alloc()
        {
            Inventory_Storage.Draw_Init(Recipy);
        }
        public override void Draw_Dispose()
        {
            Inventory_Storage.Draw_Free();
        }

        public override void Update()
        {
            base.Update();
        }
        public override void Draw()
        {
            base.Draw();

            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.BR, -50, -10, 0xFFFFFF,
            //    "Recipy:\n" + Recipy, true);
            //MainContext.Text_Buff.InsertBL(
            //    (+0.5f, -0.5f), 0xFFFFFF, 20f, 2f,
            //    "Recipy:\n" + Recipy);
            Text_Info("Recipy:\n" + Recipy);

            Recipy.Draw(Graphic.Icon_Prog, Graphic.Tick / 64.0, +0.75f, -0.75f, 0.4f);
            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.BR, 0, 0, 0xFFFFFF, Tick + " Recipy");
            //MainContext.Text_Buff.InsertBR(
            //    (+0.5f, -0.5f - 1), 0xFFFFFF, 20f, 2f,
            //    Tick + " Recipy");
            Text_Type(Tick + " Recipy");

            Inventory_Storage.Draw(Recipy);

            if (Crafting)
            {
                Tick++;
                if (Tick >= Recipy.Ticks)
                {
                    if (Inventory_Storage.CostCanDeduct(CostInn) && Inventory_Storage.CostCanRefund(CostOut))
                    {
                        Inventory_Storage.CostDeduct(CostInn);
                        Inventory_Storage.CostRefund(CostOut);
                    }
                    Crafting = false;
                    Tick = 0;
                }
            }
        }
        public override void Draw_Inv_Icon(float x, float y)
        {
            Recipy.Draw(Graphic.Icon_Prog, Graphic.Tick / 64.0, x, y, 0.1f);
        }

        public override void Func1()
        {
            Buildings.RecipySet(Hover, Recipy);
        }
        public override void Func2()
        {
            if (Crafting) { return; }

            if (Inventory_Storage.CostCanDeduct(CostInn) && Inventory_Storage.CostCanRefund(CostOut))
            {
                Crafting = true;
                Tick = 0;
            }
        }
    }
}
