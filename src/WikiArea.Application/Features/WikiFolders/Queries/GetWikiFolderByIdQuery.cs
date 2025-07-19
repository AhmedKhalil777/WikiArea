using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiFolders.Queries;

public record GetWikiFolderByIdQuery(string Id) : IRequest<WikiFolderDto?>; 