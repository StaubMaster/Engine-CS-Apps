
using Engine3D.Entity;
using Engine3D.GraphicsOld;

namespace VoidFactory.Production.Data
{
    partial class DATA_Thing
    {
        public static BodyStatic[] Bodys;
        public static float[] Icon_Scales;
        public static void BodysCreate()
        {
            Icon_Scales = new float[Bodys.Length];
            for (int i = 0; i < Bodys.Length; i++)
            {
                Bodys[i].BufferCreate();
                Bodys[i].BufferFill();

                Icon_Scales[i] = (float)(0.2 / Bodys[i].BoxFit().MaxSideLen());
            }
        }
        public static void BodysDelete()
        {
            Icon_Scales = null;
            for (int i = 0; i < Bodys.Length; i++)
            {
                Bodys[i].BufferDelete();
            }
            Bodys = null;
        }



        public readonly uint Idx;
        public readonly string Name;
        public readonly string ID;
        public readonly string Cat;

        public DATA_Thing(uint idx, string name, string id, string cat)
        {
            Idx = idx;
            Name = name;
            ID = id;
            Cat = cat;
        }
        public static DATA_Thing FromString(string[] str, DATA_Thing[] things)
        {
            return FindID(things, str[0]);
        }

        public void Draw()
        {
            Bodys[Idx].BufferDraw();
        }
        public void Draw(IconProgram program, float x, float y, float scale = 1.0f)
        {
            program.UniPos(x, y);
            program.UniScale(Icon_Scales[Idx] * scale);
            Bodys[Idx].BufferDraw();
        }
        public override string ToString()
        {
            return (Name + "[" + Idx + "]");
        }

        public static bool CompareInn(DATA_Thing template, DATA_Thing compare)
        {
            if (template == null)
                return true;
            if (compare == null)
                return false;
            return (template.Idx == compare.Idx);
        }
        public static bool CompareOut(DATA_Thing template, DATA_Thing compare)
        {
            if (template == null)
                return true;
            if (compare == null)
                return true;
            return (template.Idx == compare.Idx);
        }

        public static DATA_Thing FindID(DATA_Thing[] things, string id)
        {
            for (int i = 0; i < things.Length; i++)
            {
                if (things[i].ID == id)
                    return things[i];
            }
            return null;
        }
    }
}
