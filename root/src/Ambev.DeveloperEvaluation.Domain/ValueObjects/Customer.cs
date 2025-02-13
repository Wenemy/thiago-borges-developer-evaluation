using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects
{
    public class Customer : ValueObject
    {
        public Guid CustomerId { get; private set; }
        public string Name { get; private set; }

        public Customer(Guid customerId, string name)
        {
            CustomerId = customerId;
            Name = name;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CustomerId;
        }
    }
}
