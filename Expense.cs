using Newtonsoft.Json;

namespace CostSplitter
{
    internal record Expense
    {
        public string Creditor { get; }

        public string Description { get; }

        public int CategoryId { get; }

        public decimal Amount { get; }

        [JsonConstructor]
        public Expense(string creditor, string description, int categoryId, decimal amount)
        {
            Creditor = creditor;
            Description = description;
            CategoryId = categoryId;
            Amount = amount;
        }
    }
}
