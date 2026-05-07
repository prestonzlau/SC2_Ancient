using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace SC2.SC2Code.Extensions;

public static class CustomHoverTip
{
    public static HoverTip GetStaticHoverTip(string locEntry) {
        
        const string locTable = "static_hover_tips";
        return new HoverTip(
            new LocString(locTable, locEntry + ".title"), 
            new LocString(locTable, locEntry + ".description")
        );
    }
}