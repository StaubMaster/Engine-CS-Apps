using System;
using System.Collections.Generic;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.OutPut;
using Engine3D.Graphics;
using Engine3D.Graphics.Display;
using Engine3D.BodyParse;
using Engine3D.Miscellaneous;

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

        private BodyElemUniShader BodyUniFull_Shader;
        private BodyElemUniWireShader BodyUniWire_Shader;

        private UserInterfaceBodyShader UserInterface_Shader;

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


        PolySoma Scene;
        ArrayList<bool> BodyIsSelected;
        string FilePath;



        ChangeMouseDrag3D Edit_Change_Mouse;

        UI_Indicator Indicator_FanS;
        UI_Indicator Indicator_FanR;
        UI_Indicator Indicator_Disk;
        UI_Indicator Indicator_Fold;
        UI_Indicator Indicator_Test;
        UI_Indicator Indicator_ObjectAdd;
        UI_Indicator Indicator_ObjectSub;

        UI_Indicator[] Indicator_MouseDragMain;
        UI_Indicator[] Indicator_MouseDragSnap;
        bool Hovering_Indicator;



        private void Mouse_Hover_Find()
        {
            Edit_Index_Hovering = -1;
            if (Hovering_Indicator) { return; }

            double HoverDistance = double.PositiveInfinity;
            for (int i = 0; i < Scene.AllBodys.Count; i++)
            {
                Intersekt.RayInterval inter = Scene.AllBodys[i].Intersekt(MainCamera.Ray);
                if (inter.Is)
                {
                    if (inter.Interval < HoverDistance)
                    {
                        HoverDistance = inter.Interval;
                        Edit_Index_Hovering = i;
                    }
                }
            }
        }



        private void Draw_Default()
        {
            BodyUniFull_Shader.Use();
            BodyUniFull_Shader.OtherColorInter.T0(1.0f);
            BodyUniFull_Shader.LightRange.Value(0.1f, 1.0f);

            for (int i = 0; i < Scene.AllBodys.Count; i++)
            {
                Scene.AllBodys[i].Draw(BodyUniFull_Shader);
            }
        }
        private void Draw_Wire()
        {
            BodyUniWire_Shader.Use();
            for (int i = 0; i < Scene.AllBodys.Count; i++)
            {
                Scene.AllBodys[i].Draw(BodyUniWire_Shader);
            }
        }
        private void Draw_SelectHover_Mono()
        {
            BodyUniFull_Shader.Use();
            BodyUniFull_Shader.OtherColorInter.T1(1.0f);
            BodyUniFull_Shader.LightRange.Value(1.0f, 1.0f);

            for (int i = 0; i < Scene.AllBodys.Count; i++)
            {
                BodyUniFull_Shader.OtherColor.Value(ColorSelectHover(i == Edit_Index_Hovering, i == Edit_Index_Selected));
                Scene.AllBodys[i].Draw(BodyUniFull_Shader);
            }
        }
        private void Draw_SelectHover_Mult()
        {
            BodyUniFull_Shader.Use();
            BodyUniFull_Shader.OtherColorInter.T1(1.0f);
            BodyUniFull_Shader.LightRange.Value(1.0f, 1.0f);

            for (int i = 0; i < Scene.AllBodys.Count; i++)
            {
                BodyUniFull_Shader.OtherColor.Value(ColorSelectHover(i == Edit_Index_Hovering, BodyIsSelected[i]));
                Scene.AllBodys[i].Draw(BodyUniFull_Shader);
            }
        }



        private void Update_View()
        {
            TMovement.FlatX(ref MainCamera.Trans, MainWindow.MoveByKeys(0.1, 10), MainWindow.SpinByMouse());
            MainCamera.Update(MainWindow.MouseRay());

            BodyUniFull_Shader.View.Value(MainCamera.Trans);
            BodyUniWire_Shader.View.Value(MainCamera.Trans);
        }
        private void Update_Keys()
        {
            if (MainWindow.CheckKey(Keys.F1).IsPressed()) { Edit_Change_Mouse.CycleMove(); }
            if (MainWindow.CheckKey(Keys.F2).IsPressed()) { Edit_Change_Mouse.SnapToggleMove(); }
            if (MainWindow.CheckKey(Keys.F3).IsPressed()) { Edit_Change_Mouse.CycleSpin(); }
            if (MainWindow.CheckKey(Keys.F4).IsPressed()) { Edit_Change_Mouse.SnapToggleSpin(); }
            if (MainWindow.CheckKey(Keys.F5).IsPressed()) { ShaderDisplayState = ShaderDisplayState.Next(); }
        }
        private void Update_Mouse_Change()
        {
            if (!Edit_Change_Mouse.IsSelected)
            {
                Mouse_Hover_Find();
            }

            if (MainWindow.CheckKey(MouseButton.Left).IsPressed() && !Hovering_Indicator)
            {
                if (Edit_Change_Mouse.IsHovering)
                {
                    Edit_Change_Mouse.HoverSelect();
                }
                else
                {
                    Edit_Index_Selected = Edit_Index_Hovering;
                    if (Edit_Index_Selected != -1)
                    {
                        Edit_Change_Mouse.Trans_Calc(Scene.AllBodys[Edit_Index_Selected].Trans);
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
                /*if (Edit_Change_Mouse.Trans_Changed != null)
                {
                    //Edit_Change_Mouse.Trans_Calc(Edit_Change_Mouse.Trans_Changed);

                    DisplayBody temp = Scene.AllBodys[Edit_Index_Selected];
                    temp.Trans = Edit_Change_Mouse.Trans_Changed;
                    Scene.AllBodys[Edit_Index_Selected] = temp;

                    Edit_Change_Mouse.Trans_Calc(Scene.AllBodys[Edit_Index_Selected].Trans);
                    Edit_Change_Mouse.Change_Reset();
                }*/
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
            Indicator_Disk.UpdateSize(winSize.Item1, winSize.Item2);
            Indicator_Fold.UpdateSize(winSize.Item1, winSize.Item2);
            Indicator_Test.UpdateSize(winSize.Item1, winSize.Item2);
            Indicator_ObjectAdd.UpdateSize(winSize.Item1, winSize.Item2);
            Indicator_ObjectSub.UpdateSize(winSize.Item1, winSize.Item2);

            (float, float) MousePixel = MainWindow.MousePixel();
            Indicator_Disk.UpdateHover(MousePixel.Item1, MousePixel.Item2);
            Indicator_Fold.UpdateHover(MousePixel.Item1, MousePixel.Item2);
            Indicator_Test.UpdateHover(MousePixel.Item1, MousePixel.Item2);
            Indicator_ObjectAdd.UpdateHover(MousePixel.Item1, MousePixel.Item2);
            Indicator_ObjectSub.UpdateHover(MousePixel.Item1, MousePixel.Item2);

            Hovering_Indicator = false;
            if (Indicator_Disk.isHover) { Hovering_Indicator = true; }
            if (Indicator_Fold.isHover) { Hovering_Indicator = true; }
            if (Indicator_Test.isHover) { Hovering_Indicator = true; }
            if (Indicator_ObjectAdd.isHover) { Hovering_Indicator = true; }
            if (Indicator_ObjectSub.isHover) { Hovering_Indicator = true; }
        }



        private void Draw_Scene()
        {
            if (ShaderDisplayState == EnumFunctions.EShaderDisplayState.Default)
            {
                Draw_Default();
            }
            else if (ShaderDisplayState == EnumFunctions.EShaderDisplayState.SelectHoverMono)
            {
                Draw_Wire();
                Draw_SelectHover_Mono();
            }
            else if (ShaderDisplayState == EnumFunctions.EShaderDisplayState.SelectHoverMult)
            {
                Draw_Wire();
                Draw_SelectHover_Mult();
            }
        }
        private void Draw_Mouse_Change()
        {
            /*if (Edit_Change_Mouse.Trans_Changed != null && Edit_Index_Selected != -1)
            {
                BodyUniWire_Shader.Use();
                BodyUniWire_Shader.Trans.Value(Edit_Change_Mouse.Trans_Changed);
                Scene.AllBodys[Edit_Index_Selected].Draw(BodyUniWire_Shader);
            }*/

            MainWindow.ClearBufferDepth();
            BodyUniFull_Shader.Use();
            BodyUniFull_Shader.OtherColorInter.T0(1.0f);
            Edit_Change_Mouse.Indicator_Draw(BodyUniFull_Shader);
        }
        private void Draw_UI()
        {
            MainWindow.ClearBufferDepth();

            UserInterface_Shader.Use();
            UserInterface_Shader.ScreenRatio.Value(MainWindow.Size_Float2());
            Indicator_FanS.Draw(UserInterface_Shader);
            Indicator_FanR.Draw(UserInterface_Shader);
            Indicator_Disk.Draw(UserInterface_Shader);
            Indicator_Fold.Draw(UserInterface_Shader);
            Indicator_Test.Draw(UserInterface_Shader);
            Indicator_ObjectAdd.Draw(UserInterface_Shader);
            Indicator_ObjectSub.Draw(UserInterface_Shader);
            Edit_Change_Mouse.DrawUI(UserInterface_Shader, MainCamera.Trans, Indicator_MouseDragMain, Indicator_MouseDragSnap);
        }



        Func<string> Func_Load;
        Func<string> Func_Save;

        private void Frame_Func()
        {
            BodyUniFull_Shader.ScreenRatio.Value(MainWindow.Size_Float2());
            BodyUniWire_Shader.ScreenRatio.Value(MainWindow.Size_Float2());

            Update_View();
            Update_Keys();
            Update_UI();
            Update_Mouse_Change();

            if (MainWindow.CheckKey(MouseButton.Left).IsPressed())
            {
                if (Indicator_Disk.isHover)
                {
                    Scene_Save();
                }
                if (Indicator_Fold.isHover)
                {
                    Scene_Load();
                }
                if (Indicator_Test.isHover)
                {
                    ConsoleLog.Log("Reset FilePath");
                    FilePath = null;
                }
                if (Indicator_ObjectAdd.isHover)
                {
                    Scene.Edit_Insert_PolyAbs(Func_Load());
                }
                if (Indicator_ObjectSub.isHover)
                {
                    Scene.Edit_Remove_Body(Edit_Index_Selected);
                    Edit_Index_Hovering = -1;
                    Edit_Index_Selected = -1;
                    Edit_Change_Mouse.Trans_Calc(Transformation3D.Null());
                    Edit_Change_Mouse.Change_Reset();
                }
            }

            Draw_Scene();
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
                str += "Display State: " + ShaderDisplayState.ToString();
                if (ShaderDisplayState == EnumFunctions.EShaderDisplayState.SelectHoverMono)
                {
                    str += "\nSelected Index: " + Edit_Index_Selected;
                    if (Edit_Index_Selected != -1)
                    {
                        DisplayBody body = Scene.AllBodys[Edit_Index_Selected];
                        Transformation3D trans = body.Trans;
                        AxisBox3D box = body.Body.BoxFit();

                        str += "\nPos: " + UnitToString.Metric(trans.Pos);
                        //str += "\nRot: " + trans.Rot;

                        str += "\nMin: " + UnitToString.Metric(box.Min);
                        str += "\nMax: " + UnitToString.Metric(box.Max);
                        str += "\nScl: " + UnitToString.Metric(box);

                        str += "\nCorn:Face: " + body.Body.PHedra.CornerCount() + ":" + body.Body.PHedra.FaceCount();
                    }
                }
                Text_Buffer.InsertTL((0, -4), Text_Buffer.Default_TextSize, 0x000000, str);
            }

            (float, float) winSize = MainWindow.Size_Float2();
            MainWindow.ClearBufferDepth();
            Text_Buffer.DisplayConverter.X.PixelSize = winSize.Item1;
            Text_Buffer.DisplayConverter.Y.PixelSize = winSize.Item2;

            Text_Shader.Use();
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
            BodyIsSelected = new ArrayList<bool>(Scene.AllBodys.Count);
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

        public void Init_Shader()
        {
            string shaderDir = "E:/Programmieren/VS_Code/OpenTK/Engine3D/Engine3D/Shaders/";

            BodyUniFull_Shader = new BodyElemUniShader(shaderDir);
            BodyUniFull_Shader.Use();
            BodyUniFull_Shader.Depth.Value(MainCamera.Depth.Near, MainCamera.Depth.Far);
            BodyUniFull_Shader.LightSolar.Value(!(new Point3D(1, 1, 1)));
            BodyUniFull_Shader.LightRange.Value(0.1f, 1.0f);
            BodyUniFull_Shader.OtherColor.Value(0xFF0000);
            BodyUniFull_Shader.OtherColorInter.T0(1.0f);

            BodyUniWire_Shader = new BodyElemUniWireShader(shaderDir);
            BodyUniWire_Shader.Use();
            BodyUniWire_Shader.Depth.Value(MainCamera.Depth.Near, MainCamera.Depth.Far);
            BodyUniWire_Shader.OtherColor.Value(0x000000);
            BodyUniWire_Shader.OtherColorInter.T1(1.0f);

            UserInterface_Shader = new UserInterfaceBodyShader(shaderDir);
            UserInterface_Shader.Use();

            ShaderDisplayState = EnumFunctions.EShaderDisplayState.SelectHoverMono;

            Text_Buffer = new TextBuffer();
            Text_Buffer.Bind_Pallets();
            Text_Buffer.DisplayConverter = new DisplayPointConverter(1000, 1000);
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

            Indicator_FanS = new UI_Indicator(+050, -50, 100, 100, UI_Anchor.YX_MP, TBodyFile.LoadTextFile(dirPicto + "ComputerFanStator.ymt"), 0.6f, UI_Indicator.EAnimationType.None);
            Indicator_FanR = new UI_Indicator(+050, -50, 100, 100, UI_Anchor.YX_MP, TBodyFile.LoadTextFile(dirPicto + "ComputerFanRotor.ymt"), 0.6f, UI_Indicator.EAnimationType.Spin);
            Indicator_Disk = new UI_Indicator(+150, -50, 100, 100, UI_Anchor.YX_MP, TBodyFile.LoadTextFile(dirPicto + "OpticalDisk.ymt"), 1.0f, UI_Indicator.EAnimationType.Disk);
            Indicator_Fold = new UI_Indicator(+250, -50, 100, 100, UI_Anchor.YX_MP, TBodyFile.LoadTextFile(dirPicto + "RingFolder.ymt"), 1.6f, UI_Indicator.EAnimationType.Diag);
            Indicator_Test = new UI_Indicator(+350, -50, 100, 100, UI_Anchor.YX_MP, TBodyFile.LoadTextFile(dirPicto + "Test.ymt"), 1.0f, UI_Indicator.EAnimationType.Disk);
            Indicator_ObjectAdd = new UI_Indicator(+450, -50, 100, 100, UI_Anchor.YX_MP, TBodyFile.LoadTextFile(dirMeta + "Inn.txt"), 1.2f, UI_Indicator.EAnimationType.Disk);
            Indicator_ObjectSub = new UI_Indicator(+550, -50, 100, 100, UI_Anchor.YX_MP, TBodyFile.LoadTextFile(dirMeta + "Out.txt"), 1.2f, UI_Indicator.EAnimationType.Disk);

            Indicator_MouseDragMain = new UI_Indicator[6]
            {
                new UI_Indicator(-050, -50, 100, 100, UI_Anchor.YX_PP, TBodyFile.LoadTextFile(dirMove + "AxisY.txt"), 1.0f, UI_Indicator.EAnimationType.View),
                new UI_Indicator(-050, -50, 100, 100, UI_Anchor.YX_PP, TBodyFile.LoadTextFile(dirMove + "AxisX.txt"), 1.0f, UI_Indicator.EAnimationType.View),
                new UI_Indicator(-050, -50, 100, 100, UI_Anchor.YX_PP, TBodyFile.LoadTextFile(dirMove + "AxisC.txt"), 1.0f, UI_Indicator.EAnimationType.View),
                new UI_Indicator(-150, -50, 100, 100, UI_Anchor.YX_PP, TBodyFile.LoadTextFile(dirSpin + "RingY.txt"), 1.0f, UI_Indicator.EAnimationType.View),
                new UI_Indicator(-150, -50, 100, 100, UI_Anchor.YX_PP, TBodyFile.LoadTextFile(dirSpin + "RingX.txt"), 1.0f, UI_Indicator.EAnimationType.View),
                new UI_Indicator(-150, -50, 100, 100, UI_Anchor.YX_PP, TBodyFile.LoadTextFile(dirSpin + "RingC.txt"), 1.0f, UI_Indicator.EAnimationType.View),
            };

            Indicator_MouseDragSnap = new UI_Indicator[]
            {
                new UI_Indicator(-050, -150, 100, 100, UI_Anchor.YX_PP, TBodyFile.LoadTextFile(dirMove + "MoveSnap0.txt"), 1.0f, UI_Indicator.EAnimationType.None),
                new UI_Indicator(-050, -150, 100, 100, UI_Anchor.YX_PP, TBodyFile.LoadTextFile(dirMove + "MoveSnap1.txt"), 1.0f, UI_Indicator.EAnimationType.None),
                new UI_Indicator(-150, -150, 100, 100, UI_Anchor.YX_PP, TBodyFile.LoadTextFile(dirSpin + "SpinSnap0.txt"), 1.0f, UI_Indicator.EAnimationType.None),
                new UI_Indicator(-150, -150, 100, 100, UI_Anchor.YX_PP, TBodyFile.LoadTextFile(dirSpin + "SpinSnap1.txt"), 1.0f, UI_Indicator.EAnimationType.None),
            };

            ConsoleLog.NewLine();

            Hovering_Indicator = false;
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
            MainCamera.Depth.Far = 10000.0f;

            FilePath = null;
            Scene = new PolySoma();
            Scene.Edit_Begin();

            string dir = "E:/Zeug/YMT/";
            Init_Shader();
            Init_Mouse_Edit(dir);
            Init_UI(dir);
            //Init_Scene(dir + "RealLifePersonal/Scene.txt");

            MainWindow.Run();
            MainWindow.Term();
        }
    }
}
