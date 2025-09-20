using System;
using System.Collections.Generic;
using System.IO;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.Abstract2D;
using Engine3D.Entity;

using Engine3D.Graphics;
using Engine3D.Graphics.Telematry;
using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Manager;
using Engine3D.Graphics.PolyHedraInstance.PH_3D;
using Engine3D.Graphics.PolyHedraInstance.PH_UI;
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

        private TextBuffer Text_Buffer;

        private AxisBoxBuffer Box_Buffer;

        private PolyHedra_Shader_Manager PH_Man;

        private EntryContainerDynamic<PolyHedraInstance_3D_Data>.Entry[] TestBodys;

        private UserInterfaceManager UI_Man;

        private Angle3D UI_Spin;


        private Chunk2D.Collection Chunks;



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

            TransPorter.Draw();
        }

        private void DebugAllBodysSpinUpdate()
        {
            PolyHedraInstance_3D_Data data;
            for (int i = 0; i < TestBodys.Length; i++)
            {
                data = TestBodys[i][0];
                data.Trans.Rot.A += 0.01f;
                TestBodys[i][0] = data;
            }
        }
        private void InstDraw()
        {
            PH_Man.InstShader.Use();

            this.PH_3D.Update();

            //IO_Port.Bodys.Update();
            if (Interaction.Draw_Ports)
            {
                PH_Man.LightRange.ChangeData(new RangeData(1.0f, 1.0f));
                //IO_Port.Bodys.Draw();
                PH_Man.LightRange.ChangeData(new RangeData(0.1f, 1.0f));
            }

            //BLD_Base.Bodys.Update();
            //BLD_Base.Bodys.Draw();

            //DATA_Thing.Bodys.Update();
            //DATA_Thing.Bodys.Draw();

            //Chunk2D.SURF_Object.Bodys.Update();
            //Chunk2D.SURF_Object.Bodys.Draw();

            this.PH_3D.Draw();
        }

        private void Update_Solar()
        {
            Solar.Rot.A -= 0.01f;
            Point3D solar = !(Solar.Pos + Solar.Rot);

            PH_Man.LightSolar.ChangeData(solar);
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

            PH_Man.View.ChangeData(view.Trans);
        }
        private void Update_WinSize()
        {
            (float, float) winSize = win.Size_Float2();

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

            //Chunk_Shader_Old.Use();
            if (!Interaction.Draw_Gray)
            {
                //Chunk_Shader_Old.GrayInter.T0(1.0f);
                PH_Man.GrayInter.ChangeData(LInterData.LIT0());
            }
            else
            {
                //Chunk_Shader_Old.GrayInter.T1(1.0f);
                PH_Man.GrayInter.ChangeData(LInterData.LIT1());
            }

            //Chunks.Draw(Chunk_Shader_Old);
            //Chunk_Shader_Old.GrayInter.T0(1.0f);

            Chunk2D_Graphics.Chunk_Shader.Use();
            Chunks.Draw();
            PH_Man.GrayInter.ChangeData(LInterData.LIT0());
        }
        private void Frame_Box()
        {
            PH_Man.AxisBoxShader.Use();
            Box_Buffer.BindList();
            Box_Buffer.Draw();
        }
        private void Frame_Text()
        {
            //Text_Buffer.Bind_Strings();
            //Text_Buffer.Test_Characters();
            //Text_Buffer.Test_ASCII();
            //Text_Buffer.Test_Direction();
            //Text_Buffer.Test_Char();

            Text_Buffer.Bind_Strings();

            UI_Man.TextShader.Use();
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

            GL.Clear(ClearBufferMask.DepthBufferBit);
            Frame_Inv();

            Frame_Text();
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

                this.PolyHedras_ConCat(bodys_Meta);
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
                this.PolyHedras_ConCat(DATA_Thing.Interpret.GetBodys());

                for (int i = 0; i < fileDatas.Length; i++) { DATA_Recipy.Interpret.SetFile(fileDatas[i], Things); }
                Recipys = DATA_Recipy.Interpret.GetRecipy();

                for (int i = 0; i < fileDatas.Length; i++) { BLD_Base.Interpreter.SetFile(fileDatas[i], Things); }
                Templates = BLD_Base.Interpreter.GetTemplates();
                this.PolyHedras_ConCat(BLD_Base.Interpreter.GetBodys());

                for (int i = 0; i < fileDatas.Length; i++) { Chunk2D.Interpret.SetFile(fileDatas[i], Things); }
                Chunk2D.LayerGen = Chunk2D.Interpret.GetLayers();

                for (int i = 0; i < fileDatas.Length; i++) { Chunk2D.SURF_Object.Interpret.SetFile(fileDatas[i], Things); }
                Chunk2D.SurfThingTemplates = Chunk2D.SURF_Object.Interpret.GetTemplates();
                this.PolyHedras_ConCat(Chunk2D.SURF_Object.Interpret.GetBodys());
            }

            BodyStatic[] bodys_things;
            BodyStatic[] bodys_buildings;
            BodyStatic[] bodys_objects;

            {
                bodys_things = DATA_Thing.Interpret.GetBodys();
                bodys_buildings = BLD_Base.Interpreter.GetBodys();
                bodys_objects = Chunk2D.SURF_Object.Interpret.GetBodys();
            }

            DATA_Thing.IconScales_Create(DATA_Thing.Interpret.GetBodys());

            {
                DATA_Thing.Interpret.Delete();
                DATA_Recipy.Interpret.Delete();
                BLD_Base.Interpreter.Delete();
                Chunk2D.Interpret.Delete();
                Chunk2D.SURF_Object.Interpret.Delete();
            }

            Engine3D.ConsoleLog.LogDone("Sorting Bodys");

            {
                this.PH_3D = new PolyHedraInstance_3D_Array(this.PolyHedras);
                this.PH_UI = new UIBody_Array(this.PolyHedras);
            }
        }
        private void BodysNull()
        {
            //DATA_Thing.Bodys = null;
            //BLD_Base.Bodys = null;
            //IO_Port.Bodys = null;
            //Chunk2D.SURF_Object.Bodys = null;

            //Inventory_Interface.Meta_Bodys = null;
            //Inventory_Interface.BLD_Bodys = null;
            //Inventory_Interface.DATA_Thing_Bodys = null;
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
            TransPorter.Create();

            //TransPorter.Program.UniProj(new RenderDepthFactors(view.DepthN, view.DepthF), view.Fov);
            //view.UniDepth(TransPorter.Program);
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

            Text_Buffer = new TextBuffer();
            //Text_Buffer.Telematry = new Telematry.TelematryBuffer();
            Text_Buffer.Bind_Pallets();

            Box_Buffer = new AxisBoxBuffer();
            //Box_Buffer.Telematry = new Telematry.TelematryBuffer();



            Chunk2D_Graphics.Chunk_Shader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "Chunk2D/Chunk2D.vert"),
                ShaderCode.FromFile(shaderDir + "Chunk2D/Chunk2D.geom"),
                ShaderCode.FromFile(shaderDir + "Chunk2D/Chunk2D.frag"),
            });

            Chunk2D_Graphics.UniChunk_TileSize = new GenericDataUniform<Chunk2D_Int>("tiles_size");
            Chunk2D_Graphics.UniChunk_TilesPerSide = new GenericDataUniform<Chunk2D_Int>("tiles_per_side");
            Chunk2D_Graphics.UniChunk_Pos = new GenericDataUniform<Chunk2D_Pos>("chunk_pos");

            IO_TransPorter.Collection.Shader = new GenericShader(new ShaderCode[]
            {
                ShaderCode.FromFile(shaderDir + "TransPorter/TP.vert"),
                ShaderCode.FromFile(shaderDir + "TransPorter/TP.geom"),
                ShaderCode.FromFile(shaderDir + "TransPorter/TP.frag"),
            });

            PH_Man = new PolyHedra_Shader_Manager(
                shaderDir,
                new GenericShader[]{
                    Chunk2D_Graphics.Chunk_Shader,
                    IO_TransPorter.Collection.Shader,
                },
                new GenericUniformBase[]
                {
                    Chunk2D_Graphics.UniChunk_TileSize,
                    Chunk2D_Graphics.UniChunk_TilesPerSide,
                    Chunk2D_Graphics.UniChunk_Pos
                }
            );
            PH_Man.Depth.ChangeData(view.Depth);
            Chunk2D_Graphics.UniChunk_TileSize.ChangeData(new Chunk2D_Int(Chunk2D.Tile_Size));
            Chunk2D_Graphics.UniChunk_TilesPerSide.ChangeData(new Chunk2D_Int(Chunk2D.Tiles_Per_Side));

            //PolyHedra cube = PolyHedra.Generate.Cube();
            UI_Man = new UserInterfaceManager(shaderDir);
            //UI_Man.Depth.ChangeData(new DepthData(-1000, +1000));
            UI_Spin = new Angle3D(0, -0.5f, 0);

            Ref_Graphics();

            {
                float dist = 0.0f;

                TestBodys = new EntryContainerBase<PolyHedraInstance_3D_Data>.Entry[this.PH_3D.Length];
                for (int i = 0; i < this.TestBodys.Length; i++)
                {
                    AxisBox3D box = this.PolyHedras[i].CalcBox();
                    ConsoleLog.Log("Box[" + i + "]" + box.Min.Y + " : " + box.Max.Y + " | " + dist);

                    dist -= box.Min.Y;

                    TestBodys[i] = this.PH_3D[i].Alloc(1);
                    TestBodys[i][0] = new PolyHedraInstance_3D_Data(new Transformation3D(new Point3D(dist + 10, 0, 120)));

                    dist += box.Max.Y;
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

            view.Depth = new DepthData(1.0f, 1000.0f);


            IO_Port.game = this;
            DATA_Thing.game = this;
            BLD_Base.game = this;
            Chunk2D.game = this;
            Inventory_Interface.game = this;
            IO_TransPorter.game = this;


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
