using System.Collections.Generic;

namespace CostSplitter
{
    internal record CategoryInformation
    {
        #region Public properties
        public string DisplayName { get; }
        
        public int CategoryId { get; }

        public decimal Total { get; }

        public decimal TotalPerPerson { get; }

        public List<(string name, decimal amount)> Details { get; } = new();
        #endregion
        
        public CategoryInformation(string displayName, int categoryId, decimal total, decimal totalPerPerson)
        {
            DisplayName = displayName;
            CategoryId = categoryId;
            Total = total;
            TotalPerPerson = totalPerPerson;
        }
        
        public void AddNameAndAmount(string name, decimal amount)
        {
            Details.Add((name, amount));
        }
    }
}
