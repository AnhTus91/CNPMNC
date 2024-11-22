using DAPMver1.Models;

namespace DAPMver1.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context,VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
