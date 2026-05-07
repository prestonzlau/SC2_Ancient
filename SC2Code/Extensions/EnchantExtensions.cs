using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Runs.History;
using SC2.SC2Code.Enchantments;

namespace SC2.SC2Code.Extensions;

public static class EnchantExtensions
    {
        private record EnchantmentOption(Action<CardModel> Apply, Func<CardModel, bool> CanApply = null);
        
        private static List<EnchantmentOption> BuildEnchantmentPool()
        {
            var pool = new List<EnchantmentOption>();
            //Pool Count for weighting purposes, certain types are commented out and may be added back if maybe too strong.
            pool.AddMultiple(1, card => CustomEnchant<Adroit>(card, 3));
            //Clone rest action is connected to pael relic.
            pool.AddMultiple(1, card => CustomEnchant<Clone>(card, 4));
            pool.AddMultiple(1, card => CustomEnchant<Corrupted>(card, 1), c => c.Type == CardType.Attack);
            pool.AddMultiple(1, card => CustomEnchant<Glam>(card, 1));
            pool.AddMultiple(1, card => CustomEnchant<Goopy>(card, 1),c => c.GainsBlock);
            pool.AddMultiple(1, card => CustomEnchant<Imbued>(card, 1)/*,c => c.Type == CardType.Skill*/);
            pool.AddMultiple(1, card => CustomEnchant<Inky>(card, 1), c => c.Type == CardType.Attack);
            pool.AddMultiple(1, card => CustomEnchant<Instinct>(card, 1), c => c.Type == CardType.Attack);
            pool.AddMultiple(1, card => CustomEnchant<Momentum>(card, 5), c => c.Type == CardType.Attack);
            pool.AddMultiple(1, card => CustomEnchant<Nimble>(card, 2),c => c.GainsBlock);
            pool.AddMultiple(1, card => CustomEnchant<PerfectFit>(card, 1));
            pool.AddMultiple(1, card => CustomEnchant<RoyallyApproved>(card, 1)/*,c => c.Type is CardType.Skill or CardType.Attack*/);
            pool.AddMultiple(1, card => CustomEnchant<Sharp>(card, 2), c => c.Type == CardType.Attack);
            pool.AddMultiple(1, card => CustomEnchant<Sharp>(card, 3), c => c.Type == CardType.Attack);
            pool.AddMultiple(1, card => CustomEnchant<Swift>(card, 1));
            pool.AddMultiple(1, card => CustomEnchant<Swift>(card, 2));
            pool.AddMultiple(1, card => CustomEnchant<Swift>(card, 3));
            pool.AddMultiple(1, card => CustomEnchant<SoulsPower>(card, 1),c => c.CanonicalKeywords.Contains(CardKeyword.Exhaust));
            pool.AddMultiple(1, card => CustomEnchant<Sown>(card, 1));
            pool.AddMultiple(1, card => CustomEnchant<Spiral>(card, 1));    
            pool.AddMultiple(1, card => CustomEnchant<Steady>(card, 1),c => (!(c.CanonicalKeywords.Contains(CardKeyword.Ethereal))));
            pool.AddMultiple(1, card => CustomEnchant<TezcatarasEmber>(card, 1), c => c.Type == CardType.Attack);
            pool.AddMultiple(1, card => CustomEnchant<Vigorous>(card, 8), c => c.Type == CardType.Attack);
            //Custom Enchantments here
            pool.AddMultiple(1, card => CustomEnchant<Corrosive>(card, 1), c => c.Type == CardType.Attack);
            
            return pool;
        }
        

        private static void AddMultiple(this List<EnchantmentOption> list, int count, Action<CardModel> apply, Func<CardModel, bool> canApply = null)
        {
            for (int i = 0; i < count; i++)
            {
                list.Add(new EnchantmentOption(apply, canApply));
            }
        }

        private static readonly Lazy<List<EnchantmentOption>> EnchantmentPool = new(BuildEnchantmentPool);

        public static void ApplyRandomEnchantment(CardModel cardToEnchant)
        {
            var rng = cardToEnchant.Owner.RunState.Rng.Niche;
            var pool = EnchantmentPool.Value;

            // Filter options valid for this card's type
            var validOptions = pool
                .Where(opt => opt.CanApply == null || opt.CanApply(cardToEnchant))
                .ToList();

            if (validOptions.Count == 0)
                return; 

            int index = rng.NextInt(0, validOptions.Count);
            validOptions[index].Apply(cardToEnchant);
        }
        public static T? CustomEnchant<T>(CardModel card, Decimal amount) where T : EnchantmentModel
        {
            return CustomEnchant(ModelDb.Enchantment<T>().ToMutable(), card, amount) as T;
        }

        public static EnchantmentModel? CustomEnchant(
            EnchantmentModel enchantment,
            CardModel card,
            Decimal amount)
        {
            enchantment.AssertMutable();
            if (card.Enchantment == null)
            {
                card.EnchantInternal(enchantment, amount);
                enchantment.ModifyCard();
            }
            else if (card.Enchantment.GetType() == enchantment.GetType())
                card.Enchantment.Amount += (int) amount;
            else
                throw new InvalidOperationException($"Cannot enchant {card.Id} with {enchantment.Id} because it already has enchantment {card.Enchantment.Id}.");
            card.FinalizeUpgradeInternal();
            CardPile pile = card.Pile;
            if (pile != null && pile.Type == PileType.Deck)
                card.Owner.RunState.CurrentMapPointHistoryEntry?.GetEntry(card.Owner.NetId).CardsEnchanted.Add(new CardEnchantmentHistoryEntry(card, enchantment.Id));
            return card.Enchantment;
        }
    }