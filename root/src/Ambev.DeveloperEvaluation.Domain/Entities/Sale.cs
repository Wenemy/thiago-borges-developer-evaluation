using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public string SaleNumber { get; private set; }
        public DateTime SaleDate { get; private set; }
        public Guid CustomerId { get; private set; }
        [NotMapped]
        public Customer Customer { get; private set; }
        public Guid BranchId { get; private set; }
        [NotMapped]
        public Branch Branch { get; private set; }
        public ICollection<SaleItem> Items { get; private set; }
        public bool IsCancelled { get; private set; }


        public Sale(string saleNumber, DateTime saleDate, Customer customer, Branch branch)
        {
            SaleNumber = saleNumber;
            SaleDate = saleDate;
            if (customer != null)
                CustomerId = customer.CustomerId;
            Customer = customer;
            if (branch != null)
                BranchId = branch.BranchId;
            Branch = branch;
            Items = new List<SaleItem>();
            IsCancelled = false;
        }

        private Sale() { }

        public void AddItem(Product product, int quantity, decimal unitPrice)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == product.ProductId);

            if (existingItem != null)
            {
                existingItem.UpdateQuantity(existingItem.Quantity + quantity);
            }
            else
            {
                var newItem = new SaleItem(Id, product, quantity, unitPrice);
                Items.Add(newItem);
            }
        }

        public void AddItems(IEnumerable<SaleItem> items)
        {
            foreach (var item in items)
            {
                AddItem(item.Product, item.Quantity, item.UnitPrice);
            }
        }

        public void RemoveItem(Guid productId)
        {
            var itemToRemove = Items.FirstOrDefault(i => i.ProductId == productId);
            if (itemToRemove != null)
            {
                Items.Remove(itemToRemove);
            }
        }

        public void ClearItems()
        {
            Items.Clear();
        }

        public decimal TotalAmount => Items.Sum(i => i.TotalAmount);

        public void Cancel()
        {
            IsCancelled = true;
        }

        public ValidationResultDetail Validate()
        {
            var validator = new SaleValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }
    }
}
