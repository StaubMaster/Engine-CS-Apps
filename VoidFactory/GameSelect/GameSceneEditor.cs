using System;
using System.Collections.Generic;
using System.IO;

using Engine3D;
using Engine3D.Abstract;
using Engine3D.Entity;
using Engine3D.Graphics;
using Engine3D.Graphics.Forms;

namespace VoidFactory.GameSelect
{
    class GameSceneEditor : Game3D
    {
        private Func<string> File_Load_Func;
        private Func<string> File_Save_Func;
        private string File_Load_Name;
        private string File_Save_Name;
        private KeyPress Key_Scene_Load;
        private KeyPress Key_Scene_Save;

        private TextProgram Text_Prog;
        private TextBuffers Text_Buff;
        private TransUniProgram Draw_Prog_Fill;
        private BoxProgram Box_Prog_Axis;

        private BodyStatic View_Ray_Body;
        private Ray View_Ray;
        private int Hover_Idx;
        private Punkt Hover_Pos;

        private struct MovePlaneS
        {
            public static byte MousePlaneSelect;
            public static KeyPress PlaneChangeKey;
            public static void Update()
            {
                if (PlaneChangeKey.Check())
                {
                    MousePlaneSelect++;
                    if (MousePlaneSelect > 3)
                        MousePlaneSelect = 0;
                }
            }
            public static string ToStringStatic()
            {
                string str = "";
                str += "switch move plane [" + PlaneChangeKey + "]\n";
                str += "plane ";
                if (MousePlaneSelect == 0) { str += "none"; }
                if (MousePlaneSelect == 1) { str += "CY"; }
                if (MousePlaneSelect == 2) { str += "YX"; }
                if (MousePlaneSelect == 3) { str += "XC"; }
                str += "\n";
                return str;
            }



            public Punkt Origin;

            public void Reset()
            {
                Origin = null;
            }
            public Punkt Cross(Ray ray)
            {
                if (Origin == null)
                    return null;

                if (MousePlaneSelect != 0)
                {
                    double t = double.NaN;

                    if (MousePlaneSelect == 1)
                        t = ray.Plane_Schnitt_Interval(Origin, new Punkt(0, 0, 1), new Punkt(1, 0, 0));
                    if (MousePlaneSelect == 2)
                        t = ray.Plane_Schnitt_Interval(Origin, new Punkt(1, 0, 0), new Punkt(0, 1, 0));
                    if (MousePlaneSelect == 3)
                        t = ray.Plane_Schnitt_Interval(Origin, new Punkt(0, 1, 0), new Punkt(0, 0, 1));

                    if (Ray.IsScalar(t))
                        return ray.Scale(t);
                }
                return +Origin;
            }
            public BoundBox Box(Punkt pos)
            {
                if (Origin != null)
                    return BoundBox.MinMax(pos, Origin);
                return BoundBox.MinMax(pos, pos);
            }
        }
        private struct MoveDragSingle
        {
/*
    when clicking on an object, move that object on a plane
    when no plane is selected
 */

            public int Index;
            public Punkt Origin;
            public Punkt Target;
            public Punkt Plane;

            //public byte MousePlaneSelect;
            //public KeyPress PlaneChangeKey;

            public void reset()
            {
                Index = -1;
                Origin = null;
                Target = null;
                Plane = null;
            }
            public bool isMoving()
            {
                return Index != -1;
            }
        };
        private MovePlaneS MovePlane;
        private Punkt PlaneCross;
        private KeyToggle MoveLockKey;
        private BoundBox MoveBox;
        private BoxBuffer MoveBox_Buff;

        private List<BodyStatic> Body_List;
        struct BodyInstance
        {
            public static GameSceneEditor Editor;

            public int Index;
            public Transformation Trans;

            public bool isSelected;

            public BodyInstance(int idx, Transformation trans)
            {
                Index = idx;
                Trans = trans;
                //isMove = false;
                //relMove = null;
                isSelected = false;
            }

            public Transformation MovedTrans()
            {
                //if (isMove)
                //    return Editor.MoveTarget.TBack(relMove).TBack(Trans);
                //else
                //    return Trans;
                return Trans;
            }
            public double Intersekt(Ray ray)
            {
                return Editor.Body_List[Index].Intersekt(ray, MovedTrans(), out _);
            }
            public void Draw()
            {
                Editor.Draw_Prog_Fill.UniTrans(new RenderTrans(MovedTrans()));
                Editor.Body_List[Index].BufferDraw();

                if (isSelected)
                {
                    BoundBox box = Editor.Body_List[Index].BoxFit().Shift(Trans.Pos);

                    //Editor.Box_Prog_Axis.UniView(new RenderTrans(Editor.view.Trans));
                    Editor.view.UniTrans(Editor.Box_Prog_Axis);
                    Editor.MoveBox_Buff.Data(new BoundBox.BoxRenderData[] { new BoundBox.BoxRenderData(box, 0) });
                    Editor.MoveBox_Buff.Draw();
                }
            }

            public void SelectToggle()
            {
                isSelected = !isSelected;
            }
        }
        private List<BodyInstance> Instance_List;

        public GameSceneEditor(Action externDelete, Func<string> func_load, Func<string> func_save) : base(externDelete)
        {
            File_Load_Func = func_load;
            File_Save_Func = func_save;

            Text_Prog = new TextProgram("Text",
                "Text/Text.vert",
                "Text/Text.geom",
                "Frag/Direct.frag");
            Text_Buff = new TextBuffers();

            Draw_Prog_Fill = new TransUniProgram("Draw Fill",
                "Vert_Unif_noCol.vert",
                "Geom_Norm_Color.geom",
                "Frag/Light_Depth.frag");

            Box_Prog_Axis = new BoxProgram("Box",
                "Box/Box_Fixed.vert",
                "Box/Box_Fixed_Axis.geom",
                "Frag/Direct.frag");
            MoveBox_Buff = new BoxBuffer();
        }

        private int CalcHoverIdx(out double scl)
        {
            scl = double.PositiveInfinity;
            int idx = -1;

            double t;
            for (int i = 0; i < Instance_List.Count; i++)
            {
                t = Instance_List[i].Intersekt(View_Ray);
                if (t < scl)
                {
                    scl = t;
                    idx = i;
                }
            }

            if (idx == -1)
                scl = double.NaN;
            return idx;
        }
        private void UpdateView()
        {
            TMovement.FlatX(ref view.Trans, win.VelPos_Key(), win.VelRot_Mouse());
            view.Update();

            (float, float) mouseWin = win.MouseFromMiddle();
            mouseWin.Item1 /= 500.0f;
            mouseWin.Item2 /= 500.0f;
            Punkt mouse = new Punkt(mouseWin.Item1, mouseWin.Item2, 1);
            mouse = mouse - view.Trans.Rot;
            View_Ray = new Ray(view.Trans.Pos, mouse);
        }
        private void UpdateMove()
        {
            MovePlaneS.Update();

            //  update select
            {
                if (Hover_Idx != -1 && win.Mouse_L.CheckClickD())
                {
                    BodyInstance inst = Instance_List[Hover_Idx];
                    inst.SelectToggle();
                    Instance_List[Hover_Idx] = inst;
                }
            }

            //  update move plane
            {
                MovePlane.Origin = new Punkt();
                PlaneCross = MovePlane.Cross(View_Ray);

                if (win.Mouse_L.CheckClickD())
                {

                }

                if (win.Mouse_L.CheckHold())
                {
                    if (!MoveLockKey.Check())
                    {
                        PlaneCross = MovePlane.Cross(View_Ray);
                    }
                }

                if (win.Mouse_L.CheckClickU())
                {

                }
            }

            {
                string move_str = "";
                move_str += MovePlaneS.ToStringStatic();
                move_str += "lock move [" + MoveLockKey + "]\n";
                if (MoveLockKey.Check()) { move_str += "locked\n"; }
                Text_Buff.Insert(TextBuffers.Corner.TopLef, 0, 0, 0xFFFFFF, move_str);
            }

            if (PlaneCross != null)
            {
                Draw_Prog_Fill.UniTrans(new RenderTrans(PlaneCross));
                Body_List[0].BufferDraw();

                //Box_Prog_Axis.UniView(new RenderTrans(view.Trans));
                view.UniTrans(Box_Prog_Axis);
                MoveBox = MovePlane.Box(PlaneCross);
                MoveBox_Buff.Data(new BoundBox.BoxRenderData[] { new BoundBox.BoxRenderData(MoveBox, 0) });
                MoveBox_Buff.Draw();
            }
        }
        protected override void Frame()
        {
            //if (Key_Scene_Load.Check())
            //    File_Load_Name = File_Load_Func();
            //if (Key_Scene_Save.Check())
            //    File_Save_Name = File_Save_Func();

            UpdateView();

            {
                view.UniTrans(Draw_Prog_Fill);
                Draw_Prog_Fill.UniTrans(new RenderTrans(View_Ray.Scale(2.0), new Winkl(View_Ray.Dir))); View_Ray_Body.BufferDraw();
                Draw_Prog_Fill.UniTrans(new RenderTrans(View_Ray.Scale(2.2), new Winkl(View_Ray.Dir))); View_Ray_Body.BufferDraw();
                Draw_Prog_Fill.UniTrans(new RenderTrans(View_Ray.Scale(2.4), new Winkl(View_Ray.Dir))); View_Ray_Body.BufferDraw();

                Hover_Idx = CalcHoverIdx(out double t);
                Hover_Pos = View_Ray.Scale(t);
            }

            UpdateMove();

            //win.UText.BufferFill(Text_Buff);
            {
                //string file_str = "";
                //file_str += "\nLoad: " + File_Load_Name + ":" + Key_Scene_Load.Check();
                //file_str += "\nSave: " + File_Save_Name + ":" + Key_Scene_Save.Check();
                //file_str += "\n" + view.ToString();
                //file_str += "\nPlane: " + MovePlaneSelect + ":" + MovePlaneChange.Check();
                //Text_Buff.Insert(TextBuffers.Corner.TopLef, 0, 0, 0xFFFFFF, file_str);
            }

            {
                //Draw_Prog_Fill.UniView(new RenderTrans(view.Trans));
                view.UniTrans(Draw_Prog_Fill);
                for (int i = 0; i < Instance_List.Count; i++)
                    Instance_List[i].Draw();
            }

            {
                string str = "";
                str += "selected:\n";
                for (int i = 0; i < Instance_List.Count; i++)
                    str += Instance_List[i].isSelected + "[" + i + "]" + "\n";
                Text_Buff.Insert(TextBuffers.Corner.TopRig, 0, 0, 0xFFFFFF, str, false);
            }



            Text_Prog.Use();
            Text_Buff.Fill_Strings();
            Text_Buff.Draw();
        }

        public override void Create()
        {
            if (Running) { return; }
            Running = true;

            ConsoleLog.Log("Create SceneEditor");
            ConsoleLog.TabInc();
            base.Create();

            Key_Scene_Load = new KeyPress(OpenTK.Windowing.GraphicsLibraryFramework.Keys.F5);
            Key_Scene_Save = new KeyPress(OpenTK.Windowing.GraphicsLibraryFramework.Keys.F6);
            win.KeyChecks.Add(Key_Scene_Load);
            win.KeyChecks.Add(Key_Scene_Save);



            Text_Prog.Create();
            Text_Buff.Create();
            Text_Buff.Fill_Pallets();

            Draw_Prog_Fill.Create();
            Draw_Prog_Fill.UniSolar(new Punkt(1, 2, 3));
            //Draw_Prog_Fill.UniProj(new RenderDepthFactors(view.DepthN, view.DepthF), view.Fov);
            view.UniDepth(Draw_Prog_Fill);

            Box_Prog_Axis.Create();
            //Box_Prog_Axis.UniProj(new RenderDepthFactors(view.DepthN, view.DepthF), view.Fov);
            view.UniDepth(Box_Prog_Axis);




            MovePlaneS.PlaneChangeKey = new KeyPress(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Q);
            win.KeyChecks.Add(MovePlaneS.PlaneChangeKey);
            MovePlane = new MovePlaneS();
            MovePlane.Reset();

            MoveLockKey = new KeyToggle(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W);
            win.KeyChecks.Add(MoveLockKey);
            MoveBox_Buff.Create();



            View_Ray_Body = BodyStatic.File.Load("E:/Programmieren/Spiel Zeug/3D/RingC.txt");
            View_Ray_Body.BufferCreate();
            View_Ray_Body.BufferFill();
            Body_List = new List<BodyStatic>();
            Body_List.Add(BodyStatic.File.Load("E:/Programmieren/Spiel Zeug/3D/Meta/AxisCross.txt"));
            Body_List.Add(BodyStatic.File.Load("E:/Programmieren/Spiel Zeug/3D/Meta/Box_Hex.txt"));
            for (int i = 0; i < Body_List.Count; i++)
                BodyStatic.BufferCreate(Body_List[i]);

            BodyInstance.Editor = this;
            Instance_List = new List<BodyInstance>();
            Instance_List.Add(new BodyInstance(1, new Transformation(new Punkt(0, 0, 0))));
            Instance_List.Add(new BodyInstance(1, new Transformation(new Punkt(10, 0, 10))));

            ConsoleLog.TabDec();
            ConsoleLog.Log("");
        }
        public override void Delete()
        {
            if (!Running) { return; }
            Running = false;

            ConsoleLog.Log("Delete SceneEditor");
            ConsoleLog.TabInc();
            base.Delete();



            Key_Scene_Load = null;
            Key_Scene_Save = null;

            Text_Prog.Delete();
            Text_Buff.Delete();

            Draw_Prog_Fill.Delete();

            Box_Prog_Axis.Delete();


            MovePlaneS.PlaneChangeKey = null;
            MoveBox_Buff.Delete();



            View_Ray_Body.BufferDelete();
            for (int i = 0; i < Body_List.Count; i++)
                BodyStatic.BufferDelete(Body_List[i]);
            Body_List = null;
            Instance_List = null;



            ConsoleLog.TabDec();
            ConsoleLog.Log("");
        }
    }
}

/*
Seperate Program:
Save/Load Scene
    [i]C:path/to.file
        the first time a number is used, it specified what file to use for a Object
    [i] 0.0 +1 -2 0.0 0.0 0.0
        the next times specify where to place the Object

look at Scene
    move / look around
    light

modify Scene
    click on Object to select it
    hold/toggle button to select multiple
    select Object from global list
    show selected
        full-bright
        blink
        bounding box

    transform Object
        hold button to show move-cross (axis-cross)
        click and drag on an axis to move Oject in that direction
        left(right) mouse to move in this axis
        right(left) mouse to move on plane of other 2 axis

    add/Remove Object
        Select File for Object

    reload Scene

Mouse:
    click select
    drag
    scroll

Buttons:
    [Tab]   toggle to move View
    [Shift] hold to select multi
    toggle to show global list
    hold to show move-cross
    press to add file
    press to (re)load
    press to save
*/