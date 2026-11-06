using CSharpFunctionalExtensions;

namespace Domain.ValueObjects;

// The full card number and CVV are not stored (PCI DSS): only the mask, holder and expiry date are kept
public class PaymentInfo : ValueObject
{
    public string MaskedCardNumber { get; protected set; }
    public string ExpirationDate { get; protected set; }
    public string CardHolderName { get; protected set; }

    protected PaymentInfo() { }

    protected PaymentInfo(string maskedCardNumber, string expirationDate, string cardHolderName)
    {
        MaskedCardNumber = maskedCardNumber;
        ExpirationDate = expirationDate;
        CardHolderName = cardHolderName;
    }

    public static PaymentInfo Create(string cardNumber, string expirationDate, string cardHolderName)
    {
        var digits = new string(cardNumber.Where(char.IsDigit).ToArray());
        var lastDigits = digits.Length >= 4 ? digits[^4..] : digits;

        return new PaymentInfo($"**** **** **** {lastDigits}", expirationDate.Trim(), cardHolderName.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return $"{MaskedCardNumber} {ExpirationDate} {CardHolderName}";
    }
}
