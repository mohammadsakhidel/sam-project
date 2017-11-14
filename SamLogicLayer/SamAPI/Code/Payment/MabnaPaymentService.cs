using RamancoLibrary.Utilities;
using SamUtils.Objects.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SamAPI.Code.Payment
{
    public class MabnaPaymentService : IPaymentService
    {
        #region Ctors:
        #endregion

        #region Props:
        public string UniqueID { get; set; }
        public string Signature { get; private set; }
        #endregion

        #region CONSTS:
        public const string MID = "693402866100106";
        public const string TID = "69001764";
        public const string PUBLIC_KEY = "<RSAKeyValue><Modulus>hE7cwvaWfnZ0NM9KVy+qTa5J+LxnWy03934R+TLFWLBSD7rmlEgvRJ202vBvadQCCjbI0GgW835L4HbuU8/KhF2X2EFdm50grZf/OzO159K2gJhk640pYk3bMTMiRzm8wD5Gpox3pgMR3fVZMF32TjwIdNLnmoPm7kwJG8y9TJU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public const string PRIVATE_KEY = "<RSAKeyValue><Modulus>mb8uGciCkFUseOfH/X3Kch0qJAAWhVSPV/C1uUc4A2tdaPMYMIKpv4QekWsVX0JjieBoYtIvHP8Ro1kypQO0nmJVqci9ILjZQpWPYkw3JhxaGO0ji8yHwxaQ1HzaEGepCxIIipZ90gEAzYQOt7ZvKvou9F+L8bUcLEnPsz0GyeU=</Modulus><Exponent>AQAB</Exponent><P>yYz9J8DHL5YX+foMVFzOs06yXg1Wjlr8KpfA+WT/9LSwuZaGUuH8UGUfhW4n4tLAqDZObghyN9h+ei3aHbfw6Q==</P><Q>w0ggiMTMxbtUB2GONtDJ5sF53+sTswzcFftJNEIk/oJKUpaFqtSBqRTKuY7Gosx8hmNccgzeI6eFPD8UoWjTnQ==</Q><DP>kFxw0vFsefQatkzYWfCjiKDcdys8jPg0V9mcOcWS41YsorFjAqikzXywyCRvYzKrFZDYOk3IaaGibaa77L9cSQ==</DP><DQ>SIeAdevKNmKGKhukq11Or+MKNg1kiqrrD0r+fGdYwCJ6IkR/rtwwsDDlvpd11T9nvK4oxr9avhlZFfBD5FM/zQ==</DQ><InverseQ>s4HjmsSZiWTwxhUUn1c6+1zuK2ECAn7znrbSwDoduLAAyao2YIeMuFSwQT3yslOw3tJZ7V6OO56eILMAtJYOsg==</InverseQ><D>bZbCS4qK1kp/6qZ7/Qo+1VxIf2S4qMz/Z9WiErDbEkTjTj2Izns0d9i62TiVAVOXE94rSdU709VUQzT/TnP3t5rPiAxy/BkbqC33QrksD/wAqX6qpbw/QerKffw59W8JJramH/TSj+jVkwXWiawLahqlDMCYVKZFEv8Ldbevz8E=</D></RSAKeyValue>";
        #endregion

        #region Implementation:
        public string BankPageUrl
        {
            get { return "https://mabna.shaparak.ir/"; }
        }
        public string CallbackUrl
        {
            get { return "https://www.samsys.ir/payment/mabnacallback"; }
        }
        public string ProviderName
        {
            get { return "mabnacard"; }
        }
        public string GetToken(int amount)
        {
            #region Validation:
            if (string.IsNullOrEmpty(CallbackUrl))
                throw new Exception("Callback Url is not specified.");
            #endregion

            #region Prerequisits:
            RSACryptoServiceProvider publicCryptoProvider = new RSACryptoServiceProvider();
            publicCryptoProvider.FromXmlString(PUBLIC_KEY);

            RSACryptoServiceProvider privateCryptoProvider = new RSACryptoServiceProvider();
            privateCryptoProvider.FromXmlString(PRIVATE_KEY);
            #endregion

            #region Amount:
            var amountBytes = Encoding.UTF8.GetBytes(amount.ToString());
            var amountEncrypted = publicCryptoProvider.Encrypt(amountBytes, false);
            var amountBase64 = Convert.ToBase64String(amountEncrypted);
            #endregion

            #region MID:
            var midBytes = Encoding.UTF8.GetBytes(MID);
            var midEncrypted = publicCryptoProvider.Encrypt(midBytes, false);
            var midBase64 = Convert.ToBase64String(midEncrypted);
            #endregion

            #region CallbackUrl:
            var callBackUrlBytes = Encoding.UTF8.GetBytes(CallbackUrl);
            var callBackUrlEncrypted = publicCryptoProvider.Encrypt(callBackUrlBytes, false);
            var callBackUrlBase64 = Convert.ToBase64String(callBackUrlEncrypted);
            #endregion

            #region TID:
            var tidBytes = Encoding.UTF8.GetBytes(TID);
            var tidEncrypted = publicCryptoProvider.Encrypt(tidBytes, false);
            var tidBase64 = Convert.ToBase64String(tidEncrypted);
            #endregion

            #region CRN:
            UniqueID = DateTime.Now.ToString("fffssMMHHyyyyddmm");
            var crnBytes = Encoding.UTF8.GetBytes(UniqueID);
            var crnEncrypted = publicCryptoProvider.Encrypt(crnBytes, false);
            var crnBase64 = Convert.ToBase64String(crnEncrypted);
            #endregion

            #region Signature:
            Signature = amount + UniqueID + MID + CallbackUrl + TID;
            var signatureBytes = privateCryptoProvider.SignData(Encoding.UTF8.GetBytes(Signature), new SHA1CryptoServiceProvider());
            var signatureBase64 = Convert.ToBase64String(signatureBytes);
            #endregion

            #region Call Service:
            var dto = new Mabnacard.TokenSpace.tokenDTO()
            {
                AMOUNT = amountBase64,
                CRN = crnBase64,
                MID = midBase64,
                REFERALADRESS = callBackUrlBase64,
                TID = tidBase64,
                SIGNATURE = signatureBase64
            };

            var client = new Mabnacard.TokenSpace.TokenServiceClient();
            var response = client.reservation(dto);
            if (response.result != 0)
                throw new PaymentException($"Token inquery failed with error code: {response.result}");
            #endregion

            return response.token;
        }
        public bool Verify(string uniqueId, string referenceCode)
        {
            #region Prerequisits:
            RSACryptoServiceProvider publicCryptoProvider = new RSACryptoServiceProvider();
            publicCryptoProvider.FromXmlString(PUBLIC_KEY);

            RSACryptoServiceProvider privateCryptoProvider = new RSACryptoServiceProvider();
            privateCryptoProvider.FromXmlString(PRIVATE_KEY);
            #endregion

            #region MID:
            var midBytes = Encoding.UTF8.GetBytes(MID);
            var midEncrypted = publicCryptoProvider.Encrypt(midBytes, false);
            var midBase64 = Convert.ToBase64String(midEncrypted);
            #endregion

            #region CRN(UNIQUEID):
            var crnBytes = Encoding.UTF8.GetBytes(uniqueId);
            var crnEncrypted = publicCryptoProvider.Encrypt(crnBytes, false);
            var crnBase64 = Convert.ToBase64String(crnEncrypted);
            #endregion

            #region REF(TRN):
            var refBytes = Encoding.UTF8.GetBytes(referenceCode);
            var refEncrypted = publicCryptoProvider.Encrypt(refBytes, false);
            var refBase64 = Convert.ToBase64String(refEncrypted);
            #endregion

            #region Signature:
            Signature = MID + referenceCode + uniqueId;
            var signatureBytes = privateCryptoProvider.SignData(Encoding.UTF8.GetBytes(Signature), new SHA1CryptoServiceProvider());
            var signatureBase64 = Convert.ToBase64String(signatureBytes);
            #endregion

            #region Call Service:
            var dto = new Mabnacard.TransactionSpace.confirmationDTO()
            {
                CRN = crnBase64,
                MID = midBase64,
                TRN = refBase64,
                SIGNATURE = signatureBase64
            };

            var client = new Mabnacard.TransactionSpace.TransactionReferenceClient();
            var response = client.sendConfirmation(dto);

            if (string.IsNullOrEmpty(response.RESCODE) || (Convert.ToInt32(response.RESCODE) != 0 && Convert.ToInt32(response.RESCODE) != 101))
                throw new Exception($"Payment confirmation failed with error code: {response.RESCODE}");
            #endregion

            return true;
        }
        public bool Reverse(string referenceCode)
        {
            #region Prerequisits:
            RSACryptoServiceProvider privateCryptoProvider = new RSACryptoServiceProvider();
            privateCryptoProvider.FromXmlString(PRIVATE_KEY);
            #endregion

            #region Signature:
            Signature = MID + referenceCode;
            var signatureBytes = privateCryptoProvider.SignData(Encoding.UTF8.GetBytes(Signature), new SHA1CryptoServiceProvider());
            var signatureBase64 = Convert.ToBase64String(signatureBytes);
            #endregion

            #region Call Service:
            var dto = new Mabnacard.ReverseSpace.reversalDto()
            {
                MID = MID,
                RRN = referenceCode,
                SIGNATURE = signatureBase64
            };

            var client = new Mabnacard.ReverseSpace.ReverseTransactionClient();
            var response = client.sendReversal(dto);

            if (string.IsNullOrEmpty(response) || (response != "0" && response != "00"))
                throw new Exception($"Payment reverse failed with error: {response}");
            #endregion

            return true;
        }
        #endregion
    }
}