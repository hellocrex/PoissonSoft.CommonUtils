using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace PoissonSoft.Telegram
{
    public class TelegramSendMessageHandler : IRequestHandler<TelegramSendMessageRequest, TelegramSendMessageResponse>
    {
        TelegramBotClient client;
        private readonly TelegramBotConfig config;


        public TelegramSendMessageHandler(TelegramBotConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.client = new TelegramBotClient(this.config.Token);
        }

        public async Task<TelegramSendMessageResponse> Handle(TelegramSendMessageRequest request, CancellationToken cancellationToken)
        {
            Message message = await client.SendTextMessageAsync(new ChatId(request.ChatId),
                    request.Message, ParseMode.Html);
            return new TelegramSendMessageResponse { Success = (message != null) };
        }
    }

    public class TelegramSendMessageResponse
    {
        public bool Success { get; set; }

        public override string ToString()
        {
            return Success ? "Успех." : "Что-то пошло не так.";
        }
    }

    public class TelegramSendMessageRequest : IRequest<TelegramSendMessageResponse>
    {
        public long ChatId { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return "Отправка сообщения в телеграм...";
        }
    }

    public class TelegramBotConfig
    {
        public string Token { get; set; }
    }
}
