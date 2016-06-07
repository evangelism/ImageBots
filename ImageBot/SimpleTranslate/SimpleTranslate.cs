using System;
using System.Text;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace BingServices
{
    public class SimpleTranslator
    {

        public string ClientID { get; set; }
        public string ClientSecret { get; set; }

        protected AdmAuthentication Auth;

        public SimpleTranslator(string id, string secret)
        {
            ClientID = id;
            ClientSecret = secret;
        }
        public async Task<string> Translate(string text, string from, string to)
        {
            if (Auth==null)
            {
                Auth = new AdmAuthentication(ClientID, ClientSecret);
                await Auth.Init();
            }
            var at = Auth.AccessToken.access_token;
            string uri = "http://api.microsofttranslator.com/v2/Http.svc/Translate?text=" + Uri.EscapeDataString(text) + "&from=" + from + "&to=" + to;
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp(uri);
            httpWebRequest.Headers["Authorization"] = "Bearer " + at;
            WebResponse response = null;
            response = await httpWebRequest.GetResponseAsync();
            string translation;
            using (Stream stream = response.GetResponseStream())
            {
                System.Runtime.Serialization.DataContractSerializer dcs = new System.Runtime.Serialization.DataContractSerializer(Type.GetType("System.String"));
                translation = (string)dcs.ReadObject(stream);
            }
            return translation;
        }

        [DataContract]
        public class AdmAccessToken
        {
            [DataMember]
            public string access_token { get; set; }
            [DataMember]
            public string token_type { get; set; }
            [DataMember]
            public string expires_in { get; set; }
            [DataMember]
            public string scope { get; set; }
        }
        public class AdmAuthentication
        {
            public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
            private string clientId;
            private string clientSecret;
            private string request;
            private AdmAccessToken token;
            private DateTime renewed = DateTime.Now;
            private const int RefreshTokenDuration = 9;
            public AdmAuthentication(string clientId, string clientSecret)
            {
                this.clientId = clientId;
                this.clientSecret = clientSecret;
            }

            public async Task Init()
            {
                request = string.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com", clientId, Uri.EscapeDataString(clientSecret));
                token = await HttpPost(DatamarketAccessUri, this.request);
            }

            public AdmAccessToken AccessToken
            {
                get
                {
                    if (renewed.AddMinutes(RefreshTokenDuration) < DateTime.Now) RenewAccessToken();
                    return this.token;
                }
            }
            private async void RenewAccessToken()
            {
                AdmAccessToken newAccessToken = await HttpPost(DatamarketAccessUri, this.request);
                this.token = newAccessToken;
            }

            private async Task<AdmAccessToken> HttpPost(string DatamarketAccessUri, string requestDetails)
            {
                //Prepare OAuth request 
                WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                byte[] bytes = Encoding.UTF8.GetBytes(requestDetails);
                // webRequest.ContentLength = bytes.Length;
                using (Stream outputStream = await webRequest.GetRequestStreamAsync())
                {
                    outputStream.Write(bytes, 0, bytes.Length);
                }
                using (WebResponse webResponse = await webRequest.GetResponseAsync())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(AdmAccessToken));
                    //Get deserialized object from JSON stream
                    AdmAccessToken token = (AdmAccessToken)serializer.ReadObject(webResponse.GetResponseStream());
                    return token;
                }
            }
        }
    }
}

