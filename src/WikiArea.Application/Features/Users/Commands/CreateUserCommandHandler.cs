using AutoMapper;
using MediatR;
using WikiArea.Application.DTOs;
using WikiArea.Core.Entities;
using WikiArea.Core.Enums;
using WikiArea.Core.Interfaces;

namespace WikiArea.Application.Features.Users.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userRole = UserRole.FromName(request.Role);
        
        var user = new User(
            request.Username,
            request.Email,
            request.DisplayName,
            request.AdfsId,
            userRole,
            request.Department)
        {
            CreatedBy = _currentUserService.UserId,
            UpdatedBy = _currentUserService.UserId
        };

        await _userRepository.AddAsync(user, cancellationToken);
        
        return _mapper.Map<UserDto>(user);
    }
} 