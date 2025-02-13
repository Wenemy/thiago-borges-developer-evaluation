using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
    /// <summary>
    /// Command for cancel sale.
    /// </summary>
    /// <remarks>
    /// This command is used to capture the required data for cancel sale,  
    /// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
    /// that returns a <see cref="CancelSaleResult"/>.
    /// 
    /// The data provided in this command is validated using the 
    /// <see cref="CancelSaleCommandValidator"/> which extends 
    /// <see cref="AbstractValidator{T}"/> to ensure that the fields are correctly 
    /// populated and follow the required rules.
    /// </remarks>
    public class CancelSaleCommand : IRequest<CancelSaleResult>
    {
        public Guid Id { get; set; }
    }
}
