using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class FractalNoiseShaderStep : ShaderStep<FractalNoiseShaderStep.FractalNoiseParameters>
{
    protected override string FragmentShader => "FractalNoise";
    public override ShaderType Type => ShaderType.FractalNoise;

    public override bool ShouldRender => Strength > 0;
    
    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = new FractalNoiseParameters
    {
        TexSize = current.Size,
        Time = (float)Time.Current,
        Strength = Strength,
        Speed = Strength2
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct FractalNoiseParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Time;
        public UniformFloat Strength;
        public UniformFloat Speed;
        private readonly UniformPadding12 pad1;
    }
}
