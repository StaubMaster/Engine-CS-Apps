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
        /*  put in here
         *  PolyHedra_3D Stuff
         *  PolyHedra_UI Stuff
         *  Text Stuff
         */

        /* PolyHedra Array
         *  right now Meta, Buildings and Things are seperate but do the same thing
         *  put them all together
         *  
         *  PolyHedras are used for 3D and UI
         *  have PolyHedra[] of all PolyHedras that are used
         */

        protected DisplayArea win;
        protected DisplayCamera view;

        protected Action ExternDelete;
        protected Action<string> CommandFunction;

        protected bool Running;

        public PolyHedra[] PolyHedras;

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
    }
}
