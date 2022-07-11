using CommandLine;

namespace CostSplitter
{
    internal class CostSplitterImporterArguments
    {
        [Option(longName: "persons-and-expenses-list", shortName: 'p', Required = true, HelpText = "The list of persons and groups to include")]
        public string? PersonsAndExpensesListPath { get; set; }

        [Option(longName: "culture", shortName: 'c', HelpText = "The culture to use (sets currency and similar)", Default = "sv-SE")]
        public string? CultureString { get; set; }

        [Option(longName: "generate-html", HelpText = "Generate HTML.", Default = false)]
        public bool GenerateHtml { get; set; }

        [Option(longName: "html-target-path", shortName: 't', HelpText = "Where to place the generated HTML files, defaults to current folder")]
        public string? HtmlTargetPath { get; set; }

        [Option(longName: "use-local-js-and-css", HelpText = "Use local Javascript and CSS resources", Default = false)]
        public bool UseLocalJsAndCss { get; set; }

        [Option(longName: "local-js-and-css-path", HelpText = $"The path to a folder containing the local Javascript and CSS resources. The folder is expected to contain the files '{Program.MainCssFileName}', '{Program.BootstrapJavascriptFileName}' and '{Program.JqueryJavascriptFileName}'")]
        public string? LocalJsAndCssPath { get; set; }

        [Option(longName: "output-result-to-console", HelpText = "Print result to console", Default = false)]
        public bool OutputResultToConsole { get; set; }

        [Option(longName: "print-category-details", HelpText = "Print details of categories to console", Default = false)]
        public bool PrintCategoryDetails { get; set; }

    }
}
