using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects
{
    public class Product : ValueObject
    {
        public Guid ProductId { get; private set; }
        public string Title { get; private set; }

        public Product(Guid productId, string title)
        {
            ProductId = productId;
            Title = title;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProductId;
        }
    }
}
