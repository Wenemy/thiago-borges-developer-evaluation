using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects
{
    public class Branch : ValueObject
    {
        public Guid BranchId { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }

        public Branch(Guid branchId, string name, string address)
        {
            BranchId = branchId;
            Name = name;
            Address = address;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BranchId;
        }
    }
}
