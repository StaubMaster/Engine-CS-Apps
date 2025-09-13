using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine3D.Abstract3D;

namespace VoidFactory
{
    public partial class PolygonCalc : Form
    {
        public PolygonCalc()
        {
            InitializeComponent();

            Poly_Num = 0;
            Poly_Size = 1;
            Layer_Num = 1;
            Layer_Size = 1;

            {
                Wnk_Select.Items.Clear();
                Wnk_Select.Items.Add(new Item_Name(new Transformation3D(new Point3D(0, +1, 0), new Angle3D(1, 0, 0)), "A (YC) +X"));
                Wnk_Select.Items.Add(new Item_Name(new Transformation3D(new Point3D(+1, 0, 0), new Angle3D(0, 1, 0)), "S (XC) +Y"));
                Wnk_Select.Items.Add(new Item_Name(new Transformation3D(new Point3D(0, 0, +1), new Angle3D(0, 0, 1)), "D (YX) +C"));
                Wnk_Select.Items.Add(new Item_Name(new Transformation3D(new Point3D(0, -1, 0), new Angle3D(1, 0, 0)), "A (YC) -X"));
                Wnk_Select.Items.Add(new Item_Name(new Transformation3D(new Point3D(-1, 0, 0), new Angle3D(0, 1, 0)), "S (XC) -Y"));
                Wnk_Select.Items.Add(new Item_Name(new Transformation3D(new Point3D(0, 0, -1), new Angle3D(0, 0, 1)), "D (YX) -C"));
                Wnk_Select.SelectedIndex = 0;
            }

            {
                Dir_Select.Items.Clear();
                Dir_Select.Items.Add(new Item_Name(new Point3D(+1, 0, 0), "Y+"));
                Dir_Select.Items.Add(new Item_Name(new Point3D(-1, 0, 0), "Y-"));
                Dir_Select.Items.Add(new Item_Name(new Point3D(0, +1, 0), "X+"));
                Dir_Select.Items.Add(new Item_Name(new Point3D(0, -1, 0), "X-"));
                Dir_Select.Items.Add(new Item_Name(new Point3D(0, 0, +1), "C+"));
                Dir_Select.Items.Add(new Item_Name(new Point3D(0, 0, -1), "C-"));
                Dir_Select.SelectedIndex = 0;
            }

            Point_Format = "+0.00;-0.00; 0.00";
            Index_Format = "0";

            Offset = Point3D.Default();
        }

        uint Poly_Num;
        double Poly_Size;

        uint Layer_Num;
        double Layer_Size;

        struct Item_Name
        {
            public readonly object Item;
            public readonly string Name;

            public Item_Name(object item, string name)
            {
                Item = item;
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }
        Angle3D Rotation;
        Point3D Direction;
        Point3D Origin;

        Point3D Offset;

        private void Poly_Num_ValueChanged(object sender, EventArgs e)
        {
            Poly_Num = (uint)num_Poly_Num.Value;
            Calculate();
        }
        private void Poly_Size_ValueChanged(object sender, EventArgs e)
        {
            Poly_Size = (double)num_Poly_Size.Value;
            Calculate();
        }
        private void Layer_Num_ValueChanged(object sender, EventArgs e)
        {
            Layer_Num = (uint)num_Layer_Num.Value;
            Calculate();
        }
        private void Layer_Size_ValueChanged(object sender, EventArgs e)
        {
            Layer_Size = (float)num_Layer_Size.Value;
            Calculate();
        }



        private void Offset_Y_ValueChanged(object sender, EventArgs e)
        {
            Offset.Y = (float)Offset_Y.Value;
            Calculate();
        }
        private void Offset_X_ValueChanged(object sender, EventArgs e)
        {
            Offset.X = (float)Offset_X.Value;
            Calculate();
        }
        private void Offset_C_ValueChanged(object sender, EventArgs e)
        {
            Offset.C = (float)Offset_C.Value;
            Calculate();
        }
        private void Offset_reset_Click(object sender, EventArgs e)
        {
            Offset_Y.Value = 0;
            Offset_X.Value = 0;
            Offset_C.Value = 0;
            Calculate();
        }



        private Point3D[] CalcDisk()
        {
            Point3D[] Ring = new Point3D[Poly_Num];

            double wnk;
            Angle3D winkel = Angle3D.Default();
            Point3D rotated;

            Points_Flat.Text = "";
            for (uint i = 0; i < Poly_Num; i++)
            {
                wnk = (Angle3D.Full * i) / Poly_Num;
                winkel.A = Rotation.A * wnk;
                winkel.S = Rotation.S * wnk;
                winkel.D = Rotation.D * wnk;

                rotated = Origin + winkel;
                rotated = rotated * (float)Poly_Size;
                rotated = rotated + Offset;

                Ring[i] = rotated;

                Points_Flat.Text += rotated.Y.ToString(Point_Format);
                Points_Flat.Text += " : ";
                Points_Flat.Text += rotated.X.ToString(Point_Format);
                Points_Flat.Text += " : ";
                Points_Flat.Text += rotated.C.ToString(Point_Format);
                Points_Flat.Text += "\n";
            }

            return Ring;
        }
        private void Print_Poly(Point3D[] Ring, uint l, ref string file_p)
        {
            Point3D layer, ring;

            layer = Direction * (float)(l * Layer_Size);
            for (uint i = 0; i < Poly_Num; i++)
            {
                ring = Ring[i] + layer;

                file_p += "p";
                file_p += " " + ring.Y.ToString(Point_Format);
                file_p += " " + ring.X.ToString(Point_Format);
                file_p += " " + ring.C.ToString(Point_Format);
                file_p += "\n";
            }
        }
        private void Print_Layer(uint l, ref string file_o)
        {
            uint[] indexe = new uint[4];

            for (uint i = 0; i < Poly_Num; i++)
            {
                indexe[0] = ((l - 1) * Poly_Num) + ((i + 0) % Poly_Num);
                indexe[1] = ((l - 1) * Poly_Num) + ((i + 1) % Poly_Num);
                indexe[2] = ((l - 0) * Poly_Num) + ((i + 0) % Poly_Num);
                indexe[3] = ((l - 0) * Poly_Num) + ((i + 1) % Poly_Num);

                file_o += "o";
                file_o += " +" + indexe[0].ToString(Index_Format);
                file_o += " +" + indexe[1].ToString(Index_Format);
                file_o += " +" + indexe[2].ToString(Index_Format);
                file_o += " +" + indexe[3].ToString(Index_Format);
                file_o += "\n";
            }
        }
        private void Calculate()
        {
            Point3D[] Ring = CalcDisk();

            string file_p = "";
            Print_Poly(Ring, 0, ref file_p);

            string file_o = "";
            for (uint l = 1; l < Layer_Num; l++)
            {
                file_p += "\n";
                file_o += "\n";

                Print_Poly(Ring, l, ref file_p);
                Print_Layer(l, ref file_o);
            }

            string file_v = "";
            file_v += "v";
            file_v += " +" + (Poly_Num * Layer_Num);
            file_v += " +" + (Poly_Num * (Layer_Num - 1) * 2);

            Points_Body.Text = file_p + file_o + file_v;
        }
        private void Manual_Click(object sender, EventArgs e)
        {
            Calculate();
        }



        private void Axis_Select_SelectedIndexChanged(object sender, EventArgs e)
        {
            object item_sel = Wnk_Select.SelectedItem;
            if (item_sel != null)
            {
                Transformation3D trans;
                trans = (Transformation3D)((Item_Name)item_sel).Item;
                Rotation = trans.Rot;
                Direction = trans.Pos;
                Calculate();
            }
        }
        private void Orig_Select_SelectedIndexChanged(object sender, EventArgs e)
        {
            object item_sel = Dir_Select.SelectedItem;
            if (item_sel != null)
            {
                Origin = (Point3D)((Item_Name)item_sel).Item;
                Calculate();
            }
        }


        private string Point_Format;
        private string Index_Format;

        private void Point_Lead_ValueChanged(object sender, EventArgs e)
        {
            int lead_0_num = (int)Point_Lead.Value;
            string lead_0_str = new string('0', lead_0_num);

            label_Point_0.Text = "Point: " + lead_0_str + ",00";
            Point_Format = "";
            Point_Format += "+" + lead_0_str + ".00;";
            Point_Format += "-" + lead_0_str + ".00;";
            Point_Format += " " + lead_0_str + ".00";

            Calculate();
        }
        private void Index_Lead_ValueChanged(object sender, EventArgs e)
        {
            int lead_0_num = (int)Index_Lead.Value;
            string lead_0_str = new string('0', lead_0_num);

            label_Index_0.Text = "Index: " + lead_0_str;
            Index_Format = lead_0_str;

            Calculate();
        }
    }
}
