
using VoidFactory.Production.Transfer;

namespace VoidFactory.Production.Data
{
    class DATA_Buffer
    {
        public DATA_Thing Thing;
        public uint Num;

        public DATA_Buffer()
        {
            Thing = null;
            Num = 0;
        }
        public DATA_Buffer(DATA_Thing thing, uint num = 0)
        {
            Thing = thing;
            Num = num;
        }
        public static DATA_Buffer FromString(string[] str, DATA_Thing[] things)
        {
            return new DATA_Buffer(
                DATA_Thing.FindID(things, str[0]),
                uint.Parse(str[1])
                );
        }
        public static DATA_Buffer[] FromString(string[][] str, DATA_Thing[] things)
        {
            DATA_Buffer[] buffers = new DATA_Buffer[str.Length];
            for (int b = 0; b < buffers.Length; b++)
                buffers[b] = FromString(str[b], things);
            return buffers;
        }

        public void tryInnS(ref DATA_Thing thing)
        {
            if (DATA_Thing.CompareInn(Thing, thing))
            {
                if (Num == 0)
                    Thing = thing;
                Num++;
                thing = null;
            }
        }
        public void tryOutS(ref DATA_Thing thing)
        {
            if (thing == null && Num != 0)
            {
                thing = Thing;
                Num--;
                if (Num == 0)
                    Thing = null;
            }
        }

        public void tryInnA(DATA_Buffer buffer)
        {
            if (Num == 0)
                Thing = buffer.Thing;

            if (Thing == buffer.Thing)
                Num += buffer.Num;
        }
        public void tryOutA(DATA_Buffer buffer)
        {
            if (Thing == buffer.Thing)
                Num -= buffer.Num;

            if (Num == 0)
                Thing = null;
        }

        public override string ToString()
        {
            return (Num + " " + Thing);
        }
    }
}
