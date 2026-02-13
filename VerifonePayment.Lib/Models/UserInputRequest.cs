using System;
using System.Collections.Generic;
using VerifoneSdk;

namespace VerifonePayment.Lib.Models
{
    /// <summary>
    /// Represents a user input request from the payment device.
    /// </summary>
    public class UserInputRequest
    {
        /// <summary>
        /// Gets the request parameters from the device.
        /// </summary>
        public RequestParameters RequestParameters { get; internal set; }

        /// <summary>
        /// Gets the user input event response object.
        /// </summary>
        public UserInputEventResponse Response { get; internal set; }

        /// <summary>
        /// Gets the input type required by the device.
        /// </summary>
        public string InputType
        {
            get
            {
                try
                {
                    return RequestParameters?.InputType?.ToString() ?? "Unknown";
                }
                catch
                {
                    return "Unknown";
                }
            }
        }

        /// <summary>
        /// Gets the message to display to the user.
        /// </summary>
        public string Message
        {
            get
            {
                try
                {
                    // Try to get message through reflection since SDK structure may vary
                    var messageProperty = RequestParameters?.GetType().GetProperty("Message");
                    return messageProperty?.GetValue(RequestParameters)?.ToString() ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the prompt text for user input.
        /// </summary>
        public string Prompt
        {
            get
            {
                try
                {
                    // Try to get prompt through reflection since SDK structure may vary
                    var promptProperty = RequestParameters?.GetType().GetProperty("Prompt");
                    return promptProperty?.GetValue(RequestParameters)?.ToString() ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets whether this request requires user input.
        /// </summary>
        public bool RequiresInput
        {
            get
            {
                try
                {
                    // Check if input type indicates an input is required
                    var inputType = RequestParameters?.InputType?.ToString();
                    return !string.IsNullOrEmpty(inputType) && 
                           inputType != "DISPLAY_ONLY" && 
                           inputType != "ACKNOWLEDGE";
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets available options for selection inputs.
        /// </summary>
        public List<string> Options
        {
            get
            {
                try
                {
                    var options = new List<string>();
                    // Try to extract options from request parameters
                    // This would depend on the actual SDK structure
                    return options;
                }
                catch
                {
                    return new List<string>();
                }
            }
        }

        /// <summary>
        /// Gets the minimum input length (for text inputs).
        /// </summary>
        public int? MinLength
        {
            get
            {
                try
                {
                    // Try to extract min length if available through reflection
                    var minLengthProperty = RequestParameters?.GetType().GetProperty("MinLength");
                    var value = minLengthProperty?.GetValue(RequestParameters);
                    return value as int?;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the maximum input length (for text inputs).
        /// </summary>
        public int? MaxLength
        {
            get
            {
                try
                {
                    // Try to extract max length if available through reflection
                    var maxLengthProperty = RequestParameters?.GetType().GetProperty("MaxLength");
                    var value = maxLengthProperty?.GetValue(RequestParameters);
                    return value as int?;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets whether the input is masked (for passwords/PINs).
        /// </summary>
        public bool IsMasked
        {
            get
            {
                try
                {
                    var inputType = InputType.ToUpperInvariant();
                    return inputType.Contains("PIN") || 
                           inputType.Contains("PASSWORD") || 
                           inputType.Contains("SECURE");
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the UserInputRequest class.
        /// </summary>
        /// <param name="requestParameters">The request parameters from the device</param>
        /// <param name="response">The response object to populate</param>
        internal UserInputRequest(RequestParameters requestParameters, UserInputEventResponse response)
        {
            RequestParameters = requestParameters;
            Response = response;
        }

        /// <summary>
        /// Sets the user's text response.
        /// </summary>
        /// <param name="textValue">The text value entered by the user</param>
        public void SetTextResponse(string textValue)
        {
            try
            {
                if (Response?.ResponseValues != null)
                {
                    // Try to set text value using reflection
                    var textProperty = Response.ResponseValues.GetType().GetProperty("TextValue");
                    if (textProperty != null && textProperty.CanWrite)
                    {
                        textProperty.SetValue(Response.ResponseValues, textValue ?? string.Empty);
                    }
                    else
                    {
                        // Try alternative property names
                        var textProp = Response.ResponseValues.GetType().GetProperty("Text") ??
                                      Response.ResponseValues.GetType().GetProperty("Value") ??
                                      Response.ResponseValues.GetType().GetProperty("StringValue");

                        if (textProp?.CanWrite == true)
                        {
                            textProp.SetValue(Response.ResponseValues, textValue ?? string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting text response: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the user's numeric response.
        /// </summary>
        /// <param name="numericValue">The numeric value entered by the user</param>
        public void SetNumericResponse(decimal numericValue)
        {
            try
            {
                if (Response?.ResponseValues != null)
                {
                    // Try to set numeric value using reflection
                    var numericProperty = Response.ResponseValues.GetType().GetProperty("NumericValue");
                    if (numericProperty != null && numericProperty.CanWrite)
                    {
                        // Convert to VerifoneSdk.Decimal if needed
                        var verifonDecimal = VerifoneSdk.Decimal.FromDecimal(ref numericValue);
                        numericProperty.SetValue(Response.ResponseValues, verifonDecimal);
                    }
                    else
                    {
                        // Try alternative property names
                        var numProp = Response.ResponseValues.GetType().GetProperty("Numeric") ??
                                     Response.ResponseValues.GetType().GetProperty("Amount") ??
                                     Response.ResponseValues.GetType().GetProperty("DecimalValue");

                        if (numProp?.CanWrite == true)
                        {
                            if (numProp.PropertyType == typeof(VerifoneSdk.Decimal))
                            {
                                var verifonDecimal = VerifoneSdk.Decimal.FromDecimal(ref numericValue);
                                numProp.SetValue(Response.ResponseValues, verifonDecimal);
                            }
                            else
                            {
                                numProp.SetValue(Response.ResponseValues, numericValue);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting numeric response: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the user's selection response.
        /// </summary>
        /// <param name="selectedIndex">The index of the selected option</param>
        public void SetSelectionResponse(int selectedIndex)
        {
            try
            {
                if (Response?.ResponseValues != null)
                {
                    // Try to set selection using reflection
                    var selectionProperty = Response.ResponseValues.GetType().GetProperty("SelectedIndex");
                    if (selectionProperty != null && selectionProperty.CanWrite)
                    {
                        selectionProperty.SetValue(Response.ResponseValues, selectedIndex);
                    }
                    else
                    {
                        // Try alternative property names
                        var selProp = Response.ResponseValues.GetType().GetProperty("Selection") ??
                                     Response.ResponseValues.GetType().GetProperty("Index") ??
                                     Response.ResponseValues.GetType().GetProperty("Choice");

                        if (selProp?.CanWrite == true)
                        {
                            selProp.SetValue(Response.ResponseValues, selectedIndex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting selection response: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets a confirmation response (OK/Cancel).
        /// </summary>
        /// <param name="confirmed">True if user confirmed, false if cancelled</param>
        public void SetConfirmationResponse(bool confirmed)
        {
            try
            {
                if (Response?.ResponseValues != null)
                {
                    // Try to set confirmation using reflection
                    var confirmProperty = Response.ResponseValues.GetType().GetProperty("Confirmed");
                    if (confirmProperty != null && confirmProperty.CanWrite)
                    {
                        confirmProperty.SetValue(Response.ResponseValues, confirmed);
                    }
                    else
                    {
                        // Try alternative property names
                        var confProp = Response.ResponseValues.GetType().GetProperty("Confirmation") ??
                                      Response.ResponseValues.GetType().GetProperty("Accepted") ??
                                      Response.ResponseValues.GetType().GetProperty("Result");

                        if (confProp?.CanWrite == true)
                        {
                            confProp.SetValue(Response.ResponseValues, confirmed);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting confirmation response: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a summary of the user input request.
        /// </summary>
        /// <returns>Summary string</returns>
        public string GetRequestSummary()
        {
            var summary = $"Input Type: {InputType}\n";
            
            if (!string.IsNullOrEmpty(Message))
                summary += $"Message: {Message}\n";
            
            if (!string.IsNullOrEmpty(Prompt))
                summary += $"Prompt: {Prompt}\n";
            
            summary += $"Requires Input: {RequiresInput}\n";
            summary += $"Is Masked: {IsMasked}\n";
            
            if (MinLength.HasValue)
                summary += $"Min Length: {MinLength.Value}\n";
            
            if (MaxLength.HasValue)
                summary += $"Max Length: {MaxLength.Value}\n";
            
            if (Options.Count > 0)
                summary += $"Options: {string.Join(", ", Options)}\n";

            return summary;
        }

        /// <summary>
        /// Returns a string representation of the user input request.
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return $"UserInputRequest [{InputType}] - {(RequiresInput ? "Requires Input" : "Display Only")}";
        }
    }

    /// <summary>
    /// Event arguments for user input requests.
    /// </summary>
    public class UserInputRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the user input request.
        /// </summary>
        public UserInputRequest Request { get; }

        /// <summary>
        /// Gets or sets whether the request has been handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Initializes a new instance of the UserInputRequestEventArgs class.
        /// </summary>
        /// <param name="request">The user input request</param>
        public UserInputRequestEventArgs(UserInputRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }
    }
}