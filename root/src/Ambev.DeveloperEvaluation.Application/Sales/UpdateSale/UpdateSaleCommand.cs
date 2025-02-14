using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    /// <summary>
    /// Command for update a sale.
    /// </summary>
    /// <remarks>
    /// This command is used to capture the required data for updating a sale,  
    /// It implements <see cref="IRequest{TResponse}"/> to initiate the request 
    /// that returns a <see cref="UpdateSaleResult"/>.
    /// 
    /// The data provided in this command is validated using the 
    /// <see cref="UpdateSaleCommandValidator"/> which extends 
    /// <see cref="AbstractValidator{T}"/> to ensure that the fields are correctly 
    /// populated and follow the required rules.
    /// </remarks>
    public class UpdateSaleCommand : IRequest<UpdateSaleResult>
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public Guid CustomerId { get; set; }
        public Guid BranchId { get; set; }
        public ICollection<UpdateSaleItemCommand> Items { get; set; } = new List<UpdateSaleItemCommand>();
    }
}
