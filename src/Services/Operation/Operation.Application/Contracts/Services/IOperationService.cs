namespace Operation.Application.Contracts.Services;

public interface IOperationService
{
    Task SendInfoAddedOperationAsync(CancellationToken cancellationToken);
}
