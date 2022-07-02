using System.Collections.Generic;
using Newtonsoft.Json;

namespace CostSplitter
{
    internal class SplitterInput
    {
        public string ProjectName { get; }
        public List<ExpenseCategory> Categories { get; }
        
        public List<Person> Persons { get; }

        public List<Expense> Expenses { get; }

        [JsonConstructor]
        public SplitterInput(string projectName, List<ExpenseCategory> categories, List<Person> persons, List<Expense> expenses)
        {
            ProjectName = projectName;
            Categories = categories;
            Persons = persons;
            Expenses = expenses;
        }
    }
}
