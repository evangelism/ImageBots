using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BingServices
{

    public class ImageDesc
    {
        public string Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Name { get; set; }
        public string EncodingFormat { get; set; }

    }

    public class ImageSearch
    {
        protected string BingUrl = "https://bingapis.azure-api.net/api/v5/images/search";

        protected string ApiKey;

        public ImageSearch(string ApiKey)
        {
            this.ApiKey = ApiKey;
        }

        public async Task<List<ImageDesc>> Search(string q, int no)
        {
            var r = WebRequest.Create(BingUrl+$"?count={no}&q={Uri.EscapeDataString(q)}");
            r.Headers["Ocp-Apim-Subscription-Key"] = ApiKey;
            var rs = await r.GetResponseAsync();
            var rss = rs.GetResponseStream();
            var tr = new StreamReader(rss);
            dynamic res = Newtonsoft.Json.JsonConvert.DeserializeObject(await tr.ReadToEndAsync());
            var L = new List<ImageDesc>();
            foreach (var x in res.value)
            {
                var O = new ImageDesc()
                {
                    Name = x.name,
                    Width = x.width,
                    Height = x.height,
                    Url = x.contentUrl,
                    ThumbnailUrl = x.thumbnailUrl,
                    EncodingFormat = x.encodingFormat
                };
                L.Add(O);
            }
            return L;
        }

    }
}
