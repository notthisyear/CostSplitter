using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace CostSplitter
{
    internal class HtmlFile
    {
        #region Private fields
        private readonly string _targetFolder;
        private readonly string _fileName;
        private readonly string _applicationName;
        private readonly string _title;
        private readonly StringBuilder _contentBuilder;
        private readonly NumberFormatInfo _formattingInformation;
        #endregion

        public HtmlFile(string targetFolder, string fileName, string applicationName, string title, NumberFormatInfo formattingInformations)
        {
            _targetFolder = targetFolder;
            _fileName = fileName;
            _applicationName = applicationName;
            _title = title;
            _formattingInformation = formattingInformations;

            _contentBuilder = new();
        }

        public void FlushFile()
        {
            string fullPath = Path.Join(_targetFolder, _fileName);
            string targetFolder = Path.GetDirectoryName(fullPath) ?? string.Empty;
            if (!string.IsNullOrEmpty(targetFolder) && !Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            File.WriteAllText($"{Path.Join(_targetFolder, _fileName)}.html", _contentBuilder.ToString());
        }

        public void StartDocument()
        {
            _contentBuilder.AppendLine("<!DOCTYPE html>");
            _contentBuilder.AppendLine("<html lang=\"en\">");
        }

        public void EndDocument()
        {
            _contentBuilder.AppendLine("</html>");
        }

        public void StartHtmlBody()
        {
            _contentBuilder.AppendLine("<body>");
        }

        public void EndHtmlBody()
        {
            _contentBuilder.AppendLine("</body>");
        }

        public void InsertHtmlHead(bool useLocalPath, string? stylesheetParentFolder)
        {
            if (useLocalPath && string.IsNullOrEmpty(stylesheetParentFolder))
                throw new ArgumentNullException(nameof(stylesheetParentFolder));


            _contentBuilder.AppendLine("<head>");
            _contentBuilder.AppendLine("<meta charset=\"utf-8\">");
            _contentBuilder.AppendLine($"<title>{_applicationName} | {_title}</title>");
            if (useLocalPath)
                _contentBuilder.AppendLine($"<link rel=\"stylesheet\" href=\"{stylesheetParentFolder}/{Program.MainCssFileName}\">");
            else
                _contentBuilder.AppendLine("<link rel=\"stylesheet\" href=\"https://bootswatch.com/5/darkly/bootstrap.min.css\">");
            _contentBuilder.AppendLine("</head>");
        }

        public void InsertNavigationBar(string currentlySelectedOption, string mainFileName, params (string displayName, string path)[] options)
        {
            _contentBuilder.AppendLine("<nav class=\"navbar navbar-expand-lg navbar-dark bg-dark\">");
            _contentBuilder.AppendLine("<div class=\"container-fluid\">");

            _contentBuilder.Append("<a class=\"navbar-brand\" href=");
            if (currentlySelectedOption.Equals(_fileName, StringComparison.InvariantCulture))
                _contentBuilder.AppendLine($"\"#\">{_applicationName}</a>");
            else
                _contentBuilder.AppendLine($"\"../{mainFileName}\">{_applicationName}</a>");

            _contentBuilder.Append("<button class=\"navbar-toggler\" type=\"button\" data-bs-toggle=\"collapse\" ");
            _contentBuilder.Append("data-bs-target=\"#navbarColor01\" aria-controls=\"navbarColor01\" aria-expanded=\"false\" ");
            _contentBuilder.AppendLine("aria-label=\"Toggle navigation\">");
            _contentBuilder.AppendLine("<span class=\"navbar-toggler-icon\"></span>");
            _contentBuilder.AppendLine("</button>");

            _contentBuilder.AppendLine("<div class=\"collapse navbar-collapse\" id=\"navbarColor01\">");
            _contentBuilder.AppendLine("<ul class=\"navbar-nav me-auto\">");

            foreach ((string displayName, string path) in options)
            {
                bool isSelected = displayName.Equals(currentlySelectedOption, StringComparison.InvariantCultureIgnoreCase);
                string actualPath = isSelected ? "#" : $"{path}.html";
                string linkClass = isSelected ? "nav-link active" : "nav-link";

                _contentBuilder.AppendLine("<li class=\"nav-item\">");
                _contentBuilder.AppendLine($"<a class=\"{linkClass}\" href=\"{actualPath}\">{displayName}");

                if (isSelected)
                    _contentBuilder.AppendLine("<span class=\"visually-hidden\">(current)</span>");

                _contentBuilder.AppendLine("</a>");
                _contentBuilder.AppendLine("</li>");
            }

            _contentBuilder.AppendLine("</ul>");
            _contentBuilder.AppendLine("</div>");
            _contentBuilder.AppendLine("</div>");
            _contentBuilder.AppendLine("</nav>");
        }

        public void InsertHeadline(string pageHeadline)
        {
            _contentBuilder.AppendLine("<div class=\"mt-5 d-flex justify-content-center\">");
            _contentBuilder.AppendLine($"<h2>{pageHeadline} of <small class=\"text-muted\">{_title}</small></h2>");
            _contentBuilder.AppendLine("</div>");
        }

        public void InsertSummaryTable(decimal totalExpenses, int numberOfPeople, int numberOfCategories)
        {
            _contentBuilder.AppendLine("<div class=\"mt-5 row justify-content-center\">");
            _contentBuilder.AppendLine("<div class=\"col-auto\">");
            _contentBuilder.AppendLine("<table class=\"table table-hover\">");
            _contentBuilder.AppendLine("<tbody>");
            _contentBuilder.AppendLine("<tr>");
            _contentBuilder.AppendLine("<td class=\"text-muted\">Total expenses</th>");
            _contentBuilder.AppendLine($"<td>{totalExpenses.FormatAsCurrency(_formattingInformation)}</td>");
            _contentBuilder.AppendLine("</tr>");
            _contentBuilder.AppendLine("<tr>");
            _contentBuilder.AppendLine("<th class=\"text-muted\">Number of people:</th>");
            _contentBuilder.AppendLine($"<td>{numberOfPeople}</td>");
            _contentBuilder.AppendLine("</tr>");
            _contentBuilder.AppendLine("<tr>");
            _contentBuilder.AppendLine("<th class=\"text-muted\">Number of categories:</th>");
            _contentBuilder.AppendLine($"<td>{numberOfCategories}</td>");
            _contentBuilder.AppendLine("</tr>");
            _contentBuilder.AppendLine("</tbody>");
            _contentBuilder.AppendLine("</table>");
            _contentBuilder.AppendLine("</div>");
            _contentBuilder.AppendLine("</div>");
        }

        public void InsertCategorySummary(List<CategoryInformation> categoryInformation)
        {
            _contentBuilder.AppendLine("<div class=\"mt-5\">");
            _contentBuilder.AppendLine("<div class=\"row justify-content-center\">");
            _contentBuilder.AppendLine("<div class=\"col-auto\">");
            _contentBuilder.AppendLine("<h4>Categories</h4>");

            _contentBuilder.AppendLine("<div class=\"mt-3 accordion\" id=\"categoryAccordion\">");

            int i = 1;
            foreach (var catInfo in categoryInformation)
            {
                _contentBuilder.AppendLine("<div class=\"accordion-item\">");
                _contentBuilder.AppendLine($"<h2 class=\"accordion-header\" id=\"heading{i}\">");
                _contentBuilder.Append("<button class=\"accordion-button collapsed\" type=\"button\"  ");
                _contentBuilder.Append($"data-bs-toggle=\"collapse\" data-bs-target=\"#collapse{i}\" aria-expanded=\"false\" ");
                _contentBuilder.AppendLine($"aria-controls=\"collapse{i}\">");

                _contentBuilder.AppendLine("<ul style=\"list-style-type: none;\" class=\"px-5\">");
                _contentBuilder.AppendLine($"<li><h5><b>{catInfo.DisplayName}: {catInfo.Total.FormatAsCurrency(_formattingInformation)}</b></h5></li>");
                _contentBuilder.AppendLine($"<li><small class=\"text-muted\">{catInfo.TotalPerPerson.FormatAsCurrency(_formattingInformation)} per person</small></li>");
                _contentBuilder.AppendLine("</ul>");

                _contentBuilder.AppendLine("</button>");
                _contentBuilder.AppendLine("</h2>");
                _contentBuilder.Append($"<div id=\"collapse{i}\" class=\"accordion-collapse collapse\" ");
                _contentBuilder.AppendLine($"aria-labelledby=\"heading{i}\" data-bs-parent=\"#categoryAccordion\" style=\"\">");
                _contentBuilder.AppendLine("<div class=\"accordion-body\">");
                _contentBuilder.AppendLine("<ul class=\"list-group\">");


                foreach (var (name, amount) in catInfo.Details)
                {
                    _contentBuilder.AppendLine("<li class=\"list-group-item d-flex justify-content-between align-items-center\">");
                    _contentBuilder.AppendLine(name);
                    string textType = amount < 0 ? "text-danger" : "text-success";
                    _contentBuilder.AppendLine($"<span class=\"badge bg-primary rounded-pill {textType}\">{amount.FormatAsCurrency(_formattingInformation)}</span>");
                    _contentBuilder.AppendLine("</li>");
                }

                _contentBuilder.AppendLine("</ul>");
                _contentBuilder.AppendLine("</div>");
                _contentBuilder.AppendLine("</div>");
                _contentBuilder.AppendLine("</div>");

                i++;
            }
        }

        public void InsertExpensesTable(List<Expense> expenses, List<ExpenseCategory> categories)
        {
            _contentBuilder.AppendLine("<div class=\"mt-5 row justify-content-center\">");
            _contentBuilder.AppendLine("<div class=\"col-auto\">");
            _contentBuilder.AppendLine("<table class=\"table table-hover\">");

            _contentBuilder.AppendLine("<thead>");
            _contentBuilder.AppendLine("<tr>");
            _contentBuilder.AppendLine("<th scope=\"col\">Description</th>");
            _contentBuilder.AppendLine("<th scope=\"col\">Category</th>");
            _contentBuilder.AppendLine("<th scope=\"col\">Sum</th>");
            _contentBuilder.AppendLine("<th scope=\"col\">Creditor</th>");
            _contentBuilder.AppendLine("</tr>");
            _contentBuilder.AppendLine("</thead>");

            _contentBuilder.AppendLine("<tbody>");
            string[] tableRowColors = new string[] { "table-dark", "table-secondary" };
            int colorIdx = 1;
            foreach (var expense in expenses)
            {
                colorIdx = (colorIdx + 1) % 2;
                _contentBuilder.AppendLine($"<tr class=\"{tableRowColors[colorIdx]}\">");
                _contentBuilder.AppendLine($"<td>{expense.Description}</td>");
                _contentBuilder.AppendLine($"<td>{categories.FirstOrDefault(x => x.Id == expense.CategoryId)!.DisplayName}</td>");
                _contentBuilder.AppendLine($"<td>{expense.Amount.FormatAsCurrency(_formattingInformation)}</td>");
                _contentBuilder.AppendLine($"<td>{expense.Creditor}</td>");
                _contentBuilder.AppendLine("</tr>");
            }
            _contentBuilder.AppendLine("</tbody>");
            _contentBuilder.AppendLine("</table>");
            _contentBuilder.AppendLine("</div>");
            _contentBuilder.AppendLine("</div>");
        }

        public void InsertResultTable(List<Person> persons)
        {
            _contentBuilder.AppendLine("<div class=\"mt-5 row justify-content-center\">");
            _contentBuilder.AppendLine("<div class=\"col-auto\">");
            _contentBuilder.AppendLine("<table class=\"table table-hover\">");

            _contentBuilder.AppendLine("<tbody>");
            string[] tableRowColors = new string[] { "table-dark", "table-secondary" };
            int colorIdx = 1;
            decimal sum = 0;
            foreach (var person in persons)
            {
                colorIdx = (colorIdx + 1) % 2;
                string textClass = person.Total < 0 ? "text-danger" : "text-success";
                _contentBuilder.AppendLine($"<tr class=\"{tableRowColors[colorIdx]}\">");
                _contentBuilder.AppendLine($"<td>{person.Name}</td>");
                _contentBuilder.AppendLine($"<td class=\"{textClass} text-end\">{person.Total.FormatAsCurrency(_formattingInformation)}</td>");
                _contentBuilder.AppendLine("</tr>");
                sum += person.Total;
            }
            _contentBuilder.AppendLine("</tbody>");
            _contentBuilder.AppendLine("</table>");
            _contentBuilder.AppendLine("</div>");
            _contentBuilder.AppendLine("</div>");
        }

        public void InsertScripts(bool useLocalPath, string? scriptsFolder)
        {
            if (useLocalPath && string.IsNullOrEmpty(scriptsFolder))
                throw new ArgumentNullException(nameof(scriptsFolder));

            if (useLocalPath) 
            { 
                _contentBuilder.AppendLine($"<script src=\"{scriptsFolder}/{Program.JqueryJavascriptFileName}\"></script>");
                _contentBuilder.AppendLine($"<script src=\"{scriptsFolder}/{Program.BootstrapJavascriptFileName}\"></script>");
            }
            else
            {
                _contentBuilder.AppendLine("<script src=\"https://code.jquery.com/jquery-3.6.0.min.js\" integrity=\"sha256-/xUj+3OJU5yExlq6GSYGSHk7tPXikynS7ogEvDej/m4=\" crossorigin=\"anonymous\"></script>");
                _contentBuilder.AppendLine("<script src=\"https://cdn.jsdelivr.net/npm/bootstrap@5.2.0-beta1/dist/js/bootstrap.min.js\" integrity=\"sha384-kjU+l4N0Yf4ZOJErLsIcvOU2qSb74wXpOhqTvwVx3OElZRweTnQ6d31fXEoRD1Jy\" crossorigin=\"anonymous\"></script>");
            }
        }
    }
}
