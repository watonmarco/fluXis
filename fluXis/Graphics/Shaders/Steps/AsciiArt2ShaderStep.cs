using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class AsciiArt2ShaderStep : ShaderStep<AsciiArt2ShaderStep.AsciiArt2Parameters>
{
    protected override string FragmentShader => "AsciiArt2";
    public override ShaderType Type => ShaderType.AsciiArt2;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        TexSize = current.Size,
        Strength = Strength,
        CellSize = Strength2
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct AsciiArt2Parameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Strength;
        public UniformFloat CellSize;
    }
}
