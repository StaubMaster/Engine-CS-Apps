using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract2D;
using Engine3D.Abstract3D;

using VoidFactory.GameSelect;
using VoidFactory.Production;
using VoidFactory.Surface2D;

namespace VoidFactory.Inventory
{
    class Category
    {
        /*
         * make an Array of UIBody Entrys for all the stuff
         * want to show thing in corner
         * so cant stop drawing UIBody Insts
         * add all the Entrys when Inventroy is "opened"
         * and remove them when it is "closed"
         * 
         */

        private readonly string Name;
        private Interaction[] Items;
        private bool Visible;

        public Category(string name)
        {
            Name = name;
            Visible = false;
        }

        public void Draw_Icons_Alloc()
        {
            if (Visible) { return; }
            Visible = true;

            Engine3D.ConsoleLog.Log("Cat Alloc");
            int i = 0;
            for (float y = +2; y >= -2; y--)
            {
                for (float x = -6; x <= +6; x++)
                {
                    if (i < Items.Length)
                    {
                        Items[i].Draw_Icon_Alloc(Inventory_Interface.gPos.WithOffset((x, y)), Inventory_Interface.gSize);
                    }
                    i++;
                }
            }
        }
        public void Draw_Icons_Update()
        {
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i].Draw_Icon_Update();
            }
        }
        public void Draw_Icons_Dispose()
        {
            if (!Visible) { return; }
            Visible = false;

            for (int i = 0; i < Items.Length; i++)
            {
                Items[i].Draw_Icon_Dispose();
            }
        }

        public int Hover(Point2D mouse)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].Hover(mouse))
                {
                    return i;
                }
            }
            return -1;
        }
        public Point2D GetOffset(int idx)
        {
            return Items[idx].GetOffset();
        }

        public void Draw()
        {
            /*
            int idx = 0;

            for (float y = +0.6f; y > -0.6f; y -= 0.2f)
            {
                for (float x = -0.6f; x < +0.6f; x += 0.1f)
                {
                    if (idx >= Items.Length)
                        return;

                    Items[idx].Draw_Inv_Icon(x, y);

                    idx++;
                }
            }
            */

            int x, y;
            float offX, offY;
            for (int i = 0; i < Items.Length; i++)
            {
                x = i % 13;
                y = i / 13;

                offX = -0.6f + 0.1f * x;
                offY = +0.6f - 0.2f * y;

                Items[i].Draw_Inv_Icon(offX, offY);
            }
        }
        public static void Draw(int idx)
        {
            int x, y;
            x = idx % 13;
            y = idx / 13;

            float offX, offY;
            offX = -0.6f + 0.1f * x;
            offY = +0.6f - 0.2f * y;

            Interaction.Draw_Inv_Box(offX, offY);
        }
        public override string ToString()
        {
            return Items.Length + ":" + Name;
        }

        public int Length
        {
            get
            {
                return Items.Length;
            }
        }
        public Interaction this[int idx]
        {
            get
            {
                if (idx >= 0 && idx < Items.Length)
                    return Items[idx];
                return null;
            }
        }

        private List<Interaction> ItemList;
        public void SortCreate()
        {
            ItemList = new List<Interaction>();
        }
        public void SortDelete()
        {
            Items = ItemList.ToArray();
            ItemList = null;
        }

        public static void Sort(Category catAll, Category catType, Category[] catUser, string catName, Interaction item)
        {
            catAll.ItemList.Add(item);
            catType.ItemList.Add(item);

            for (int c = 0; c < catUser.Length; c++)
            {
                if (catUser[c].Name == catName)
                {
                    catUser[c].ItemList.Add(item);
                    return;
                }
            }
            catUser[catUser.Length - 1].ItemList.Add(item);
        }
    }
}
