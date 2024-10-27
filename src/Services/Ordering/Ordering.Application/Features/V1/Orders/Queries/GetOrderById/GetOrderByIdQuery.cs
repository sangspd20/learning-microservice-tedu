using MediatR;
using Ordering.Application.Common.Models;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders;

public class GetOrderByIdQuery : IRequest<ApiResult<OrderDto>>
{
    public GetOrderByIdQuery(long id)
    {
        Id = id;
    }

    public long Id { get; }
}