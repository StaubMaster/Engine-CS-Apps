using Engine3D.Abstract3D;
using Engine3D.Abstract2D;
using Engine3D.GraphicsOld;
using Engine3D.Graphics.Display;
using Engine3D.Graphics.Display2D.UserInterface;
using Engine3D.Graphics.Display2D;
using Engine3D.Graphics;
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

        private static UIGridPosition gPos;
        private static UIGridSize gSize;

        public static UIBody[] Meta_Bodys;
        public static EntryContainerDynamic<UIBodyData>.Entry[] Meta_Insts;
        public static UIBody[] BLD_Bodys;
        public static EntryContainerDynamic<UIBodyData>.Entry[] BLD_Insts;
        public static UIBody[] DATA_Thing_Bodys;
        public static EntryContainerDynamic<UIBodyData>.Entry[] DATA_Thing_Insts;

        public static void Draw_Init()
        {
            if (IsDraw) { return; }
            IsDraw = true;

            Meta_Insts = new EntryContainerBase<UIBodyData>.Entry[Meta_Bodys.Length];
            BLD_Insts = new EntryContainerBase<UIBodyData>.Entry[BLD_Bodys.Length];
            DATA_Thing_Insts = new EntryContainerBase<UIBodyData>.Entry[DATA_Thing_Bodys.Length];

            gPos = new UIGridPosition(UIAnchor.MM(), new Point2D(0.0f, 0.0f), UICorner.MM());
            gSize = new UIGridSize(new Point2D(100.0f, 100.0f), 25.0f);

            int i = 0;
            int j;
            for (float y = +2; y >= -2; y--)
            {
                for (float x = -6; x <= +6; x++)
                {
                    j = i;
                    if (j < Meta_Insts.Length)
                    {
                        Meta_Insts[j] = Meta_Bodys[j].Alloc(1);
                        Meta_Insts[j][0] = new UIBodyData(gPos.WithOffset(x, y), gSize, 0.2f, new Angle3D(1.0f, -0.5f, 0));
                    }
                    else
                    {
                        j = j - BLD_Insts.Length;
                        if (j < BLD_Insts.Length)
                        {
                            BLD_Insts[j] = BLD_Bodys[j].Alloc(1);
                            BLD_Insts[j][0] = new UIBodyData(gPos.WithOffset(x, y), gSize, 0.1f, new Angle3D(1.0f, -0.5f, 0));
                        }
                        else
                        {
                            j = j - BLD_Insts.Length;
                            if (j < DATA_Thing_Insts.Length)
                            {
                                DATA_Thing_Insts[j] = DATA_Thing_Bodys[j].Alloc(1);
                                DATA_Thing_Insts[j][0] = new UIBodyData(gPos.WithOffset(x, y), gSize, 0.4f, new Angle3D(1.0f, -0.5f, 0));
                            }
                        }
                    }
                    i++;
                }
            }
        }
        public static void Draw_Free()
        {
            if (!IsDraw) { return; }
            IsDraw = false;

            for (int i = 0; i < Meta_Insts.Length; i++)
            {
                Meta_Insts[i].Free();
            }
            Meta_Insts = null;

            for (int i = 0; i < BLD_Insts.Length; i++)
            {
                BLD_Insts[i].Free();
            }
            BLD_Insts = null;

            for (int i = 0; i < DATA_Thing_Insts.Length; i++)
            {
                DATA_Thing_Insts[i].Free();
            }
            DATA_Thing_Insts = null;
        }
        public static void Draw_Inst()
        {
            if (Meta_Insts != null)
            {
                UIBodyData data;
                for (int i = 0; i < Meta_Insts.Length; i++)
                {
                    if (Meta_Insts[i] != null)
                    {
                        data = Meta_Insts[i][0];
                        data.Trans.Rot.A += 0.01f;
                        Meta_Insts[i][0] = data;
                    }
                }
            }
            if (BLD_Insts != null)
            {
                UIBodyData data;
                for (int i = 0; i < BLD_Insts.Length; i++)
                {
                    if (BLD_Insts[i] != null)
                    {
                        data = BLD_Insts[i][0];
                        data.Trans.Rot.A += 0.01f;
                        BLD_Insts[i][0] = data;
                    }
                }
            }
            if (DATA_Thing_Insts != null)
            {
                UIBodyData data;
                for (int i = 0; i < DATA_Thing_Insts.Length; i++)
                {
                    if (DATA_Thing_Insts[i] != null)
                    {
                        data = DATA_Thing_Insts[i][0];
                        data.Trans.Rot.A += 0.01f;
                        DATA_Thing_Insts[i][0] = data;
                    }
                }
            }

            for (int i = 0; i < Meta_Bodys.Length; i++)
            {
                Meta_Bodys[i].DataUpdate();
                Meta_Bodys[i].DrawInst();
            }
            for (int i = 0; i < BLD_Bodys.Length; i++)
            {
                BLD_Bodys[i].DataUpdate();
                BLD_Bodys[i].DrawInst();
            }
            for (int i = 0; i < DATA_Thing_Bodys.Length; i++)
            {
                DATA_Thing_Bodys[i].DataUpdate();
                DATA_Thing_Bodys[i].DrawInst();
            }
        }

        public static void Update_Mouse(Point2D mouse, TextBuffer text)
        {
            string str = "";

            {
                UIBodyData instData = Meta_Insts[3][0];
                instData.Pos.Offset = new Point2D(-3, +2);
                Meta_Insts[3][0] = instData;
            }

            int hoverIDX = -1;
            float hoverX = float.NaN;
            float hoverY = float.NaN;

            for (int i = 0; i < BLD_Insts.Length; i++)
            {
                UIBodyData data = BLD_Insts[i][0];
                Point2D sizeHalf = new Point2D(data.Size.Size.X * 0.5f, data.Size.Size.Y * 0.5f);

                Point2D center;
                center.X = data.Pos.Pixel.X + data.Pos.Offset.X * (data.Size.Size.X + data.Size.Padding);
                center.Y = data.Pos.Pixel.Y + data.Pos.Offset.Y * (data.Size.Size.Y + data.Size.Padding);

                Point2D anchor;
                anchor.X = ((data.Pos.Normal.X + 1) / 2) * SizeRatio.SizeW;
                anchor.Y = ((data.Pos.Normal.Y + 1) / 2) * SizeRatio.SizeH;

                AxisBox2D box = AxisBox2D.MinMax((center + anchor) - sizeHalf, (center + anchor) + sizeHalf);

                if (box.Intersekt(mouse))
                {
                    hoverX = data.Pos.Offset.X;
                    hoverY = data.Pos.Offset.Y;

                    UIBodyData instData = Meta_Insts[3][0];
                    instData.Pos.Offset = data.Pos.Offset;
                    Meta_Insts[3][0] = instData;
                }
            }

            str += "Hover IDX: " + hoverIDX + "\n";
            str += "Hover X: " + hoverX + "\n";
            str += "Hover Y: " + hoverY + "\n";
            text.InsertTL((0, -4), text.Default_TextSize, 0xFFFFFF, str);
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
            Category_Idx++;
            Category_Idx = Category_Idx % Categorys.Length;
            Category_Idx = Category_Idx + Categorys.Length;
            Category_Idx = Category_Idx % Categorys.Length;
            Category_Ref = Categorys[Category_Idx];
            Tool_Select(false);
        }
        public static void Cat_Prev()
        {
            Category_Idx--;
            Category_Idx = Category_Idx % Categorys.Length;
            Category_Idx = Category_Idx + Categorys.Length;
            Category_Idx = Category_Idx % Categorys.Length;
            Category_Ref = Categorys[Category_Idx];
            Tool_Select(false);
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

        public static void Tool_Init()
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
