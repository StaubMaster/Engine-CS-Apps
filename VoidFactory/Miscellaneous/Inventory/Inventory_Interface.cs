using Engine3D.Abstract3D;
using Engine3D.Abstract2D;
using Engine3D.GraphicsOld;
using Engine3D.Graphics.Display;
using Engine3D.Graphics.Display2D.UserInterface;
using Engine3D.Graphics.Display2D;
using Engine3D.Graphics;
using Engine3D.Entity;
using Engine3D.Miscellaneous.EntryContainer;
using Engine3D.DataStructs;

using VoidFactory.Production.Data;
using VoidFactory.Production.Buildings;
using VoidFactory.Surface2D;

namespace VoidFactory.Inventory
{
    static class Inventory_Interface
    {
        private static bool IsDraw;

        public static SizeRatio SizeRatio;

        public static UIGridPosition gPos;
        public static UIGridSize gSize;



        public static UIBody_Array Meta_Bodys;
        public static UIBody_Array BLD_Bodys;
        public static UIBody_Array DATA_Thing_Bodys;

        public static EntryContainerDynamic<UIBody_Data>.Entry MetaHovering;
        public static EntryContainerDynamic<UIBody_Data>.Entry MetaSelected;
        public static int Hovering_Idx;
        public static int Selected_Idx;



        public static void Draw_Init()
        {
            if (IsDraw) { return; }
            IsDraw = true;

            gPos = new UIGridPosition(UIAnchor.MM(), new Point2D(0.0f, 0.0f), UICorner.MM());
            gSize = new UIGridSize(new Point2D(100.0f, 100.0f), 25.0f);

            MetaHovering = Meta_Bodys[3].Alloc(1);
            MetaSelected = Meta_Bodys[4].Alloc(1);
            MetaHovering[0] = new UIBody_Data(gPos, gSize, 0.3f, new Angle3D(0, -0.5f, 0));
            MetaSelected[0] = new UIBody_Data(gPos, gSize, 0.3f, new Angle3D(0, -0.5f, 0));
            Select_None();
            Hover_None();
        }
        public static void Draw_Free()
        {
            if (!IsDraw) { return; }
            IsDraw = false;

            MetaHovering.Dispose();
            MetaSelected.Dispose();
            MetaHovering = null;
            MetaSelected = null;
        }
        public static void Draw_UI_Insts()
        {
            Category_Ref.Draw_Icons_Update();

            if (Hovering_Idx != -1 && MetaHovering != null)
            {
                UIBody_Data instData = MetaHovering[0];
                instData.Trans.Rot.A += 0.01f;
                MetaHovering[0] = instData;
            }

            if (Selected_Idx != -1 && MetaSelected != null)
            {
                UIBody_Data instData = MetaSelected[0];
                instData.Trans.Rot.A += 0.01f;
                MetaSelected[0] = instData;
            }

            Meta_Bodys.Update();
            Meta_Bodys.Draw();

            BLD_Bodys.Update();
            BLD_Bodys.Draw();

            DATA_Thing_Bodys.Update();
            DATA_Thing_Bodys.Draw();
        }
        public static void Draw_UI_Info(TextBuffer text)
        {
            string str = "";
            int sum;

            sum = 0;
            for (int i = 0; i < Meta_Bodys.Length; i++)
            {
                sum += Meta_Bodys[i].Count;
            }
            str += "Meta: " + sum + "\n";

            sum = 0;
            for (int i = 0; i < BLD_Bodys.Length; i++)
            {
                sum += BLD_Bodys[i].Count;
            }
            str += "Building: " + sum + "\n";

            sum = 0;
            for (int i = 0; i < DATA_Thing_Bodys.Length; i++)
            {
                sum += DATA_Thing_Bodys[i].Count;
            }
            str += "Thing: " + sum + "\n";

            text.InsertTR((0, -3), text.Default_TextSize, 0x000000, str);
        }

        public static void Hover_None()
        {
            UIBody_Data instData = MetaHovering[0];
            instData.Pos.Offset = Point2D.Null();
            MetaHovering[0] = instData;

            Hovering_Idx = -1;
        }
        public static void Hover_Idx(int idx)
        {
            UIBody_Data instData = MetaHovering[0];
            //instData.Pos.Offset = InventoryInsts[idx].Pos.Offset;
            instData.Pos.Offset = Category_Ref.GetOffset(idx);
            MetaHovering[0] = instData;
            Hovering_Idx = idx;
        }
        public static void Select_None()
        {
            UIBody_Data instData = MetaSelected[0];
            instData.Pos.Offset = Point2D.Null();
            MetaSelected[0] = instData;

            Selected_Idx = -1;
        }
        public static void Select_Idx(int idx)
        {
            UIBody_Data instData = MetaSelected[0];
            //instData.Pos.Offset = InventoryInsts[idx].Pos.Offset;
            instData.Pos.Offset = Category_Ref.GetOffset(idx);
            MetaSelected[0] = instData;
            Selected_Idx = idx;

            Tool_Select();
        }

        public static void Mouse_Hover(Point2D mouse)
        {
            /*Hover_None();
            if (InventoryInsts != null)
            {
                for (int i = 0; i < InventoryInsts.Length; i++)
                {
                    AxisBox2D box = InventoryInsts[i].PixelBoxNoPadding((SizeRatio.SizeW, SizeRatio.SizeH));
                    if (box.Intersekt(mouse))
                    {
                        Hover_Idx(i);
                    }
                }
            }*/

            int idx = Category_Ref.Hover(mouse);
            if (idx != -1)
            {
                Hover_Idx(idx);
            }
            else
            {
                Hover_None();
            }
        }
        public static void Mouse_Select()
        {
            if (Hovering_Idx != Selected_Idx)
            {
                if (Hovering_Idx != -1)
                {
                    Select_Idx(Hovering_Idx);
                }
                else
                {
                    Select_None();
                }
            }
        }



        /*
            Thing Storage:
                All Things and how Many of each I Have
            Interactions:
                Surface
                    create Cost of what To Mine/Place
                    check if Cost can be Deducted/Refunded
                Recipy
                    create Cost of Inn and Out
                    check if Inn can Deduct and Out can Refund

            Pre Cache Costs
                + don't change (at least I can't think of why it should)
                - more memory (no Idea how Much is currenty being taken up)
            no Cacheing
                how do I get a Referance to Thing to compare
                    Recipys already have it
                    If I add Cost for Buildings
                        pre Cached anyway from File
                    Surface change
                        Create new Interaction that takes Thing
        */

        private static Category CatAll;
        private static Category CatTools;
        private static Category CatThings;
        private static Category CatBuildings;
        private static Category CatRecipys;
        private static Category[] CatUser;

        private static Category[] Categorys;
        private static Category Category_Ref;
        private static int Category_Idx;

        private static int Tool_Idx;
        private static Interaction Tool;

        public static void Create()
        {
            CatAll = new Category("ALL");
            CatTools = new Category("Tool");
            CatThings = new Category("Things");
            CatBuildings = new Category("Buildings");
            CatRecipys = new Category("Recipys");
            CatUser = new Category[]
            {
                new Category("surface"),
                new Category("trans"),
                new Category("construction"),
                new Category("general"),
                new Category("electronic"),
                new Category("void"),
                new Category("other"),
            };

            Categorys = new Category[5 + CatUser.Length];
            Categorys[0] = CatAll;
            Categorys[1] = CatTools;
            Categorys[2] = CatThings;
            Categorys[3] = CatBuildings;
            Categorys[4] = CatRecipys;
            for (int i = 0; i < CatUser.Length; i++)
            {
                Categorys[i + 5] = CatUser[i];
            }

            Category_Idx = 0;
            Category_Ref = Categorys[Category_Idx];

            Tool_Idx = 0;
            Tool = null;
        }
        public static void Delete()
        {
            Cat_Hide();
            Categorys = null;
        }
        public static void Sort(DATA_Thing[] things, BLD_Base.Template_Base[] templates, DATA_Recipy[] recipys, Chunk2D.SURF_Object.Template[] surfTemp)
        {
            for (int i = 0; i < Categorys.Length; i++)
                Categorys[i].SortCreate();

            Category.Sort(CatAll, CatTools, CatUser, "surface", new Inter_Surf2D_Tile());
            Category.Sort(CatAll, CatTools, CatUser, "surface", new Inter_Surf2D_Rad());
            Category.Sort(CatAll, CatTools, CatUser, "trans", new Inter_Connect());
            Category.Sort(CatAll, CatTools, CatUser, "construction", new Inter_Remover());

            for (int i = 0; i < things.Length; i++)
                Category.Sort(CatAll, CatThings, CatUser, things[i].Cat, new Inter_Thing(things[i]));
            for (int i = 0; i < templates.Length; i++)
                Category.Sort(CatAll, CatBuildings, CatUser, templates[i].Category, new Inter_Surf2D_Building(templates[i]));
            for (int i = 0; i < recipys.Length; i++)
                Category.Sort(CatAll, CatRecipys, CatUser, recipys[i].Cat, new Inter_Recipy(recipys[i]));
            for (int i = 0; i < surfTemp.Length; i++)
                Category.Sort(CatAll, CatRecipys, CatUser, "surface", new Inter_Surf2D_Object(surfTemp[i]));

            for (int i = 0; i < Categorys.Length; i++)
                Categorys[i].SortDelete();
        }



        public static void Cat_Next()
        {
            Cat_Hide();
            Category_Idx++;
            Category_Idx = Category_Idx % Categorys.Length;
            Category_Idx = Category_Idx + Categorys.Length;
            Category_Idx = Category_Idx % Categorys.Length;
            Category_Ref = Categorys[Category_Idx];
            Cat_Show();
            Tool_Select(false);
        }
        public static void Cat_Prev()
        {
            Cat_Hide();
            Category_Idx--;
            Category_Idx = Category_Idx % Categorys.Length;
            Category_Idx = Category_Idx + Categorys.Length;
            Category_Idx = Category_Idx % Categorys.Length;
            Category_Ref = Categorys[Category_Idx];
            Cat_Show();
            Tool_Select(false);
        }
        public static void Cat_Show()
        {
            Category_Ref.Draw_Icons_Alloc();
        }
        public static void Cat_Hide()
        {
            Category_Ref.Draw_Icons_Dispose();
        }
        public static void Cat_Draw()
        {
            Interaction.Text_Buffer.InsertBR(
                (0, 0), Interaction.Text_Buffer.Default_TextSize, 0xFFFFFF,
                Tool_Idx + "/" + Categorys[Category_Idx].ToString());
            //Interaction.MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.BR, 0, -1, 0xFFFFFF,
            //    Tool_Idx + "/" + Categorys[Category_Idx].ToString());
            Categorys[Category_Idx].Draw();

            if (Tool != null)
            {
                Category.Draw(Tool_Idx);
            }
        }



        private static void Tool_Select()
        {
            if (Tool != null)
            {
                Tool.Draw_Dispose();
            }

            Tool_Idx = Selected_Idx;
            if (Tool_Idx != -1)
            {
                Tool = Category_Ref[Tool_Idx];
            }
            else
            {
                Tool = null;
            }

            if (Tool != null)
            {
                Tool.Draw_Alloc();
            }
        }
        private static void Tool_Select(bool select)
        {
            if (select)
            {
                Tool_Idx = Tool_Idx % Category_Ref.Length;
                Tool_Idx = Tool_Idx + Category_Ref.Length;
                Tool_Idx = Tool_Idx % Category_Ref.Length;
                Tool = Category_Ref[Tool_Idx];
            }
            else
            {
                Tool_Idx = 0;
                Tool = null;
            }

            Tool_Init();
        }
        public static void Tool_Change(int diff)
        {
            if (diff == 0) { return; }
            Tool_Idx += diff;
            Tool_Select(true);
        }

        private static void Tool_Init()
        {
            if (Tool != null) { Tool.Init(); }
            else
            {
                Interaction.Graphic.Draw_Gray = false;
                Interaction.Graphic.Draw_Ports = false;
            }
        }
        public static void Tool_Update()
        {
            if (Tool != null) { Tool.Update(); }
        }
        public static void Tool_Draw()
        {
            if (Tool != null) { Tool.Draw(); }
        }
        public static void Tool_Func1()
        {
            if (Tool != null) { Tool.Func1(); }
        }
        public static void Tool_Func2()
        {
            if (Tool != null) { Tool.Func2(); }
        }
    }
}
