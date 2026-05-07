using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace SC2.SC2Code;

//You're recommended but not required to keep all your code in this package and all your assets in the SC2 folder.
[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "SC2"; //At the moment, this is used only for the Logger and harmony names.

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        Harmony harmony = new(ModId);

        harmony.PatchAll();
    }
}
