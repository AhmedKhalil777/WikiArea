using MediatR;
using WikiArea.Application.DTOs;

namespace WikiArea.Application.Features.WikiPages.Queries;

public record GetWikiPageBySlugQuery(string Slug) : IRequest<WikiPageDto?>; 