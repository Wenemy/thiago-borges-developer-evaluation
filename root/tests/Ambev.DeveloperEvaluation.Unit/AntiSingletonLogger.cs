using Serilog;
using Serilog.Sinks.InMemory;

namespace Ambev.DeveloperEvaluation.Unit
{
    //Devido ao logger ser um singleton global, tenho que limpar o sink em cada teste
    //https://github.com/serilog/serilog/wiki/Lifecycle-of-Loggers
    public class AntiSingletonLogger : IDisposable
    {
        public InMemorySink Sink { get; }

        public AntiSingletonLogger()
        {
            Sink = new InMemorySink();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink(Sink)
                .CreateLogger();
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}
