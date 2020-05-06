using System.Web.Mvc;
using MySite.Logging;
using MySite.Models;
using MySite.Attributes;
using MySite.Helpers;
using MySite.Helpers.Payment;
using Newtonsoft.Json.Linq;

namespace MySite.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly ILog logger = LogProvider.For<PaymentController>();

        [NoCache]
        public ActionResult Index(PaymentViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePaymentRequest()
        {
            //var redirectUrl = SendMomoPaymentRequest();
            var redirectUrl = string.Empty;
            return Json(new { message = "", success = true, returnUrl = redirectUrl }, JsonRequestBehavior.AllowGet);
        }

        private string SendMomoPaymentRequest()
        {
            var redirectUrl = string.Empty;

            var tranId = Utility.GenTranIdWithPrefix("DT");
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOVO8720190607";
            string accessKey = "Guobcx2Dr5pvH3FP";
            string serectkey = "5HxEVTqdEsoCXuBp9Z6mZBhgiltetjUT";
            string orderInfo = "Vip bạc (200k)";
            string returnUrl = "http://localhost:58743/Payment/PaymentResult?tranId=" + tranId;
            string notifyurl = "http://localhost:58743/Payment/PaymentResult?tranId=" + tranId;

            string amount = "250000";
            string orderid = tranId;
            string requestId = tranId;
            string extraData = "merchantName=;merchantId=";//pass empty value if your merchant does not have stores else merchantName=[storeName]; merchantId=[storeId] to identify a transaction map with a physical store

            //before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            logger.Debug("RawHash = " + rawHash);

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            logger.Debug("Signature = " + signature);
            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "requestType", "captureMoMoWallet" },
                { "extraData", extraData },
                { "signature", signature }

            };

            logger.Debug("Json request to MoMo: " + message.ToString());

            string responseFromMomo = MomoPaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            var responseString = responseFromMomo;
            JObject jmessage = JObject.Parse(responseFromMomo);

            logger.Debug("Return from MoMo: " + jmessage.ToString());

            redirectUrl = jmessage.GetValue("payUrl").ToString();

            return redirectUrl;
        }

        public ActionResult PaymentResult(PaymentViewModel model)
        {
            return View(model);
        }

        #region Helper



        #endregion

    }
}
