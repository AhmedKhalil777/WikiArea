using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiPages.Queries;

public record GetWikiPageByIdQuery(string Id) : IRequest<WikiPageDto?>; 