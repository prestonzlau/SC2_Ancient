using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization;
using SC2.SC2Code.Relics;

namespace SC2.SC2Code.RestSite;

public sealed class AdrenalOverloadRestSiteOption(Player owner) : RestSiteOption(owner)
{
    private static readonly string CustomIconPath = "res://SC2/images/ui/rest_site/option_adrenal_overload.png";

    public static bool WasAdrenalOverloadSelected { get; private set; }

    public static void ResetState()
    {
        WasAdrenalOverloadSelected = false;
    }

    public override string OptionId => "ADRENAL_OVERLOAD";
    public override IEnumerable<string> AssetPaths => [CustomIconPath];

    public override LocString Description
    {
        get
        {
            LocString locString = new LocString("rest_site_ui", "OPTION_" + OptionId + ".description");
            return locString;
        }
    }

    public override async Task<bool> OnSelect()
    {
        WasAdrenalOverloadSelected = true;
        if (Owner.Relics.Any(r => r is ZerglingGland)){
            await Owner.GetRelic<ZerglingGland>()!.AdrenalOverloadRest(Owner);
        }
        return true;
    }
}