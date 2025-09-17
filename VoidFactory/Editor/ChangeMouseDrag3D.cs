using System;

using Engine3D.Abstract3D;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;
using Engine3D.Graphics.Display3D;
using Engine3D.Graphics.Display2D.UserInterface;
using Engine3D.Miscellaneous.EntryContainer;

using VoidFactory.Miscellaneous.Display;

namespace VoidFactory.Editor
{
    /* Split
     *  All Move and all Spin should be their own thing ?
     *  mayybe later
     */
    abstract class DragChange_Base
    {

    }
    class DragChange_MoveX : DragChange_Base
    {

    }
    struct ChangeMouseDrag3D
    {
        private enum RotType
        {
            Non = 0,
            Abs = 1,
            Rel = 2,
        }

        public Ray3D ViewRay;

        private RotType Move_RotType;
        private RotType Spin_RotType;
        private bool Move_Snap;
        private bool Spin_Snap;

        private Point3D MoveOrigin;
        private Point3D MoveAxisY;
        private Point3D MoveAxisX;
        private Point3D MoveAxisC;

        private Angle3D SpinOrigin;
        private Angle3D SpinRingA;
        private Angle3D SpinRingS;
        private Angle3D SpinRingD;

        private bool IsNull;

        private Transformation3D[] Indicator_Trans;

        private PolyHedra[] Bodys;
        private PolyHedraInstance_3D_Array Bodys_3D;
        private EntryContainerBase<PolyHedraInstance_3D_Data>.Entry[] Insts_3D;

        private int Hovering;
        private int Selected;
        public bool IsSelected { get { return (Selected != -1); } }
        public bool IsHovering { get { return (Hovering != -1); } }

        public Transformation3D Trans_Changed;

        public ChangeMouseDrag3D(PolyHedra[] bodys)
        {
            ViewRay = new Ray3D();

            Move_RotType = RotType.Non;
            Spin_RotType = RotType.Abs;
            Move_Snap = false;
            Spin_Snap = false;

            MoveOrigin = Point3D.Null();
            MoveAxisY = Point3D.Null();
            MoveAxisX = Point3D.Null();
            MoveAxisC = Point3D.Null();

            SpinOrigin = Angle3D.Null();
            SpinRingA = Angle3D.Null();
            SpinRingS = Angle3D.Null();
            SpinRingD = Angle3D.Null();

            IsNull = true;

            /*Indicator_Body = bodys;
            Indicator_Buffer = new BodyElemBuffer[Indicator_Body.Length];
            for (int i = 0; i < Indicator_Body.Length; i++)
            {
                Indicator_Buffer[i] = Indicator_Body[i].ToBuffer();
            }*/
            Indicator_Trans = null;

            Bodys = bodys;
            Bodys_3D = new PolyHedraInstance_3D_Array(Bodys);
            Insts_3D = new EntryContainerBase<PolyHedraInstance_3D_Data>.Entry[Bodys_3D.Length];
            for (int i = 0; i < Bodys_3D.Length; i++)
            {
                PolyHedraInstance_3D_Data data = new PolyHedraInstance_3D_Data(new Transformation3D(Point3D.Null()));
                Insts_3D[i] = Bodys_3D[i].Alloc(1);
                Insts_3D[i][0] = data;
            }

            Hovering = -1;
            Selected = -1;
            Trans_Changed = Transformation3D.Null();
        }

        public void Trans_Calc(Transformation3D trans)
        {
            if (trans.Is())
            {
                MoveOrigin = trans.Pos;
                MoveAxisY = new Point3D(1, 0, 0);
                MoveAxisX = new Point3D(0, 1, 0);
                MoveAxisC = new Point3D(0, 0, 1);

                SpinOrigin = trans.Rot;
                SpinRingA = new Angle3D(SpinOrigin.A, 0, 0);
                SpinRingS = new Angle3D(SpinOrigin.A, SpinOrigin.S, 0);
                SpinRingD = new Angle3D(SpinOrigin.A, SpinOrigin.S, SpinOrigin.D);

                IsNull = false;
            }
            else
            {
                MoveOrigin = Point3D.Null();
                MoveAxisY = Point3D.Null();
                MoveAxisX = Point3D.Null();
                MoveAxisC = Point3D.Null();

                SpinOrigin = Angle3D.Null();
                SpinRingA = Angle3D.Null();
                SpinRingS = Angle3D.Null();
                SpinRingD = Angle3D.Null();

                IsNull = true;
            }
        }
        public void HoverSelect()
        {
            Selected = Hovering;
        }
        public void Hover_Find()
        {
            if (Selected != -1) { return; }

            Hovering = -1;
            Selected = -1;
            Trans_Changed = Transformation3D.Null();

            if (Indicator_Trans == null) { return; }

            double dist = double.PositiveInfinity;

            PolyHedraInstance_3D_Data data;
            for (int i = 0; i < Insts_3D.Length; i++)
            {
                data = Insts_3D[i][0];
                Intersekt.RayInterval inter = Bodys[i].Intersekt(ViewRay, data.Trans);
                if (inter.Is)
                {
                    if (inter.Interval < dist)
                    {
                        Hovering = i;
                        dist = inter.Interval;
                    }
                }
            }

            for (int i = 0; i < Insts_3D.Length; i++)
            {
                data = Insts_3D[i][0];
                if (Hovering == -1 || Hovering == i)
                {
                    data.GrayLInter = Engine3D.DataStructs.LInterData.LIT0();
                }
                else
                {
                    data.GrayLInter = Engine3D.DataStructs.LInterData.LIT1();
                }
                Insts_3D[i][0] = data;
            }
        }

        public void Indicator_Trans_Calc()
        {
            Indicator_Trans = null;
            if (!Trans_Changed.Is()) { return; }
            Indicator_Trans = new Transformation3D[6];

            if (Move_RotType == RotType.Non)
            {
                Indicator_Trans[0] = new Transformation3D(Trans_Changed.Pos);
                Indicator_Trans[1] = new Transformation3D(Trans_Changed.Pos);
                Indicator_Trans[2] = new Transformation3D(Trans_Changed.Pos);
            }
            if (Move_RotType == RotType.Abs)
            {
                Indicator_Trans[0] = new Transformation3D(Trans_Changed.Pos, new Angle3D(Trans_Changed.Rot.A, Trans_Changed.Rot.S, 0));
                Indicator_Trans[1] = new Transformation3D(Trans_Changed.Pos, new Angle3D(Trans_Changed.Rot.A, 0, 0));
                Indicator_Trans[2] = new Transformation3D(Trans_Changed.Pos, new Angle3D(Trans_Changed.Rot.A, Trans_Changed.Rot.S, Trans_Changed.Rot.D));
            }
            if (Move_RotType == RotType.Rel)
            {
                Indicator_Trans[0] = new Transformation3D(Trans_Changed.Pos, Trans_Changed.Rot);
                Indicator_Trans[1] = new Transformation3D(Trans_Changed.Pos, Trans_Changed.Rot);
                Indicator_Trans[2] = new Transformation3D(Trans_Changed.Pos, Trans_Changed.Rot);
            }

            if (Spin_RotType == RotType.Non)
            {
                Indicator_Trans[3] = new Transformation3D(Trans_Changed.Pos);
                Indicator_Trans[4] = new Transformation3D(Trans_Changed.Pos);
                Indicator_Trans[5] = new Transformation3D(Trans_Changed.Pos);
            }
            if (Spin_RotType == RotType.Abs)
            {
                Indicator_Trans[3] = new Transformation3D(Trans_Changed.Pos, new Angle3D(Trans_Changed.Rot.A, Trans_Changed.Rot.S, 0));
                Indicator_Trans[4] = new Transformation3D(Trans_Changed.Pos, new Angle3D(Trans_Changed.Rot.A, 0, 0));
                Indicator_Trans[5] = new Transformation3D(Trans_Changed.Pos, new Angle3D(Trans_Changed.Rot.A, Trans_Changed.Rot.S, Trans_Changed.Rot.D));
            }
            if (Spin_RotType == RotType.Rel)
            {
                Indicator_Trans[3] = new Transformation3D(Trans_Changed.Pos, Trans_Changed.Rot);
                Indicator_Trans[4] = new Transformation3D(Trans_Changed.Pos, Trans_Changed.Rot);
                Indicator_Trans[5] = new Transformation3D(Trans_Changed.Pos, Trans_Changed.Rot);
            }

            PolyHedraInstance_3D_Data data;
            for (int i = 0; i < Insts_3D.Length; i++)
            {
                data = Insts_3D[i][0];
                data.Trans = TowardView(Indicator_Trans[i]);
                Insts_3D[i][0] = data;
            }

            for (int i = 0; i < Indicator_Trans.Length; i++)
            {
                Indicator_Trans[i] = TowardView(Indicator_Trans[i]);
            }
        }
        public Transformation3D TowardView(Transformation3D trans)
        {
            if (!trans.Is()) { return trans; }
            Point3D diff = trans.Pos - ViewRay.Pos;
            trans.Pos = ViewRay.Pos + ((!diff) * 10);
            return trans;
        }
        public void Draw()
        {
            Bodys_3D.Update();
            Bodys_3D.Draw();
        }

        private Transformation3D Snap(Transformation3D trans)
        {
            if (!trans.Is()) { return Transformation3D.Null(); }

            if (Move_Snap)
            {
                double move_snap_num = 0.1;
                trans.Pos.Y = (float)(Math.Round(trans.Pos.Y / move_snap_num) * move_snap_num);
                trans.Pos.X = (float)(Math.Round(trans.Pos.X / move_snap_num) * move_snap_num);
                trans.Pos.C = (float)(Math.Round(trans.Pos.C / move_snap_num) * move_snap_num);
            }

            if (Spin_Snap)
            {
                double spin_snap_num = 15 * (Math.Tau / 360);
                trans.Rot.A = (float)(Math.Round(trans.Rot.A / spin_snap_num) * spin_snap_num);
                trans.Rot.S = (float)(Math.Round(trans.Rot.S / spin_snap_num) * spin_snap_num);
                trans.Rot.D = (float)(Math.Round(trans.Rot.D / spin_snap_num) * spin_snap_num);
            }

            return trans;
        }

        private Transformation3D MoveY()
        {
            Ray3D ray = new Ray3D();
            if (Move_RotType == RotType.Non) { ray = new Ray3D(MoveOrigin, MoveAxisY); }
            if (Move_RotType == RotType.Abs) { ray = new Ray3D(MoveOrigin, MoveAxisY - SpinRingS); }
            if (Move_RotType == RotType.Rel) { ray = new Ray3D(MoveOrigin, MoveAxisY - SpinOrigin); }

            Intersekt.Ray_Ray(ViewRay, out Intersekt.RayInterval tv, ray, out Intersekt.RayInterval t);
            if (tv.Interval < 0) { return Transformation3D.Null(); }

            return new Transformation3D(t.Pos, SpinOrigin);
        }
        private Transformation3D MoveX()
        {
            Ray3D ray = new Ray3D();
            if (Move_RotType == RotType.Non) { ray = new Ray3D(MoveOrigin, MoveAxisX); }
            if (Move_RotType == RotType.Abs) { ray = new Ray3D(MoveOrigin, MoveAxisX - SpinRingA); }
            if (Move_RotType == RotType.Rel) { ray = new Ray3D(MoveOrigin, MoveAxisX - SpinOrigin); }

            Intersekt.Ray_Ray(ViewRay, out Intersekt.RayInterval tv, ray, out Intersekt.RayInterval t);
            if (tv.Interval < 0) { return Transformation3D.Null(); }

            return new Transformation3D(t.Pos, SpinOrigin);
        }
        private Transformation3D MoveC()
        {
            Ray3D ray = new Ray3D();
            if (Move_RotType == RotType.Non) { ray = new Ray3D(MoveOrigin, MoveAxisC); }
            if (Move_RotType == RotType.Abs) { ray = new Ray3D(MoveOrigin, MoveAxisC - SpinRingD); }
            if (Move_RotType == RotType.Rel) { ray = new Ray3D(MoveOrigin, MoveAxisC - SpinOrigin); }

            Intersekt.Ray_Ray(ViewRay, out Intersekt.RayInterval tv, ray, out Intersekt.RayInterval t);
            if (tv.Interval < 0) { return Transformation3D.Null(); }

            return new Transformation3D(t.Pos, SpinOrigin);
        }

        private Transformation3D SpinY()
        {
            Point3D dirX = Point3D.Default();
            Point3D dirC = Point3D.Default();
            if (Spin_RotType == RotType.Abs) { dirX = MoveAxisX - SpinRingS; dirC = MoveAxisC - SpinRingS; }
            if (Spin_RotType == RotType.Rel) { dirX = MoveAxisX - SpinOrigin; dirC = MoveAxisC - SpinOrigin; }

            Intersekt.RayInterval t = Intersekt.Ray_Plane(ViewRay, MoveOrigin, dirX, dirC);
            if (t.Interval < 0) { return Transformation3D.Null(); }

            Point3D rel = !(t.Pos - MoveOrigin);
            Point3D norm = !(dirX ^ dirC);
            double det = norm % (dirX ^ rel);
            double angleTau = -Math.Atan2(det, dirX % rel);

            //double angle360 = angleTau * (360 / Math.Tau);
            //angle360 = angle360 / 15;
            //angle360 = Math.Round(angle360);
            //angle360 = angle360 * 15;
            //angleTau = angle360 * (Math.Tau / 360);

            Angle3D rot = new Angle3D(0, angleTau, 0);
            if (Spin_RotType == RotType.Abs) { rot = SpinOrigin.Add(rot); }
            if (Spin_RotType == RotType.Rel) { rot = SpinOrigin + rot; }
            return new Transformation3D(MoveOrigin, rot);
        }
        private Transformation3D SpinX()
        {
            Point3D dirC = Point3D.Default();
            Point3D dirY = Point3D.Default();
            if (Spin_RotType == RotType.Abs) { dirC = MoveAxisC - SpinRingA; dirY = MoveAxisY - SpinRingA; }
            if (Spin_RotType == RotType.Rel) { dirC = MoveAxisC - SpinOrigin; dirY = MoveAxisY - SpinOrigin; }

            Intersekt.RayInterval t = Intersekt.Ray_Plane(ViewRay, MoveOrigin, dirC, dirY);
            if (t.Interval < 0) { return Transformation3D.Null(); }

            Point3D rel = !(t.Pos - MoveOrigin);
            Point3D norm = !(dirC ^ dirY);
            double det = norm % (dirC ^ rel);
            double angleTau = +Math.Atan2(det, dirC % rel);

            //double angle360 = angleTau * (360 / Math.Tau);
            //angle360 = angle360 / 15;
            //angle360 = Math.Round(angle360);
            //angle360 = angle360 * 15;
            //angleTau = angle360 * (Math.Tau / 360);

            Angle3D rot = new Angle3D(angleTau, 0, 0);
            if (Spin_RotType == RotType.Abs) { rot = SpinOrigin.Add(rot); }
            if (Spin_RotType == RotType.Rel) { rot = SpinOrigin + rot; }
            return new Transformation3D(MoveOrigin, rot);
        }
        private Transformation3D SpinC()
        {
            Point3D dirY = Point3D.Default();
            Point3D dirX = Point3D.Default();
            if (Spin_RotType == RotType.Abs) { dirY = MoveAxisY - SpinRingD; dirX = MoveAxisX - SpinRingD; }
            if (Spin_RotType == RotType.Rel) { dirY = MoveAxisY - SpinOrigin; dirX = MoveAxisX - SpinOrigin; }

            Intersekt.RayInterval t = Intersekt.Ray_Plane(ViewRay, MoveOrigin, dirY, dirX);
            if (t.Interval < 0) { return Transformation3D.Null(); }

            Point3D rel = !(t.Pos - MoveOrigin);
            Point3D norm = !(dirY ^ dirX);
            double det = norm % (dirY ^ rel);
            double angleTau = +Math.Atan2(det, dirY % rel);

            //double angle360 = angleTau * (360 / Math.Tau);
            //angle360 = angle360 / 15;
            //angle360 = Math.Round(angle360);
            //angle360 = angle360 * 15;
            //angleTau = angle360 * (Math.Tau / 360);

            Angle3D rot = new Angle3D(0, 0, angleTau);
            if (Spin_RotType == RotType.Abs) { rot = SpinOrigin.Add(rot); }
            if (Spin_RotType == RotType.Rel) { rot = SpinOrigin + rot; }
            return new Transformation3D(MoveOrigin, rot);
        }

        public void CycleMove()
        {
            switch (Move_RotType)
            {
                case RotType.Non: Move_RotType = RotType.Abs; break;
                case RotType.Abs: Move_RotType = RotType.Rel; break;
                case RotType.Rel: Move_RotType = RotType.Non; break;
            }
        }
        public void CycleSpin()
        {
            switch (Spin_RotType)
            {
                case RotType.Abs: Spin_RotType = RotType.Rel; break;
                case RotType.Rel: Spin_RotType = RotType.Abs; break;
            }
        }
        public void SnapToggleMove()
        {
            Move_Snap = !Move_Snap;
        }
        public void SnapToggleSpin()
        {
            Spin_Snap = !Spin_Snap;
        }

        public void Change_Reset()
        {
            Hovering = -1;
            Selected = -1;
            Indicator_Trans = null;
            Trans_Changed = Transformation3D.Null();
        }
        public void Changed_Trans_Calc()
        {
            switch (Selected)
            {
                case 0: Trans_Changed = Snap(MoveY()); return;
                case 1: Trans_Changed = Snap(MoveX()); return;
                case 2: Trans_Changed = Snap(MoveC()); return;

                case 3: Trans_Changed = Snap(SpinY()); return;
                case 4: Trans_Changed = Snap(SpinX()); return;
                case 5: Trans_Changed = Snap(SpinC()); return;
            }

            if (!IsNull)
            {
                Trans_Changed = new Transformation3D(MoveOrigin, SpinOrigin);
            }
            else
            {
                Trans_Changed = Transformation3D.Null();
            }
        }
        public string ToInfo()
        {
            string str = "";

            str += "\nViewRay Pos Y: " + ViewRay.Pos.Y;
            str += "\nViewRay Pos X: " + ViewRay.Pos.X;
            str += "\nViewRay Pos C: " + ViewRay.Pos.C;
            str += "\nViewRay Dir Y: " + ViewRay.Dir.Y;
            str += "\nViewRay Dir X: " + ViewRay.Dir.X;
            str += "\nViewRay Dir C: " + ViewRay.Dir.C;

            str += "\nHovering: " + Hovering;
            str += "\nSelected: " + Selected;

            str += "\nTransChanged  Is  : " + Trans_Changed.Is();
            str += "\nTransChanged Pos Y: " + Trans_Changed.Pos.Y;
            str += "\nTransChanged Pos X: " + Trans_Changed.Pos.X;
            str += "\nTransChanged Pos C: " + Trans_Changed.Pos.C;

            return str;
        }

        public void DrawUI(CShaderUserInterfaceBody shader, Transformation3D Trans, UI_Indicator[] main, UI_Indicator[] snap)
        {
            if (Indicator_Trans != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    main[i].Draw(shader, (Indicator_Trans[i].Rot.InvertPls() + Trans.Rot));
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    main[i].Draw(shader, (Trans.Rot));
                }
            }

            snap[0].Draw(shader);
            if (Move_Snap)
            {
                snap[1].Draw(shader);
            }

            snap[2].Draw(shader);
            if (Spin_Snap)
            {
                snap[3].Draw(shader);
            }
        }
        public void DrawUI(UserInterfaceBodyShader shader, Transformation3D Trans, UI_Indicator[] main, UI_Indicator[] snap)
        {
            if (Indicator_Trans != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    main[i].Draw(shader, (Indicator_Trans[i].Rot.InvertPls() + Trans.Rot));
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    main[i].Draw(shader, (Trans.Rot));
                }
            }

            snap[0].Draw(shader);
            if (Move_Snap)
            {
                snap[1].Draw(shader);
            }

            snap[2].Draw(shader);
            if (Spin_Snap)
            {
                snap[3].Draw(shader);
            }
        }
    }
}
