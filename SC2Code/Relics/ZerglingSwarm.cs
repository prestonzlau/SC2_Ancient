using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace SC2.SC2Code.Relics;


[Pool(typeof(EventRelicPool))]

public  class ZerglingSwarm : Sc2RelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    
    public override bool ShowCounter => true;
    private int _zerglingCount;
    
    public override int DisplayAmount
    {
        get
        {
            return _zerglingCount;
        }
    }
    
    [SavedProperty]
    private int ZerglingCount
    {
        get => _zerglingCount;
        set
        {
            AssertMutable();
            _zerglingCount = value;
            InvokeDisplayAmountChanged();
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1M,ValueProp.Unpowered), new DynamicVar("Zerglings",3M)];
    
    public override Task AfterObtained()
    {
        ZerglingSwarm zerglingSwarm = this;
        zerglingSwarm.ZerglingCount+=DynamicVars["Zerglings"].IntValue;
        return Task.CompletedTask;
    }
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        ZerglingSwarm zerglingSwarm = this;
        if (player != zerglingSwarm.Owner)
            return;
        zerglingSwarm.Flash();
        for (int i = 0; i < zerglingSwarm._zerglingCount; ++i){
            Creature target = zerglingSwarm.Owner.RunState.Rng.CombatTargets.NextItem((IEnumerable<Creature>) zerglingSwarm.Owner.Creature.CombatState?.HittableEnemies);
            if (target == null)
                return;
            VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_attack_blunt");
            IEnumerable<DamageResult> unused = await CreatureCmd.Damage(choiceContext, target, zerglingSwarm.DynamicVars.Damage, zerglingSwarm.Owner.Creature);
        }

    }
    
    public override Task AfterCombatVictory(CombatRoom room)
    {
        ZerglingSwarm zerglingSwarm = this;
        if (room.RoomType != RoomType.Elite)
        {
            return Task.CompletedTask;
        }

        zerglingSwarm.ZerglingCount+=DynamicVars["Zerglings"].IntValue;
        Flash();
        return Task.CompletedTask;
    }
}