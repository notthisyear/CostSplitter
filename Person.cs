using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CostSplitter
{
    internal class Person
    {
        #region Public properties
        public string Name { get; init; }

        public decimal TotalExpenses
        {
            get 
            { 
                decimal totalExpenses = 0;
                _expenses.ForEach(x => totalExpenses += x.Amount);
                return totalExpenses;
            }
        }

        public decimal Total => TotalExpenses - _totalDebt;
        #endregion

        #region Private fields
        private readonly List<int> _categoriesIncludedIn = new();
        private readonly List<Expense> _expenses = new();
        private decimal _totalDebt = 0;
        #endregion

        [JsonConstructor]
        public Person(string name, int[] categories)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            if (categories is not null && categories.Length > 0)
                Array.ForEach(categories, x => _categoriesIncludedIn.Add(x));
        }

        #region Public methods
        public void AddExpense(Expense expense)
            => _expenses.Add(expense);

        public void AddDebt(decimal debt)
            => _totalDebt += debt;

        public List<Expense> GetExpensesForCategory(int categoryId)
            => _expenses.Where(x => x.CategoryId == categoryId).ToList();

        public decimal GetSumOfExpensesForCategory(int categoryId)
            => _expenses.Where(x => x.CategoryId == categoryId).Select(x => x.Amount).Sum();

        public bool IsIncludedInCategory(int categoryId)
            => _categoriesIncludedIn.Contains(categoryId);
        #endregion
    }
}
