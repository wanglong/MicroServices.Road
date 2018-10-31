﻿// TODO: LocalExecuteAsync unknown 

using System;
using System.Reflection;
using System.Threading.Tasks;
using Rpc.Common.Easy.Rpc.Communally.Entitys;
using Rpc.Common.Easy.Rpc.Communally.Entitys.Messages;
using Rpc.Common.Easy.Rpc.Transport;

namespace Rpc.Common.Easy.Rpc.Runtime.Server.Impl
{
    public class DefaultServiceExecutor : IServiceExecutor
    {
        private readonly IServiceEntryLocate _serviceEntryLocate;

        public DefaultServiceExecutor(IServiceEntryLocate serviceEntryLocate)
        {
            _serviceEntryLocate = serviceEntryLocate;
        }

        /// <summary>
        /// 执行消息转换后的服务
        /// </summary>
        /// <param name="sender">消息发送者</param>
        /// <param name="message">调用消息</param>
        public async Task ExecuteAsync(IMessageSender sender, TransportMessage message)
        {
            if (!message.IsInvokeMessage()) return;

            RemoteInvokeMessage remoteInvokeMessage;
            try
            {
                remoteInvokeMessage = message.GetContent<RemoteInvokeMessage>();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"将接收到的消息反序列化成 TransportMessage<RemoteInvokeMessage> 时发送了错误{exception}");
                return;
            }

            // 定位远程调用的消息服务实体
            var entry = _serviceEntryLocate.Locate(remoteInvokeMessage);
            if (entry == null)
            {
                Console.WriteLine($"根据服务Id：{remoteInvokeMessage.ServiceId}，找不到服务条目");
                return;
            }

            Console.WriteLine("准备执行本地逻辑");

            var resultMessage = new RemoteInvokeResultMessage();

            // 是否需要等待执行
            if (entry.Descriptor.WaitExecution())
            {
                //执行本地代码
                await LocalExecuteAsync(entry, remoteInvokeMessage, resultMessage);
                //向客户端发送调用结果
                await SendRemoteInvokeResult(sender, message.Id, resultMessage);
            }
            else
            {
                //通知客户端已接收到消息
                await SendRemoteInvokeResult(sender, message.Id, resultMessage);
                //确保新起一个线程执行，不堵塞当前线程
                await Task.Factory.StartNew(async () =>
                    {
                        //执行本地代码
                        await LocalExecuteAsync(entry, remoteInvokeMessage, resultMessage);
                    },
                    TaskCreationOptions.LongRunning);
            }
        }

        private async Task LocalExecuteAsync(ServiceEntity entry, RemoteInvokeMessage remoteInvokeMessage,
            RemoteInvokeResultMessage resultMessage)
        {
            try
            {
                var result = await entry.Func(remoteInvokeMessage.Parameters);
                var task = result as Task;

                if (task == null)
                {
                    resultMessage.Result = result;
                }
                else
                {
                    task.Wait();

                    var taskType = task.GetType().GetTypeInfo();
                    if (taskType.IsGenericType)
                        resultMessage.Result = taskType.GetProperty("Result").GetValue(task);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                resultMessage.ExceptionMessage = GetExceptionMessage(exception);
            }
        }

        private async Task SendRemoteInvokeResult(IMessageSender sender, string messageId,
            RemoteInvokeResultMessage resultMessage)
        {
            try
            {
                Console.WriteLine("准备发送响应消息");
                await sender.SendAndFlushAsync(TransportMessage.CreateInvokeResultMessage(messageId, resultMessage));
                Console.WriteLine("响应消息发送成功");
            }
            catch (Exception exception)
            {
                Console.WriteLine("发送响应消息时候发生了异常" + exception);
            }
        }

        private static string GetExceptionMessage(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var message = exception.Message;
            if (exception.InnerException != null)
            {
                message += "|InnerException:" + GetExceptionMessage(exception.InnerException);
            }

            return message;
        }
    }
}