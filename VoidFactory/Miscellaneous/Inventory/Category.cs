using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;

using VoidFactory.GameSelect;
using VoidFactory.Production;
using VoidFactory.Surface2D;

namespace VoidFactory.Inventory
{
    class Category
    {
        private readonly string Name;
        private Interaction[] Items;

        public Category(string name)
        {
            Name = name;
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
