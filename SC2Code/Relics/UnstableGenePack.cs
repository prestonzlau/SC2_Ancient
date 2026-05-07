using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rooms;

namespace SC2.SC2Code.Relics;



[Pool(typeof(EventRelicPool))]
public  class UnstableGenePack : Sc2RelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    
    public override async Task AfterCombatVictory(CombatRoom _)
    {
        UnstableGenePack unstableGenePack = this;
        if (unstableGenePack.Owner.Creature.IsDead)
            return;
        IEnumerable<CardModel> cardModels = PileType.Deck.GetPile(Owner).Cards.Where(c => c.IsUpgradable).ToList().StableShuffle(Owner.RunState.Rng.Niche).Take(2);
        IEnumerable<CardModel> cardModels2 = PileType.Deck.GetPile(Owner).Cards.Where(c => c.IsUpgraded).ToList().StableShuffle(Owner.RunState.Rng.Niche).Take(1);
        NRun.Instance?.GlobalUi.GridCardPreviewContainer.ForceMaxColumnsUntilEmpty(3);
        foreach (CardModel card in cardModels)
        {
            CardCmd.Upgrade(card, CardPreviewStyle.GridLayout);
            await Cmd.CustomScaledWait(0.3f, 0.5f);
        }

        foreach (CardModel card in cardModels2)
        {
            CardCmd.Downgrade(card);
            CardCmd.Preview(card, style: CardPreviewStyle.MessyLayout);
            await Cmd.CustomScaledWait(0.3f, 0.5f);
        }
    }

}