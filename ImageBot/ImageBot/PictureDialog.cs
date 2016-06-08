using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace ImageBot
{
    [Serializable]
    public class PictureDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            var Langs = new LanguageAgent[]
            {
                new LanguageAgent("en","English"),
                new LanguageAgent("ru","Russian"),
                new LanguageAgent("es","Spanish")
            };

            foreach (var L in Langs) await L.LoadPhrase(message.Text);

            var repl = message.CreateReplyMessage($"Here is how the word {message.Text} looks like in different languages", "en");
            repl.Attachments = new List<Attachment>();
            foreach(var L in Langs)
            {
                repl.Attachments.Add(L.GetAttachment());
            }
            await context.PostAsync(repl);
            context.Wait(MessageReceivedAsync);
        }
    }
}