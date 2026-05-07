using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace SC2.SC2Code.Relics;


[Pool(typeof(EventRelicPool))]

public  class RoachClaw : Sc2RelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [(HoverTipFactory.FromPower<DexterityPower>()),(HoverTipFactory.FromPower<StrengthPower>())];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DexterityPower>(3M),new PowerVar<StrengthPower>(1M)];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        RoachClaw roachClaw = this;
        if (!(room is CombatRoom))
            return;
        roachClaw.Flash();
        DexterityPower unused = await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), roachClaw.Owner.Creature, roachClaw.DynamicVars.Dexterity.BaseValue, roachClaw.Owner.Creature, null);
    }

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        RoachClaw roachClaw = this;
        if (side != roachClaw.Owner.Creature.Side)
            return;
        roachClaw.Flash();
        //Plus one strength and minus one dexterity.
        StrengthPower unused = await PowerCmd.Apply<StrengthPower>(choiceContext, roachClaw.Owner.Creature, roachClaw.DynamicVars.Strength.BaseValue, roachClaw.Owner.Creature, null);
        DexterityPower unused2 = await PowerCmd.Apply<DexterityPower>(choiceContext, roachClaw.Owner.Creature, -roachClaw.DynamicVars.Strength.BaseValue, roachClaw.Owner.Creature, null);
    }
}