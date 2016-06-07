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
            await context.PostAsync(message.CreateReplyMessage($"I will now look for different pictures of {message.Text}", "en"));
            var L0 = new LanguageAgent("en", "English");
            await L0.LoadPhrase(message.Text);
            await L0.SendMessage(context, message);
            var L1 = new LanguageAgent("ru", "Russian");
            await L1.LoadPhrase(message.Text);
            await L1.SendMessage(context, message);
            var L2 = new LanguageAgent("es", "Spanish");
            await L2.LoadPhrase(message.Text);
            await L2.SendMessage(context, message);
            context.Wait(MessageReceivedAsync);
        }
    }
}