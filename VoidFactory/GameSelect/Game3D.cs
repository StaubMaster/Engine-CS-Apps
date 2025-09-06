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
            public int Tick;

            public Point3D Solar;

            public Ray3D View_Ray;

            public bool Draw_Gray;
            public int Draw_Gray_Exclude_Idx;
            public bool Draw_Ports;

            public IconProgram Icon_Prog;
            private Angle3D Icon_Spin;
            public float[] Icon_Spin_flt;

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

            public void UpdateView(DisplayCamera view)
            {
                //Trans_Light_Norm.UniProj(depth, fov);
                //Trans_Light_Gray.UniProj(depth, fov);
                //Trans_Direct.UniProj(depth, fov);
            }
            public void Update(DisplayCamera view)
            {
                Tick++;

                //RenderTrans viewRender = new RenderTrans(view.Trans);
                //Trans_Light_Norm.UniView(viewRender);
                //Trans_Light_Gray.UniView(viewRender);
                //Trans_Direct.UniView(viewRender);

                {
                    Icon_Spin.A = Tick / 64.0;
                    int idx = 0;
                    Icon_Spin.FloatsSinCos(Icon_Spin_flt, ref idx);
                    Icon_Prog.UniRot(Icon_Spin_flt);
                }
            }
        }
        /*
        public class WorldData
        {
            public Chunk2D[] chunks;
            public BLD_Base.Collection Buildings;
            public IO_TransPorter.Collection TransPorter;            
        }
        */
    }
}
