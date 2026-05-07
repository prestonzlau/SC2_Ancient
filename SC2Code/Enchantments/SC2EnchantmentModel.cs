using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Helpers;

namespace SC2.SC2Code.Enchantments;

public abstract class Sc2EnchantmentModel : CustomEnchantmentModel
{
    protected override string CustomIconPath => $"res://SC2/images/enchantments/{GetIconFileName()}.png";

    private string GetIconFileName()
    {
        var className = GetType().Name;
        return StringHelper.Slugify(className).Replace('-', '_').ToLowerInvariant();
    }
}