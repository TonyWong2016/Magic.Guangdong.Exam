using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
namespace Magic.Guangdong.Assistant
{
    public class ProgressManager : IDisposable
    {
        private readonly Channel<ProgressMessage> _progressChannel;
        private readonly ReaderWriterLockSlim _lock = new();
        public ProgressManager()
        {
            _progressChannel = Channel.CreateUnbounded<ProgressMessage>();
        }

        public async Task PublishProgressAsync(string taskId, string message, CancellationToken cancellationToken = default)
        {
            var progressMessage = new ProgressMessage { TaskId = taskId, Message = message };
            await _progressChannel.Writer.WriteAsync(progressMessage, cancellationToken);
        }

        public ChannelReader<ProgressMessage> GetProgressReader() => _progressChannel.Reader;

        public void Dispose()
        {
            _progressChannel.Writer.TryComplete();
            _lock.Dispose();
        }
    }

    public class ProgressMessage
    {
        public string TaskId { get; set; } // 任务ID，用于区分不同任务
        public string Message { get; set; } // 进度消息
    }
}
