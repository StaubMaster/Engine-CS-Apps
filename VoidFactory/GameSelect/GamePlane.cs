using System;
using System.Collections.Generic;
using System.IO;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.Abstract2D;
using Engine3D.GraphicsOld;
using Engine3D.GraphicsOld.Forms;
using Engine3D.Entity;

using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform;
using Engine3D.OutPut.Uniform.Generic.Float;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;
using Engine3D.Graphics.Display3D;
using Engine3D.Graphics.Display2D.UserInterface;
using Engine3D.Graphics.Display;
using Engine3D.Graphics.Telematry;
using Engine3D.Graphics.Manager;
using Engine3D.DataStructs;

using Engine3D.Miscellaneous.EntryContainer;

using VoidFactory.Surface2D;
using VoidFactory.Surface2D.Graphics;
using VoidFactory.Production.Data;
using VoidFactory.Production.Transfer;
using VoidFactory.Production.Buildings;
using VoidFactory.Inventory;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace VoidFactory.GameSelect
{
    /*  Instances
     *  Ignore the stuff where all are drawn one way except for one
     *  possibly split into 2 Groups that are drawn differently
     *      all Gray except one
     *      one without Light
     */
    /*  PolyHedra
     *      finally start using Textures
     *      fix FileInterpretation Variables
     *      use FileInterpretation for PolyHedra
     *      allow PolyHedra to be made of multiple Files
     */
    partial class GamePlane : Game3D
    {
        private Transformation3D Solar;

        private BodyElemUniShader BodyUni_Shader;

        private TextShader Text_Shader;
        private TextBuffer Text_Buffer;

        private AxisBoxShader Box_Shader;
        private AxisBoxBuffer Box_Buffer;

        private PolyHedra_Shader_Manager PH_Man;
        private PHEI_Buffer InstBuffer;

        private EntryContainerDynamic<PolyHedraInstance_3D_Data>.Entry[][] TestBodys;

        private UserInterfaceManager UI_Man;
        private UIBody_Buffer UIBodyBuffer;
        private Angle3D UI_Spin;


        private Chunk2D.Collection Chunks;
        private Chunk2D_Shader Chunk_Shader;



        private DATA_Thing[] Things;
        private BLD_Base.Template_Base[] Templates;
        private DATA_Recipy[] Recipys;

        public BLD_Base.Collection Buildings;
        public IO_TransPorter.Collection TransPorter;



        public GamePlane(Action externDelete) : base(externDelete)
        {

        }



        private void EntitiesInfo()
        {
            string str = "";
            str += Buildings.Count() + ":  Converter\n";
            str += TransPorter.Count() + ":TransPorter\n";
            //str += Chunk2D.SURF_Object.BodysCountInfo() + "\n";
            //str += Chunk2D_Buffer.SBufferEntrys.ToInfo();
            Text_Buffer.InsertTR(
                (0, 0), Text_Buffer.Default_TextSize, 0xFFFFFF,
                str);
        }
        private void EntitiesUpdate()
        {
            Buildings.Update();
            TransPorter.Update();

            EntitiesInfo();
        }
        private void EntitiesDraw()
        {
            //if (!Graphic.Draw_Gray)
            //if (Graphic.Draw_Ports)

            TransPorter.Draw(BodyUni_Shader);
        }

        private void DebugAllBodysSpinUpdate()
        {
            Angle3D rot = new Angle3D(Angle3D.Deg45, 0, 0);

            PolyHedraInstance_3D_Data data;

            for (int i = 0; i < TestBodys[0].Length; i++)
            {
                data = TestBodys[0][i][0];
                data.Trans = new Transformation3D(new Point3D(i * 10, 0, 120), rot);
                TestBodys[0][i][0] = data;
            }

            for (int i = 0; i < TestBodys[1].Length; i++)
            {
                data = TestBodys[1][i][0];
                data.Trans = new Transformation3D(new Point3D(i * 10, 0, 140), rot);
                TestBodys[1][i][0] = data;
            }

            for (int i = 0; i < TestBodys[2].Length; i++)
            {
                data = TestBodys[2][i][0];
                data.Trans = new Transformation3D(new Point3D(i * 30, 0, 170), rot);
                TestBodys[2][i][0] = data;
            }
        }
        private void TowerDraw()
        {
            EntryContainerDynamic<PolyHedraInstance_3D_Data> instData = new EntryContainerDynamic<PolyHedraInstance_3D_Data>();

            EntryContainerDynamic<PolyHedraInstance_3D_Data>.Entry entry = instData.Alloc(4);
            entry[0] = new PolyHedraInstance_3D_Data(new Transformation3D(new Point3D(-320, 20, -320)));
            entry[1] = new PolyHedraInstance_3D_Data(new Transformation3D(new Point3D(+320, -100, -320)), new ColorUData(0xFF0000), LInterData.LIT1());
            entry[2] = new PolyHedraInstance_3D_Data(new Transformation3D(new Point3D(+320, -100, +320)), new ColorUData(0x00FF00), LInterData.LIT1());
            entry[3] = new PolyHedraInstance_3D_Data(new Transformation3D(new Point3D(-320, -100, +320)), new ColorUData(0x0000FF), LInterData.LIT1());

            InstBuffer.Bind_Inst_Trans(instData.Data, instData.Data.Length);
            PH_Man.InstShader.Use();
            InstBuffer.Draw_Inst();
        }
        private void UI_Test()
        {
            UIGridPosition gPos = new UIGridPosition(UIAnchor.MM(), new Point2D(0.0f, 0.0f), UICorner.MM());
            UIGridSize gSize = new UIGridSize(new Point2D(50.0f, 50.0f), 25.0f);

            UIBody_Data[] UIData = new UIBody_Data[]
            {
                new UIBody_Data(gPos.WithOffset(new Point2D( 0.0f,  0.0f)), gSize, 1.0f, new Angle3D(UI_Spin.A, 0, 0)),
                new UIBody_Data(gPos.WithOffset(new Point2D(+1.0f,  0.0f)), gSize, 1.0f, UI_Spin),
                new UIBody_Data(gPos.WithOffset(new Point2D(+1.0f, -1.0f)), gSize, 1.0f, new Transformation3D(new Point3D(0.0f, 0, 0), UI_Spin)),
                new UIBody_Data(gPos.WithOffset(new Point2D(+1.0f, -1.0f)), gSize, 1.0f, new Transformation3D(new Point3D(3.0f, 0, 0), UI_Spin)),
            };

            UI_Spin.A += 0.01f;

            UIBodyBuffer.Bind_Inst(UIData, UIData.Length);
            UI_Man.UIBodyShader.Use();
            UIBodyBuffer.Draw_Inst();
        }
        private void InstDraw()
        {
            PH_Man.InstShader.Use();

            IO_Port.Bodys.Update();
            if (Interaction.Draw_Ports)
            {
                PH_Man.LightRange.ChangeData(new RangeData(1.0f, 1.0f));
                IO_Port.Bodys.Draw();
                PH_Man.LightRange.ChangeData(new RangeData(0.1f, 1.0f));
            }

            BLD_Base.Bodys.Update();
            BLD_Base.Bodys.Draw();

            DATA_Thing.Bodys.Update();
            DATA_Thing.Bodys.Draw();

            Chunk2D.SURF_Object.Bodys.Update();
            Chunk2D.SURF_Object.Bodys.Draw();
        }

        private void Update_Solar()
        {
            Solar.Rot.A += 0.01f;
            Point3D solar = !(Solar.Pos + Solar.Rot);

            PH_Man.LightSolar.ChangeData(solar);
            Chunk_Shader.LightSolar.Value(solar);
        }
        private void Update_View()
        {
            TMovement.FlatX(ref view.Trans, win.MoveByKeys(), win.SpinByMouse());
            view.Update(win.MouseRay());

            {
                string format = "+0000.00;-0000.00; 0000.00";
                string str = "";
                str += "Pos.Y:" + view.Trans.Pos.Y.ToString(format) + "\n";
                str += "Pos.X:" + view.Trans.Pos.X.ToString(format) + "\n";
                str += "Pos.C:" + view.Trans.Pos.C.ToString(format) + "\n";
                Text_Buffer.InsertTL((0, 0), Text_Buffer.Default_TextSize, 0xFFFFFF, str);
            }

            BodyUni_Shader.View.Value(view.Trans);
            Chunk_Shader.View.Value(view.Trans);
            TransPorter.Program.UniView(new RenderTrans(view.Trans));
            PH_Man.View.ChangeData(view.Trans);
            Box_Shader.View.Value(view.Trans);
        }
        private void Update_WinSize()
        {
            (float, float) winSize = win.Size_Float2();
            BodyUni_Shader.ScreenRatio.Value(winSize);
            Chunk_Shader.ScreenRatio.Value(winSize);
            Box_Shader.ScreenRatio.Value(winSize);

            SizeRatio winRatio = new SizeRatio(winSize.Item1, winSize.Item2);
            Inventory_Interface.SizeRatio = winRatio;
            PH_Man.ViewPortSizeRatio.ChangeData(winRatio);
            UI_Man.ScreenRatio.ChangeData(winRatio);
        }

        private void Frame_Inv()
        {
            Inventory_Interface.Tool_Change((int)-win.MouseScroll());

            if (win.MouseLocked)
            {
                Inventory_Interface.Draw_Free();
                Inventory_Interface.Cat_Hide();

                if (win.CheckKey(MouseButton.Left).IsPressed()) { Inventory_Interface.Tool_Func1(); }
                if (win.CheckKey(MouseButton.Right).IsPressed()) { Inventory_Interface.Tool_Func2(); }
            }
            else
            {
                Inventory_Interface.Draw_Init();
                Inventory_Interface.Cat_Show();

                //Inventory_Interface.Cat_Draw();
                Inventory_Interface.Mouse_Hover(win.MousePixel());

                if (win.CheckKey(Keys.PageUp).IsPressed()) { Inventory_Interface.Cat_Next(); }
                if (win.CheckKey(Keys.PageDown).IsPressed()) { Inventory_Interface.Cat_Prev(); }

                if (win.CheckKey(MouseButton.Left).IsPressed()) { Inventory_Interface.Mouse_Select(); }
            }

            Inventory_Interface.Tool_Update();

            string str = "";
            str += "Hovering: " + Inventory_Interface.Hovering_Idx + "\n";
            str += "Selected: " + Inventory_Interface.Selected_Idx + "\n";
            Text_Buffer.InsertTL((0, -4), Text_Buffer.Default_TextSize, 0xFFFFFF, str);

            UI_Man.UIBodyShader.Use();
            Inventory_Interface.Draw_UI_Insts();
            Inventory_Interface.Draw_UI_Info(Text_Buffer);

            Inventory_Interface.Tool_Draw();
        }
        private void Frame_Chunk()
        {
            Chunk2D_Buffer.SUpdate();

            Chunk_Shader.Use();
            if (!Interaction.Draw_Gray)
            {
                Chunk_Shader.GrayInter.T0(1.0f);
            }
            else
            {
                Chunk_Shader.GrayInter.T1(1.0f);
            }
            Chunks.Draw(Chunk_Shader);
            Chunk_Shader.GrayInter.T0(1.0f);
        }
        private void Frame_Box()
        {
            Box_Shader.Use();
            Box_Buffer.BindList();
            Box_Buffer.Draw();
        }
        private void Frame_Text()
        {
            Text_Shader.Use();
            Text_Shader.ScreenRatio.Value(win.Size_Float2());

            //Text_Buffer.Bind_Strings();
            //Text_Buffer.Test_Characters();
            //Text_Buffer.Test_ASCII();
            //Text_Buffer.Test_Direction();
            //Text_Buffer.Test_Char();

            Text_Buffer.Bind_Strings();

            Text_Buffer.Draw();
        }
        protected override void Frame()
        {
            if (win.CheckKey(Keys.Pause).IsPressed())
            {
                ConsoleLog.Log("Pause Break: Garbage Collect");
                GC.Collect();
            }

            if (win.CheckKey(Keys.F5).IsPressed())
            {
                Init_Shaders();
            }

            Update_Solar();
            Update_View();
            Update_WinSize();

            DebugAllBodysSpinUpdate();

            EntitiesUpdate();
            EntitiesDraw();

            Frame_Chunk();
            Frame_Box();

            InstDraw();

            TowerDraw();

            GL.Clear(ClearBufferMask.DepthBufferBit);
            Frame_Inv();
            //UI_Test();

            Frame_Text();
        }



        private PolyHedra[] AssamblePolyHedra(PolyHedra[] polyhedras, params BodyStatic[][] bodys)
        {
            int len = polyhedras.Length;
            for (int i = 0; i < bodys.Length; i++)
            {
                len += bodys[i].Length;
            }

            PolyHedra[] arr = new PolyHedra[len];
            len = 0;
            for (int i = 0; i < polyhedras.Length; i++)
            {
                arr[len] = polyhedras[i];
            }

            for (int j = 0; j < bodys.Length; j++)
            {
                for (int i = 0; i < bodys[j].Length; i++)
                {
                    arr[len] = bodys[j][i].ToPolyHedra();
                }
            }

            return arr;
        }
        private void FromFiles()
        {
            string folder = "E:/Programmieren/VS_Code/OpenTK/VoidFactory/VoidFactory/Config/";

            string[] filePaths = new string[]
            {
                "Void.txt",
                "Building.txt",
                "Raw.txt",
                "Relays.txt",
                "Manufacturing.txt",
                "Iron.txt",
                "Copper.txt",
                "Electronics.txt",
                "SurfaceLayerNoise.txt",
                "SurfaceThings.txt",
            };

            /*string[] fileTexts = new string[]
            {
                File.ReadAllText(folder + "Void.txt"),
                File.ReadAllText(folder + "Building.txt"),
                File.ReadAllText(folder + "Raw.txt"),
                File.ReadAllText(folder + "Relays.txt"),
                File.ReadAllText(folder + "Manufacturing.txt"),
                File.ReadAllText(folder + "Iron.txt"),
                File.ReadAllText(folder + "Copper.txt"),
                File.ReadAllText(folder + "Electronics.txt"),
                File.ReadAllText(folder + "SurfaceLayerNoise.txt"),
                File.ReadAllText(folder + "SurfaceThings.txt"),
            };*/

            FileInterpret.FileStruct[] fileDatas = new FileInterpret.FileStruct[filePaths.Length];
            for (int i = 0; i < fileDatas.Length; i++)
            {
                fileDatas[i] = new FileInterpret.FileStruct(File.ReadAllText(folder + filePaths[i]));
            }

            /*FileInterpret.FileStruct[] fileDatas = new FileInterpret.FileStruct[]
            {
                new FileInterpret.FileStruct(File.ReadAllText(folder + "Void.txt")),

                new FileInterpret.FileStruct(File.ReadAllText(folder + "Building.txt")),
                new FileInterpret.FileStruct(File.ReadAllText(folder + "Raw.txt")),

                new FileInterpret.FileStruct(File.ReadAllText(folder + "Relays.txt")),
                new FileInterpret.FileStruct(File.ReadAllText(folder + "Manufacturing.txt")),

                new FileInterpret.FileStruct(File.ReadAllText(folder + "Iron.txt")),
                new FileInterpret.FileStruct(File.ReadAllText(folder + "Copper.txt")),

                new FileInterpret.FileStruct(File.ReadAllText(folder + "Electronics.txt")),



                new FileInterpret.FileStruct(File.ReadAllText(folder + "SurfaceLayerNoise.txt")),
                new FileInterpret.FileStruct(File.ReadAllText(folder + "SurfaceThings.txt")),
            };
            */

            InterpretFiles(fileDatas);
        }
        private void InterpretFiles(FileInterpret.FileStruct[] fileDatas)
        {
            /*
            Engine3D.ConsoleLog.LogProgress("Interpret Meta");
            PolyHedra[] bodys_Meta;
            {
                string ymtDir = "E:/Zeug/YMT/";

                bodys_Meta = new PolyHedra[]
                {
                    PolyHedra.Generate.Error(),

                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Transporter.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/AxisCross10.txt"),

                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Box_Hex.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Box_Oct.txt"),

                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Inn.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Inn_Box_Hex.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Inn_Box_Oct.txt"),

                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Out.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Out_Box_Hex.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Out_Box_Oct.txt"),
                };

                IO_Port.Bodys = new PHEI_Array(bodys_Meta);
                Inventory_Interface.Meta_Bodys = new UIBody_Array(bodys_Meta);
            }
            Engine3D.ConsoleLog.LogProgress("Interpret Things");
            BodyStatic[] bodys_Things;
            {
                DATA_Thing.Interpret.Create();
                for (int i = 0; i < fileDatas.Length; i++) { DATA_Thing.Interpret.SetFile(fileDatas[i]); }
                Things = DATA_Thing.Interpret.GetThings();
                {
                    bodys_Things = DATA_Thing.Interpret.GetBodys();
                    DATA_Thing.IconScales_Create(bodys_Things);
                    DATA_Thing.Bodys = new PHEI_Array(bodys_Things);
                    Inventory_Interface.DATA_Thing_Bodys = new UIBody_Array(bodys_Things);
                }
                DATA_Thing.Interpret.Delete();
            }
            Engine3D.ConsoleLog.LogProgress("Interpret Recipys");
            {
                DATA_Recipy.Interpret.Create();
                for (int i = 0; i < fileDatas.Length; i++) { DATA_Recipy.Interpret.SetFile(fileDatas[i], Things); }
                Recipys = DATA_Recipy.Interpret.GetRecipy();
                DATA_Recipy.Interpret.Delete();
            }
            Engine3D.ConsoleLog.LogProgress("Interpret Buildings");
            BodyStatic[] bodys_buildings;
            {
                BLD_Base.Interpreter.Create();
                for (int i = 0; i < fileDatas.Length; i++) { BLD_Base.Interpreter.SetFile(fileDatas[i], Things); }
                Templates = BLD_Base.Interpreter.GetTemplates();
                {
                    bodys_buildings = BLD_Base.Interpreter.GetBodys();
                    BLD_Base.Bodys = new PHEI_Array(bodys_buildings);
                    Inventory_Interface.BLD_Bodys = new UIBody_Array(bodys_buildings);
                }
                BLD_Base.Interpreter.Delete();
            }
            Engine3D.ConsoleLog.LogProgress("Interpret Chunk Layers");
            {
                Chunk2D.Interpret.Create();
                for (int i = 0; i < fileDatas.Length; i++) { Chunk2D.Interpret.SetFile(fileDatas[i], Things); }
                Chunk2D.LayerGen = Chunk2D.Interpret.GetLayers();
                Chunk2D.Interpret.Delete();
            }
            Engine3D.ConsoleLog.LogProgress("Interpret Chunk Object");
            BodyStatic[] bodys_objects;
            {
                Chunk2D.SURF_Object.Interpret.Create();
                for (int i = 0; i < fileDatas.Length; i++) { Chunk2D.SURF_Object.Interpret.SetFile(fileDatas[i], Things); }
                bodys_objects = Chunk2D.SURF_Object.Interpret.GetBodys();
                Chunk2D.SURF_Object.Bodys = new PHEI_Array(bodys_objects);
                Chunk2D.SurfThingTemplates = Chunk2D.SURF_Object.Interpret.GetTemplates();
                Chunk2D.SURF_Object.Interpret.Delete();
            }
            Engine3D.ConsoleLog.LogDone("Interpreting Bodys");
            */

            Engine3D.ConsoleLog.LogProgress("Interpret Meta");
            PolyHedra[] bodys_Meta;
            {
                string ymtDir = "E:/Zeug/YMT/";

                bodys_Meta = new PolyHedra[]
                {
                    PolyHedra.Generate.Error(),
            
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Transporter.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/AxisCross10.txt"),
            
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Box_Hex.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Box_Oct.txt"),
            
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Inn.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Inn_Box_Hex.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Inn_Box_Oct.txt"),
            
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Out.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Out_Box_Hex.txt"),
                    Engine3D.BodyParse.TBodyFile.LoadTextFile(ymtDir + "Meta/Out_Box_Oct.txt"),
                };
            }
            Engine3D.ConsoleLog.LogProgress("Interpret Things");

            {
                DATA_Thing.Interpret.Create();
                DATA_Recipy.Interpret.Create();
                BLD_Base.Interpreter.Create();
                Chunk2D.Interpret.Create();
                Chunk2D.SURF_Object.Interpret.Create();
            }

            {
                for (int i = 0; i < fileDatas.Length; i++) { DATA_Thing.Interpret.SetFile(fileDatas[i]); }
                Things = DATA_Thing.Interpret.GetThings();

                for (int i = 0; i < fileDatas.Length; i++) { DATA_Recipy.Interpret.SetFile(fileDatas[i], Things); }
                Recipys = DATA_Recipy.Interpret.GetRecipy();

                for (int i = 0; i < fileDatas.Length; i++) { BLD_Base.Interpreter.SetFile(fileDatas[i], Things); }
                Templates = BLD_Base.Interpreter.GetTemplates();

                for (int i = 0; i < fileDatas.Length; i++) { Chunk2D.Interpret.SetFile(fileDatas[i], Things); }
                Chunk2D.LayerGen = Chunk2D.Interpret.GetLayers();

                for (int i = 0; i < fileDatas.Length; i++) { Chunk2D.SURF_Object.Interpret.SetFile(fileDatas[i], Things); }
                Chunk2D.SurfThingTemplates = Chunk2D.SURF_Object.Interpret.GetTemplates();
            }

            BodyStatic[] bodys_things;
            BodyStatic[] bodys_buildings;
            BodyStatic[] bodys_objects;

            {
                bodys_things = DATA_Thing.Interpret.GetBodys();
                bodys_buildings = BLD_Base.Interpreter.GetBodys();
                bodys_objects = Chunk2D.SURF_Object.Interpret.GetBodys();
            }

            {
                IO_Port.Bodys = new PHEI_Array(bodys_Meta);
                Inventory_Interface.Meta_Bodys = new UIBody_Array(bodys_Meta);

                DATA_Thing.Bodys = new PHEI_Array(bodys_things);
                Inventory_Interface.DATA_Thing_Bodys = new UIBody_Array(bodys_things);

                BLD_Base.Bodys = new PHEI_Array(bodys_buildings);
                Inventory_Interface.BLD_Bodys = new UIBody_Array(bodys_buildings);

                Chunk2D.SURF_Object.Bodys = new PHEI_Array(bodys_objects);
            }

            DATA_Thing.IconScales_Create(bodys_things);

            {
                DATA_Thing.Interpret.Delete();
                DATA_Recipy.Interpret.Delete();
                BLD_Base.Interpreter.Delete();
                Chunk2D.Interpret.Delete();
                Chunk2D.SURF_Object.Interpret.Delete();
            }

            Engine3D.ConsoleLog.LogDone("Sorting Bodys");
        }
        private void BodysNull()
        {
            DATA_Thing.Bodys = null;
            BLD_Base.Bodys = null;
            IO_Port.Bodys = null;
            Chunk2D.SURF_Object.Bodys = null;

            Inventory_Interface.Meta_Bodys = null;
            Inventory_Interface.BLD_Bodys = null;
            Inventory_Interface.DATA_Thing_Bodys = null;
        }

        private void Chunk_Create()
        {
            Chunk2D_Buffer.SCreate();
            Chunks = new Chunk2D.Collection();
            Chunks.Create();
        }
        private void Chunk_Delete()
        {
            Chunks.Delete();
            Chunks = null;
            Chunk2D_Buffer.SDelete();
        }

        private void Entities_Create()
        {
            Buildings = new BLD_Base.Collection();
            Buildings.Create();

            TransPorter = new IO_TransPorter.Collection();
            TransPorter.Program = new TransPorterProgram("TransPorter",
                "TransPorter/TP.vert",
                "TransPorter/TP.geom",
                "Frag/Direct.frag");
            TransPorter.Create();

            //TransPorter.Program.UniProj(new RenderDepthFactors(view.DepthN, view.DepthF), view.Fov);
            //view.UniDepth(TransPorter.Program);
            TransPorter.Program.UniDepth(new RenderDepthFactors(view.Depth.Near, view.Depth.Far));
        }
        private void Entities_Delete()
        {
            Recipys = null;
            Templates = null;


            Buildings.Delete();
            Buildings = null;

            TransPorter.Delete();
            TransPorter = null;
        }

        private void Ref_Graphics()
        {
            Interaction.BodyUni_Shader = BodyUni_Shader;
            Interaction.Text_Buffer = Text_Buffer;
            Interaction.Box_Buffer = Box_Buffer;
            Interaction.View = view;

            Inter_Port.Buildings = Buildings;
            Inter_Connect.TransPorter = TransPorter;
            Inter_Surface2D_Hit.Chunks = Chunks;
            Inter_Surf2D_Building.Buildings = Buildings;
            Inter_Building.Buildings = Buildings;

            BLD_Surf_Collector.Chunks = Chunks;

            Inventory_Storage.Text_Buffer = Text_Buffer;
        }

        private void Inv_Create()
        {
            Inventory_Interface.Create();
            Inventory_Interface.Sort(Things, Templates, Recipys, Chunk2D.SurfThingTemplates);

            //InventoryNextKey = new KeyPress(Keys.PageUp);
            //InventoryPrevKey = new KeyPress(Keys.PageDown);
            //win.KeyChecks.Add(InventoryNextKey);
            //win.KeyChecks.Add(InventoryPrevKey);

            Inventory_Storage.Create(Things);
        }
        private void Inv_Delete()
        {
            Inventory_Interface.Delete();
            //InventoryNextKey = null;
            //InventoryPrevKey = null;

            Inter_Port.Buildings = null;
            Inter_Connect.TransPorter = null;
            Inter_Surface2D_Hit.Chunks = null;
            Inter_Surf2D_Building.Buildings = null;
            Inter_Building.Buildings = null;

            BLD_Surf_Collector.Chunks = null;

            Inventory_Storage.Delete();
        }

        private void Init_Shaders()
        {
            string shaderDir = "E:/Programmieren/VS_Code/OpenTK/Engine3D/Engine3D/Shaders/";

            //MainContext = new DisplayContext(shaderDir);

            BodyUni_Shader = new BodyElemUniShader(shaderDir);

            Text_Shader = new TextShader(shaderDir);
            Text_Buffer = new TextBuffer();
            //Text_Buffer.Telematry = new Telematry.TelematryBuffer();
            Text_Buffer.Bind_Pallets();

            Box_Shader = new AxisBoxShader(shaderDir);
            Box_Buffer = new AxisBoxBuffer();
            //Box_Buffer.Telematry = new Telematry.TelematryBuffer();



            BodyUni_Shader.Use();
            BodyUni_Shader.Depth.Value(view.Depth.Near, view.Depth.Far);
            BodyUni_Shader.LightRange.Value(0.1f, 1.0f);
            BodyUni_Shader.OtherColor.Value(0xFFFFFFFF);
            BodyUni_Shader.OtherColorInter.T0(1.0f);

            Box_Shader.Use();
            Box_Shader.Depth.Value(view.Depth.Near, view.Depth.Far);

            Chunk_Shader = new Chunk2D_Shader(shaderDir);
            Chunk_Shader.Use();
            Chunk_Shader.TilesSize.Set(new int[] { Chunk2D.Tile_Size });
            Chunk_Shader.TilesPreSide.Set(new int[] { Chunk2D.Tiles_Per_Side });
            Chunk_Shader.Depth.Value(view.Depth.Near, view.Depth.Far);

            PH_Man = new PolyHedra_Shader_Manager(shaderDir);
            PH_Man.Depth.ChangeData(new DepthData(view.Depth.Near, view.Depth.Far));

            PolyHedra tower = Engine3D.BodyParse.TBodyFile.LoadTextFile("E:/Zeug/YMT/Tower/YMT/Tower_Segmented_60m.ymt");
            InstBuffer = new PHEI_Buffer();
            tower.ToBuffer(InstBuffer);

            PolyHedra cube = PolyHedra.Generate.Cube();
            UI_Man = new UserInterfaceManager(shaderDir);
            UIBodyBuffer = new UIBody_Buffer();
            cube.ToBuffer(UIBodyBuffer);
            //UI_Man.Depth.ChangeData(new DepthData(-1000, +1000));
            UI_Spin = new Angle3D(0, -0.5f, 0);

            Ref_Graphics();

            {
                TestBodys = new EntryContainerBase<PolyHedraInstance_3D_Data>.Entry[3][];

                TestBodys[0] = new EntryContainerBase<PolyHedraInstance_3D_Data>.Entry[IO_Port.Bodys.Length];
                for (int i = 0; i < IO_Port.Bodys.Length; i++)
                {
                    TestBodys[0][i] = IO_Port.Bodys[i].Alloc(1);
                    TestBodys[0][i][0] = new PolyHedraInstance_3D_Data(Transformation3D.Default());
                }

                TestBodys[1] = new EntryContainerBase<PolyHedraInstance_3D_Data>.Entry[DATA_Thing.Bodys.Length];
                for (int i = 0; i < DATA_Thing.Bodys.Length; i++)
                {
                    TestBodys[1][i] = DATA_Thing.Bodys[i].Alloc(1);
                    TestBodys[1][i][0] = new PolyHedraInstance_3D_Data(Transformation3D.Default());
                }

                TestBodys[2] = new EntryContainerBase<PolyHedraInstance_3D_Data>.Entry[BLD_Base.Bodys.Length];
                for (int i = 0; i < BLD_Base.Bodys.Length; i++)
                {
                    TestBodys[2][i] = BLD_Base.Bodys[i].Alloc(1);
                    TestBodys[2][i][0] = new PolyHedraInstance_3D_Data(Transformation3D.Default());
                }
            }
        }



        /* get rid of these
         * make multiThreaded ?
         */
        public override void Create()
        {
            if (Running) { return; }
            ConsoleLog.Log("Create GamePlane");
            ConsoleLog.TabInc();
            base.Create();

            Solar = new Transformation3D(new Point3D(-2, +3, -1));

            view.Depth.Near = 1.0f;
            view.Depth.Far = 1000.0f;



            FromFiles();

            Entities_Create();
            Chunk_Create();

            Inv_Create();



            Init_Shaders();

            ConsoleLog.TabDec();
            ConsoleLog.Log("");

            Running = true;
        }
        public override void Delete()
        {
            if (!Running) { return; }
            Running = false;
            ConsoleLog.Log("Delete GamePlane");
            ConsoleLog.TabInc();
            base.Delete();



            BodysNull();

            Entities_Delete();
            Chunk_Delete();

            Inv_Delete();

            ConsoleLog.TabDec();
            ConsoleLog.Log("");
        }
    }
}
