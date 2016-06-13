using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Net.Http;

namespace ImageBot
{

    public class MicroResult
    {
        public string ImageUrl { get; set; }
        public string Locale { get; set; }
        public string ResultText { get; set; }
        public string Desc { get; set; }
    }

    [Serializable]
    public class PictureDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public bool UseMicroservice = false;

        protected string strip(string s)
        {
            if (s.Contains(":")) return s.Substring(0, s.IndexOf(':') - 1);
            if (s.Length > 128) return s.Substring(0, 128);
            return s;
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            var repl = message.CreateReplyMessage($"Here is how the word {message.Text} looks like in different languages", "en");
            repl.Attachments = new List<Attachment>();

            if (UseMicroservice)
            {
                var http = new HttpClient();
                // var rstring = await http.GetStringAsync("http://botsf.northeurope.cloudapp.azure.com:8983/api/values?intext=" + Uri.EscapeDataString(message.Text));
                var rstring = await http.GetStringAsync("http://botmachine.azure-api.net/api/values?intext="+ Uri.EscapeDataString(message.Text));
                var res = Newtonsoft.Json.JsonConvert.DeserializeObject<MicroResult[]>(rstring);
                foreach(var x in res)
                {
                    repl.Attachments.Add(new Attachment()
                    {
                        Text = $"{x.ResultText} -- {strip(x.Desc)}",
                        Title = x.ResultText,
                        TitleLink = x.ImageUrl,
                        ThumbnailUrl = x.ImageUrl,
                        FallbackText = x.ResultText
                    });
                }
            }
            else
            {

                var Langs = new LanguageAgent[]
                {
                new LanguageAgent("en","English"),
                new LanguageAgent("ru","Russian"),
                new LanguageAgent("cs","Czech"),
                new LanguageAgent("fr","French"),
                new LanguageAgent("he","Hebrew"),
                new LanguageAgent("zh-CHT","Chinese")
                };

                foreach (var L in Langs) await L.LoadPhraseMicroservice(message.Text);

                foreach (var L in Langs)
                {
                    repl.Attachments.Add(L.GetAttachment());
                }
            }
            await context.PostAsync(repl);
            context.Wait(MessageReceivedAsync);
        }
    }
}