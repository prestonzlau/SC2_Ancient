using BaseLib.Abstracts;
using BaseLib.Utils;
using SC2.SC2Code.Relics;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using SC2.SC2Code.Extensions;

namespace SC2.SC2Code.Ancients;


[Pool(typeof(AncientEventModel))]

public class Overlord : CustomAncientModel
{
    
    public override string CustomScenePath => "res://"+"sc2-overlord.tscn".AncientImagePath();
    public override string CustomMapIconOutlinePath => "res://"+"map_icon_outline.png".AncientImagePath();
    public override string CustomMapIconPath => "res://"+"map_icon.png".AncientImagePath();
    public override string CustomRunHistoryIconPath  => "res://"+"sc2-overlord.png".AncientImagePath();
    public override string CustomRunHistoryIconOutlinePath  => "res://"+"sc2-overlord_outline.png".AncientImagePath();

    protected override OptionPools MakeOptionPools => new(
        OptionPool1,
        OptionPool2,
        OptionPool3);
    private WeightedList<AncientOption> OptionPool1 =>
    [
        AncientOption<CorrosiveSpines>(3),
        AncientOption<UnstableGenePack>(3),
        AncientOption<EvolutionSlurry>(2)
    ];
    private WeightedList<AncientOption> OptionPool2 =>
    [
        AncientOption<MutaliskHeart>(3), 
        AncientOption<RoachClaw>(3),
        AncientOption<ZerglingGland>(2)
    ];
    private WeightedList<AncientOption> OptionPool3 =>
    [
        AncientOption<ZerglingSwarm>(3), 
        AncientOption<LivingCarapace>(3),
        AncientOption<CreepTumor>(2)
    ]; 
    
    public override bool IsValidForAct(ActModel act)
    {
        return act.ActNumber() == 2;
    }
    
    public override IEnumerable<EventOption> AllPossibleOptions => [
        RelicOption<CorrosiveSpines>(),
        RelicOption<UnstableGenePack>(),
        RelicOption<EvolutionSlurry>(),
        RelicOption<MutaliskHeart>(),
        RelicOption<RoachClaw>(),
        RelicOption<ZerglingGland>(),
        RelicOption<ZerglingSwarm>(),
        RelicOption<LivingCarapace>(),
        RelicOption<CreepTumor>(),
    ];
}