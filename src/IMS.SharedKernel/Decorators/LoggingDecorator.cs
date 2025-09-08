using IMS.SharedKernel.CQRS;
using IMS.SharedKernel.ResultPattern;
using Microsoft.Extensions.Logging;

namespace IMS.SharedKernel.Decorators;

public static class LoggingDecorator
{
    public sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        private static readonly string CommandName = typeof(TCommand).Name; // Cached to reduce reflection overhead

        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing command {Command}", CommandName);

            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {Command}", CommandName);
            }
            else
            {
                using (logger.BeginScope(new Dictionary<string, object> { ["Error"] = result.Error! }))
                {
                    logger.LogError("Completed command {Command} with error", CommandName);
                }
            }

            return result;
        }
    }

    public sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private static readonly string CommandName = typeof(TCommand).Name;
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing command {Command}", CommandName);

            Result result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed command {Command}", CommandName);
            }
            else
            {
                using (logger.BeginScope(new Dictionary<string, object> { ["Error"] = result.Error! }))
                {
                    logger.LogError("Completed command {Command} with error", CommandName);
                }
            }

            return result;
        }
    }

    public sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        private static readonly string QueryName = typeof(TQuery).Name;
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            logger.LogInformation("Processing query {Query}", QueryName);

            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed query {Query}", QueryName);
            }
            else
            {
                using (logger.BeginScope(new Dictionary<string, object> { ["Error"] = result.Error! }))
                {
                    logger.LogError("Completed query {Query} with error", QueryName);
                }
            }

            return result;
        }
    }
}
