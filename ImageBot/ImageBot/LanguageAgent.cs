using BingServices;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ImageBot
{
    [Serializable]
    public class LanguageAgent
    {
        public string Lang { get; private set; }
        public string Language { get; private set; }
        public string PhraseEn { get; private set; }
        public string Phrase { get; private set; }
        public ImageDesc Image { get; private set; }

        public LanguageAgent(string lang, string language)
        {
            Lang = lang;
            Language = language;
        }

        public async Task LoadPhrase(string s)
        {
            PhraseEn = s;
            if (Lang == "en") Phrase = PhraseEn;
            else
            {
                var tr = new SimpleTranslator(Config.BingTranslatorAppId, Config.BingTranslatorAppKey);
                Phrase = await tr.Translate(s, "en", Lang);
            }
            var ims = new ImageSearch(Config.BingImgSearchApiKey);
            var res = await ims.Search(Phrase, 50);
            Image = (from z in res
                     where z.Width < 400 && z.Width > 200
                     where z.EncodingFormat == "jpeg"
                     select z).FirstOrDefault();
        }

        public async Task SendMessage(IDialogContext context, Message message)
        {
            Message repl;
            repl = message.CreateReplyMessage($"{PhraseEn} in {Language} is {Phrase}", "en");
            await context.PostAsync(repl);
            repl = message.CreateReplyMessage(Phrase, Lang);
            if (Image != null)
            {
                repl.Attachments = new List<Attachment>();
                repl.Attachments.Add(new Attachment()
                { ContentType = $"image/{Image.EncodingFormat}", ContentUrl = Image.Url });
                /* repl.Attachments.Add(new Attachment()
                {
                    Title = LS.Phrase,
                    TitleLink = LS.Image.Url,
                    Text = LS.Phrase,
                    FallbackText = LS.Phrase,
                    ThumbnailUrl = LS.Image.ThumbnailUrl
                }); */
            }
            await context.PostAsync(repl);
        }
    }
}