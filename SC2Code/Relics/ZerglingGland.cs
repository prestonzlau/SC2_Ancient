using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using SC2.SC2Code.Extensions;
using SC2.SC2Code.RestSite;

namespace SC2.SC2Code.Relics;





[Pool(typeof(EventRelicPool))]
public  class ZerglingGland : Sc2RelicModel
{
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    private int _adrenalCombats;
    private bool _adrenalActive;
    public override bool ShowCounter => true;
    

    
    public override int DisplayAmount
    {
        get
        {
            return _adrenalCombats;
        }
    }
    [SavedProperty]
    private int AdrenalCombats
         {
             get => _adrenalCombats;
             set
             {
                 AssertMutable();
                 _adrenalCombats = value;
                 InvokeDisplayAmountChanged();
             }
         }
    private bool AdrenalActive
    {
        get => _adrenalActive;
        set
        {
            AssertMutable();
            _adrenalActive = value;
        }
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DexterityPower>(1M),new PowerVar<StrengthPower>(1M), new EnergyVar(1)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get => [CustomHoverTip.GetStaticHoverTip("SC2-ADRENAL_OVERLOAD"),HoverTipFactory.ForEnergy(this),(HoverTipFactory.FromPower<DexterityPower>()),(HoverTipFactory.FromPower<StrengthPower>())];
    }
    public Task AdrenalOverloadRest(Player player)
    {
        ZerglingGland zerglingGland = this;
        zerglingGland.AdrenalCombats+=(2-zerglingGland.AdrenalCombats);
        Flash();
        return Task.CompletedTask;
    }

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner)
            return false;
        options.Add(new AdrenalOverloadRestSiteOption(player));
        return true;
    }
    
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        ZerglingGland zerglingGland = this;
        if (!(room is CombatRoom))
            return;
        if (zerglingGland.AdrenalCombats > 0)
        {
            zerglingGland._adrenalActive = true;
        }
        else
        {
            return;
        }

        DexterityPower unusedDexterityPower = await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), zerglingGland.Owner.Creature, zerglingGland.DynamicVars.Dexterity.BaseValue, zerglingGland.Owner.Creature, null);
        StrengthPower unusedStrengthPower = await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), zerglingGland.Owner.Creature, zerglingGland.DynamicVars.Strength.BaseValue, zerglingGland.Owner.Creature, null);
    }
    
    public override Decimal ModifyMaxEnergy(Player player, Decimal amount)
    {
        ZerglingGland zerglingGland = this;
        return player == Owner&&zerglingGland._adrenalActive ? amount + this.DynamicVars.Energy.IntValue:amount;
    }
    
    public override Task AfterCombatVictory(CombatRoom room)
    {
        ZerglingGland zerglingGland = this;
        if (zerglingGland.AdrenalCombats > 0)
        {
            zerglingGland.AdrenalCombats -= 1;
            Flash();
            zerglingGland._adrenalActive = false;
        }
        return Task.CompletedTask;
    }
}