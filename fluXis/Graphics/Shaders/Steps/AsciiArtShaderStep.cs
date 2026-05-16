using System.Runtime.InteropServices;
using fluXis.Map.Structures.Events;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;

namespace fluXis.Graphics.Shaders.Steps;

public class AsciiArtShaderStep : ShaderStep<AsciiArtShaderStep.AsciiArtParameters>
{
    protected override string FragmentShader => "AsciiArt";
    public override ShaderType Type => ShaderType.AsciiArt;

    public override void UpdateParameters(IFrameBuffer current) => ParameterBuffer.Data = ParameterBuffer.Data with
    {
        TexSize = current.Size,
        Strength = Strength,
        CellSize = Strength2
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public record struct AsciiArtParameters
    {
        public UniformVector2 TexSize;
        public UniformFloat Strength;
        public UniformFloat CellSize;
    }
}
