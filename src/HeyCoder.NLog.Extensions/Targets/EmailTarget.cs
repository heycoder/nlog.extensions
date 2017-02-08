using System;
using System.Collections.Concurrent;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using HeyCoder.NLog.Extensions.Utils;

namespace HeyCoder.NLog.Extensions.Targets
{
    [Target("Email")]
    public sealed class EmailTarget : TargetWithLayout
    {
        private static ConcurrentDictionary<string, MessageBag> _bagDictionary = new ConcurrentDictionary<string, MessageBag>();

        [RequiredParameter]
        public string AppName { get; set; }

        [RequiredParameter]
        public string Host { get; set; }

        [RequiredParameter]
        public string Port { get; set; }

        [RequiredParameter]
        public string UserName { get; set; }

        [RequiredParameter]
        public string Password { get; set; }

        [RequiredParameter]
        public string From { get; set; }

        [RequiredParameter]
        public string To { get; set; }

        public bool IsHtml { get; set; } = true;

        public string DisplayName { get; set; }

        public int MaxErrorCount { get; set; } = 1000;

        public int SendErrorCount { get; set; } = 1000;

        public int ExpiredTime { get; set; } = 5 * 60 * 1000;

        public string SplitString
        {
            get { return IsHtml ? "<hr/>" : "\r\n"; }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);
            var stackTraceInfo = StackTraceUtil.GetStackTrace();
            var key = string.Format("{0}-{1}-{2}", stackTraceInfo.ClassName,
                stackTraceInfo.MethodName,
                stackTraceInfo.FileLineNumber);
            var messageBag = GetMessageBag(key);
            messageBag.AppendMessage(logMessage);
            if (messageBag.NeedSend(MaxErrorCount, ExpiredTime))
            {
                lock (messageBag)
                {
                    if (messageBag.NeedSend(MaxErrorCount, ExpiredTime))
                    {
                        messageBag.SendMessage(key, SendErrorCount, SplitString, SendTheMessageToRemoteHost);
                    }
                }

            }
        }

        private bool SendTheMessageToRemoteHost(string logKey, string message)
        {
            try
            {
                MailUtil.SendEmail(new SendMail
                {
                    Host = this.Host,
                    UserName = this.UserName,
                    Password = this.Password,
                    FromMailAddress = this.From,
                    DisplayName = this.DisplayName ?? this.UserName,
                    Subject = string.Format("【{0}】{1}-{2}-告警", HostUtil.GetHostIp(), AppName, logKey),
                    Body = message,
                    IsBodyHtml = IsHtml,
                    ToMailAddressList = To.Split(',').ToList()
                });
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private MessageBag GetMessageBag(string key)
        {
            if (_bagDictionary.ContainsKey(key))
            {
                return _bagDictionary[key];
            }
            var bag = new MessageBag();
            return _bagDictionary.GetOrAdd(key, bag);
        }



    }


    internal class MessageBag
    {
        private DateTime StarTime { get; set; } = DateTime.Now;

        private ConcurrentBag<string> MessageList { get; set; } = new ConcurrentBag<string>();

        public bool NeedSend(int maxErrorCount, int expiredTime)
        {
            return MessageList.Count == 1
                || MessageList.Count >= maxErrorCount
                || (DateTime.Now - StarTime).TotalMilliseconds >= expiredTime;
        }

        public void AppendMessage(string message)
        {
            MessageList.Add(message);
        }

        public void SendMessage(string key, int sendErrorCount, string spltString, Func<string, string, bool> func)
        {
            var messageList = MessageList.Skip(MessageList.Count - sendErrorCount).Take(sendErrorCount);
            var message = messageList.Aggregate(string.Empty, (current, msg) => current + msg + spltString);
            if (func != null && func(key, message))
            {
                if (MessageList.Count > 1)
                {
                    MessageList = new ConcurrentBag<string>();
                    StarTime = DateTime.Now;
                }
            }

        }
    }
}
