using System;
using System.Collections.Generic;
using System.IO;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;
using Engine3D.GraphicsOld.Forms;
using Engine3D.Entity;

using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform;
using Engine3D.OutPut.Uniform.Generic.Float;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;
using Engine3D.Graphics.Display;
using Engine3D.Graphics.Telematry;
using Engine3D.Graphics.Manager;

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
    partial class GamePlane : Game3D
    {
        /*  What should this be?
                factory game similar to factorio
                non-flat plane
                Tools?
                Minecraft-y feel
                nice looking terrain

            Surface:
                different layers made of different materials stacked on top of each other
            Stuff on Terrain:
                Ore / Veins
                [Trees / Grass]
            Automation:
                Collection:
                    Miner: (from Stuff on Surface)
                        get Ore
                        [get Wood]
                    Miner: (from Surface)
                        get Dirt
                        get Clay
                        get Stone
                Processing:
                    Crusher:
                        Ore to Powder and Stone
                        [Stone to Sand]
                    Smelter:
                        Powder to Bar
                        [Powder to Simple Components]
                        Clay to Brick
                    Assambler:
                        Bar to Simple Components
                        Simple Components to Complex Components
                    Crystalizer:
                        [Sand to Quartz]
                        [Powder to more Bars]
         */

        /*  TO ADD:
                GUI ?
                Save/Load

                Layers:
                    Sand:
                        Glass ?
                        Bottles ?

                Plants:
                    Kork:
                        Leather:
                            Belts ?
                    Apple:
                        Alcohol ?
                    Wheat:
                        Bread ?

                    Piler
                        throws Layer Material on a Pile
         */

        /*  Purpose of Material:

                Clay / Brick  :  Building
                    Building Base
                    Wall

                Iron
                    RealLife: building Material
                    Used for:
                        Mashines / Construction
                        Steel
                    Variants:
                        Iron Ore
                        Iron Pile
                        Iron Nugget
                        Iron Bar
                        Iron Stack  (not sure if needed)
                        Iron Wire   (not sure if needed) (Paperclips?)
                        Iron Coil   (not sure if needed) (Spring)
                        Iron Gear
                        Iron Pipe   fluids

                Copper
                    RealLife: Conducts
                    Used for:
                        Electronics
                        Coil
                            for Floating (Relays)
                    Variants:
                        Copper Ore
                        Copper Pile
                        Copper Nugget
                        Copper Bar      (not sure if needed)
                        Copper Stack    (not sure if needed)
                        Copper Wire
                        Copper Coil
                        Copper Gear
                        Copper Pipe     fluids
        */

        private GraphicsData Graphic;
        private BodyElemUniShader BodyUni_Shader;
        private TextShader Text_Shader;
        private TextBuffer Text_Buffer;
        private AxisBoxShader Box_Shader;
        private AxisBoxBuffer Box_Buffer;

        /*
         *
         */

        private PolyHedraTestManager PolyHedraMan;
        private BodyElemInstBuffer InstBuffer;


        private DisplayPolyHedra TestTower;

        private Chunk2D.Collection Chunks;
        private Chunk2D_Shader Chunk_Shader;



        private Cache<string, BodyStatic> Static_Body_Cache;

        private DATA_Thing[] Things;
        private BLD_Base.Template_Base[] Templates;
        private DATA_Recipy[] Recipys;

        public BLD_Base.Collection Buildings;
        public IO_TransPorter.Collection TransPorter;



        public GamePlane(Action externDelete) : base(externDelete)
        {
            Graphic = new GraphicsData();

            Graphic.Icon_Prog = new IconProgram("Icon",
                "Icon/Icon.vert",
                "Geom_Norm_Color.geom",
                "Frag/Light.frag");
        }


        private void EntitiesUpdate()
        {
            Buildings.Update();
            TransPorter.Update();

            string str = "";
            str += Buildings.Count() +   ":  Converter\n";
            str += TransPorter.Count() + ":TransPorter\n";
            //Text_Buff.Insert(TextBuffer.ScreenCorner.TR, 0, 0, 0xFFFFFF, str);
            Text_Buffer.InsertTR(
                (0, 0), Text_Buffer.Default_TextSize, 0xFFFFFF,
                str);
        }
        private void EntitiesDraw()
        {
            //if (!Graphic.Draw_Gray)
            //if (Graphic.Draw_Ports)

            //Buildings.Draw(Shader_Default, Graphic.Draw_Gray_Exclude_Idx);
            //TransPorter.Draw(Shader_Default);

            Buildings.Draw(BodyUni_Shader, Graphic.Draw_Gray_Exclude_Idx);
            TransPorter.Draw(BodyUni_Shader);
        }

        private void BodysDraw()
        {
            Transformation3D trans = new Transformation3D();
            trans.Rot = new Angle3D((Graphic.Tick * Angle3D.Full) / 512, 0, 0);
            trans.Pos.X = 0;

            BodyUni_Shader.Use();

            DisplayPolyHedra[] bodys;

            trans.Pos.C = 120;
            bodys = new DisplayPolyHedra[]
            {
                IO_Port.BodyError,
                IO_Port.BodyTransPorter,
                IO_Port.BodyAxis,

                IO_Port.BodyHex,
                IO_Port.BodyOct,

                IO_Port.BodyInn,
                IO_Port.BodyInnHex,
                IO_Port.BodyInnOct,

                IO_Port.BodyOut,
                IO_Port.BodyOutHex,
                IO_Port.BodyOutOct,
            };

            for (int i = 0; i < bodys.Length; i++)
            {
                trans.Pos.Y = i * 10;
                BodyUni_Shader.Trans.Value(trans);
                bodys[i].Draw();
            }

            trans.Pos.C = 140;
            for (int i = 0; i < DATA_Thing.Bodys.Length; i++)
            {
                trans.Pos.Y = i * 10;
                BodyUni_Shader.Trans.Value(trans);
                DATA_Thing.Bodys[i].BufferDraw();
            }

            trans.Pos.C = 170;
            for (int i = 0; i < BLD_Base.Bodys.Length; i++)
            {
                trans.Pos.Y = i * 30;
                BodyUni_Shader.Trans.Value(trans);
                BLD_Base.Bodys[i].BufferDraw();
            }
        }
        private void DrawManTest()
        {
            PolyHedraMan.SoloShader.Use();
            PolyHedraMan.ScreenRatio.ChangeData(win.Size_Float2());
            PolyHedraMan.View.ChangeData(view.Trans);

            List<BodyElemInstBuffer.STrans3D> instTrans = new List<BodyElemInstBuffer.STrans3D>();

            Transformation3D trans = new Transformation3D();
            trans.Rot = new Angle3D((Graphic.Tick * Angle3D.Full) / 512, 0, 0);
            trans.Pos.X = 10;

            DisplayPolyHedra[] bodys;

            trans.Pos.C = 120;
            bodys = new DisplayPolyHedra[]
            {
                IO_Port.BodyError,
                IO_Port.BodyTransPorter,
                IO_Port.BodyAxis,

                IO_Port.BodyHex,
                IO_Port.BodyOct,

                IO_Port.BodyInn,
                IO_Port.BodyInnHex,
                IO_Port.BodyInnOct,

                IO_Port.BodyOut,
                IO_Port.BodyOutHex,
                IO_Port.BodyOutOct,
            };

            for (int i = 0; i < bodys.Length; i++)
            {
                trans.Pos.Y = i * 10;
                PolyHedraMan.Trans.ChangeData(trans);
                bodys[i].Draw();
                instTrans.Add(new BodyElemInstBuffer.STrans3D(trans));
            }

            trans.Pos.C = 140;
            for (int i = 0; i < DATA_Thing.Bodys.Length; i++)
            {
                trans.Pos.Y = i * 10;
                PolyHedraMan.Trans.ChangeData(trans);
                DATA_Thing.Bodys[i].BufferDraw();
                instTrans.Add(new BodyElemInstBuffer.STrans3D(trans));
            }

            trans.Pos.C = 170;
            for (int i = 0; i < BLD_Base.Bodys.Length; i++)
            {
                trans.Pos.Y = i * 30;
                PolyHedraMan.Trans.ChangeData(trans);
                BLD_Base.Bodys[i].BufferDraw();
                instTrans.Add(new BodyElemInstBuffer.STrans3D(trans));
            }

            InstBuffer.Bind_InstTrans(instTrans.ToArray());
            PolyHedraMan.InstShader.Use();
            InstBuffer.Draw();
        }

        private void Frame_Solar()
        {
            //solar = !(new Punkt(-1, +3, -2));
            Graphic.Solar = !(new Point3D(-2, +1, 0));
            Graphic.Solar = Graphic.Solar + new Angle3D(Graphic.Tick / 255.0, 0, 0);
        }
        private void Frame_View()
        {
            TMovement.FlatX(ref view.Trans, win.MoveByKeys(), win.SpinByMouse());
            view.Update(win.MouseRay());
            //Text_Buff.Insert(TextBuffer.ScreenCorner.TL, 0, 0, 0xFFFFFF, view.ToString());
            string format = "+0000.00;-0000.00; 0000.00";
            {
                string str = "";
                str += "Pos.Y:" + view.Trans.Pos.Y.ToString(format) + "\n";
                str += "Pos.X:" + view.Trans.Pos.X.ToString(format) + "\n";
                str += "Pos.C:" + view.Trans.Pos.C.ToString(format);
                Text_Buffer.InsertTL(
                    (0, 0), Text_Buffer.Default_TextSize, 0xFFFFFF,
                    str);
            }

            if (!win.CheckKey(Keys.Q).IsPressed()) { Graphic.View_Ray = view.Trans.ToRay(); }

            Graphic.Update(view);
            //  Update View (and Solar)

            BodyUni_Shader.Use();
            BodyUni_Shader.View.Value(view.Trans);
            BodyUni_Shader.ScreenRatio.Value(win.Size_Float2());

            //TransPorter.Program.UniView(new RenderTrans(view.Trans));
            //view.UniTrans(TransPorter.Program);
            TransPorter.Program.UniView(new RenderTrans(view.Trans));
        }
        private void Frame_Inv()
        {
            Inventory_Interface.Tool_Change((int)-win.MouseScroll());

            if (win.CheckKey(Keys.PageUp).IsPressed()) { Inventory_Interface.Cat_Next(); }
            if (win.CheckKey(Keys.PageDown).IsPressed()) { Inventory_Interface.Cat_Prev(); }

            if (win.CheckKey(MouseButton.Left).IsPressed()) { Inventory_Interface.Tool_Func1(); }
            if (win.CheckKey(MouseButton.Right).IsPressed()) { Inventory_Interface.Tool_Func2(); }

            Inventory_Interface.Tool_Update();

            if (!win.MouseLocked)
            {
                Inventory_Interface.Cat_Draw();
            }

            Inventory_Interface.Tool_Draw();
        }
        private void Frame_Draw()
        {
            EntitiesUpdate();
            EntitiesDraw();
            BodysDraw();
        }
        private void Frame_Chunk()
        {
            //Chunk_Prog_Norm.UniView(new RenderTrans(view.Trans));
            //Chunk_Prog_Gray.UniView(new RenderTrans(view.Trans));
            //view.UniTrans(Chunk_Prog_Norm);
            //view.UniTrans(Chunk_Prog_Gray);

            Chunk_Shader.Use();
            Chunk_Shader.ScreenRatio.Value(win.Size_Float2());
            Chunk_Shader.View.Value(view.Trans);
            if (!Graphic.Draw_Gray)
            {
                Chunk_Shader.GrayInter.T0(1.0f);
            }
            else
            {
                Chunk_Shader.GrayInter.T1(1.0f);
            }
            Chunks.Draw(Chunk_Shader);
            Chunk_Shader.GrayInter.T0(1.0f);

            //Chunk_Prog_Norm.UniView(new RenderTrans(view.Trans));
            //Chunk_Prog_Gray.UniView(new RenderTrans(view.Trans));
            //Chunk_Prog_Norm.UniSolar(Graphic.Solar);
            //Chunk_Prog_Gray.UniSolar(Graphic.Solar);
            //if (!Graphic.Draw_Gray)
            //    Chunks.Draw(Chunk_Prog_Norm);
            //else
            //    Chunks.Draw(Chunk_Prog_Gray);

            BodyUni_Shader.Use();
            if (!Graphic.Draw_Gray)
            {
                BodyUni_Shader.GrayInter.T0(1.0f);
            }
            else
            {
                BodyUni_Shader.GrayInter.T1(1.0f);
            }
            Chunks.Draw(BodyUni_Shader);
            BodyUni_Shader.GrayInter.T0(1.0f);

            //Chunks.Draw(Graphic.Trans_Light_Norm);
            //OAR_Shader.Use();
            //Chunks.Draw(Shader_Default);
            //Chunks.Draw(OAR_Shader);
        }
        private void Frame_Box()
        {
            Box_Shader.Use();
            Box_Shader.ScreenRatio.Value(win.Size_Float2());
            Box_Shader.View.Value(view.Trans);

            Box_Buffer.BindList();
            Box_Buffer.Draw();
            //Box_Buffer.Bind(new BoundBox.BoxRenderData[0]);
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
            //win.UText.BufferFill(Text_Buff);

            //if (Chunk_Save_Load.Check())
            //{
            //    Chunk.File.Save_Text("E:/Programmieren/VS_Code/Spiel Zeug/Maps/Surface/chunk.txt", in chunks[0]);
            //    Chunk.File.Load_Text("E:/Programmieren/VS_Code/Spiel Zeug/Maps/Surface/chunk.txt", out chunks[0]);
            //}

            //TelematryDataBuffer telematry_data_text = Text_Buffer.Telematry.Report();
            //TelematryDataBuffer telematry_data_box = Box_Buffer.Telematry.Report();

            if (win.CheckKey(Keys.F5).IsPressed())
            {
                Init_Shaders();
            }

            (float, float) winSize = win.Size_Float2();
            Text_Buffer.DisplayConverter.X.PixelSize = winSize.Item1;
            Text_Buffer.DisplayConverter.Y.PixelSize = winSize.Item2;

            Frame_Solar();
            Frame_View();

            BodyUni_Shader.Use();
            BodyUni_Shader.Trans.Value(new Transformation3D(new Point3D(-320, 0, -320)));
            TestTower.Draw();

            Frame_Draw();
            Frame_Chunk();
            Frame_Box();

            DrawManTest();


            GL.Clear(ClearBufferMask.DepthBufferBit);
            Frame_Inv();

            /*bool showRenderTelematry = false;
            if (showRenderTelematry)
            {
                Text_Buffer.Insert(
                    new Engine3D.Graphics.TextOrientation(new Normal1Point(-1.0f, 0.0f), Engine3D.Graphics.TextDirection.DiagRC, (+0.5f, 0.0f)),
                    Text_Buffer.Default_TextSize, 0xFFFFFF, telematry_data_text.ToMultiLine());

                Text_Buffer.Insert(
                    new Engine3D.Graphics.TextOrientation(new Normal1Point(0.0f, 0.0f), Engine3D.Graphics.TextDirection.DiagRC, (+0.5f, 0.0f)),
                    Text_Buffer.Default_TextSize, 0xFFFFFF, telematry_data_box.ToMultiLine());
            }*/

            Frame_Text();
        }



        private void Cache_Create()
        {
            TestTower = new DisplayPolyHedra("E:/Zeug/YMT/Tower/YMT/Tower.ymt");

            //string folder = "E:/Programmieren/Spiel Zeug/3D/";
            string folder = "E:/Zeug/YMT/";

            Static_Body_Cache = new Cache<string, BodyStatic>();

            Static_Body_Cache.Insert("RED", folder + "FigurRED.txt");
            Static_Body_Cache.Insert("GRN", folder + "FigurGRN.txt");
            Static_Body_Cache.Insert("BLU", folder + "FigurBLU.txt");

            Static_Body_Cache.Insert("platform0",   folder + "Maschine/Platform.txt");
            Static_Body_Cache.Insert("platform1",   folder + "Maschine/Platform_1.txt");
            Static_Body_Cache.Insert("platform2",   folder + "Maschine/Platform_2.txt");

            Static_Body_Cache.FuncAllIO(BodyStatic.File.Load);
            Static_Body_Cache.Insert("error", BodyStatic.Create.ErrorBody());
            Static_Body_Cache.FuncAllO(BodyStatic.BufferCreate);
        }
        private void Cache_Delete()
        {
            Static_Body_Cache.FuncAllO(BodyStatic.BufferDelete);
            Static_Body_Cache = null;
        }

        private void FromFiles()
        {
            string folder = "E:/Programmieren/VS_Code/OpenTK/VoidFactory/VoidFactory/Config/";
            string[] fileTexts = new string[]
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
            };

            FileInterpret.FileStruct[] fileDatas = new FileInterpret.FileStruct[fileTexts.Length];
            /*for (int f = 0; f < fileDatas.Length; f++)
            {
                fileDatas[f] = new FileInterpret.FileStruct(fileTexts[f]);
            }*/
            fileDatas[0] = new FileInterpret.FileStruct(fileTexts[0]);
            fileDatas[1] = new FileInterpret.FileStruct(fileTexts[1]);
            fileDatas[2] = new FileInterpret.FileStruct(fileTexts[2]);
            fileDatas[3] = new FileInterpret.FileStruct(fileTexts[3]);
            fileDatas[4] = new FileInterpret.FileStruct(fileTexts[4]);
            fileDatas[5] = new FileInterpret.FileStruct(fileTexts[5]);
            fileDatas[6] = new FileInterpret.FileStruct(fileTexts[6]);
            fileDatas[7] = new FileInterpret.FileStruct(fileTexts[7]);
            fileDatas[8] = new FileInterpret.FileStruct(fileTexts[8]);
            fileDatas[9] = new FileInterpret.FileStruct(fileTexts[9]);
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


            DATA_Thing.Interpret.Create();
            for (int i = 0; i < fileDatas.Length; i++)
                DATA_Thing.Interpret.SetFile(fileDatas[i]);
            Things = DATA_Thing.Interpret.GetThings();
            DATA_Thing.Bodys = DATA_Thing.Interpret.GetBodys();
            DATA_Thing.Interpret.Delete();
            DATA_Thing.BodysCreate();


            DATA_Recipy.Interpret.Create();
            for (int i = 0; i < fileDatas.Length; i++)
                DATA_Recipy.Interpret.SetFile(fileDatas[i], Things);
            Recipys = DATA_Recipy.Interpret.GetRecipy();
            DATA_Recipy.Interpret.Delete();


            BLD_Base.Interpreter.Create();
            for (int i = 0; i < fileDatas.Length; i++)
                BLD_Base.Interpreter.SetFile(fileDatas[i], Things);
            Templates = BLD_Base.Interpreter.GetTemplates();
            BLD_Base.Bodys = BLD_Base.Interpreter.GetBodys();
            BLD_Base.Interpreter.Delete();
            BLD_Base.BodysCreate();




            Chunk2D.Interpret.Create();
            for (int i = 0; i < fileDatas.Length; i++)
                Chunk2D.Interpret.SetFile(fileDatas[i], Things);
            Chunk2D.LayerGen = Chunk2D.Interpret.GetLayers();
            Chunk2D.Interpret.Delete();

            Chunk2D.SURF_Object.Interpret.Create();
            for (int i = 0; i < fileDatas.Length; i++)
                Chunk2D.SURF_Object.Interpret.SetFile(fileDatas[i], Things);
            Chunk2D.SURF_Object.Bodys = Chunk2D.SURF_Object.Interpret.GetBodys();

            Chunk2D.SurfThingTemplates = Chunk2D.SURF_Object.Interpret.GetTemplates();
            Chunk2D.SURF_Object.Interpret.Delete();
            Chunk2D.SURF_Object.BodysCreate();
        }

        private void Chunk_Create()
        {
            Chunks = new Chunk2D.Collection();
            Chunks.Create();
        }
        private void Chunk_Delete()
        {
            Chunk2D.SURF_Object.BodysDelete();

            Chunks.Delete();
            Chunks = null;
        }

        private void Entities_Create()
        {
            string ymtDir = "E:/Zeug/YMT/";

            IO_Port.BodyInn = new DisplayPolyHedra(ymtDir + "Meta/Inn.txt");
            IO_Port.BodyInnHex = new DisplayPolyHedra(ymtDir + "Meta/Inn_Box_Hex.txt");
            IO_Port.BodyInnOct = new DisplayPolyHedra(ymtDir + "Meta/Inn_Box_Oct.txt");

            IO_Port.BodyOut = new DisplayPolyHedra(ymtDir + "Meta/Out.txt");
            IO_Port.BodyOutHex = new DisplayPolyHedra(ymtDir + "Meta/Out_Box_Hex.txt");
            IO_Port.BodyOutOct = new DisplayPolyHedra(ymtDir + "Meta/Out_Box_Oct.txt");

            IO_Port.BodyHex = new DisplayPolyHedra(ymtDir + "Meta/Box_Hex.txt");
            IO_Port.BodyOct = new DisplayPolyHedra(ymtDir + "Meta/Box_Oct.txt");

            IO_Port.BodyTransPorter = new DisplayPolyHedra(ymtDir + "Meta/Transporter.txt");
            IO_Port.BodyAxis = new DisplayPolyHedra(ymtDir + "Meta/AxisCross10.txt");
            IO_Port.BodyError = new DisplayPolyHedra("");



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
            //IO_Port.Bodys = null;

            DATA_Thing.BodysDelete();
            BLD_Base.BodysDelete();

            Recipys = null;
            Templates = null;


            Buildings.Delete();
            Buildings = null;

            TransPorter.Delete();
            TransPorter = null;
        }

        private void Ref_Graphics()
        {
            Interaction.Graphic = Graphic;
            Interaction.BodyUni_Shader = BodyUni_Shader;
            Interaction.Text_Buffer = Text_Buffer;
            Interaction.Box_Buffer = Box_Buffer;

            Inter_Port.Buildings = Buildings;
            Inter_Connect.TransPorter = TransPorter;
            Inter_Surface2D_Hit.Chunks = Chunks;
            Inter_Surf2D_Building.Buildings = Buildings;
            Inter_Building.Buildings = Buildings;

            BLD_Surf_Collector.Chunks = Chunks;

            Inventory_Storage.Graphic = Graphic;
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

            Interaction.Graphic = null;
            Inter_Port.Buildings = null;
            Inter_Connect.TransPorter = null;
            Inter_Surface2D_Hit.Chunks = null;
            Inter_Surf2D_Building.Buildings = null;
            Inter_Building.Buildings = null;

            BLD_Surf_Collector.Chunks = null;

            Inventory_Storage.Graphic = null;
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

            PolyHedraMan = new PolyHedraTestManager(shaderDir);
            PolyHedraMan.Depth.ChangeData(view.Depth.Near, view.Depth.Far);

            InstBuffer = new BodyElemInstBuffer();
            IO_Port.BodyHex.PHedra.ToBufferElem(InstBuffer);

            Ref_Graphics();
        }

        public override void Create()
        {
            if (Running) { return; }
            ConsoleLog.Log("Create GamePlane");
            ConsoleLog.TabInc();
            base.Create();

            //view.renderDepth.Calc(1, 1000);
            view.Depth.Near = 1.0f;
            view.Depth.Far = 1000.0f;


            //View_Ray_Key = new KeyToggle(Keys.Q);
            //win.KeyChecks.Add(View_Ray_Key);

            Graphic.Create();
            //Graphic.UpdateView(new RenderDepthFactors(view.DepthN, view.DepthF), view.Fov);
            Graphic.UpdateView(view);

            //DebugReloadShaders = new KeyPress(Keys.F5);
            //win.KeyChecks.Add(DebugReloadShaders);

            FromFiles();

            Cache_Create();
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


            //View_Ray_Key = null;

            Graphic.Delete();


            Cache_Delete();
            Entities_Delete();
            Chunk_Delete();

            Inv_Delete();

            ConsoleLog.TabDec();
            ConsoleLog.Log("");
        }
    }
}
