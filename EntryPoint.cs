using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace CostSplitter
{
    internal class EntryPoint
    {
        public static void RunProgram(string applicationName, CostSplitterImporterArguments args)
        {
            var numberFormat = new CultureInfo(args.CultureString!, false).NumberFormat;
            var rawContent = File.ReadAllText(args.PersonsAndExpensesListPath!);

            (SplitterInput? input, Exception? e) = rawContent.DeserializeJsonString<SplitterInput>(convertSnakeCaseToPascalCase: true);
            if (e != default)
                throw e;

            foreach (var person in input!.Persons)
            {
                var matchingExpenses = input!.Expenses.Where(x => x.Creditor.Equals(person.Name, StringComparison.InvariantCulture));
                if (matchingExpenses.Any()) {
                    foreach (var expense in matchingExpenses)
                        person.AddExpense(expense);
                }
            }

            var categoryInformation = AddDebtFromEachCategoryAndGetExpensesPerCategory(input!.Persons, input!.Categories);

            if (args.GenerateHtml)
                GenerateHtmlFiles(applicationName, input!.ProjectName, args.HtmlTargetPath!, input!.Persons, input!.Categories, input!.Expenses, categoryInformation, args.UseLocalJsAndCss, args.LocalJsAndCssPath, numberFormat);

            if (args.OutputResultToConsole) 
            { 
                ConsoleWriter.OutputResultToConsole(input!.Persons, categoryInformation, applicationName, input!.ProjectName, numberFormat, args.PrintCategoryDetails);
                Console.ReadLine();
            }
        }

        private static List<CategoryInformation> AddDebtFromEachCategoryAndGetExpensesPerCategory(List<Person> persons, List<ExpenseCategory> categories)
        {
            List<CategoryInformation> categoryInformation = new();
            foreach (ExpenseCategory c in categories) 
            { 
                var categoryTotal = persons.Select(x => x.GetSumOfExpensesForCategory(c.Id)).Sum();
                var personsIncluded = persons.Where(x => x.IsIncludedInCategory(c.Id));
                var costPerPerson = categoryTotal / personsIncluded.Count();

                CategoryInformation catInfo = new(c.DisplayName, c.Id, categoryTotal, costPerPerson);

                foreach (var p in personsIncluded)
                {
                    p.AddDebt(costPerPerson);
                    var currentTotal = p.GetSumOfExpensesForCategory(c.Id) - costPerPerson;
                    catInfo.AddNameAndAmount(p.Name, currentTotal);
                }
                categoryInformation.Add(catInfo);
            }

            return categoryInformation;
        }

        private static void GenerateHtmlFiles(string applicationName, string projectName, string targetFolder, List<Person> persons, List<ExpenseCategory> categories, List<Expense> expenses, List<CategoryInformation> categoryInformation, bool useLocalCssAndJs, string? pathToLocalCssAndJs, NumberFormatInfo formattingInformation)
        {
            // Main file
            HtmlFile file = new(targetFolder, "main", applicationName, projectName, formattingInformation);

            file.StartDocument();
            file.InsertHtmlHead(useLocalCssAndJs, pathToLocalCssAndJs);
            file.StartHtmlBody();

            file.InsertNavigationBar("Summary", "main", ("Summary", "main"), ("Expenses", "pages/expenses"), ("Result", "pages/result"));
            file.InsertHeadline("Summary");

            var total = persons.Select(x => x.TotalExpenses).Sum();
            file.InsertSummaryTable(total, persons.Count, categoryInformation.Count);
            file.InsertCategorySummary(categoryInformation);

            file.InsertScripts(useLocalCssAndJs, pathToLocalCssAndJs);
            file.EndHtmlBody();

            file.EndDocument();
            file.FlushFile();

            // Expenses file
            file = new(targetFolder, "pages/expenses", applicationName, projectName, formattingInformation);

            file.StartDocument();
            file.InsertHtmlHead(useLocalCssAndJs, pathToLocalCssAndJs);
            file.StartHtmlBody();

            file.InsertNavigationBar("Expenses", "main", ("Summary", "../main"), ("Expenses", "expenses"), ("Result", "result"));
            file.InsertHeadline("Expenses");

            file.InsertExpensesTable(expenses, categories);

            file.InsertScripts(useLocalCssAndJs, pathToLocalCssAndJs);
            file.EndHtmlBody();

            file.EndDocument();
            file.FlushFile();

            // Result file
            file = new(targetFolder, "pages/result", applicationName, projectName, formattingInformation);

            file.StartDocument();
            file.InsertHtmlHead(useLocalCssAndJs, pathToLocalCssAndJs);
            file.StartHtmlBody();

            file.InsertNavigationBar("Result", "main", ("Summary", "../main"), ("Expenses", "expenses"), ("Result", "result"));
            file.InsertHeadline("Result");

            file.InsertResultTable(persons);

            file.InsertScripts(useLocalCssAndJs, pathToLocalCssAndJs);
            file.EndHtmlBody();

            file.EndDocument();
            file.FlushFile();
        }
    }
}