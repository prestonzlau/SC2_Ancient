using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace SC2.SC2Code.Relics;


[Pool(typeof(EventRelicPool))]
public class MutaliskHeart : Sc2RelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    
    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(15M, ValueProp.Unpowered), new HpLossVar(15M), new HealVar(4M)
    ];
    
    public override async Task AfterObtained()
    {
        MutaliskHeart mutaliskHeart = this;
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), mutaliskHeart.Owner.Creature, mutaliskHeart.DynamicVars.HpLoss.BaseValue, false);
        IEnumerable<DamageResult> unused = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), mutaliskHeart.Owner.Creature, mutaliskHeart.DynamicVars.Damage, null, null);
    }
    
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        MutaliskHeart mutaliskHeart = this;
        mutaliskHeart.Flash();
        await CreatureCmd.Heal(mutaliskHeart.Owner.Creature, mutaliskHeart.DynamicVars.Heal.BaseValue);
    }

}