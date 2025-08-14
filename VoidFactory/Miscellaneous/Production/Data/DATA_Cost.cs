
namespace VoidFactory.Production.Data
{
    class DATA_Cost
    {
        //public static GameSelect.Game3D.GraphicsData Graphic;
        /*public void Draw(float x, float y)
        {
            Graphic.Icon_Prog.UniScale(0.025f);
            Graphic.Icon_Prog.UniPos(x, y);
            Graphic.Icon_Prog.UniRot(Graphic.Icon_Spin_flt);
            for (int i = 0; i < Sum.Length; i++)
            {
                Sum[i].Thing.Draw();
                x += 0.05f;
            }
        }*/

        public DATA_Buffer[] Sum;

        public DATA_Cost(DATA_Buffer[] sum)
        {
            Sum = sum;
        }
        public DATA_Cost(DATA_Thing thing, uint num)
        {
            if (thing != null)
                Sum = new DATA_Buffer[1]{ new DATA_Buffer(thing, num) };
            else
                Sum = new DATA_Buffer[0];
        }
        public DATA_Cost()
        {
            Sum = new DATA_Buffer[0];
        }
        public static DATA_Cost FromString(string[][] str, DATA_Thing[] things)
        {
            return new DATA_Cost(DATA_Buffer.FromString(str, things));
        }

        public void Accumulate(DATA_Buffer buffer)
        {
            if (buffer.Num == 0)
                return;

            for (int i = 0; i < Sum.Length; i++)
            {
                if (Sum[i].Thing == buffer.Thing)
                {
                    Sum[i].Num += buffer.Num;
                    return;
                }
            }

            DATA_Buffer[] sum = new DATA_Buffer[Sum.Length + 1];
            for (int i = 0; i < Sum.Length; i++)
                sum[i] = Sum[i];
            sum[Sum.Length] = buffer;
            Sum = sum;
        }
        public DATA_Buffer Deccumulate()
        {
            if (Sum.Length == 0)
                return null;

            DATA_Buffer buffer;

            DATA_Buffer[] sum = new DATA_Buffer[Sum.Length - 1];
            for (int i = 0; i < sum.Length; i++)
                sum[i] = Sum[i];
            buffer = Sum[sum.Length];
            Sum = sum;

            return buffer;
        }

        private static int Can_Deduct(DATA_Buffer cost, DATA_Buffer[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (cost.Thing == buffer[i].Thing)
                {
                    if (buffer[i].Num - cost.Num < buffer[i].Num)
                        return i;
                }
            }
            return -1;
        }
        private static int Can_Refund(DATA_Buffer cost, DATA_Buffer[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                if (cost.Thing == buffer[i].Thing)
                {
                    if (buffer[i].Num + cost.Num > buffer[i].Num)
                        return i;
                }
            }
            return -1;
        }

        public bool Can_Deduct(DATA_Buffer[] buffer)
        {
            for (int c = 0; c < Sum.Length; c++)
            {
                if (Can_Deduct(Sum[c], buffer) == -1)
                    return false;
            }
            return true;
        }
        public bool Can_Refund(DATA_Buffer[] buffer)
        {
            for (int c = 0; c < Sum.Length; c++)
            {
                if (Can_Refund(Sum[c], buffer) == -1)
                    return false;
            }
            return true;
        }

        public void Deduct(DATA_Buffer[] buffer)
        {
            for (int c = 0; c < Sum.Length; c++)
            {
                int idx = Can_Deduct(Sum[c], buffer);
                buffer[idx].Num -= Sum[c].Num;
            }
        }
        public void Refund(DATA_Buffer[] buffer)
        {
            for (int c = 0; c < Sum.Length; c++)
            {
                int idx = Can_Refund(Sum[c], buffer);
                buffer[idx].Num += Sum[c].Num;
            }
        }
    }
}
