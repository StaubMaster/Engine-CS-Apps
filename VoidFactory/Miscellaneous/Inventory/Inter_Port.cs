
using VoidFactory.Production.Data;
using VoidFactory.Production.Transfer;
using VoidFactory.Production.Buildings;
using Engine3D.Graphics;
using Engine3D.Graphics.PolyHedraInstance.PH_3D;
using Engine3D.Miscellaneous.EntryContainer;

namespace VoidFactory.Inventory
{
    abstract class Inter_Port : Interaction
    {
        public static BLD_Base.Collection Buildings;

        protected IO_Port.Select_Port Hover;

        public override void Init()
        {
            Interaction.Draw_Gray = true;
            Interaction.Draw_Gray_Exclude_Idx = -1;
            Interaction.Draw_Ports = true;
        }

        public override void Update()
        {
            Hover = Buildings.Port_Select(Interaction.View.Ray);
        }
        public override void Draw()
        {
            //Hover.Draw_Hover(Graphic.Trans_Direct);
            //Hover.Draw_Hover(MainContext.Shader_Default);
            Hover.Draw_Hover();

            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.HoriL, 0, -9, 0xFFFFFF,
            //    "Hover Port:\n" + Hover);
            Text_Buffer.InsertTL(
                (0, -10), Text_Buffer.Default_TextSize, 0xFFFFFF,
                "Hover Port:\n" + Hover);
        }
    }
    class Inter_Connect : Inter_Port
    {
        public static IO_TransPorter.Collection TransPorter;

        private IO_Port.Select_Port Select;
        private EntryContainerBase<PolyHedraInstance_3D_Data>.Entry HoverInst;
        private EntryContainerBase<PolyHedraInstance_3D_Data>.Entry SelectInst;

        public override void Draw_Icon_Alloc(UIGridPosition pos, UIGridSize size)
        {
            InstRef = new UI_Meta(pos, size, IO_Port.MetaBodyIndex.TransPorter);
        }

        public override void Draw_Dispose()
        {
            if (HoverInst != null)
            {
                HoverInst.Dispose();
                HoverInst = null;
            }
            if (SelectInst != null)
            {
                SelectInst.Dispose();
                SelectInst = null;
            }
        }

        public override void Draw()
        {
            base.Draw();

            //Select.Draw_Select(Graphic.Trans_Direct);
            //Select.Draw_Select(MainContext.Shader_Default);
            Select.Draw_Select();

            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.HoriL, 0, -3, 0xFFFFFF,
            //    "Select Port:\n" + Hover);
            Text_Buffer.InsertTL(
                (0, -4), Text_Buffer.Default_TextSize, 0xFFFFFF,
                "Select Port:\n" + Select);

            if (HoverInst != null)
            {
                HoverInst.Dispose();
                HoverInst = null;
            }
            if (Hover.Valid)
            {
                if (Hover.InnOut)
                {
                    HoverInst = IO_Port.game.PH_3D[(int)IO_Port.MetaBodyIndex.OutHex].Alloc(1);
                }
                else
                {
                    HoverInst = IO_Port.game.PH_3D[(int)IO_Port.MetaBodyIndex.InnHex].Alloc(1);
                }
                HoverInst[0] = new PolyHedraInstance_3D_Data(new Engine3D.Abstract3D.Transformation3D(Hover.Pos));
            }

            if (SelectInst != null)
            {
                SelectInst.Dispose();
                SelectInst = null;
            }
            if (Select.Valid)
            {
                if (Select.InnOut)
                {
                    SelectInst = IO_Port.game.PH_3D[(int)IO_Port.MetaBodyIndex.OutOct].Alloc(1);
                }
                else
                {
                    SelectInst = IO_Port.game.PH_3D[(int)IO_Port.MetaBodyIndex.InnOct].Alloc(1);
                }
                SelectInst[0] = new PolyHedraInstance_3D_Data(new Engine3D.Abstract3D.Transformation3D(Select.Pos));
            }

            //Graphic.Icon_Prog.UniScale(0.04f);
            //Graphic.Icon_Prog.UniPos(+0.75f, -0.75f);
            //IO_Port.BodyTransPorter.DrawMain();
            //IO_Port.game.PH_3D[(int)IO_Port.MetaBodyIndex.TransPorter].DrawMain();
            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.BR, 0, 0, 0xFFFFFF, "Connect");
            //MainContext.Text_Buff.InsertBR(
            //    (-0.5f, +0.5f + 1), 0xFFFFFF, 20f, 2f,
            //    "Connect");
            Text_Type("Connect");
        }
        public override void Draw_Inv_Icon(float x, float y)
        {
            //Graphic.Icon_Prog.UniScale(0.01f);
            //Graphic.Icon_Prog.UniPos(x, y);
            //IO_Port.BodyTransPorter.DrawMain();
            //IO_Port.game.PH_3D[(int)IO_Port.MetaBodyIndex.TransPorter].DrawMain();
        }

        public override void Func1()
        {
            if (Select.IsSame(Hover))
            {
                Select.Reset();
            }
            else if (Select.CanConnect(Hover))
            {
                TransPorter.Add(Buildings.Port_Connect(Hover, Select));
                Select.Reset();
                Hover.Reset();
            }
            else
            {
                Select = Hover;
            }
        }
    }
    class Inter_Thing : Interaction
    {
        private DATA_Thing Thing;

        private UI_Entry_Array StorageInst;

        public Inter_Thing(DATA_Thing thing)
        {
            Thing = thing;

            StorageInst = null;
        }

        public override void Draw_Icon_Alloc(UIGridPosition pos, UIGridSize size)
        {
            InstRef = new UI_Thing(pos, size, Thing);
        }
        public override void Draw_Dispose()
        {
            if (StorageInst != null)
            {
                StorageInst.Dispose();
                StorageInst = null;
            }
        }

        public override void Update()
        {

        }
        public override void Draw()
        {
            if (StorageInst != null)
            {
                StorageInst.Dispose();
                StorageInst = null;
            }

            //Thing.Draw(Graphic.Icon_Prog, +0.75f, -0.75f);
            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.BR, 0, 0, 0xFFFFFF, "Thing");
            //MainContext.Text_Buff.InsertBR(
            //    (-0.5f, +0.5f + 1), 0xFFFFFF, 20f, 2f,
            //    "Thing");
            Text_Type("Thing");

            //Inventory_Storage.Draw(Thing);
            StorageInst = Inventory_Storage.Alloc_Thing(Thing, 1);
        }
        public override void Draw_Inv_Icon(float x, float y)
        {
            //Thing.Draw(Graphic.Icon_Prog, x, y, 0.5f);
        }
    }
}
