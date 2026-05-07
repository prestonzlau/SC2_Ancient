using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using SC2.SC2Code.Extensions;

namespace SC2.SC2Code.Relics;



[Pool(typeof(EventRelicPool))]

public class CreepTumor : Sc2RelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    
    public override bool ShowCounter => true;
    private int _biomassCount;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [(HoverTipFactory.Static(StaticHoverTip.Fatal)),CustomHoverTip.GetStaticHoverTip("SC2-Overkill")];

    int GetLevel(int biomass)
    {
        return (int)((Math.Sqrt(1 + 8 * (biomass / 50.0)) - 1) / 2);
    }
    

    int GetNextThreshold(int biomass)
    {
        int level = GetLevel(biomass);
        return 50 * (level+1);
    }
    
    
    public override int DisplayAmount
    {
        get
        {
            int level = (int)((Math.Sqrt(1 + 8 * (_biomassCount / 50.0)) - 1) / 2);
            return _biomassCount - (50 * (level * (level + 1) / 2));
        }
    }
    
    [SavedProperty]
    private int BiomassCount
    {
        get => _biomassCount;
        set
        {
            AssertMutable();
            _biomassCount = value;
            
            InvokeDisplayAmountChanged();
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars
    {
        
        get
        {   int level = GetLevel(BiomassCount);
            int biomassThreshold = GetNextThreshold(BiomassCount);
            return
            [
                new EnergyVar(1),
                new StringVar("level",level.ToString()),
                new StringVar("biomass",biomassThreshold.ToString())
            ];
        }
    }
    
    public override Decimal ModifyMaxEnergy(Player player, Decimal amount)
    {
        CreepTumor creepTumor = this;
        UpdateBioMass();
        //cheat for triangular formula
        return player == Owner ? amount + (int)((Math.Sqrt(1 + 8 * (creepTumor.BiomassCount / 50.0)) - 1) / 2)*DynamicVars.Energy.IntValue:amount;
    }

    
    public override Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer,
        DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        base.AfterDamageGiven(choiceContext, dealer, result, props, target, cardSource);
        CreepTumor creepTumor = this;
        if (target.Side == creepTumor.Owner.Creature.Side || target.IsSecondaryEnemy)
            return Task.CompletedTask;
        if (dealer == null)
        {
            return Task.CompletedTask;
        }

        var combatState = dealer.CombatState;
        
        if (combatState == null)
        {
            return Task.CompletedTask;
        }

        var overkillDamage = result.OverkillDamage;
        if (overkillDamage <= 0)
        {
            return Task.CompletedTask;
        }
        var unused = creepTumor.BiomassCount += overkillDamage;
        UpdateBioMass();
        return Task.CompletedTask;
    }
    
    private void UpdateBioMass()
    {
        int level = GetLevel(BiomassCount);
        int biomassThreshold = GetNextThreshold(BiomassCount);
        ((StringVar) DynamicVars["level"]).StringValue = level.ToString();
        ((StringVar) DynamicVars["biomass"]).StringValue = biomassThreshold.ToString();
        this.InvokeDisplayAmountChanged();
    }
}