using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Abstract;
using Engine3D.Graphics;
using Engine3D.Graphics.Forms;
using Engine3D.Noise;
using Engine3D.Entity;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace VoidFactory.GameSelect
{
    class GameBodyEditor : Game3D
    {
        private Func<string> File_Load;
        private Func<string> File_Save;
        private string File_Load_Name;
        private string File_Save_Name;


        private TransUniProgram Prog_Fill;
        private TransUniProgram Prog_Depth;
        private TransUniProgram Prog_Wire;
        private TransUniProgram Prog_Sel_Side;
        private TransUniProgram Prog_Sel_Corn;
        private TextProgram Text_Prog;
        private TextBuffers Text_Buff;

        private BodyDynamic Work_Body;
        private Transformation Work_Trans;

        private double ViewDist;
        private Transformation ViewBack;
        private KeyPress View_Reset;

        private Ray SelectMouseRay;
        private int SelectHoverSideIdx;
        private int SelectHoverCornIdx;

        private KeyPress Key_Add;
        private KeyPress Key_Sub;
        private KeyPress Key_Place;
        private KeyPress Key_Reset;

        private class MenuSelect
        {
            public readonly string Name;
            public readonly MenuSelect[] Menu;
            public readonly Action Func;

            private int Index;
            private bool ReDirect;

            public MenuSelect(string name, Action func)
            {
                Name = name;
                Menu = null;
                Func = func;

                Index = -1;
                ReDirect = false;
            }
            public MenuSelect(string name, MenuSelect[] menu)
            {
                Name = name;
                Menu = menu;
                Func = null;

                Index = 0;
                ReDirect = false;
            }

            public void Index_Inc()
            {
                if (ReDirect)
                    Menu[Index].Index_Inc();
                else if (Menu != null)
                    Index = ((Index + Menu.Length) + 1) % Menu.Length;
            }
            public void Index_Dec()
            {
                if (ReDirect)
                    Menu[Index].Index_Dec();
                else if (Menu != null)
                    Index = ((Index + Menu.Length) - 1) % Menu.Length;
            }

            public bool ReDirect_Inc()
            {
                if (ReDirect)
                {
                    Menu[Index].ReDirect_Inc();
                }
                else
                {
                    if (Menu == null)
                        return false;
                    ReDirect = true;
                }
                return true;
            }
            public bool ReDirect_Dec()
            {
                if (ReDirect)
                {
                    ReDirect = Menu[Index].ReDirect_Dec();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public Action Function()
            {
                if (ReDirect)
                    return Menu[Index].Function();
                return Func;
            }

            public void ToString(ref string str, string tab)
            {
                if (Menu != null)
                {
                    string tabber = tab + " ";
                    for (int i = 0; i < Menu.Length; i++)
                    {
                        str += tab;
                        str += (Index == i) ? '<' : ' ';
                        str += Menu[i].Name;
                        str += (Index == i) ? '>' : ' ';
                        str += '\n';

                        if (ReDirect && i == Index)
                            Menu[i].ToString(ref str, tabber);
                    }
                }
                else
                {
                    str += tab + " Func \n";
                }
            }
        }
        private MenuSelect Menu_Lvl;
        private KeyList Menu_KeyList;
        private Action Menu_Func;

        private Transformation Axis_Trans;
        private BodyStatic Axis_Body;
        private KeyToggle Axis_Toggle;

        private Punkt Move_Hori;
        private Punkt Move_Vert;
        private KeyToggle Move_Toggle;

        private PolygonCalc Form_Poly;

        public GameBodyEditor(Action externDelete, Func<string> func_load, Func<string> func_save) : base(externDelete)
        {
            File_Load = func_load;
            File_Save = func_save;

            Prog_Fill = new TransUniProgram("Fill",
                "Vert_Unif_noCol.vert",
                "Geom_Norm_Color.geom",
                "Frag/Direct.frag");
            Prog_Depth = new TransUniProgram("Depth",
                "Vert_Unif_noCol.vert",
                "Geom_Norm_Color.geom",
                "Frag/Depth.frag");
            Prog_Wire = new TransUniProgram("Wire",
                "Vert_Unif_noCol.vert",
                "Wire/Geom_Wire.geom",
                "Frag/Direct.frag");

            Prog_Sel_Corn = new TransUniProgram("Corn",
                "Wire/Vert_Select.vert",
                "Wire/Geom_Box.geom",
                "Frag/Direct.frag");
            Prog_Sel_Side = new TransUniProgram("Side",
                "Vert_Unif_noCol.vert",
                "Wire/Geom_Select.geom",
                "Frag/Light.frag");

            Text_Prog = new TextProgram("Text",
                "Text/Text.vert",
                "Text/Text.geom",
                "Frag/Direct.frag");
            Text_Buff = new TextBuffers();

            CommandFunction = Command;
        }

        private void Body_New()
        {
            if (Work_Body != null)
            {
                Work_Body.BufferDelete();
                Work_Body = null;
            }

            Work_Body = new BodyDynamic();
            if (Work_Body != null)
            {
                Work_Body.BufferCreate();
                Work_Body.BufferFill();
            }
        }
        private void Body_Load()
        {
            if (Work_Body != null)
            {
                Work_Body.BufferDelete();
                Work_Body = null;
            }

            if (!string.IsNullOrEmpty(File_Load_Name))
                Work_Body = BodyStatic.File.Load(File_Load_Name).ToDynamic();
            if (Work_Body != null)
            {
                Work_Body.BufferCreate();
                Work_Body.BufferFill();
            }
        }
        private void Body_Save()
        {
            if (Work_Body == null) { return; }

            BodyDynamic.File.Save(File_Save_Name, Work_Body);
        }
        private void Command(string cmd)
        {
            if (cmd.StartsWith("Dist"))
            {
                cmd = cmd.Remove(0, 4);
                double d;
                if (double.TryParse(cmd, out d))
                    ViewDist = d;
                else
                    ConsoleLog.Log("Dist: Parse Error");
            }
            else if (cmd.StartsWith("DepthN"))
            {
                cmd = cmd.Remove(0, 6);
                float f;
                if (float.TryParse(cmd, out f))
                {
                    //view.DepthN = f;
                    //view.ChangeDepthN(f);
                    view.renderDepth.Near = f;
                    RenProgUniDepth();
                }
                else
                    ConsoleLog.Log("DepthN: Parse Error");
            }
            else if (cmd.StartsWith("DepthF"))
            {
                cmd = cmd.Remove(0, 6);
                float f;
                if (float.TryParse(cmd, out f))
                {
                    //view.DepthF = f;
                    //view.ChangeDepthF(f);
                    view.renderDepth.Far = f;
                    RenProgUniDepth();
                }
                else
                    ConsoleLog.Log("DepthF: Parse Error");
            }
            else if (cmd.StartsWith("New"))
            {
                File_Load_Name = null;
                Body_New();
            }
            else if (cmd == "Load" || cmd == "load")
            {
                File_Load_Name = File_Load();
                Body_Load();
            }
            else if (cmd.StartsWith("Save"))
            {
                File_Save_Name = File_Save();
                Body_Save();
            }
            else if (cmd.StartsWith("Null"))
            {
                File_Load_Name = null;
                if (Work_Body != null)
                {
                    Work_Body.BufferDelete();
                    Work_Body = null;
                }
            }
            else if (cmd == "poly" || cmd == "polz")
            {
                Form_Poly.ShowDialog();
            }
            else
                ConsoleLog.Log("User:" + cmd);
        }

        private void MouseRayCalc()
        {
            (float, float) mouseWin = win.MouseFromMiddle();
            mouseWin.Item1 /= 500.0f;
            mouseWin.Item2 /= 500.0f;

            Punkt mouse;
            mouse = new Punkt(mouseWin.Item1, mouseWin.Item2, 1);
            mouse = (mouse - view.Trans.Rot);

            SelectMouseRay = new Ray(ViewBack.Pos, mouse);
        }

        private void Menu_Update()
        {
            if (Menu_KeyList.Index() == 0)
                Menu_Lvl.Index_Inc();
            if (Menu_KeyList.Index() == 1)
                Menu_Lvl.Index_Dec();
            if (Menu_KeyList.Index() == 2)
            {
                Menu_Lvl.ReDirect_Inc();
                Menu_Func = Menu_Lvl.Function();
            }
            if (Menu_KeyList.Index() == 3)
            {
                Menu_Lvl.ReDirect_Dec();
                Menu_Func = Menu_Lvl.Function();
            }

            string str = "";
            Menu_Lvl.ToString(ref str, "");
            Text_Buff.Insert(-0.99f, +0.80f, 0xFFFFFF, false, str);
        }

        private void View_Scroll_Update()
        {
            float scroll = win.MouseScroll();
            if (scroll > 0)
                ViewDist = Math.Max(ViewDist - 1.0, -10);
            if (scroll < 0)
                ViewDist = Math.Min(ViewDist + 1.0, 100);
        }
        private void Update_View()
        {
            TMovement.FlatX(ref view.Trans, win.VelPos_Key() * ViewDist * 0.001, null);
            view.Update();

            View_Scroll_Update();

            if (View_Reset.Check())
                view.Trans.Pos = new Punkt();

            string str = ":View\n";
            str += ViewDist.ToString("00") + "d:Dist\n";
            str += view.renderDepth.Near + "f:DepthN\n";
            str += view.renderDepth.Far + "f:DepthF\n";
            str += "\n";
            str += view.Trans.Pos.Y.ToString("0.000") + ":Y\n";
            str += view.Trans.Pos.X.ToString("0.000") + ":X\n";
            str += view.Trans.Pos.C.ToString("0.000") + ":C\n";
            str += "\n";
            str += "Y++ [ ]\n";
            str += "Y-- [c]\n";
            str += "X++ [E]\n";
            str += "X-- [D]\n";
            str += "C++ [F]\n";
            str += "C-- [S]\n";
            str += "Reset " + View_Reset +"\n";
            str += "Toggle to Move " + Move_Toggle + "\n";
            Text_Buff.Insert(+0.99f, +0.95f, 0xFFFFFF, true, str);
        }
        private void Update_Move()
        {
            View_Scroll_Update();

            Axis_Trans.Rot = view.Trans.Rot.Axis();
            Move_Hori = Axis_Trans.TFore(new Punkt(1, 0, 0));
            Move_Vert = Axis_Trans.TFore(new Punkt(0, 1, 0));

            Punkt scaled = win.VelPos_Key() * (0.001 * ViewDist);
            Punkt shifted = new Punkt();
            shifted += Move_Hori * scaled.Y;
            shifted += Move_Vert * scaled.C;

            if (Work_Body != null)
                Work_Body.Set_Ecken_Move(shifted);

            string str = ":Move\n";
            str += ViewDist.ToString("00") + "d:Dist\n";
            {
                Punkt p = null;
                if (Work_Body != null)
                    p = Work_Body.Get_Ecken_Move(SelectHoverCornIdx);
                if (p != null)
                {
                    str += p.Y.ToString("0.00") + ":Y\n";
                    str += p.X.ToString("0.00") + ":X\n";
                    str += p.C.ToString("0.00") + ":C\n";
                }
                else
                {
                    str += ":Y\n";
                    str += ":X\n";
                    str += ":C\n";
                }
            }
            str += "\n";
            str += "Y++ [F]\n";
            str += "Y-- [S]\n";
            str += "X++ [E]\n";
            str += "X-- [D]\n";
            str += "Toggle to View " + Move_Toggle + "\n";
            Text_Buff.Insert(+0.99f, +0.95f, 0xFFFFFF, true, str);
        }


        private void Update_Ecken()
        {
            if (Work_Body == null) { return; }

            if (Key_Add.Check())
                Work_Body.Ecken_Add(+view.Trans.Pos);
            if (Key_Sub.Check())
                Work_Body.Ecken_Sub();

            if (Key_Reset.Check())
            {
                Work_Body.Select_Ecken_Non();
                SelectHoverCornIdx = -1;
            }
            else
            {
                Work_Body.NähsteEcke(SelectMouseRay, out SelectHoverCornIdx);
                if (win.MouseL())
                    Work_Body.Select_Ecken_Idx(SelectHoverCornIdx, false, true);
                Work_Body.Select_Ecken_Idx(SelectHoverCornIdx, true, false);
            }
        }
        private void Update_Seitn()
        {
            if (Work_Body == null) { return; }

            if (Key_Add.Check())
                Work_Body.Seitn_Add();
            if (Key_Sub.Check())
                Work_Body.Seitn_Sub();

            if (Key_Reset.Check())
            {
                Work_Body.Select_Seitn_Non();
                SelectHoverSideIdx = -1;
            }
            else if (Key_Place.Check())
            {
                Work_Body.Seitn_Flip();
            }
            else
            {
                Work_Body.Intersekt(SelectMouseRay, out SelectHoverSideIdx);

                if (win.MouseL())
                    Work_Body.Select_Seitn_Idx(SelectHoverSideIdx, false, true);

                Work_Body.Select_Seitn_Idx(SelectHoverSideIdx, true, false);
            }
        }

        protected override void Frame()
        {
            win.UText.BufferFill(Text_Buff);
            {
                string file_str = "";
                file_str += "\nFile Load:" + File_Load_Name;
                file_str += "\nFile Save:" + File_Save_Name;
                Text_Buff.Insert(-0.99f, +0.97f, 0xFFFFFF, false, file_str);
            }

            TMovement.FlatX(ref view.Trans, null, win.VelRot_Mouse());
            view.Update();
            ViewBack = new Transformation(view.Trans.TFore(new Punkt(0, 0, -ViewDist)), view.Trans.Rot);
            MouseRayCalc();

            if (Move_Toggle.Check())
                Update_Move();
            else
                Update_View();

            if (Work_Body != null)
            {
                Work_Body.Select_Ecken_Idx(SelectHoverCornIdx, false, false);
                Work_Body.Select_Seitn_Idx(SelectHoverSideIdx, false, false);
            }
            SelectHoverCornIdx = -1;
            SelectHoverSideIdx = -1;

            Menu_Update();
            if (Menu_Func != null)
                Menu_Func();

            string str = "";
            if (Work_Body != null)
            {
                RenProgDraw_Ecken(Prog_Sel_Corn);
                RenProgDraw_Seitn(Prog_Sel_Side);
                //RenProgDraw_Color(Prog_Wire);

                str += "Toggle Axis [Z]\n";
                str += "Add " + Key_Add + "\n";
                str += "Sub " + Key_Sub + "\n";
                str += "Place " + Key_Place + "\n";
                str += "Reset " + Key_Reset + "\n";
                str += SelectHoverCornIdx + ":Hover E\n";
                str += SelectHoverSideIdx + ":Hover S\n";
                str += Work_Body.Ecken_Count() + ":Ecken\n";
                str += Work_Body.Seitn_Count() + ":Seitn\n";
            }
            else
            {
                str += "\n[No Body]";
            }
            Text_Buff.Insert(+0.99f, -0.40f, 0xFFFFFF, true, str);

            Text_Prog.Use();
            Text_Buff.Fill_Strings();
            Text_Buff.Draw();

            if (Axis_Toggle.Check())
            {
                Prog_Fill.Use();
                //Prog_Fill.UniView(Rechnen.RenderFloats(ViewBack));
                //Prog_Fill.UniTrans(Rechnen.RenderFloats(Axis_Trans));
                Prog_Fill.UniView(new RenderTrans(ViewBack));
                Prog_Fill.UniTrans(new RenderTrans(Axis_Trans));
                Axis_Body.BufferDraw();
            }
        }

        private void RenProgUniDepth()
        {
            //Prog_Fill.UniProj(view.DepthN, view.DepthF, view.Fov);
            //Prog_Depth.UniProj(view.DepthN, view.DepthF, view.Fov);
            //Prog_Wire.UniProj(view.DepthN, view.DepthF, view.Fov);
            //Prog_Sel_Corn.UniProj(view.DepthN, view.DepthF, view.Fov);

            view.UniDepth(Prog_Fill);
            view.UniDepth(Prog_Depth);
            view.UniDepth(Prog_Wire);
            view.UniDepth(Prog_Sel_Corn);
        }
        private void RenProgCreate(TransUniProgram Program)
        {
            Program.Create();
            //Program.UniProj(view.DepthN, view.DepthF, view.Fov);
            view.UniDepth(Program);
        }

        private void RenProgDraw_Ecken(TransUniProgram Program)
        {
            //Program.UniView(Rechnen.RenderFloats(ViewBack));
            //Program.UniTrans(Rechnen.RenderFloats(Work_Trans));
            Program.UniView(new RenderTrans(ViewBack));
            Program.UniTrans(new RenderTrans(Work_Trans));
            Work_Body.BufferDraw_Ecken_Selection();
        }
        private void RenProgDraw_Seitn(TransUniProgram Program)
        {
            //Program.UniView(Rechnen.RenderFloats(ViewBack));
            //Program.UniTrans(Rechnen.RenderFloats(Work_Trans));
            Program.UniView(new RenderTrans(ViewBack));
            Program.UniTrans(new RenderTrans(Work_Trans));
            Work_Body.BufferDraw_Seitn_Selection();
        }
        private void RenProgDraw_Color(TransUniProgram Program)
        {
            //Program.UniView(Rechnen.RenderFloats(ViewBack));
            //Program.UniTrans(Rechnen.RenderFloats(Work_Trans));
            Program.UniView(new RenderTrans(ViewBack));
            Program.UniTrans(new RenderTrans(Work_Trans));
            Work_Body.BufferDraw_Color();
        }


        public override void Create()
        {
            if (Running) { return; }
            ConsoleLog.Log("Create Editor");
            ConsoleLog.TabInc();
            base.Create();
            view.renderDepth.Calc(0.1f, 10000f);
            ViewDist = 10;


            RenProgCreate(Prog_Fill);
            RenProgCreate(Prog_Depth);
            RenProgCreate(Prog_Wire);
            RenProgCreate(Prog_Sel_Corn);
            RenProgCreate(Prog_Sel_Side);


            Text_Prog.Create();
            Text_Buff.Create();
            Text_Buff.Fill_Pallets();

            Work_Trans = new Transformation();

            {
                Axis_Trans = new Transformation();

                Axis_Body = BodyStatic.File.Load("E:/Programmieren/VS_Code/Spiel Zeug/3D/AxisCross.txt");
                Axis_Body.BufferCreate();
                Axis_Body.BufferFill();

                //Axis_Toggle = new KeyToggle(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Z);
                win.KeyChecks.Add(Axis_Toggle);
            }

            SelectMouseRay = null;
            SelectHoverSideIdx = -1;
            SelectHoverCornIdx = -1;

            {
                //Key_Add = new KeyPress(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Insert);
                //Key_Sub = new KeyPress(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Delete);
                //Key_Place = new KeyPress(OpenTK.Windowing.GraphicsLibraryFramework.Keys.P);
                //Key_Reset = new KeyPress(OpenTK.Windowing.GraphicsLibraryFramework.Keys.O);
                win.KeyChecks.Add(Key_Add);
                win.KeyChecks.Add(Key_Sub);
                win.KeyChecks.Add(Key_Place);
                win.KeyChecks.Add(Key_Reset);
            }

            {
                Move_Hori = new Punkt(1, 0, 0);
                Move_Vert = new Punkt(0, 1, 0);

                //Move_Toggle = new KeyToggle(OpenTK.Windowing.GraphicsLibraryFramework.Keys.L);
                //View_Reset = new KeyPress(OpenTK.Windowing.GraphicsLibraryFramework.Keys.K);
                win.KeyChecks.Add(Move_Toggle);
                win.KeyChecks.Add(View_Reset);
            }

            {
                /*Menu_KeyList = new KeyList(new OpenTK.Windowing.GraphicsLibraryFramework.Keys[]
                {
                    OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down,
                    OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up,
                    OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right,
                    OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left,
                });*/
                win.KeyChecksL.Add(Menu_KeyList);

                Menu_Lvl = new MenuSelect("Menu", new MenuSelect[]
                {
                    new MenuSelect("Ecken", Update_Ecken),
                    new MenuSelect("Seitn", Update_Seitn),
                });
            }

            Form_Poly = new PolygonCalc();

            ConsoleLog.TabDec();
            ConsoleLog.Log("");
            Running = true;
        }
        public override void Delete()
        {
            if (!Running) { return; }
            Running = false;

            ConsoleLog.Log("Delete Editor");
            ConsoleLog.TabInc();
            base.Delete();


            Prog_Fill.Delete();
            Prog_Depth.Delete();
            Prog_Wire.Delete();
            Prog_Sel_Corn.Delete();
            Prog_Sel_Side.Delete();

            Text_Prog.Delete();
            Text_Buff.Delete();


            if (Work_Body != null)
            {
                Work_Body.BufferDelete();
                Work_Body = null;
            }
            Work_Trans = null;


            Axis_Body.BufferDelete();
            Axis_Body = null;
            Axis_Trans = null;

            Form_Poly = null;

            ConsoleLog.TabDec();
            ConsoleLog.Log("");
        }
    }
}
