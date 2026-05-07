using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace SC2.SC2Code.Card.Uncommon;    

[Pool(typeof(ColorlessCardPool))]
public sealed class CorrosiveStrike() : CustomCardModel(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CorrosiveStrike corrosiveStrike = this;
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        AttackCommand attackCommand = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
        Decimal poisonunblockedDamage =
            attackCommand.Results.Sum(r => r.UnblockedDamage)* 0.5M;
        if (poisonunblockedDamage < 1M)
            return;
        PoisonPower? unused = await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, poisonunblockedDamage, corrosiveStrike.Owner.Creature, corrosiveStrike);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
    }
}