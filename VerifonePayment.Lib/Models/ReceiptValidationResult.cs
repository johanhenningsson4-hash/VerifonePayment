using System;
using System.Collections.Generic;
using System.Linq;

namespace VerifonePayment.Lib.Models
{
    /// <summary>
    /// Represents the result of receipt validation.
    /// </summary>
    public class ReceiptValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether the receipt is valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Gets a summary message of the validation result.
        /// </summary>
        public string Summary { get; private set; }

        /// <summary>
        /// Gets the list of validation issues (errors).
        /// </summary>
        public IReadOnlyList<string> Issues { get; private set; }

        /// <summary>
        /// Gets the list of validation warnings.
        /// </summary>
        public IReadOnlyList<string> Warnings { get; private set; }

        /// <summary>
        /// Gets the total number of issues and warnings.
        /// </summary>
        public int TotalIssuesAndWarnings => Issues.Count + Warnings.Count;

        /// <summary>
        /// Gets a value indicating whether there are any warnings.
        /// </summary>
        public bool HasWarnings => Warnings.Count > 0;

        /// <summary>
        /// Gets a value indicating whether there are any issues.
        /// </summary>
        public bool HasIssues => Issues.Count > 0;

        /// <summary>
        /// Initializes a new instance of the ReceiptValidationResult class.
        /// </summary>
        /// <param name="isValid">Whether the receipt is valid</param>
        /// <param name="summary">Summary message</param>
        /// <param name="issues">List of issues (optional)</param>
        /// <param name="warnings">List of warnings (optional)</param>
        public ReceiptValidationResult(bool isValid, string summary, 
            IEnumerable<string> issues = null, IEnumerable<string> warnings = null)
        {
            IsValid = isValid;
            Summary = summary ?? throw new ArgumentNullException(nameof(summary));
            Issues = (issues?.ToList() ?? new List<string>()).AsReadOnly();
            Warnings = (warnings?.ToList() ?? new List<string>()).AsReadOnly();
        }

        /// <summary>
        /// Gets a detailed report of the validation results.
        /// </summary>
        /// <returns>Formatted validation report</returns>
        public string GetDetailedReport()
        {
            var report = $"=== RECEIPT VALIDATION REPORT ===\n";
            report += $"Status: {(IsValid ? "VALID" : "INVALID")}\n";
            report += $"Summary: {Summary}\n";
            report += $"Issues: {Issues.Count}, Warnings: {Warnings.Count}\n\n";

            if (HasIssues)
            {
                report += "ISSUES (Critical):\n";
                for (int i = 0; i < Issues.Count; i++)
                {
                    report += $"  {i + 1}. {Issues[i]}\n";
                }
                report += "\n";
            }

            if (HasWarnings)
            {
                report += "WARNINGS (Non-critical):\n";
                for (int i = 0; i < Warnings.Count; i++)
                {
                    report += $"  {i + 1}. {Warnings[i]}\n";
                }
                report += "\n";
            }

            if (!HasIssues && !HasWarnings)
            {
                report += "No issues or warnings found.\n";
            }

            report += "=== END VALIDATION REPORT ===";

            return report;
        }

        /// <summary>
        /// Gets a compact summary of the validation results.
        /// </summary>
        /// <returns>Compact validation summary</returns>
        public string GetCompactSummary()
        {
            if (IsValid && !HasWarnings)
                return "? Receipt is valid";

            if (IsValid && HasWarnings)
                return $"?? Receipt is valid with {Warnings.Count} warnings";

            return $"? Receipt is invalid ({Issues.Count} issues, {Warnings.Count} warnings)";
        }

        /// <summary>
        /// Returns a string representation of the validation result.
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return GetCompactSummary();
        }
    }
}