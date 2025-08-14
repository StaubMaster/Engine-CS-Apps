
using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;
using Engine3D.Graphics.Display;

using VoidFactory.Production.Data;
using VoidFactory.Production.Buildings;
using VoidFactory.Surface2D;

namespace VoidFactory.Inventory
{
    static class Inventory_Interface
    {
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
                Categorys[i + 5] = CatUser[i];

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
