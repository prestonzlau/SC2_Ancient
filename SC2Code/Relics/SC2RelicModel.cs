using System.Text.RegularExpressions;
using BaseLib.Abstracts;
using SC2.SC2Code.Extensions;

namespace SC2.SC2Code.Relics;

public abstract class Sc2RelicModel : CustomRelicModel
{
    private string IconFileName => GetType().Name.ToSnakeCase();

    public override string PackedIconPath => $"{IconFileName}.png".RelicImagePath();

    protected override string PackedIconOutlinePath =>
        Godot.ResourceLoader.Exists($"{IconFileName}_outline.png".RelicImagePath())
            ? $"{IconFileName}_outline.png".RelicImagePath()
            : "relic_outline.png".RelicImagePath();

    protected override string BigIconPath => $"{IconFileName}.png".BigRelicImagePath();
    
}