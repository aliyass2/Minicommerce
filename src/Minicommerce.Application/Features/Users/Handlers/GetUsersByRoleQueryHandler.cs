using System;
using AutoMapper;
using MediatR;
using Minicommerce.Application.Features.Users.Dtos;
using Minicommerce.Application.Features.Users.Queries;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Features.Users.Handlers;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, IEnumerable<UserListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUsersByRoleQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserListDto>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetByRoleAsync(request.Role, cancellationToken);
        return _mapper.Map<IEnumerable<UserListDto>>(users);
    }
}