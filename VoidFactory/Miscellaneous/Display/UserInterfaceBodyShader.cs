using Engine3D.Graphics.Shader;
using Engine3D.Graphics.Shader.Uniform.Float;

namespace VoidFactory.Miscellaneous.Display
{
    class UserInterfaceBodyShader : BaseShader
    {
        public readonly UniScreenRatio ScreenRatio;

        public readonly UniTransformation BodyTrans;
        public readonly UniFloat1 BodyScale;

        public readonly UserInterfaceUni UIRectangle;

        public UserInterfaceBodyShader(string shaderDir) : base(new ShaderCode[]
        {
            ShaderCode.FromFile(shaderDir + "UIBody.vert"),
            ShaderCode.FromFile(shaderDir + "Geom_Norm_Color.geom"),
            ShaderCode.FromFile(shaderDir + "Frag/Direct.frag"),
        })
        {
            ScreenRatio = new UniScreenRatio(this, "screenRatios");

            BodyTrans = new UniTransformation(this, "bodyTrans");
            BodyScale = new UniFloat1(this, "bodyScale", 1);

            UIRectangle = new UserInterfaceUni(this, "UIRectangle");
        }

        protected override void UpdateUniforms()
        {
            ScreenRatio.Update();

            BodyTrans.Update();
            BodyScale.Update();

            UIRectangle.Update();
        }
    }
}
