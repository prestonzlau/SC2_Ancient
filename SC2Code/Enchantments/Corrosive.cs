using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SC2.SC2Code.Enchantments;

public sealed class Corrosive : Sc2EnchantmentModel
{
    public override bool HasExtraCardText => true;

    public override bool ShowAmount => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [(HoverTipFactory.FromPower<PoisonPower>())];
    
    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        Corrosive corrosive = this;
        if (cardSource != Card || dealer == null)
        {
            return;
        }

        if (!props.IsPoweredAttack())
        {
            return;
        }

        var combatState = dealer.CombatState;
        if (combatState == null)
        {
            return;
        }

        var unblockedPoisonDamage = result.UnblockedDamage * 0.5m;
        if (unblockedPoisonDamage <= 0)
        {
            return;
        }

        PoisonPower? unused = await PowerCmd.Apply<PoisonPower>(choiceContext, target, unblockedPoisonDamage,
            corrosive.Card.Owner.Creature, corrosive.Card);

    }
}