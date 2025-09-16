
using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics.Display;
using Engine3D.Miscellaneous.EntryContainer;
using Engine3D.Graphics.Display2D.UserInterface;
using Engine3D.Graphics.Display3D;

using VoidFactory.Production.Data;
using VoidFactory.Production.Transfer;
using VoidFactory.Production.Buildings;
using VoidFactory.Surface2D;
using Engine3D.Graphics;

namespace VoidFactory.Inventory
{
    abstract class Inter_Surface2D_Hit : Interaction
    {
        public static Chunk2D.Collection Chunks;

        protected Chunk2D.SurfaceHit Hit;

        public override void Update()
        {
            Hit = Chunks.CrossSurface(View.Ray);
        }
        public override void Draw()
        {
            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.HoriL, 0, -9, 0xFFFFFF,
            //    "Hit:\n" + Hit);

            Text_Buffer.InsertBR(
                (0, -9), Text_Buffer.Default_TextSize, 0xFFFFFF,
                "Hit:\n" + Hit);
        }
    }
    class Inter_Surf2D_Object : Inter_Surface2D_Hit
    {
        private float Icon_Scale;
        private Chunk2D.SURF_Object.Template Template;
        private Transformation3D Trans;

        

        public Inter_Surf2D_Object(Chunk2D.SURF_Object.Template template)
        {
            Template = template;

            //Icon_Scale = (float)(0.2 / (Chunk2D.SURF_Object.Bodys[Template.Idx2].BoxFit().MaxSideLen()));
            Icon_Scale = (float)(0.2 / (Chunk2D.game.PH_3D[Template.Idx2].BoxFit().MaxSideLen()));
        }

        public override void Draw_Icon_Alloc(UIGridPosition pos, UIGridSize size)
        {

        }

        public override void Draw_Alloc()
        {
            base.Draw_Alloc();
        }
        public override void Draw_Dispose()
        {
            base.Draw_Dispose();
        }

        public override void Update()
        {
            base.Update();

            if (Hit.IsValid())
            {
                Trans = new Transformation3D(Hit.ChunkTile_Hit.TileLayer_Hit.Tile_Hit.Center, Angle3D.Default());
            }
            else
            {
                Trans = Transformation3D.Default();
            }
        }
        public override void Draw()
        {
            base.Draw();

            if (Hit.IsValid())
            {
                //Template.Draw(Graphic.Trans_Direct, Trans);
                //Template.Draw(MainContext.Shader_Default, Trans);
            }

            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.HoriL, 0, -4, 0xFFFFFF,
            //    "Template:\n" + Template);
            //MainContext.Text_Buff.InsertTL(
            //    (+0.5f, -0.5f), 0xFFFFFF, 20f, 2f,
            //    "Generate:\n" + Template);
            Text_Info("Generate:\n" + Template);

            //Graphic.Icon_Prog.UniPos(+0.75f, -0.75f);
            //Graphic.Icon_Prog.UniScale(0.01f);
            //Template.Draw();
            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.BR, 0, 0, 0xFFFFFF, "Surf Thing");
            Text_Buffer.InsertBR(
                (0, +1), Text_Buffer.Default_TextSize, 0xFFFFFF,
                "Surf Thing");
        }
        public override void Draw_Inv_Icon(float x, float y)
        {
            //Graphic.Icon_Prog.UniScale(Icon_Scale);
            //Graphic.Icon_Prog.UniPos(x, y);
            //Template.Draw();
        }

        public override void Func1()
        {
            Chunks.AddThing(Hit, Template.ToInstance2(Trans, 1.0, Hit.ToTileIndex()));
        }
    }
    class Inter_Surf2D_Tile : Inter_Surface2D_Hit
    {
        private UI_3D_Base TileHoverContent;

        private Entry_Array InstTile;
        private Entry_Array InstOre;

        public override void Draw_Icon_Alloc(UIGridPosition pos, UIGridSize size)
        {
            InstRef = new UI_Meta(pos, size, IO_Port.MetaBodyIndex.Axis);
        }
        public override void Draw_Dispose()
        {
            if (InstTile != null)
            {
                InstTile.Dispose();
                InstTile = null;
            }

            if (InstOre != null)
            {
                InstOre.Dispose();
                InstOre = null;
            }
        }

        public override void Draw()
        {
            base.Draw();

            if (InstTile != null)
            {
                InstTile.Dispose();
                InstTile = null;
            }

            if (Hit.ChunkTile_Hit.TileLayer_Hit.Tile_Hit.Valid)
            {
                //Graphic.Trans_Direct.UniTrans(new RenderTrans(Hit.ChunkTile_Hit.TileLayer_Hit.Tile_Hit.Cross));
                BodyUni_Shader.Use();
                BodyUni_Shader.Trans.Value(new Transformation3D(Hit.ChunkTile_Hit.TileLayer_Hit.Tile_Hit.Cross));
                //IO_Port.BodyAxis.DrawMain();
                IO_Port.game.PH_3D[(int)IO_Port.MetaBodyIndex.Axis].DrawMain();

                if (Hit.ToLayerIndex().IsValid())
                {
                    //Inventory_Storage.Draw(Chunk2D.LayerGen[Hit.ToLayerIndex().idx].Thing);
                    InstTile = Inventory_Storage.Alloc_Thing(Chunk2D.LayerGen[Hit.ToLayerIndex().idx].Thing, 1);
                }

                AxisBox3D box = Chunks.TileBox(Hit);
                if (box != null)
                {
                    //BoundBox.BoxRenderData[] boxR = new BoundBox.BoxRenderData[1];
                    //boxR[0] = new BoundBox.BoxRenderData(box, 0xFFFFFF);
                    //Graphic.Box_Buff.Data(boxR);
                    //Box_Buffer.Bind(boxR);
                    Box_Buffer.Insert(box, 0xFFFFFF);
                }

                Chunk2D.SURF_Object thing = Chunks.FindThing(Hit);
                if (thing != null)
                {
                    //Inventory_Storage.Draw(thing.Content, 1);
                }
            }

            if (InstOre != null)
            {
                for (int i = 0; i < InstOre.Length; i++)
                {
                    InstOre[i].Dispose();
                }
                InstOre = null;
            }

            string strInfo = "\n";
            if (Hit.IsValid())
            {
                Chunk2D chunk = Chunks.FindChunk(Hit.Chunk_Idx.y, Hit.Chunk_Idx.c);
                if (chunk != null)
                {
                    strInfo += chunk.ToChunkInfo() + "\n";
                }

                Chunk2D.SURF_Object thing = Chunks.FindThing(Hit);
                if (thing != null)
                {
                    strInfo += thing.ToString();

                    InstOre = Inventory_Storage.Alloc_Buffer(thing.Content, 2);
                }
            }

            Text_Info("Tile Hit: " + Hit.ToString() + strInfo);
            Text_Type("Tile Interact");
        }

        public override void Func1()
        {
            DATA_Cost cost;

            cost = Chunks.SubThing(Hit);
            if (cost != null)
            {
                Inventory_Storage.CostRefund(cost);
                return;
            }

            if (!Hit.ToLayerIndex().IsValid())
                return;

            cost = new DATA_Cost(Chunk2D.LayerGen[Hit.ToLayerIndex().idx].Thing, 1);
            if (Inventory_Storage.CostCanDeduct(cost))
            {
                Inventory_Storage.CostDeduct(cost);
                Chunks.Inc(Hit);
            }
        }
        public override void Func2()
        {
            //Engine3D.ConsoleLog.Log("Try Tile Dec()");
            DATA_Cost cost;

            cost = Chunks.SubThing(Hit);
            if (cost != null)
            {
                Inventory_Storage.CostRefund(cost);
                return;
            }
            //Engine3D.ConsoleLog.Log("Valid Free");

            if (!Hit.ToLayerIndex().IsValid())
                return;
            //Engine3D.ConsoleLog.Log("Valid Layer");

            cost = new DATA_Cost(Chunk2D.LayerGen[Hit.ToLayerIndex().idx].Thing, 1);
            if (Inventory_Storage.CostCanRefund(cost))
            {
                //Engine3D.ConsoleLog.Log("Valid cost");
                Inventory_Storage.CostRefund(cost);
                Chunks.Dec(Hit);
            }
        }
    }
    class Inter_Surf2D_Rad : Inter_Surface2D_Hit
    {
        public override void Draw_Icon_Alloc(UIGridPosition pos, UIGridSize size)
        {
            InstRef = new UI_Meta(pos, size, IO_Port.MetaBodyIndex.Axis);
        }

        public override void Draw()
        {
            base.Draw();

            if (Hit.ChunkTile_Hit.TileLayer_Hit.Tile_Hit.Valid)
            {
                //Graphic.Trans_Direct.UniTrans(new RenderTrans(Hit.ChunkTile_Hit.TileLayer_Hit.Tile_Hit.Cross));
                BodyUni_Shader.Use();
                BodyUni_Shader.Trans.Value(new Transformation3D(Hit.ChunkTile_Hit.TileLayer_Hit.Tile_Hit.Cross));
                //IO_Port.BodyAxis.DrawMain();
                IO_Port.game.PH_3D[(int)IO_Port.MetaBodyIndex.Axis].DrawMain();

                AxisBox3D box = Chunks.TileBox(Hit);
                if (box != null)
                {
                    //BoundBox.BoxRenderData[] boxR = new BoundBox.BoxRenderData[1];
                    //boxR[0] = new BoundBox.BoxRenderData(box, 0x000000);
                    //Graphic.Box_Buff.Data(boxR);
                    //Box_Buffer.Bind(boxR);
                    Box_Buffer.Insert(box, 0x000000);
                }
            }

            Text_Info("Tile Hit: " + Hit.ToString());
            Text_Type("Tile Interact Radius");
        }

        public override void Func1()
        {
            if (Hit.IsValid())
                Chunks.RadInc(Hit, 100);
        }
        public override void Func2()
        {
            if (Hit.IsValid())
                Chunks.RadDec(Hit, 100);
        }
    }
    class Inter_Surf2D_Building : Inter_Surface2D_Hit
    {
        public static BLD_Base.Collection Buildings;

        private float Icon_Scale;
        private BLD_Base.Template_Base Template;
        private Transformation3D Trans;
        private int angle;

        private Entry_Array Cost_Insts;
        private EntryContainerDynamic<PolyHedraInstance_3D_Data>.Entry InstEntry;

        public Inter_Surf2D_Building(BLD_Base.Template_Base template)
        {
            Template = template;

            //Icon_Scale = (float)(0.2 / (BLD_Base.Bodys[Template.Idx].BoxFit().MaxSideLen()));
            Icon_Scale = (float)(0.2 / (BLD_Base.game.PH_3D[Template.Idx].BoxFit().MaxSideLen()));
        }

        public override void Draw_Icon_Alloc(UIGridPosition pos, UIGridSize size)
        {
            InstRef = new UI_Building(pos, size, Template);
        }
        public override void Draw_Alloc()
        {
            if (Cost_Insts != null)
            {
                Cost_Insts.Dispose();
                Cost_Insts = null;
            }

            Cost_Insts = Inventory_Storage.Alloc_Cost_Inn(Template.Cost);

            InstEntry = BLD_Base.game.PH_3D[Template.Idx].Alloc(1);
            InstEntry[0] = new PolyHedraInstance_3D_Data(Transformation3D.Default());
        }
        public override void Draw_Dispose()
        {
            if (Cost_Insts != null)
            {
                Cost_Insts.Dispose();
                Cost_Insts = null;
            }

            InstEntry.Dispose();
            InstEntry = null;
        }

        public override void Update()
        {
            base.Update();

            PolyHedraInstance_3D_Data data = InstEntry[0];

            if (Hit.IsValid())
            {
                Trans = new Transformation3D(Hit.ChunkTile_Hit.TileLayer_Hit.Tile_Hit.Center, new Angle3D((angle / 4.0) * Angle3D.Deg360, 0, 0));
                data.Trans = Trans;
            }
            else
            {
                Trans = Transformation3D.Default();
                data.Trans = Transformation3D.Null();
            }

            InstEntry[0] = data;
        }
        public override void Draw()
        {
            base.Draw();

            if (Hit.IsValid())
            {
                //Template.Draw(Graphic.Trans_Direct, Graphic.Trans_Direct, Trans);
                //Template.Draw(MainContext.Shader_Default, MainContext.Shader_Default, Trans);
            }

            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.HoriL, 0, -4, 0xFFFFFF,
            //    "Template:\n" + Template);
            //MainContext.Text_Buff.InsertBL(
            //    (+0.5f, -0.5f), 0xFFFFFF, 20f, 2f,
            //    "Template:\n" + Template);
            Text_Info("Template:\n" + Template);

            //Graphic.Icon_Prog.UniPos(+0.75f, -0.75f);
            //Graphic.Icon_Prog.UniScale(0.01f);
            //Template.Draw();
            //MainContext.Text_Buff.Insert(TextBuffer.ScreenCorner.BR, 0, 0, 0xFFFFFF, "Template");
            //MainContext.Text_Buff.InsertBR(
            //    (-0.5f, +0.5f + 1), 0xFFFFFF, 20f, 2f,
            //    "Template");
            Text_Type("Template");

            //Inventory_Storage.DrawInn(Template.Cost);
        }
        public override void Draw_Inv_Icon(float x, float y)
        {
            //Graphic.Icon_Prog.UniScale(Icon_Scale);
            //Graphic.Icon_Prog.UniPos(x, y);
            //Template.Draw();
        }

        public override void Func1()
        {
            if (Hit.IsValid())
            {
                BLD_Base bld = null;

                if (Template.GetType() == typeof(BLD_Converter.Template))
                    bld = ((BLD_Converter.Template)Template).ToInstance(Trans);
                else if (Template.GetType() == typeof(BLD_Relay.Template))
                    bld = ((BLD_Relay.Template)Template).ToInstance(Trans);
                else if (Template.GetType() == typeof(BLD_Surf_Collector.Template))
                    bld = ((BLD_Surf_Collector.Template)Template).ToInstance(Trans, Hit.Chunk_Idx, Hit.ToTileIndex(), 3);

                Buildings.Add(bld);
            }
        }
        public override void Func2()
        {
            angle++;
            angle = angle & 0b11;
        }
    }
}
