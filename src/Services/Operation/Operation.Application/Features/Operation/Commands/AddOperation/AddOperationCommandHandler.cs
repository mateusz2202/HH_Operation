using AutoMapper;
using MediatR;
using Operation.Application.Contracts.Repositories;
using Operation.Application.Contracts.Services;
using Operation.Shared.Constans;
using Operation.Shared.Wrapper;

namespace Operation.Application.Features.Operation.Commands.AddOperation;

public record AddOperationCommand(string Code, string Name) : IRequest<Result<int>>;

public class AddOperationCommandHandler : IRequestHandler<AddOperationCommand, Result<int>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IOperationService _operationService;
    public AddOperationCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IMapper mapper,
        IOperationService operationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _operationService = operationService;
    }

    public async Task<Result<int>> Handle(AddOperationCommand request, CancellationToken cancellationToken)
    {
        var operation = _mapper.Map<Domain.Entities.Operation>(request);
        await _unitOfWork.Repository<Domain.Entities.Operation>().AddAsync(operation);
        await _unitOfWork.CommitAndRemoveCache(cancellationToken,
                                     [  ApplicationConstants.Cache.OPERATION_KEY ,
                                        ApplicationConstants.Cache.OPERATIONATTRIBUTE_KEY,
                                        ApplicationConstants.Cache.OPERATIONWITHATTRIBUTE_KEY,
                                     ]);

        await _operationService.SendInfoAddedOperationAsync(cancellationToken);
        return await Result<int>.SuccessAsync(operation.Id);
    }

}
