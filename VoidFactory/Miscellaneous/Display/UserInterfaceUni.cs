using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform;
using Engine3D.Graphics.Shader.Uniform.Float;

namespace VoidFactory.Miscellaneous.Display
{
    class UserInterfaceUni : UniFloat2
    {
        public UserInterfaceUni(BaseShader program, string name) : base(program, name, 3) { }

        public void Value(float w, float h, float y, float x, float anchor_y, float anchor_x)
        {
            float[] data = NewData();
            data[0] = w;
            data[1] = h;
            data[2] = y;
            data[3] = x;
            data[4] = anchor_y;
            data[5] = anchor_x;
            Set(data);
        }
    }
}
