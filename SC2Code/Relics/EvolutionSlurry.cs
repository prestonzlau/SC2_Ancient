using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens;
using MegaCrit.Sts2.Core.Saves.Runs;
using static SC2.SC2Code.Extensions.EnchantExtensions;

namespace SC2.SC2Code.Relics;




[Pool(typeof(EventRelicPool))]
public  class EvolutionSlurry : Sc2RelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    private bool _cloneExists;
    
    [SavedProperty]
    private bool CloneExists
    {
        get => _cloneExists;
        set
        {
            AssertMutable();
            _cloneExists = value;
        }
    }
    
    
    
    
    public override Task AfterObtained()
    {
        EvolutionSlurry evolutionSlurry = this;
        IEnumerable<CardModel> cardModels = PileType.Deck.GetPile(Owner).Cards
            .Where(c => c.Type is CardType.Attack or CardType.Skill or CardType.Power && !c.Keywords.Contains(CardKeyword.Eternal)).ToList()
            .StableShuffle(Owner.RunState.Rng.Niche).Take(10);
        NRun.Instance?.GlobalUi.GridCardPreviewContainer.ForceMaxColumnsUntilEmpty(3);
        var results = new List<CardPileAddResult>();
        foreach (CardModel card in cardModels)
        {
            CardModel cardForTransform =
                CardFactory.CreateRandomCardForTransform(card, false, evolutionSlurry.Owner.RunState.Rng.Niche);
            ApplyRandomEnchantment(cardForTransform);
            CardCmd.Transform(card, cardForTransform, CardPreviewStyle.None);
            if (cardForTransform.Enchantment is Clone)
            {
                evolutionSlurry.CloneExists = true;
            }

            var result = new CardPileAddResult { success = true, cardAdded = cardForTransform };
            results.Add(result);

        }

        NSimpleCardsViewScreen.ShowScreen(results, new LocString("relics", "SC2-EVOLUTION_SLURRY.infoText"));
        return Task.CompletedTask;
        /*public static async Task<CardPileAddResult> TransformToRandom(
            CardModel original,
            Rng rng,
            CardPreviewStyle style = CardPreviewStyle.HorizontalLayout)
        {
            return (await CardCmd.Transform(new CardTransformation(original).Yield(), rng, style)).First<CardPileAddResult>();
        }*/
    }
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        EvolutionSlurry evolutionSlurry = this;
        if (player != Owner)
            return false;
        if (!(evolutionSlurry.CloneExists))
            return false;
        if (Owner.Relics.Any(r => r is PaelsGrowth))
        {
            return false;
        }

        options.Add(new CloneRestSiteOption(player));
        return true;
    }
}