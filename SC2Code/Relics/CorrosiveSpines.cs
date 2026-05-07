using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using SC2.SC2Code.Enchantments;

namespace SC2.SC2Code.Relics;


[Pool(typeof(EventRelicPool))]
public class CorrosiveSpines : Sc2RelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    
    public override bool HasUponPickupEffect => true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get => HoverTipFactory.FromEnchantment<Corrosive>();
    }
    
    public override Task AfterObtained()
    {   
        foreach (CardModel card in PileType.Deck.GetPile(Owner).Cards.Where(c => c.Type == CardType.Attack && c.IsUpgradable && c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Strike)).ToList())
            CardCmd.Upgrade(card);
        foreach (CardModel card in PileType.Deck.GetPile(Owner).Cards
                     .Where(c =>
                         c.Type == CardType.Attack && c.Rarity == CardRarity.Basic && c.Tags.Contains(CardTag.Strike)).ToList())
            CardCmd.Enchant<Corrosive>(card, 1M);
        return Task.CompletedTask;
    }
}