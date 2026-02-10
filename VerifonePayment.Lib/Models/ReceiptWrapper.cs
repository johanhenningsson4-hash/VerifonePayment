using System;
using VerifoneSdk;

namespace VerifonePayment.Lib.Models
{
    /// <summary>
    /// Represents a receipt object with formatting and customization capabilities.
    /// Wrapper around the Verifone SDK Receipt class.
    /// </summary>
    public class ReceiptWrapper
    {
        private readonly Receipt _receipt;

        /// <summary>
        /// Initializes a new instance of the ReceiptWrapper class.
        /// </summary>
        /// <param name="receipt">The Verifone SDK Receipt object</param>
        internal ReceiptWrapper(Receipt receipt)
        {
            _receipt = receipt ?? throw new ArgumentNullException(nameof(receipt));
        }

        #region "Receipt Content Properties"

        /// <summary>
        /// Returns the final receipt as HTML.
        /// </summary>
        public string AsHtml => _receipt?.AsHtml ?? string.Empty;

        /// <summary>
        /// Returns the final receipt as plain text.
        /// </summary>
        public string AsPlainText => _receipt?.AsPlainText ?? string.Empty;

        /// <summary>
        /// The Business Info section of the receipt, as customized by the merchant.
        /// </summary>
        public string BusinessInfo => _receipt?.BusinessInfo ?? string.Empty;

        /// <summary>
        /// Return Cashier name.
        /// </summary>
        public string CashierName => _receipt?.CashierName ?? string.Empty;

        /// <summary>
        /// The footer as defined by the merchant in receipt settings.
        /// </summary>
        public string CustomFooter => _receipt?.CustomFooter ?? string.Empty;

        /// <summary>
        /// Return custom greeting message.
        /// </summary>
        public string CustomGreeting => _receipt?.CustomGreeting ?? string.Empty;

        /// <summary>
        /// The footer as modified by other applications.
        /// </summary>
        public string Footer => _receipt?.Footer ?? string.Empty;

        /// <summary>
        /// The online URL in an HTML-formatted string, including QR code if available.
        /// </summary>
        public string OnlineUrl => _receipt?.OnlineUrl ?? string.Empty;

        /// <summary>
        /// Returns the original HTML of the receipt as received from the payment application.
        /// </summary>
        public string OriginalHtml => _receipt?.OriginalHtml ?? string.Empty;

        /// <summary>
        /// The payment information, including EMV fields and compliance data.
        /// </summary>
        public string PaymentInformation => _receipt?.PaymentInformation ?? string.Empty;

        /// <summary>
        /// Return the QR code image.
        /// </summary>
        public string QrCodeImage => _receipt?.QrCodeImage ?? string.Empty;

        /// <summary>
        /// The transaction information, including basket, adjustments, offers, donations, taxes, and totals.
        /// </summary>
        public string TransactionInformation => _receipt?.TransactionInformation ?? string.Empty;

        #endregion

        #region "Receipt Configuration Properties"

        /// <summary>
        /// Returns true if the merchant has decided to include the cashier's name.
        /// </summary>
        public bool IsCashierNameIncluded => _receipt?.IsCashierNameIncluded ?? false;

        /// <summary>
        /// Returns true if the merchant has decided to include their logo.
        /// </summary>
        public bool IsLogoIncluded => _receipt?.IsLogoIncluded ?? false;

        /// <summary>
        /// Returns true if the merchant has decided to include an online url for the receipt.
        /// </summary>
        public bool IsOnlineUrlIncluded => _receipt?.IsOnlineUrlIncluded ?? false;

        /// <summary>
        /// Returns true if the merchant has decided to include the QR Code with the online url.
        /// </summary>
        public bool IsQrCodeIncluded => _receipt?.IsQrCodeIncluded ?? false;

        /// <summary>
        /// The type of receipt, generally either for the customer or for the merchant.
        /// </summary>
        public string ReceiptType
        {
            get
            {
                try
                {
                    return _receipt?.ReceiptType?.ToString() ?? "Unknown";
                }
                catch
                {
                    return "Unknown";
                }
            }
        }

        #endregion

        #region "Receipt Customization Methods"

        /// <summary>
        /// Returns true if the merchant has decided to include a custom header.
        /// </summary>
        /// <returns>True if custom header is included</returns>
        public bool HasCustomHeader()
        {
            return _receipt?.HasCustomHeader() ?? false;
        }

        /// <summary>
        /// Returns true if the merchant has decided to include a custom footer.
        /// </summary>
        /// <returns>True if custom footer is included</returns>
        public bool HasCustomFooter()
        {
            return _receipt?.HasCustomFooter() ?? false;
        }

        /// <summary>
        /// Allows additional customization of the receipt at specific sections.
        /// </summary>
        /// <param name="section">The receipt section to get text from</param>
        /// <returns>The text at the specified section</returns>
        public string GetTextAtSection(ReceiptSection section)
        {
            try
            {
                return _receipt?.GetTextAtSection(section) ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Inserts custom text at a specific receipt section.
        /// </summary>
        /// <param name="text">The text to insert</param>
        /// <param name="section">The receipt section to insert text at</param>
        /// <returns>True if the text was successfully inserted</returns>
        public bool InsertTextAtSection(string text, ReceiptSection section)
        {
            try
            {
                return _receipt?.InsertTextAtSection(text, section) ?? false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the original receipt image from the payment application.
        /// </summary>
        /// <returns>The receipt image or null if not available</returns>
        public Image GetOriginalImage()
        {
            try
            {
                return _receipt?.OriginalImage;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region "Receipt Information Methods"

        /// <summary>
        /// Gets a summary of the receipt configuration and content.
        /// </summary>
        /// <returns>A summary string of the receipt</returns>
        public string GetReceiptSummary()
        {
            var summary = $"Receipt Type: {ReceiptType}\n" +
                         $"Has Custom Header: {HasCustomHeader()}\n" +
                         $"Has Custom Footer: {HasCustomFooter()}\n" +
                         $"Includes Logo: {IsLogoIncluded}\n" +
                         $"Includes Cashier Name: {IsCashierNameIncluded}\n" +
                         $"Includes QR Code: {IsQrCodeIncluded}\n" +
                         $"Includes Online URL: {IsOnlineUrlIncluded}\n" +
                         $"Content Length (Plain): {AsPlainText.Length} chars\n" +
                         $"Content Length (HTML): {AsHtml.Length} chars";

            return summary;
        }

        /// <summary>
        /// Determines if the receipt is valid and contains content.
        /// </summary>
        /// <returns>True if the receipt contains valid content</returns>
        public bool IsValid()
        {
            return _receipt != null && 
                   (!string.IsNullOrWhiteSpace(AsPlainText) || !string.IsNullOrWhiteSpace(AsHtml));
        }

        /// <summary>
        /// Gets the receipt in the preferred format (HTML if available, otherwise plain text).
        /// </summary>
        /// <returns>The formatted receipt content</returns>
        public string GetPreferredContent()
        {
            if (!string.IsNullOrWhiteSpace(AsHtml))
                return AsHtml;
            
            return AsPlainText;
        }

        /// <summary>
        /// Exports the receipt to a formatted string for display or storage.
        /// </summary>
        /// <param name="includeMetadata">Whether to include receipt metadata</param>
        /// <returns>Formatted receipt string</returns>
        public string ExportForDisplay(bool includeMetadata = true)
        {
            var content = GetPreferredContent();
            
            if (!includeMetadata)
                return content;

            var export = "=== RECEIPT ===\n";
            export += $"Type: {ReceiptType}\n";
            export += $"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n";
            
            if (IsCashierNameIncluded && !string.IsNullOrWhiteSpace(CashierName))
                export += $"Cashier: {CashierName}\n";
            
            export += "--- CONTENT ---\n";
            export += content;
            export += "\n--- END RECEIPT ---";

            return export;
        }

        #endregion

        /// <summary>
        /// Returns a string representation of the receipt.
        /// </summary>
        /// <returns>Receipt summary string</returns>
        public override string ToString()
        {
            return $"Receipt [{ReceiptType}] - {AsPlainText.Length} chars";
        }
    }
}