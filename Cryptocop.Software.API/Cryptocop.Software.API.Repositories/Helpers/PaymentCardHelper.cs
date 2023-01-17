using System;

namespace Cryptocop.Software.API.Repositories.Helpers
{
    public class PaymentCardHelper
    {
        public static string MaskPaymentCard(string paymentCardNumber)
        {
            //get last four
            var lastFourDigits = paymentCardNumber.Substring(Math.Max(0, paymentCardNumber.Length - 4));
            //create masked string
            var masked = "************" + lastFourDigits;
            return masked;
        }
    }
}