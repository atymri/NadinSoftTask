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
                throw new ArgumentException("email address can not be null or whitespace");
            email = email.Trim();

            MailAddress mailAddress;
            try
            {
                mailAddress = new MailAddress(email);
            }
            catch (FormatException)
            {
                throw new ArgumentException($"is not in proper format: {email}");
            }

            if (mailAddress.Address != email)
                throw new ArgumentException($"is not a valid email: {email}");

            string domain = mailAddress.Host.ToLowerInvariant();
            var allowedDomains = new List<string>
            {
                "gmail.com",
                "yahoo.com",
                "outlook.com",
                "hotmail.com"
            };

            if (!allowedDomains.Contains(domain))
                throw new ArgumentException($"this domain is not supported by us: {domain}");
        }
    }
}
