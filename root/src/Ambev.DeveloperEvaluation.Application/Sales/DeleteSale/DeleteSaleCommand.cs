using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    /// <summary>
    /// Command for delete sale.
    /// </summary>
    /// <remarks>
    /// This command is used to capture the required data for delete sale,  
    /// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
    /// that returns a <see cref="DeleteSaleResult"/>.
    /// 
    /// The data provided in this command is validated using the 
    /// <see cref="DeleteSaleCommandValidator"/> which extends 
    /// <see cref="AbstractValidator{T}"/> to ensure that the fields are correctly 
    /// populated and follow the required rules.
    /// </remarks>
    public class DeleteSaleCommand : IRequest<DeleteSaleResult>
    {
        public Guid Id { get; set; }
    }
}
