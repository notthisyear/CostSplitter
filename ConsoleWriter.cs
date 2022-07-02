using System;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace CostSplitter
{
    internal static class ConsoleWriter
    {
        private const int ConsoleLeftColumnWidth = 40;

        public static void OutputResultToConsole(List<Person> persons, List<CategoryInformation> categories, string applicatioName, string projectName, NumberFormatInfo formattingInformation, bool includeDetails)
        {
            var totalExpenses = persons.Select(x => x.TotalExpenses).Sum();
            WriteHeader(applicatioName, projectName, persons.Count, categories.Count, totalExpenses, formattingInformation);

            WriteDivider();
            WriteCategorySummary(categories, includeDetails, formattingInformation);
            
            WriteDivider();
            WriteAllExpenses(persons, categories, formattingInformation);

            WriteDivider();
            WriteResult(persons, formattingInformation);
        }

        #region Private methods
        private static void WriteHeader(string applicationName, string projectName, int numberOfPersons, int numberOfCategories, decimal total, NumberFormatInfo formattingInformation)
        {
            WriteTextToConsole($"-- {applicationName} | {projectName} --\n", foregroundColor: ConsoleColor.White);
            WriteLeftColumn("Total expenses:");
            WriteTextToConsole(total.FormatAsCurrency(formattingInformation), foregroundColor: ConsoleColor.White);

            WriteLeftColumn("Number of people:");
            WriteTextToConsole($"{numberOfPersons}", foregroundColor: ConsoleColor.White);

            WriteLeftColumn("Number of categories:");
            WriteTextToConsole($"{numberOfCategories}", foregroundColor: ConsoleColor.White);
        }

        private static void WriteCategorySummary(List<CategoryInformation> categories, bool includeDetails, NumberFormatInfo formattingInformation)
        {
            WriteTextToConsole("Per category:\n");
            foreach (CategoryInformation c in categories)
            {
                WriteLeftColumn($" {c.DisplayName}:", foregroundColor: ConsoleColor.White);
                WriteTextToConsole(c.Total.FormatAsCurrency(formattingInformation), includeNewline: false, foregroundColor: ConsoleColor.White);
                WriteTextToConsole(c.TotalPerPerson.FormatAsCurrency(formattingInformation, " ({0:C} per person)"), foregroundColor: ConsoleColor.DarkGray);

                if (includeDetails)
                {
                    WriteLeftColumn("   people involved:");
                    WriteTextToConsole("");

                    foreach ((string name, decimal total) in c.Details)
                    {
                        WriteLeftColumn($"   ...{name}:");
                        WriteTextToConsole(total.FormatAsCurrency(formattingInformation), foregroundColor: total >= 0 ? ConsoleColor.Green : ConsoleColor.Red);

                    }
                    WriteTextToConsole("");
                }
            }
        }

        private static void WriteAllExpenses(List<Person> persons, List<CategoryInformation> categories, NumberFormatInfo formattingInformation)
        {
            WriteTextToConsole("Expenses:\n");

            foreach (Person p in persons)
            {
                foreach (CategoryInformation c in categories)
                {
                    var expenses = p.GetExpensesForCategory(c.CategoryId);
                    foreach (var e in expenses)
                    {
                        WriteTextToConsole($" - {e.Description} | ", includeNewline: false);
                        WriteTextToConsole($"[{c.DisplayName}]", includeNewline: false, foregroundColor: ConsoleColor.DarkCyan);
                        WriteTextToConsole(e.Amount.FormatAsCurrency(formattingInformation, " | {0:C} | "), includeNewline: false);
                        WriteTextToConsole($"({p.Name})", foregroundColor: ConsoleColor.DarkGray);
                    }
                }
            }
        }

        private static void WriteResult(List<Person> persons, NumberFormatInfo formattingInformation)
        {
            WriteTextToConsole("Result:\n");
            foreach (Person p in persons)
            {
                WriteLeftColumn($"   {p.Name}:");
                WriteTextToConsole(p.Total.FormatAsCurrency(formattingInformation), foregroundColor: p.Total >= 0 ? ConsoleColor.Green : ConsoleColor.Red);
            }
        }

        private static void WriteLeftColumn(string text, ConsoleColor foregroundColor = ConsoleColor.DarkGray)
            => WriteTextToConsole($"{text,-ConsoleLeftColumnWidth}", includeNewline: false, foregroundColor);

        private static void WriteDivider()
            => WriteTextToConsole("\n-------------------------------\n", foregroundColor: ConsoleColor.White);

        

        private static void WriteTextToConsole(string text,
                                        bool includeNewline = true,
                                        ConsoleColor foregroundColor = ConsoleColor.Gray,
                                        ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;

            Console.Write(text);
            if (includeNewline)
                Console.Write('\n');

            Console.ResetColor();
        }
        #endregion

    }
}
