using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Engine3D;
using Engine3D.Abstract3D;
using Engine3D.GraphicsOld;
using Engine3D.GraphicsOld.Forms;
using Engine3D.OutPut;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics.Display;

using VoidFactory.Production;

namespace VoidFactory.GameSelect
{
    abstract class Game3D
    {
        protected DisplayArea win;
        protected DisplayCamera view;

        protected Action ExternDelete;
        protected Action<string> CommandFunction;

        protected bool Running;

        protected Game3D(Action externDelete)
        {
            ExternDelete = externDelete;
            CommandFunction = null;
            Running = false;
        }

        protected abstract void Frame();

        public virtual void Create()
        {
            //win.Create(2000, 1000, ExternDelete, CommandFunction);
            //win.External_Frame = Frame;
            //win = new DisplayArea(2000, 1000, ExternDelete, Frame);
            win = new DisplayArea(640, 480, ExternDelete, Frame);
            view = new DisplayCamera();
        }
        public virtual void Delete()
        {
            //win.Delete();
            win.Term();
            win = null;
            view = null;
        }
        public void Run()
        {
            win.Run();
            win.Term();
        }

        public class GraphicsData
        {
            /*  remove
                    this stuff is mostly used in Inventory ?
                    fix Inventory, replace this, remove this
             */

            public int Tick { get { return _Tick; } set { _Tick = value; } }
            public int _Tick;


            public Ray3D View_Ray { get { return _View_Ray; } set { _View_Ray = value; } }
            public Ray3D _View_Ray;



            public bool Draw_Ports { get { return _Draw_Ports; } set { _Draw_Ports = value; } }
            public bool _Draw_Ports;
            public bool Draw_Gray { get { return _Draw_Gray; } set { _Draw_Gray = value; } }
            public bool _Draw_Gray;
            public int Draw_Gray_Exclude_Idx { get { return _Draw_Gray_Exclude_Idx; } set { _Draw_Gray_Exclude_Idx = value; } }
            public int _Draw_Gray_Exclude_Idx;



            public IconProgram Icon_Prog { get { return _Icon_Prog; } set { _Icon_Prog = value; } }
            public IconProgram _Icon_Prog;
            private Angle3D Icon_Spin { get { return _Icon_Spin; } set { _Icon_Spin = value; } }
            private Angle3D _Icon_Spin;
            public float[] Icon_Spin_flt { get { return _Icon_Spin_flt; } set { _Icon_Spin_flt = value; } }
            public float[] _Icon_Spin_flt;



            public void Create()
            {
                Tick = 0;

                Icon_Prog.Create();
                Icon_Spin = new Angle3D(0, -0.5, 0);
                Icon_Spin_flt = new float[6];
            }
            public void Delete()
            {
                Icon_Prog.Delete();
                //Icon_Spin = null;
                //Icon_Spin_flt = null;
            }

            public void Update()
            {
                Tick++;

                Angle3D spin;
                spin = Icon_Spin;
                spin.A = Tick / 64.0;
                Icon_Spin = spin;

                int idx = 0;
                Icon_Spin.FloatsSinCos(Icon_Spin_flt, ref idx);
                Icon_Prog.UniRot(Icon_Spin_flt);
            }
        }
    }
}
