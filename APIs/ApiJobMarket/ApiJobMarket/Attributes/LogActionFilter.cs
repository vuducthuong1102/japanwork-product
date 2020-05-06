//http://codereview.stackexchange.com/questions/123137/custom-web-api-asynchronous-filter-implementation

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

//namespace ApiJobMarket.Attributes
//{
//    public sealed class LogActionFilter : FilterAttribute, IActionFilter
//    {
//        private readonly ILogManager _logManager;

//        public LogActionFilter(ILogManager logManager)
//        {
//            _logManager = logManager;
//        }

//        private readonly Stopwatch _stopwatch = new Stopwatch();

//        public override bool AllowMultiple => false;

//        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext,
//                                                                        CancellationToken cancellationToken,
//                                                                        Func<Task<HttpResponseMessage>> continuation)
//        {
//            await InternalActionExecuting(actionContext);

//            if (actionContext.Response != null)
//            {
//                return actionContext.Response;
//            }

//            HttpActionExecutedContext executedContext;

//            try
//            {
//                var response = await continuation();

//                executedContext = new HttpActionExecutedContext(actionContext, null)
//                {
//                    Response = response
//                };
//            }
//            catch (Exception exception)
//            {
//                executedContext = new HttpActionExecutedContext(actionContext, exception);
//                _logManager.Error(exception, "LogAction recorded an error");
//            }

//            await InternalActionExecuted(executedContext);
//            _stopwatch.Reset();

//            return executedContext.Response;
//        }

//        private Task InternalActionExecuting(HttpActionContext actionContext)
//        {
//            _stopwatch.Start();
//            return Log("Executing", actionContext, 0);
//        }

//        private Task InternalActionExecuted(HttpActionExecutedContext actionExecutedContext)
//        {
//            _stopwatch.Stop();
//            return Log("Executed", actionExecutedContext.ActionContext, _stopwatch.ElapsedMilliseconds);
//        }

//        private Task Log(string action, HttpActionContext actionContext, long elapsedTime)
//        {
//            return Task.Run(() =>
//            {
//                var controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
//                var actionName = actionContext.ActionDescriptor.ActionName;
//                var parameters = string.Join(", ", actionContext.ActionArguments.Values.Select(x => x).ToArray());

//                var message = $"{action}: ctrl: {controllerName}, act: {actionName}, params: {parameters}" +
//                              $"{(elapsedTime > 0 ? "took (ms): " + elapsedTime : string.Empty)}";
//#if (DEBUG)
//                Trace.WriteLine(message, "Action filter log");
//#elif (RELEASE)
//                            _logManager.Debug(null, message, "Action filter log");
//#endif

//            });
//        }
//    }
//}