using System;
using System.Threading.Tasks;
using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using DotNetCore.CAP;
using DotNetCore.CAP.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Savorboard.CAP.InMemoryMessageQueue;

namespace AspectCore.Extensions.DependencyInjection.ConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            // sample for property injection
            var services = new ServiceCollection();
            services.AddTransient<ILogger, ConsoleLogger>();
            services.AddTransient<ISampleService, SampleService>();
            services.AddTransient<ITestEventHandler, TestEventHandler>();
            
            services.AddCap(o =>
            {
                o.UseInMemoryStorage();
                // 设置处理成功的数据在数据库中保存的时间（秒），为保证系统性能，数据会定期清理
                o.SucceedMessageExpiredAfter = 24 * 3600;
                // 设置失败重试次数
                o.FailedRetryCount = 5;
                o.Version = "bing_test";
                // 启用内存队列
                o.UseInMemoryMessageQueue();
            });
            services.ConfigureDynamicProxy();
            var serviceProvider = services.BuildServiceContextProvider();
//            var container = services.ToServiceContext();
//            container.AddType<ILogger, ConsoleLogger>();
//            container.AddType<ISampleService, SampleService>();
//            var serviceResolver = container.Build();
//            var sampleService = serviceResolver.Resolve<ISampleService>();
            var sampleService = serviceProvider.GetService<ISampleService>();
            sampleService.Invoke();
            Console.ReadKey();
        }
    }

    public interface ILogger
    {
        void Info(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine(message);
        }
    }

    public interface ISampleService
    {
        void Invoke();
    }

    public class SampleService : ISampleService
    {
        [FromServiceContext]
        public ILogger Logger { get; set; }

        [FromServiceContext]
        public ICapPublisher Publish { get; set; }
        
        public void Invoke()
        {
           Logger?.Info("sample service invoke.");
           Publish.Publish("test","aaaaaaaa");
        }
    }

    public interface ITestEventHandler
    {
        Task WritLogAsync(string message);
    }

    public class TestEventHandler : ITestEventHandler, DotNetCore.CAP.ICapSubscribe
    {
        [Test]
        [CapSubscribe("test")]
        public Task WritLogAsync(string message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }

    public class TestAttribute : AbstractInterceptorAttribute
    {
        public int[] Times { get; set; } = { 1, 100, 10000, 1000000 };
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            foreach (var times in Times)
            {
                for (var i = 0; i < times; i++)
                    await next(context);
            }
        }
    }
}