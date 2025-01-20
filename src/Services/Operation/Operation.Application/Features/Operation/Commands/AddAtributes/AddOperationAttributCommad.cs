using MediatR;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Operation.Application.Common.Exceptions;
using Operation.Application.Contracts.Repositories;
using Operation.Application.Contracts.Services;
using Operation.Shared.Constans;
using Operation.Shared.Wrapper;

namespace Operation.Application.Features.Operation.Commands.AddAtributes;

public record AddOperationAttributCommad(object Attributes) : IRequest<Result>;

public class AddOperationAttributCommaddHandler : IRequestHandler<AddOperationAttributCommad, Result>
{
    private readonly IOperationService _operationService;
    private readonly IOperationRepository _operationRepository;
    private readonly ICosmosService _cosmosService;
    public AddOperationAttributCommaddHandler(
        IOperationRepository operationRepository,
        IOperationService operationService,
        ICosmosService cosmosService)
    {
        _operationRepository = operationRepository;
        _operationService = operationService;
        _cosmosService = cosmosService;
    }

    public async Task<Result> Handle(AddOperationAttributCommad request, CancellationToken cancellationToken)
    {

        var item = JsonConvert.DeserializeObject<dynamic>(request.Attributes.ToString());

        var idOperation = (int?)((dynamic)item).id ?? throw new NotFoundException("not found operation");

        var codeOperation = (string?)((dynamic)item).code;

        if (string.IsNullOrEmpty(codeOperation))
            throw new NotFoundException("not found operation");

        var existOperation = await _operationRepository.Any(x => x.Id == idOperation && x.Code == codeOperation);
        if (!existOperation)
            throw new NotFoundException("not found operation");

        await _cosmosService
                .AddOrEdit(containerName: ApplicationConstants.CosmosDB.CONTAINER_OPERATION,
                           id: idOperation.ToString(),
                           partitionKey: new PartitionKey(codeOperation),
                           item: item,
                           cancellationToken: cancellationToken);

        await _operationService.SendInfoAddedOperationAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }

}
