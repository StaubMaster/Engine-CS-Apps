using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.OutPut;
using Engine3D.Graphics;
using Engine3D.Graphics.Display;
using Engine3D.BodyParse;
using Engine3D.Miscellaneous;
using Engine3D.Graphics.Display3D;
using Engine3D.Graphics.Display2D.UserInterface;
using Engine3D.Graphics.Manager;
using Engine3D.DataStructs;

using OpenTK.Windowing.GraphicsLibraryFramework;

using VoidFactory.Miscellaneous.Display;

namespace VoidFactory.Editor
{
    static class EnumFunctions
    {
        public enum EShaderDisplayState
        {
            Default = 0,
            SelectHoverMono = 1,
            SelectHoverMult = 2,
        }
        public static EShaderDisplayState Next(this EShaderDisplayState state)
        {
            switch (state)
            {
                case EShaderDisplayState.Default: return EShaderDisplayState.SelectHoverMono;
                case EShaderDisplayState.SelectHoverMono: return EShaderDisplayState.SelectHoverMult;
                case EShaderDisplayState.SelectHoverMult: return EShaderDisplayState.Default;
                default: return state;
            }
        }
    }
    class EditorPolySoma
    {
        /*
            Select Single
                Show Indicat for Selected
            nothing Selected
                no Indicator

            if something done with Indicator
                show "shadow" of Selected

            if ctrl is held
                round to nearest
        */
        /*
            Update Hover Body
            if selected
                Update Hover Trans

            if press
                if hovering trans
                    do trans
                if hovering body
                    change selected to hovering
                if hovering nothing
                    change selected to nothing
        */

        /*  TODO
            reset stuff when loading new scene
            scene load (better)
            scene save
            scene reload
            object insert   (crate)
            object remove   (blender/shredder)
        */

        private DisplayArea MainWindow;
        private DisplayCamera MainCamera;

        private PolyHedra_Shader_Manager PH_3D_Man;
        private UserInterfaceManager PH_UI_Man;

        private UIBody_Array UI_Array;

        private TextShader Text_Shader;
        private TextBuffer Text_Buffer;

        private int Edit_Index_Hovering;
        private int Edit_Index_Selected;

        private EnumFunctions.EShaderDisplayState ShaderDisplayState;

        /*struct SEditorBody
        {
            public DisplayBody Body;

            public bool isSelected;

            public SEditorBody(CScene.SSceneObject obj)
            {
                //Body = new DisplayBody(obj.Body, obj.Buffer, obj.Trans);

                isSelected = false;
            }
            public SEditorBody(string path)
            {
                Body = new DisplayBody(path);

                isSelected = false;
            }

            public Intersekt.RayInterval Intersekt(Ray3D ray)
            {
                return Body.Intersekt(ray);
            }

            public static (float, float, float) ColorSelectHover(bool isHover, bool isSelected)
            {
                if (!isHover)
                {
                    if (!isSelected)
                    {
                        return (1.0f, 1.0f, 1.0f);
                    }
                    else
                    {
                        return (0.5f, 1.0f, 0.5f);
                    }
                }
                else
                {
                    if (!isSelected)
                    {
                        return (0.0f, 0.5f, 1.0f);
                    }
                    else
                    {
                        return (1.0f, 0.5f, 0.0f);
                    }
                }
            }

            public void Draw(BodyElemUniShader shader)
            {
                Body.Draw(shader);
            }
            public void Draw(BodyElemUniWireShader shader)
            {
                Body.Draw(shader);
            }
        }
        List<SEditorBody> EditorBodys;*/

        private static (float, float, float) ColorSelectHover(bool isHover, bool isSelected)
        {
            if (!isHover)
            {
                if (!isSelected)
                {
                    return (1.0f, 1.0f, 1.0f);
                }
                else
                {
                    return (0.5f, 1.0f, 0.5f);
                }
            }
            else
            {
                if (!isSelected)
                {
                    return (0.0f, 0.5f, 1.0f);
                }
                else
                {
                    return (1.0f, 0.5f, 0.0f);
                }
            }
        }



        string FilePath;
        PolySoma Scene;



        struct AuxData
        {
            public bool Selected;
            public bool Hovering;

            public static AuxData Default()
            {
                AuxData data = new AuxData();
                data.Selected = false;
                data.Hovering = false;
                return data;
            }

            public ColorUData Color()
            {
                ColorUData col = new ColorUData();

                if (!Selected)
                {
                    if (!Hovering) { col.RGB = 0xFFFFFF; }
                    else { col.RGB = 0x007FFF; }
                }
                else
                {
                    if (!Hovering) { col.RGB = 0x7FFF7F; }
                    else { col.RGB = 0xFF7F00; }
                }

                return col;
            }
            public void ToggleSelectHover()
            {
                if (Hovering)
                {
                    Selected = !Selected;
                }
            }
        }
        List<AuxData> Aux;
        private void Aux_Default_All()
        {
            for (int i = 0; i < Aux.Count; i++)
            {
                Aux[i] = AuxData.Default();
            }
        }
        private void Aux_Change_Color(int idx)
        {
            PolyHedraInstance_3D_Data data;
            data = Scene.PHs_Trans[idx][0];

            data.AltColorLInter = LInterData.LIT1();
            data.AltColor = Aux[idx].Color();

            Scene.PHs_Trans[idx][0] = data;
        }
        private void Aux_Change_Color_All()
        {
            PH_3D_Man.LightRange.ChangeData(new RangeData(1.0f, 1.0f));

            for (int i = 0; i < Scene.PHs_Trans.Count; i++)
            {
                Aux_Change_Color(i);
            }
        }

        private void Aux_UnSelect_All()
        {
            AuxData aux_data;
            for (int i = 0; i < Aux.Count; i++)
            {
                aux_data = Aux[i];
                aux_data.Selected = false;
                Aux[i] = aux_data;
            }
        }
        private void Aux_Hover(int idx, bool val)
        {
            AuxData aux_data;
            aux_data = Aux[idx];
            aux_data.Hovering = val;
            Aux[idx] = aux_data;
        }
        private void Aux_Toggle_Hover()
        {
            AuxData aux_data;
            for (int i = 0; i < Aux.Count; i++)
            {
                aux_data = Aux[i];
                aux_data.ToggleSelectHover();
                Aux[i] = aux_data;
            }
        }



        ChangeMouseDrag3D Edit_Change_Mouse;

        UI_Indicator UI_Indicator_FanS;
        UI_Indicator UI_Indicator_FanR;
        UI_Indicator UI_Indicator_Disk;
        UI_Indicator UI_Indicator_Fold;
        UI_Indicator UI_Indicator_Test;
        UI_Indicator UI_Indicator_ObjectAdd;
        UI_Indicator UI_Indicator_ObjectSub;

        UI_Indicator[] UI_Indicator_MouseDragMain;
        UI_Indicator[] UI_Indicator_MouseDragSnap;
        bool Hovering_UI_Indicator;



        private void Mouse_Hover_Find()
        {
            AuxData aux_data;

            if (Edit_Index_Hovering != -1)
            {
                aux_data = Aux[Edit_Index_Hovering];
                aux_data.Hovering = false;
                Aux[Edit_Index_Hovering] = aux_data;
            }

            Edit_Index_Hovering = -1;

            if (Hovering_UI_Indicator) { return; }

            //Intersekt.RayInterval inter = Scene.Intersekt(MainCamera.Ray, out Edit_Index_Hovering);

            double HoverDistance = double.PositiveInfinity;
            for (int i = 0; i < Scene.PHs_Trans.Count; i++)
            {
                Intersekt.RayInterval inter = Scene.PHs_Trans[i].Intersekt(MainCamera.Ray, 0);
                if (inter.Is)
                {
                    if (inter.Interval < HoverDistance)
                    {
                        HoverDistance = inter.Interval;
                        Edit_Index_Hovering = i;
                    }
                }
            }

            if (Edit_Index_Hovering != -1)
            {
                aux_data = Aux[Edit_Index_Hovering];
                aux_data.Hovering = true;
                Aux[Edit_Index_Hovering] = aux_data;
            }
        }



        private void Update_View()
        {
            TMovement.FlatX(ref MainCamera.Trans, MainWindow.MoveByKeys(0.1f, 10), MainWindow.SpinByMouse());
            MainCamera.Update(MainWindow.MouseRay());
        }
        private void Update_Keys()
        {
            if (MainWindow.CheckKey(Keys.F1).IsPressed()) { Edit_Change_Mouse.CycleMove(); }
            if (MainWindow.CheckKey(Keys.F2).IsPressed()) { Edit_Change_Mouse.SnapToggleMove(); }
            if (MainWindow.CheckKey(Keys.F3).IsPressed()) { Edit_Change_Mouse.CycleSpin(); }
            if (MainWindow.CheckKey(Keys.F4).IsPressed()) { Edit_Change_Mouse.SnapToggleSpin(); }
            if (MainWindow.CheckKey(Keys.F5).IsPressed())
            {
                ShaderDisplayState = ShaderDisplayState.Next();
            }
        }
        private void Update_Mouse_Change()
        {
            if (!Edit_Change_Mouse.IsSelected)
            {
                Mouse_Hover_Find();
            }

            if (MainWindow.CheckKey(MouseButton.Left).IsPressed() && !Hovering_UI_Indicator)
            {
                if (Edit_Change_Mouse.IsHovering)
                {
                    Edit_Change_Mouse.HoverSelect();
                }
                else
                {
                    Aux_UnSelect_All();
                    Aux_Toggle_Hover();

                    Edit_Index_Selected = Edit_Index_Hovering;
                    if (Edit_Index_Selected != -1)
                    {
                        //Edit_Change_Mouse.Trans_Calc(Scene.AllBodysOld[Edit_Index_Selected].Trans);
                        Edit_Change_Mouse.Trans_Calc(Scene.PHs_Trans[Edit_Index_Selected][0].Trans);
                    }
                    else
                    {
                        Edit_Change_Mouse.Trans_Calc(Transformation3D.Null());
                    }
                    Edit_Change_Mouse.Change_Reset();
                }
                //if (Edit_Index_Hovering != -1)
                //{
                //    SEditorBody temp = EditorBodys[Edit_Index_Hovering];
                //    temp.isSelected = !temp.isSelected;
                //    EditorBodys[Edit_Index_Hovering] = temp;
                //}
            }
            if (MainWindow.CheckKey(MouseButton.Left).IsReleased())
            {
                if (Edit_Change_Mouse.Trans_Changed.Is())
                {
                    Edit_Change_Mouse.Trans_Calc(Edit_Change_Mouse.Trans_Changed);

                    //DisplayBody temp = Scene.AllBodysOld[Edit_Index_Selected];
                    //temp.Trans = Edit_Change_Mouse.Trans_Changed;
                    //Scene.AllBodysOld[Edit_Index_Selected] = temp;

                    PolyHedraInstance_3D_Data data;
                    data = Scene.PHs_Trans[Edit_Index_Selected][0];
                    data.Trans = Edit_Change_Mouse.Trans_Changed;
                    Scene.PHs_Trans[Edit_Index_Selected][0] = data;
 
                    //Edit_Change_Mouse.Trans_Calc(Scene.AllBodysOld[Edit_Index_Selected].Trans);
                }
                Edit_Change_Mouse.Change_Reset();
            }

            if (Edit_Index_Selected != -1)
            {
                Edit_Change_Mouse.ViewRay = MainCamera.Ray;
                Edit_Change_Mouse.Changed_Trans_Calc();
                Edit_Change_Mouse.Indicator_Trans_Calc();
                Edit_Change_Mouse.Hover_Find();
            }
        }
        private void Update_UI()
        {
            (float, float) winSize = MainWindow.Size_Float2();
            UI_Indicator_Disk.UpdateSize(winSize.Item1, winSize.Item2);
            UI_Indicator_Fold.UpdateSize(winSize.Item1, winSize.Item2);
            UI_Indicator_Test.UpdateSize(winSize.Item1, winSize.Item2);
            UI_Indicator_ObjectAdd.UpdateSize(winSize.Item1, winSize.Item2);
            UI_Indicator_ObjectSub.UpdateSize(winSize.Item1, winSize.Item2);

            (float, float) MousePixel = MainWindow.MousePixel();
            UI_Indicator_Disk.UpdateHover(MousePixel.Item1, MousePixel.Item2);
            UI_Indicator_Fold.UpdateHover(MousePixel.Item1, MousePixel.Item2);
            UI_Indicator_Test.UpdateHover(MousePixel.Item1, MousePixel.Item2);
            UI_Indicator_ObjectAdd.UpdateHover(MousePixel.Item1, MousePixel.Item2);
            UI_Indicator_ObjectSub.UpdateHover(MousePixel.Item1, MousePixel.Item2);

            Hovering_UI_Indicator = false;
            if (UI_Indicator_Disk.isHover) { Hovering_UI_Indicator = true; }
            if (UI_Indicator_Fold.isHover) { Hovering_UI_Indicator = true; }
            if (UI_Indicator_Test.isHover) { Hovering_UI_Indicator = true; }
            if (UI_Indicator_ObjectAdd.isHover) { Hovering_UI_Indicator = true; }
            if (UI_Indicator_ObjectSub.isHover) { Hovering_UI_Indicator = true; }

            UI_Array.Update();
        }



        private void Draw_Mouse_Change()
        {
            /*if (Edit_Index_Selected != -1)
            {
                BodyUniWire_Shader.Use();
                BodyUniWire_Shader.Trans.Value(Edit_Change_Mouse.Trans_Changed);
                Scene.AllBodysOld[Edit_Index_Selected].Draw(BodyUniWire_Shader);
            }*/

            MainWindow.ClearBufferDepth();
            PH_3D_Man.InstShader.Use();
            Edit_Change_Mouse.Draw();
        }
        private void Draw_UI()
        {
            UI_Indicator_FanS.ChangeTrans();
            UI_Indicator_FanR.ChangeTrans();
            UI_Indicator_Disk.ChangeTrans();
            UI_Indicator_Fold.ChangeTrans();
            UI_Indicator_Test.ChangeTrans();
            UI_Indicator_ObjectAdd.ChangeTrans();
            UI_Indicator_ObjectSub.ChangeTrans();
            Edit_Change_Mouse.DrawUI(MainCamera.Trans, UI_Indicator_MouseDragMain, UI_Indicator_MouseDragSnap);

            MainWindow.ClearBufferDepth();
            PH_UI_Man.ScreenRatio.ChangeData(MainWindow.SizeRatio());
            PH_UI_Man.UIBodyShader.Use();
            UI_Array.Draw();
        }



        Func<string> Func_Load;
        Func<string> Func_Save;

        private void Frame_Func()
        {
            (float, float) winSize = MainWindow.Size_Float2();

            Update_View();
            Update_Keys();
            Update_UI();
            Update_Mouse_Change();

            if (MainWindow.CheckKey(MouseButton.Left).IsPressed())
            {
                if (UI_Indicator_Disk.isHover)
                {
                    Scene_Save();
                }
                if (UI_Indicator_Fold.isHover)
                {
                    Scene_Load();
                }
                if (UI_Indicator_Test.isHover)
                {
                    ConsoleLog.Log("Reset FilePath");
                    FilePath = null;
                }
                if (UI_Indicator_ObjectAdd.isHover)
                {
                    Scene.Edit_Insert_PolyAbs(Func_Load());
                }
                if (UI_Indicator_ObjectSub.isHover)
                {
                    Scene.Edit_Remove_Body(Edit_Index_Selected);
                    Edit_Index_Hovering = -1;
                    Edit_Index_Selected = -1;
                    Edit_Change_Mouse.Trans_Calc(Transformation3D.Null());
                    Edit_Change_Mouse.Change_Reset();
                }
            }

            //Draw_Scene();
            {
                Aux_Change_Color_All();
                Scene.Update();

                PH_3D_Man.ViewPortSizeRatio.ChangeData(new SizeRatio(winSize.Item1, winSize.Item2));
                PH_3D_Man.View.ChangeData(MainCamera.Trans);

                PH_3D_Man.InstShader.Use();
                Scene.Draw();

                PH_3D_Man.InstWireShader.Use();
                Scene.Draw();
            }
            Draw_Mouse_Change();

            //MainWindow.ClearBufferDepth();
            //BodyUniFull_Shader.Use();
            //BodyUniFull_Shader.OtherColorInter.T0(1.0f);
            //BodyUniFull_Shader.Trans.Value(new Transformation(MainCamera.Ray.Scale(10.0), new Winkl(MainCamera.Ray.Dir)));
            //Indicator_MouseDragMain[0].Buffer.Draw();
            //Indicator_MouseDragMain[1].Buffer.Draw();
            //Indicator_MouseDragMain[5].Buffer.Draw();

            Draw_UI();

            {
                string str = "";
                /*str += "Display State: " + ShaderDisplayState.ToString();
                if (ShaderDisplayState == EnumFunctions.EShaderDisplayState.SelectHoverMono)
                {
                    str += "\nSelected Index: " + Edit_Index_Selected;
                    if (Edit_Index_Selected != -1)
                    {
                        DisplayBody body = Scene.AllBodysOld[Edit_Index_Selected];
                        Transformation3D trans = body.Trans;
                        AxisBox3D box = body.Body.BoxFit();

                        str += "\nPos: " + UnitToString.Metric(trans.Pos);
                        //str += "\nRot: " + trans.Rot;

                        str += "\nMin: " + UnitToString.Metric(box.Min);
                        str += "\nMax: " + UnitToString.Metric(box.Max);
                        str += "\nScl: " + UnitToString.Metric(box);

                        str += "\nCorn:Face: " + body.Body.PHedra.CornerCount() + ":" + body.Body.PHedra.FaceCount();
                    }
                }*/
                str += Edit_Change_Mouse.ToInfo();
                Text_Buffer.InsertTL((0, -5), Text_Buffer.Default_TextSize, 0x000000, str);
            }

            MainWindow.ClearBufferDepth();

            Text_Shader.Use();
            Text_Shader.ScreenRatio.Value(MainWindow.Size_Float2());
            Text_Buffer.Bind_Strings();
            Text_Buffer.Draw();
        }

        public void Scene_Load()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                FilePath = Func_Load();
            }

            Edit_Index_Hovering = -1;
            Edit_Index_Selected = -1;
            Edit_Change_Mouse.Trans_Calc(Transformation3D.Null());
            Edit_Change_Mouse.Change_Reset();

            ConsoleLog.LogLoad("Scene " + FilePath + " ...");
            Scene = PolySoma.Parse.LoadFile(FilePath);
            Scene.Edit_Begin();
            Aux = new List<AuxData>();
            for (int i = 0; i < Scene.PHs_Trans.Count; i++) { Aux.Add(AuxData.Default()); }

            Aux_Change_Color_All();
        }
        public void Scene_Save()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                FilePath = Func_Save();
            }

            ConsoleLog.LogSave("Scene " + FilePath + " ...");
            System.IO.File.WriteAllText(FilePath, Scene.ToYMT());
        }

        

        public void Init_Graphics()
        {
            string shaderDir = "E:/Programmieren/VS_Code/OpenTK/Engine3D/Engine3D/Shaders/";

            PH_3D_Man = new PolyHedra_Shader_Manager(shaderDir);
            PH_3D_Man.Depth.ChangeData(MainCamera.Depth);

            PH_UI_Man = new UserInterfaceManager(shaderDir);
            PH_UI_Man.LightRange.ChangeData(new RangeData(1.0f, 1.0f));

            ShaderDisplayState = EnumFunctions.EShaderDisplayState.SelectHoverMono;

            Text_Buffer = new TextBuffer();
            Text_Buffer.Bind_Pallets();
            Text_Shader = new TextShader(shaderDir);
        }
        public void Init_Mouse_Edit(string dir)
        {
            string dirMove = dir + "Meta/MoveAxis/";
            string dirSpin = dir + "Meta/SpinRing/";

            ConsoleLog.LogDebug("now Loading CBody(s) for Edit_Change_Mouse");
            Edit_Change_Mouse = new ChangeMouseDrag3D(new PolyHedra[]
            {
                TBodyFile.LoadTextFile(dirMove + "AxisY.txt"),
                TBodyFile.LoadTextFile(dirMove + "AxisX.txt"),
                TBodyFile.LoadTextFile(dirMove + "AxisC.txt"),

                TBodyFile.LoadTextFile(dirSpin + "RingY.txt"),
                TBodyFile.LoadTextFile(dirSpin + "RingX.txt"),
                TBodyFile.LoadTextFile(dirSpin + "RingC.txt"),
            });
            ConsoleLog.NewLine();
        }
        public void Init_UI(string dir)
        {
            ConsoleLog.LogDebug("now Loading CBody(s) for Indicator(s)");

            string dirMeta = dir + "Meta/";
            string dirMove = dir + "Meta/MoveAxis/";
            string dirSpin = dir + "Meta/SpinRing/";
            string dirPicto = dir + "Meta/Picto/";

            PolyHedra[] ph = new PolyHedra[]
            {
                PolyHedra.FromTextFile(dirPicto + "ComputerFanStator.ymt"),
                PolyHedra.FromTextFile(dirPicto + "ComputerFanRotor.ymt"),
                PolyHedra.FromTextFile(dirPicto + "OpticalDisk.ymt"),
                PolyHedra.FromTextFile(dirPicto + "RingFolder.ymt"),
                PolyHedra.FromTextFile(dirPicto + "Test.ymt"),
                PolyHedra.FromTextFile(dirMeta + "Inn.txt"),
                PolyHedra.FromTextFile(dirMeta + "Out.txt"),

                PolyHedra.FromTextFile(dirMove + "AxisY.txt"),
                PolyHedra.FromTextFile(dirMove + "AxisX.txt"),
                PolyHedra.FromTextFile(dirMove + "AxisC.txt"),
                PolyHedra.FromTextFile(dirSpin + "RingY.txt"),
                PolyHedra.FromTextFile(dirSpin + "RingX.txt"),
                PolyHedra.FromTextFile(dirSpin + "RingC.txt"),

                PolyHedra.FromTextFile(dirMove + "MoveSnap0.txt"),
                PolyHedra.FromTextFile(dirMove + "MoveSnap1.txt"),
                PolyHedra.FromTextFile(dirSpin + "SpinSnap0.txt"),
                PolyHedra.FromTextFile(dirSpin + "SpinSnap1.txt"),
            };
            UI_Array = new UIBody_Array(ph);

            UI_Indicator_FanS = new UI_Indicator(+050, -50, 100, 100, UI_Anchor.YX_MP, ph[0], UI_Array[0].Alloc(1), 0.6f, UI_Indicator.EAnimationType.None);
            UI_Indicator_FanR = new UI_Indicator(+050, -50, 100, 100, UI_Anchor.YX_MP, ph[1], UI_Array[1].Alloc(1), 0.6f, UI_Indicator.EAnimationType.Spin);
            UI_Indicator_Disk = new UI_Indicator(+150, -50, 100, 100, UI_Anchor.YX_MP, ph[2], UI_Array[2].Alloc(1), 1.0f, UI_Indicator.EAnimationType.Disk);
            UI_Indicator_Fold = new UI_Indicator(+250, -50, 100, 100, UI_Anchor.YX_MP, ph[3], UI_Array[3].Alloc(1), 1.6f, UI_Indicator.EAnimationType.Diag);
            UI_Indicator_Test = new UI_Indicator(+350, -50, 100, 100, UI_Anchor.YX_MP, ph[4], UI_Array[4].Alloc(1), 1.0f, UI_Indicator.EAnimationType.Disk);
            UI_Indicator_ObjectAdd = new UI_Indicator(+450, -50, 100, 100, UI_Anchor.YX_MP, ph[5], UI_Array[5].Alloc(1), 1.2f, UI_Indicator.EAnimationType.Disk);
            UI_Indicator_ObjectSub = new UI_Indicator(+550, -50, 100, 100, UI_Anchor.YX_MP, ph[6], UI_Array[6].Alloc(1), 1.2f, UI_Indicator.EAnimationType.Disk);

            UI_Indicator_MouseDragMain = new UI_Indicator[6]
            {
                new UI_Indicator(-050, -50, 100, 100, UI_Anchor.YX_PP, ph[07], UI_Array[07].Alloc(1), 1.0f, UI_Indicator.EAnimationType.View),
                new UI_Indicator(-050, -50, 100, 100, UI_Anchor.YX_PP, ph[08], UI_Array[08].Alloc(1), 1.0f, UI_Indicator.EAnimationType.View),
                new UI_Indicator(-050, -50, 100, 100, UI_Anchor.YX_PP, ph[09], UI_Array[09].Alloc(1), 1.0f, UI_Indicator.EAnimationType.View),
                new UI_Indicator(-150, -50, 100, 100, UI_Anchor.YX_PP, ph[10], UI_Array[10].Alloc(1), 1.0f, UI_Indicator.EAnimationType.View),
                new UI_Indicator(-150, -50, 100, 100, UI_Anchor.YX_PP, ph[11], UI_Array[11].Alloc(1), 1.0f, UI_Indicator.EAnimationType.View),
                new UI_Indicator(-150, -50, 100, 100, UI_Anchor.YX_PP, ph[12], UI_Array[12].Alloc(1), 1.0f, UI_Indicator.EAnimationType.View),
            };

            UI_Indicator_MouseDragSnap = new UI_Indicator[]
            {
                new UI_Indicator(-050, -150, 100, 100, UI_Anchor.YX_PP, ph[13], UI_Array[13].Alloc(1), 1.0f, UI_Indicator.EAnimationType.None),
                new UI_Indicator(-050, -150, 100, 100, UI_Anchor.YX_PP, ph[14], UI_Array[14].Alloc(1), 1.0f, UI_Indicator.EAnimationType.None),
                new UI_Indicator(-150, -150, 100, 100, UI_Anchor.YX_PP, ph[15], UI_Array[15].Alloc(1), 1.0f, UI_Indicator.EAnimationType.None),
                new UI_Indicator(-150, -150, 100, 100, UI_Anchor.YX_PP, ph[16], UI_Array[16].Alloc(1), 1.0f, UI_Indicator.EAnimationType.None),
            };

            ConsoleLog.NewLine();

            Hovering_UI_Indicator = false;
        }

        public EditorPolySoma(Func<string> load, Func<string> save)
        {
            Func_Load = load;
            Func_Save = save;

            ConsoleLog.LogTest("test");
            ConsoleLog.LogError("test");
            ConsoleLog.LogWarning("test");
            ConsoleLog.LogInfo("test");
            ConsoleLog.LogDebug("test");
            ConsoleLog.LogLoad("test");
            ConsoleLog.LogSave("test");

            Edit_Index_Hovering = -1;
            Edit_Index_Selected = -1;

            MainWindow = new DisplayArea(1000, 1000, null, Frame_Func);
            MainCamera = new DisplayCamera();
            MainCamera.Depth = new DepthData(0.1f, 10000.0f);

            FilePath = null;
            Scene = new PolySoma();
            Scene.Edit_Begin();
            Aux = new List<AuxData>();
            Aux_Default_All();

            string dir = "E:/Zeug/YMT/";
            Init_Graphics();
            Init_Mouse_Edit(dir);
            Init_UI(dir);
            //Init_Scene(dir + "RealLifePersonal/Scene.txt");

            MainWindow.Run();
            MainWindow.Term();
        }
    }
}
