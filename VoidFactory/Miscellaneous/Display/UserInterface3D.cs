using System;

using Engine3D.Abstract3D;
using Engine3D.Abstract2D;
using Engine3D.Graphics;
using Engine3D.Graphics.Display2D.UserInterface;
using Engine3D.Miscellaneous.EntryContainer;

namespace VoidFactory.Editor
{
    enum UI_Anchor
    {
        Y_M = 0b0001,
        Y_P = 0b0010,
        X_M = 0b0100,
        X_P = 0b1000,

        YX_MM = Y_M | X_M,
        YX_PM = Y_P | X_M,
        YX_MP = Y_M | X_P,
        YX_PP = Y_P | X_P,
    }
    static class AnchorHelp
    {
        public static Point2D ToPoint2D(this UI_Anchor anchor)
        {
            Point2D a = new Point2D(0, 0);
            if ((anchor & UI_Anchor.Y_M) != 0) { a.X = -1.0f; }
            if ((anchor & UI_Anchor.Y_P) != 0) { a.X = +1.0f; }
            if ((anchor & UI_Anchor.X_M) != 0) { a.Y = -1.0f; }
            if ((anchor & UI_Anchor.X_P) != 0) { a.Y = +1.0f; }
            return a;
        }
    }

    struct UI_Rectangle
    {
        public float Y;
        public float X;
        public float W;
        public float H;

        public UI_Anchor Anchor;
        public float AnchorPixel_Y;
        public float AnchorPixel_X;
        public float AnchorNormal_X;
        public float AnchorNormal_Y;

        public static UI_Rectangle YX_WH(float y, float x, float w, float h, UI_Anchor anchor)
        {
            UI_Rectangle rect = new UI_Rectangle();

            rect.Y = y;
            rect.X = x;
            rect.W = w;
            rect.H = h;
            rect.Anchor = anchor;
            if ((anchor & UI_Anchor.Y_M) != 0) { rect.AnchorNormal_Y = -1; }
            if ((anchor & UI_Anchor.Y_P) != 0) { rect.AnchorNormal_Y = +1; }
            if ((anchor & UI_Anchor.X_M) != 0) { rect.AnchorNormal_X = -1; }
            if ((anchor & UI_Anchor.X_P) != 0) { rect.AnchorNormal_X = +1; }

            return rect;
        }
        public static UI_Rectangle YX_YX(float y0, float x0, float y1, float x1, UI_Anchor anchor)
        {
            UI_Rectangle rect = new UI_Rectangle();

            rect.Y = y0;
            rect.X = x0;
            rect.W = y1 - y0;
            rect.H = x1 - x0;
            rect.Anchor = anchor;
            if ((anchor & UI_Anchor.Y_M) != 0) { rect.AnchorNormal_Y = -1; }
            if ((anchor & UI_Anchor.Y_P) != 0) { rect.AnchorNormal_Y = +1; }
            if ((anchor & UI_Anchor.X_M) != 0) { rect.AnchorNormal_X = -1; }
            if ((anchor & UI_Anchor.X_P) != 0) { rect.AnchorNormal_X = +1; }

            return rect;
        }

        public void Calc_W_and_Y1_to_Y0(float w)
        {
            float y1 = Y + W;
            Y = y1 - W;
            W = w;
        }
        public void Calc_Y1_and_W_to_Y0(float y1)
        {
            Y = y1 - W;
        }
        public void Calc_Y0_and_Y1_to_W(float y0)
        {
            float y1 = Y + W;
            Y = y0;
            W = y1 - y0;
        }
        public void Calc_Y1_and_Y0_to_W(float y1)
        {
            W = y1 - Y;
        }
        public void Calc_Y0_and_W_to_Y1(float y0)
        {
            Y = y0;
        }
        public void Calc_W_and_Y0_to_Y1(float w)
        {
            W = w;
        }

        public void UpdateSize(float w, float h)
        {
            if ((Anchor & UI_Anchor.Y_M) != 0) { AnchorPixel_Y = 0; }
            if ((Anchor & UI_Anchor.Y_P) != 0) { AnchorPixel_Y = w; }
            if ((Anchor & UI_Anchor.X_M) != 0) { AnchorPixel_X = 0; }
            if ((Anchor & UI_Anchor.X_P) != 0) { AnchorPixel_X = h; }
        }

        public bool Intersekt(float y, float x)
        {
            return (
                (AnchorPixel_Y + Y - W * 0.5f < y && y < AnchorPixel_Y + Y + W * 0.5f) &&
                (AnchorPixel_X + X - H * 0.5f < x && x < AnchorPixel_X + X + H * 0.5f)
                );
        }
    }

    struct UI_Indicator
    {
        UI_Rectangle Rectangle;

        public bool isHover;

        PolyHedra Body;

        AxisBox3D Box;
        Point3D Center;

        EntryContainerBase<UIBody_Data>.Entry InstData;

        float Scale;

        Angle3D Wnk;

        Transformation3D Trans;

        public enum EAnimationType
        {
            None,
            Spin,
            Disk,
            Diag,
            Item,
            View,
        }
        EAnimationType AnimationType;

        public UI_Indicator(float y, float x, float w, float h, UI_Anchor anchor, PolyHedra body, EntryContainerBase<UIBody_Data>.Entry inst_data , float scale, EAnimationType animationType)
        {
            Rectangle = UI_Rectangle.YX_WH(y, x, w, h, anchor);

            isHover = false;

            Body = body;
            Box = body.CalcBox();
            Center = body.CalcAverage();

            InstData = inst_data;

            Scale = scale;

            AnimationType = animationType;
            if (animationType == EAnimationType.Disk) { Wnk = new Angle3D(0, Math.Tau / 4, 0); }
            else if (animationType == EAnimationType.Item) { Wnk = new Angle3D(0, -Math.Tau / 8, 0); }
            else if (animationType == EAnimationType.Diag) { Wnk = new Angle3D(-Math.Tau * (3.5 / 8), -Math.Tau * (0.5 / 8), 0); }
            else { Wnk = new Angle3D(0, 0, 0); }
            Trans = Transformation3D.Default();

            InstData[0] = new UIBody_Data(
                new UIGridPosition(anchor.ToPoint2D(), new Point2D(y, x), new Point2D(0, 0)),
                new UIGridSize(new Point2D(w, h), 0.0f),
                1.0f / scale, Wnk);
        }

        public void UpdateSize(float w, float h)
        {
            Rectangle.UpdateSize(w, h);
        }
        public void UpdateHover(float mouse_y, float mouse_x)
        {
            isHover = Rectangle.Intersekt(mouse_y, mouse_x);
        }

        public void ChangeTrans()
        {
            if (AnimationType == EAnimationType.Spin) { Wnk.D += 0.05; }
            if (AnimationType == EAnimationType.Disk) { if (isHover) { Wnk.A += 0.02; } else { Wnk.A = 0; } }
            if (AnimationType == EAnimationType.Diag) { if (isHover) { Wnk.A += 0.02; } else { Wnk.A = -Math.Tau * (3.5 / 8); } }
            if (AnimationType == EAnimationType.Item) { Wnk.A += 0.02; }

            Trans.Rot = Wnk;

            UIBody_Data data;
            data = InstData[0];
            data.Trans = Trans;
            InstData[0] = data;
        }
        public void ChangeTrans(Angle3D wnk)
        {
            Trans.Rot = wnk;

            UIBody_Data data;
            data = InstData[0];
            data.Trans = Trans;
            InstData[0] = data;
        }
    }
}
