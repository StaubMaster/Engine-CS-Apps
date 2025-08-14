using System;
using System.Collections.Generic;
using System.IO;

using Engine3D;
using Engine3D.Abstract;
using Engine3D.Abstract.Simple;
using Engine3D.Abstract.Complex;
using Engine3D.Graphics;
using Engine3D.Graphics.Forms;
using Engine3D.Entity;

using VoidFactory.Surface;
using VoidFactory.Production;

namespace VoidFactory.GameSelect
{
    class GamePlaneAction
    {
        public static Game3D.GraphicsData Graphic;
        public static BLD_Base.Collection Building;
        public static IO_TransPorter.Collection TransPorter;
        public static Chunk2D_[] Chunks;

        public GamePlaneAction()
        {
    
        }

        public virtual void Init()
        {
            Graphic.Draw_Gray = false;
            Graphic.Draw_Ports = false;
        }

        public virtual void Update()
        {

        }
        public virtual void Draw()
        {

        }

        public virtual void Func1()
        {

        }
        public virtual void Func2()
        {

        }
    }
    class GameAction_PortConnect : GamePlaneAction
    {
        private IO_Port.Select_Port Hover;
        private IO_Port.Select_Port Select;
        private BodyStatic Body;

        public GameAction_PortConnect(BodyStatic body)
        {
            Hover.Reset();
            Select.Reset();
            Body = body;
        }

        public override void Init()
        {
            Graphic.Draw_Gray = true;
            Graphic.Draw_Gray_Exclude_Idx = -1;
            Graphic.Draw_Ports = true;
        }

        public override void Update()
        {
            Hover = Building.Port_Select(Graphic.View_Ray);
        }
        public override void Draw()
        {
            Graphic.Text_Buff.Insert(TextBuffers.Corner.BotLef, 0, -10, 0xFFFFFF,
                "Hover Port:\n" + Hover);
            Graphic.Text_Buff.Insert(TextBuffers.Corner.BotLef, 0, -4, 0xFFFFFF,
                "Select Port:\n" + Select);

            Hover.Draw_Hover(Graphic.Trans_Direct);
            Select.Draw_Select(Graphic.Trans_Direct);

            Graphic.Icon_Prog.UniPos(-0.9f, -0.0f);
            Graphic.Icon_Prog.UniScale(0.01f);
            Body.BufferDraw();
        }

        public override void Func1()
        {
            if (Select.IsSame(Hover))
            {
                Select.Reset();
            }
            else if (Select.CanConnect(Hover))
            {
                TransPorter.Add(Building.Port_Connect(Hover, Select));

                Select.Reset();
                Hover.Reset();
            }
            else
            {
                Select = Hover;
            }
        }
        public override void Func2()
        {
            Hover.Reset();
            Select.Reset();
        }
    }
    class GameAction_TempConverter : GamePlaneAction
    {
        private BLD_Base.Template Template;
        private Chunk2D_.Tile_Hit Hit;
        private Transformation Trans;
        private int angle;

        public GameAction_TempConverter(BLD_Base.Template temp)
        {
            Template = temp;
            angle = 0;
        }

        public override void Update()
        {
            //Hit = Chunk2D.Tile_Hit.Ray_Hit(Graphic.View_Ray, Chunks);

            if (Hit.Valid)
                Trans = new Transformation(Hit.Point, new Winkl((angle / 4.0) * Winkl.Full, 0, 0));
            else
                Trans = null;
        }
        public override void Draw()
        {
            if (Hit.Valid)
            {
                Template.Draw(Graphic.Trans_Direct, Graphic.Trans_Direct, Trans);
            }
            Graphic.Text_Buff.Insert(TextBuffers.Corner.BotLef, 0, -8, 0xFFFFFF,
                "Template:\n" + Template);

            Graphic.Text_Buff.Insert(TextBuffers.Corner.BotLef, 0, -3, 0xFFFFFF,
                "Hover Tile:\n" + Hit);

            Graphic.Icon_Prog.UniPos(-0.8f, -0.0f);
            Graphic.Icon_Prog.UniScale(0.02f);
            Template.Draw();
        }

        public override void Func1()
        {
            if (Hit.Valid)
            {
                Building.Add(Template.ToInstance(Trans));
            }
        }
        public override void Func2()
        {
            angle++;
            angle = angle & 0b11;
        }
    }
    class GameAction_RemConverter : GamePlaneAction
    {
        private BLD_Base.Select_Building Hover;

        public GameAction_RemConverter()
        {

        }

        public override void Update()
        {
            Hover = Building.Select(Graphic.View_Ray);

            Graphic.Draw_Gray_Exclude_Idx = Hover.Building_Idx;
        }
        public override void Draw()
        {
            Graphic.Text_Buff.Insert(TextBuffers.Corner.BotLef, 0, -2, 0xFFFFFF,
                "Hover Building:\n" + Hover);
            if (Hover.Valid)
            {
                Graphic.Text_Buff.Insert(TextBuffers.Corner.BotLef, 0, -20, 0xFFFFFF,
                    "Building:" + Building.StringOf(Hover.Building_Idx));
                Graphic.Trans_Direct.UniTrans(new RenderTrans(Hover.Pos));
                IO_Port.Bodys[7].BufferDraw();
            }
        }

        public override void Func1()
        {
            if (Hover.Valid)
            {
                Building.Sub(Hover);
                Hover.Reset();
            }
        }
    }
    class GameAction_TempRecipy : GamePlaneAction
    {
        private BLD_Base.Select_Building Hover;
        private IO_Recipy Recipy;

        public GameAction_TempRecipy(IO_Recipy recipy)
        {
            Recipy = recipy;
        }

        public override void Init()
        {
            Graphic.Draw_Gray = true;
        }

        public override void Update()
        {
            Hover = Building.Select(Graphic.View_Ray);

            Graphic.Draw_Gray_Exclude_Idx = Hover.Building_Idx;
        }
        public override void Draw()
        {
            Graphic.Text_Buff.Insert(TextBuffers.Corner.BotLef, 0, -2, 0xFFFFFF,
                "Hover Building:\n" + Hover);
            if (Hover.Valid)
            {
                Graphic.Text_Buff.Insert(TextBuffers.Corner.BotLef, 0, -20, 0xFFFFFF,
                    "Building:" + Building.StringOf(Hover.Building_Idx));
                Graphic.Trans_Direct.UniTrans(new RenderTrans(Hover.Pos));
                IO_Port.Bodys[7].BufferDraw();
            }

            Graphic.Text_Buff.Insert(TextBuffers.Corner.HoriLef, 0, -4, 0xFFFFFF,
                "Recipy:" + Recipy.ToString());
            Recipy.Draw(Graphic.Icon_Prog, Graphic.Tick / 64.0);
        }

        public override void Func1()
        {
            Building.RecipySet(Hover, Recipy);
        }
    }
}
