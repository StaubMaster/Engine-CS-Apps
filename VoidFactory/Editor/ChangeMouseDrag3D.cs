using System;

using Engine3D.Abstract3D;
using Engine3D.OutPut.Shader;
using Engine3D.OutPut.Uniform.Specific;
using Engine3D.Graphics;

using VoidFactory.Miscellaneous.Display;

namespace VoidFactory.Editor
{
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

        private Point3D Pos;
        private Point3D DirY;
        private Point3D DirX;
        private Point3D DirC;

        private Angle3D Rot;
        private Angle3D WnkA;
        private Angle3D WnkS;
        private Angle3D WnkD;

        private bool IsNull;

        private PolyHedra[] Indicator_Body;
        private BodyElemBuffer[] Indicator_Buffer;
        private Transformation3D[] Indicator_Trans;

        private int Hovering;
        private int Selected;
        public bool IsSelected { get { return (Selected != -1); } }
        public bool IsHovering { get { return (Hovering != -1); } }

        public Transformation3D Trans_Changed;

        public ChangeMouseDrag3D(PolyHedra[] indicator_bodys)
        {
            ViewRay = new Ray3D();

            Move_RotType = RotType.Non;
            Spin_RotType = RotType.Abs;
            Move_Snap = false;
            Spin_Snap = false;

            Pos = Point3D.Null();
            DirY = Point3D.Null();
            DirX = Point3D.Null();
            DirC = Point3D.Null();

            Rot = Angle3D.Null();
            WnkA = Angle3D.Null();
            WnkS = Angle3D.Null();
            WnkD = Angle3D.Null();

            IsNull = true;

            Indicator_Body = indicator_bodys;
            Indicator_Buffer = new BodyElemBuffer[Indicator_Body.Length];
            for (int i = 0; i < Indicator_Body.Length; i++)
            {
                Indicator_Buffer[i] = Indicator_Body[i].ToBufferElem();
            }
            Indicator_Trans = null;

            Hovering = -1;
            Selected = -1;
            Trans_Changed = Transformation3D.Null();
        }

        public void Trans_Calc(Transformation3D trans)
        {
            if (trans.Is())
            {
                Pos = trans.Pos;
                DirY = new Point3D(1, 0, 0);
                DirX = new Point3D(0, 1, 0);
                DirC = new Point3D(0, 0, 1);

                Rot = trans.Rot;
                WnkA = new Angle3D(Rot.A, 0, 0);
                WnkS = new Angle3D(Rot.A, Rot.S, 0);
                WnkD = new Angle3D(Rot.A, Rot.S, Rot.D);

                IsNull = false;
            }
            else
            {
                Pos = Point3D.Null();
                DirY = Point3D.Null();
                DirX = Point3D.Null();
                DirC = Point3D.Null();

                Rot = Angle3D.Null();
                WnkA = Angle3D.Null();
                WnkS = Angle3D.Null();
                WnkD = Angle3D.Null();

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
            //Trans_Changed = null;

            if (Indicator_Trans == null) { return; }

            double dist = double.PositiveInfinity;
            for (int i = 0; i < 6; i++)
            {
                Intersekt.RayInterval inter = Indicator_Body[i].Intersekt(ViewRay, Indicator_Trans[i]);
                if (inter.Is)
                {
                    if (inter.Interval < dist)
                    {
                        Hovering = i;
                        dist = inter.Interval;
                    }
                }
            }
        }

        public void Indicator_Trans_Calc()
        {
            Indicator_Trans = null;
            //if (Trans_Changed == null) { return; }
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
        }
        public void Indicator_Draw(CShaderTransformation shaderNorm, CShaderTransformation shaderGray)
        {
            if (Indicator_Trans == null) { return; }

            if (Hovering != -1)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i == Hovering)
                    {
                        shaderNorm.Use();
                        shaderNorm.Trans.Value(Indicator_Trans[i]);
                        Indicator_Buffer[i].Draw();
                    }
                    else
                    {
                        shaderGray.Use();
                        shaderGray.Trans.Value(Indicator_Trans[i]);
                        Indicator_Buffer[i].Draw();
                    }
                }
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    shaderNorm.Use();
                    shaderNorm.Trans.Value(Indicator_Trans[i]);
                    Indicator_Buffer[i].Draw();
                }
            }
        }
        public void Indicator_Draw(BodyElemUniShader shader)
        {
            if (Indicator_Trans == null) { return; }

            shader.Use();
            if (Hovering != -1)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i == Hovering)
                    {
                        //shaderNorm.Use();
                        //shaderNorm.Trans.Value(Indicator_Trans[i]);
                        shader.GrayInter.T0(1.0f);
                        shader.Trans.Value(Indicator_Trans[i]);
                        Indicator_Buffer[i].Draw();
                    }
                    else
                    {
                        //shaderGray.Use();
                        //shaderGray.Trans.Value(Indicator_Trans[i]);
                        shader.GrayInter.T1(1.0f);
                        shader.Trans.Value(Indicator_Trans[i]);
                        Indicator_Buffer[i].Draw();
                    }
                }
            }
            else
            {
                shader.GrayInter.T0(1.0f);
                for (int i = 0; i < 6; i++)
                {
                    //shaderNorm.Use();
                    //shaderNorm.Trans.Value(Indicator_Trans[i]);
                    shader.Trans.Value(Indicator_Trans[i]);
                    Indicator_Buffer[i].Draw();
                }
            }
        }

        private Transformation3D Snap(Transformation3D trans)
        {
            if (trans.Is()) { return Transformation3D.Null(); }

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
            if (Move_RotType == RotType.Non) { ray = new Ray3D(Pos, DirY); }
            if (Move_RotType == RotType.Abs) { ray = new Ray3D(Pos, DirY - WnkS); }
            if (Move_RotType == RotType.Rel) { ray = new Ray3D(Pos, DirY - Rot); }

            Intersekt.Ray_Ray(ViewRay, out Intersekt.RayInterval tv, ray, out Intersekt.RayInterval t);
            if (tv.Interval < 0) { return Transformation3D.Null(); }

            return new Transformation3D(t.Pos, Rot);
        }
        private Transformation3D MoveX()
        {
            Ray3D ray = new Ray3D();
            if (Move_RotType == RotType.Non) { ray = new Ray3D(Pos, DirX); }
            if (Move_RotType == RotType.Abs) { ray = new Ray3D(Pos, DirX - WnkA); }
            if (Move_RotType == RotType.Rel) { ray = new Ray3D(Pos, DirX - Rot); }

            Intersekt.Ray_Ray(ViewRay, out Intersekt.RayInterval tv, ray, out Intersekt.RayInterval t);
            if (tv.Interval < 0) { return Transformation3D.Null(); }

            return new Transformation3D(t.Pos, Rot);
        }
        private Transformation3D MoveC()
        {
            Ray3D ray = new Ray3D();
            if (Move_RotType == RotType.Non) { ray = new Ray3D(Pos, DirC); }
            if (Move_RotType == RotType.Abs) { ray = new Ray3D(Pos, DirC - WnkD); }
            if (Move_RotType == RotType.Rel) { ray = new Ray3D(Pos, DirC - Rot); }

            Intersekt.Ray_Ray(ViewRay, out Intersekt.RayInterval tv, ray, out Intersekt.RayInterval t);
            if (tv.Interval < 0) { return Transformation3D.Null(); }

            return new Transformation3D(t.Pos, Rot);
        }

        private Transformation3D SpinY()
        {
            Point3D dirX = Point3D.Default();
            Point3D dirC = Point3D.Default();
            if (Spin_RotType == RotType.Abs) { dirX = DirX - WnkS; dirC = DirC - WnkS; }
            if (Spin_RotType == RotType.Rel) { dirX = DirX - Rot; dirC = DirC - Rot; }

            Intersekt.RayInterval t = Intersekt.Ray_Plane(ViewRay, Pos, dirX, dirC);
            if (t.Interval < 0) { return Transformation3D.Null(); }

            Point3D rel = !(t.Pos - Pos);
            Point3D norm = !(dirX ^ dirC);
            double det = norm % (dirX ^ rel);
            double angleTau = -Math.Atan2(det, dirX % rel);

            //double angle360 = angleTau * (360 / Math.Tau);
            //angle360 = angle360 / 15;
            //angle360 = Math.Round(angle360);
            //angle360 = angle360 * 15;
            //angleTau = angle360 * (Math.Tau / 360);

            Angle3D rot = new Angle3D(0, angleTau, 0);
            if (Spin_RotType == RotType.Abs) { rot = Rot.Add(rot); }
            if (Spin_RotType == RotType.Rel) { rot = Rot + rot; }
            return new Transformation3D(Pos, rot);
        }
        private Transformation3D SpinX()
        {
            Point3D dirC = Point3D.Default();
            Point3D dirY = Point3D.Default();
            if (Spin_RotType == RotType.Abs) { dirC = DirC - WnkA; dirY = DirY - WnkA; }
            if (Spin_RotType == RotType.Rel) { dirC = DirC - Rot; dirY = DirY - Rot; }

            Intersekt.RayInterval t = Intersekt.Ray_Plane(ViewRay, Pos, dirC, dirY);
            if (t.Interval < 0) { return Transformation3D.Null(); }

            Point3D rel = !(t.Pos - Pos);
            Point3D norm = !(dirC ^ dirY);
            double det = norm % (dirC ^ rel);
            double angleTau = +Math.Atan2(det, dirC % rel);

            //double angle360 = angleTau * (360 / Math.Tau);
            //angle360 = angle360 / 15;
            //angle360 = Math.Round(angle360);
            //angle360 = angle360 * 15;
            //angleTau = angle360 * (Math.Tau / 360);

            Angle3D rot = new Angle3D(angleTau, 0, 0);
            if (Spin_RotType == RotType.Abs) { rot = Rot.Add(rot); }
            if (Spin_RotType == RotType.Rel) { rot = Rot + rot; }
            return new Transformation3D(Pos, rot);
        }
        private Transformation3D SpinC()
        {
            Point3D dirY = Point3D.Default();
            Point3D dirX = Point3D.Default();
            if (Spin_RotType == RotType.Abs) { dirY = DirY - WnkD; dirX = DirX - WnkD; }
            if (Spin_RotType == RotType.Rel) { dirY = DirY - Rot; dirX = DirX - Rot; }

            Intersekt.RayInterval t = Intersekt.Ray_Plane(ViewRay, Pos, dirY, dirX);
            if (t.Interval < 0) { return Transformation3D.Null(); }

            Point3D rel = !(t.Pos - Pos);
            Point3D norm = !(dirY ^ dirX);
            double det = norm % (dirY ^ rel);
            double angleTau = +Math.Atan2(det, dirY % rel);

            //double angle360 = angleTau * (360 / Math.Tau);
            //angle360 = angle360 / 15;
            //angle360 = Math.Round(angle360);
            //angle360 = angle360 * 15;
            //angleTau = angle360 * (Math.Tau / 360);

            Angle3D rot = new Angle3D(0, 0, angleTau);
            if (Spin_RotType == RotType.Abs) { rot = Rot.Add(rot); }
            if (Spin_RotType == RotType.Rel) { rot = Rot + rot; }
            return new Transformation3D(Pos, rot);
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
            //Trans_Changed = null;
        }
        public void Changed_Trans_Calc()
        {
            switch (Selected)
            {
                //case 0: Trans_Changed = Snap(MoveY()); return;
                //case 1: Trans_Changed = Snap(MoveX()); return;
                //case 2: Trans_Changed = Snap(MoveC()); return;
                //
                //case 3: Trans_Changed = Snap(SpinY()); return;
                //case 4: Trans_Changed = Snap(SpinX()); return;
                //case 5: Trans_Changed = Snap(SpinC()); return;
            }

            if (!IsNull)
            {
                Trans_Changed = new Transformation3D(Pos, Rot);
            }
            else
            {
                //Trans_Changed = null;
            }
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
