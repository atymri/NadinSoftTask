using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ProductManager.Core.Helpers
{
    public class ValidationHelper
    {
        internal static void Validate(object obj)
        {
            ValidationContext validationContext = new ValidationContext(obj);
            List<ValidationResult> validationResult = new List<ValidationResult>();
            bool IsValid = Validator.TryValidateObject(
                instance: obj,
                validationContext: validationContext,
                validationResults: validationResult,
                validateAllProperties: true
            );

            if (!IsValid)
                throw new ArgumentException(validationResult.FirstOrDefault()?.ErrorMessage);

        }

        internal static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty or whitespace");
            email = email.Trim();

            MailAddress mailAddress;
            try
            {
                mailAddress = new MailAddress(email);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Invalid email format: {email}");
            }

            if (mailAddress.Address != email)
                throw new ArgumentException($"Invalid email address: {email}");

            string domain = mailAddress.Host.ToLowerInvariant();
            var allowedDomains = new List<string>
            {
                "gmail.com",
                "yahoo.com",
                "outlook.com",
                "hotmail.com"
            };

            if (!allowedDomains.Contains(domain))
                throw new ArgumentException($"Domain not allowed: {domain}");
        }
    }
}
