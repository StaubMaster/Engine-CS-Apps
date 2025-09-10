using System;
using System.Collections.Generic;

using Engine3D.Graphics;
using Engine3D.Graphics.Display;

using VoidFactory.GameSelect;
using VoidFactory.Production.Transfer;

namespace VoidFactory.Inventory
{
    abstract class Interaction
    {
        public static Game3D.GraphicsData Graphic;
        public static BodyElemUniShader BodyUni_Shader;
        public static TextBuffer Text_Buffer;
        public static AxisBoxBuffer Box_Buffer;

        public virtual void Init()
        {
            Graphic.Draw_Gray = false;
            Graphic.Draw_Ports = false;
        }

        public abstract void Update();
        public abstract void Draw();
        public virtual void Draw_Inv_Icon(float x, float y)
        {

        }

        protected void Text_Type(string type)
        {
            Text_Buffer.InsertBR(
                (0, +1), Text_Buffer.Default_TextSize, 0xFFFFFF,
                type);
        }
        protected void Text_Info(string info)
        {
            Text_Buffer.InsertBL(
                (0, 0), Text_Buffer.Default_TextSize, 0xFFFFFF,
                info);
        }

        public virtual void Func1()
        {

        }
        public virtual void Func2()
        {

        }

        public static void Draw_Inv_Box(float x, float y)
        {
            Graphic.Icon_Prog.UniScale(0.025f);
            Graphic.Icon_Prog.UniPos(x, y);
            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);
            //IO_Port.BodyHex.DrawMain();
            IO_Port.Bodys[(int)IO_Port.MetaBodyIndex.Hex].DrawMain();
        }
    }
}
