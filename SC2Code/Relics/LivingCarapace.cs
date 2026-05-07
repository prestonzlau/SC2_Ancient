using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace SC2.SC2Code.Relics;



[Pool(typeof(EventRelicPool))]

public  class LivingCarapace : Sc2RelicModel
{
    private bool _isActivating;
    private int _rewardsSacrificed;
    public override RelicRarity Rarity =>
        RelicRarity.Ancient;
    
    public override bool ShowCounter => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Sacrifices", 3M),new HealVar(1M),new MaxHpVar(10M)];
    
    public override int DisplayAmount
    {
        get
        {
            return !this.IsActivating ? this.RewardsSacrificed % this.DynamicVars["Sacrifices"].IntValue : this.DynamicVars["Sacrifices"].IntValue;
        }
    }
    
    private bool IsActivating
    {
        get => this._isActivating;
        set
        {
            this.AssertMutable();
            this._isActivating = value;
            this.InvokeDisplayAmountChanged();
        }
    }
    
    [SavedProperty]
    public int RewardsSacrificed
    {
        get => this._rewardsSacrificed;
        set
        {
            this.AssertMutable();
            this._rewardsSacrificed = value;
            this.InvokeDisplayAmountChanged();
        }
    }
    
    public override bool TryModifyCardRewardAlternatives(Player player, CardReward cardReward, List<CardRewardAlternative> alternatives)
    {
        if (Owner != player)
        {
            return false;
        }
        alternatives.Add(new CardRewardAlternative("ABSORB", OnAbsorbCard, PostAlternateCardRewardAction.DismissScreenAndRemoveReward));
        return true;
    }

    private async Task OnAbsorbCard()
    {
        await AbsorbCard();
        await Task.CompletedTask;
    }
    
    
    public async Task AbsorbCard()
    {
        LivingCarapace livingCarapace = this;
        livingCarapace.RewardsSacrificed++;
        livingCarapace.Flash();
        await TaskHelper.RunSafely(livingCarapace.DoActivateVisuals());
        await CreatureCmd.Heal(livingCarapace.Owner.Creature, livingCarapace.DynamicVars.Heal.BaseValue);
        if (livingCarapace.RewardsSacrificed % livingCarapace.DynamicVars["Sacrifices"].IntValue != 0)
            return;
        await TaskHelper.RunSafely(livingCarapace.DoActivateVisuals());
        await CreatureCmd.GainMaxHp(livingCarapace.Owner.Creature, livingCarapace.DynamicVars.MaxHp.BaseValue);
    }

    private async Task DoActivateVisuals()
    {
        this.IsActivating = true;
        await Cmd.Wait(1f);
        this.IsActivating = false;
    }
}