using System;
using System.IO;
using System.Globalization;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace CostSplitter
{
    public static class Program
    {
        public const string ApplicationName = "Cost Splitter 2000";

        public const string MainCssFileName = "bootstrap.css";
        public const string BootstrapJavascriptFileName = "bootstrap.js";
        public const string JqueryJavascriptFileName = "jquery.js";

        private const string HelpHeading = ApplicationName + " - calculates how to split costs";
        
        public static void Main(string[] args)
        {
            Parser parser = new(x =>
            {
                x.HelpWriter = null;
                x.AutoHelp = true;
                x.AutoVersion = true;
            });

            ParserResult<CostSplitterImporterArguments> result = parser.ParseArguments<CostSplitterImporterArguments>(args);

            result.WithParsed(ValidateArguments);
            result.WithParsed(x => EntryPoint.RunProgram(ApplicationName, x));
            result.WithNotParsed(errors =>
            {
                bool isVersionRequest = errors.FirstOrDefault(x => x.Tag == ErrorType.VersionRequestedError) != default;
                bool isHelpRequest = errors.FirstOrDefault(x => x.Tag == ErrorType.HelpRequestedError) != default ||
                                        errors.FirstOrDefault(x => x.Tag == ErrorType.HelpVerbRequestedError) != default;

                string output = string.Empty;
                if (isHelpRequest)
                {
                    output = HelpText.AutoBuild(result,
                    h =>
                    {
                        h.Heading = HelpHeading;
                        return h;
                    });
                }
                else if (isVersionRequest)
                {
                    string version = string.Empty; // TODO
                    output = $"CostSplitter ({(string.IsNullOrEmpty(version) ? "no version tag found" : $"v. {version}")})";
                }
                else
                {
                    output = errors.Count() > 1 ? "ERRORS:\n" : "ERROR:\n";
                    foreach (Error error in errors)
                        output += '\t' + GetErrorText(error) + '\n';
                }
                Console.WriteLine(output);
            });
        }

        private static string GetErrorText(Error error)
        {
            return error switch
            {
                MissingValueOptionError missingValueError => $"Value for argument '{missingValueError.NameInfo.NameText}' is missing",
                UnknownOptionError unknownOptionError => $"Argument '{unknownOptionError.Token}' is unknown",
                MissingRequiredOptionError _ => $"A required option is missing value",
                SetValueExceptionError setValueExceptionError => $"Could not set value for argument '{setValueExceptionError.NameInfo.NameText}': {setValueExceptionError.Exception.Message}",
                BadFormatConversionError badFormatConversionError => $"Argument '{badFormatConversionError.NameInfo.NameText}' has bad format",
                _ => $"Argument parsing failed: '{error.Tag}'"
            };
        }

        private static void ValidateArguments(CostSplitterImporterArguments args) 
        {
            (bool allExists, string missingFile) = !args.UseLocalJsAndCss ?
                ValidateFilesExists(args.PersonsAndExpensesListPath) :
                ValidateFilesExists(args.PersonsAndExpensesListPath,
                Path.Join(args.LocalJsAndCssPath, MainCssFileName),
                Path.Join(args.LocalJsAndCssPath, BootstrapJavascriptFileName),
                Path.Join(args.LocalJsAndCssPath, JqueryJavascriptFileName));

            if (!allExists)
            {
                Console.WriteLine($"Could not find file '{missingFile}'");
                Environment.Exit(1);
            }


            if (args.GenerateHtml && string.IsNullOrEmpty(args.HtmlTargetPath))
                args.HtmlTargetPath = Directory.GetCurrentDirectory();

            if (args.GenerateHtml && !Directory.Exists(args.HtmlTargetPath))
                Directory.CreateDirectory(args.HtmlTargetPath!);
            
            try
            { 
                _ = CultureInfo.GetCultureInfo(args.CultureString ?? string.Empty, true);
            }
            catch (CultureNotFoundException)
            {
                Console.WriteLine($"ERROR: Culture '{args.CultureString ?? string.Empty}' is not known");
                Environment.Exit(1);
            }
        }

        private static (bool allExists, string firstMissingFile) ValidateFilesExists(params string?[] files) 
        {
            foreach(string? path in files)
            {
                if (string.IsNullOrEmpty(path))
                    return (false, "");

                if (!File.Exists(path))
                    return (false, path);
            }
            return (true, string.Empty);
        }
    }
}
