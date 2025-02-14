using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSalesQuery;
using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="GetSalesQueryHandler"/> class.
/// </summary>
public class GetSalesQueryHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSalesQueryHandlerTests"/> class.
    /// Sets up the test dependencies.
    /// </summary>
    public GetSalesQueryHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesQueryHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests valid query parameters When getting sales Then returns paged result
    /// </summary>
    [Fact(DisplayName = "Given valid query parameters When getting sales Then returns paged result")]
    public async Task Handle_ValidQuery_ReturnsPagedResult()
    {
        // Arrange
        var query = new SaleQueryParams { Page = 1, PageSize = 10 };

        var sale1 = new Sale(Guid.NewGuid(), "VENDA 1", DateTime.Now, new Customer(Guid.NewGuid(), "Thiago"), new Branch(Guid.NewGuid(), "", ""));
        sale1.AddItems([new SaleItem(sale1.Id, new Product(Guid.NewGuid(), ""), 10, 10)]);

        var sale2 = new Sale(Guid.NewGuid(), "VENDA 2", DateTime.Now, new Customer(Guid.NewGuid(), "Borges"), new Branch(Guid.NewGuid(), "", ""));
        sale2.AddItems([new SaleItem(sale2.Id, new Product(Guid.NewGuid(), ""), 5, 5)]);

        var sales = new List<Sale> { sale1, sale2 };

        var pagedSales = new PagedResult<Sale>(sales, query.Page, query.PageSize, 2);
        var mappedResults = new List<GetSaleResult>
        {
            new GetSaleResult { Id = sales[0].Id, SaleNumber = sales[0].SaleNumber },
            new GetSaleResult { Id = sales[1].Id, SaleNumber = sales[1].SaleNumber }
        };

        _saleRepository.GetPagedSalesAsync(query.Page, query.PageSize, query.Filters, Arg.Any<CancellationToken>()).Returns(pagedSales);
        _mapper.Map<List<GetSaleResult>>(pagedSales.Items).Returns(mappedResults);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Page.Should().Be(query.Page);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalPages.Should().Be(1);
        await _saleRepository.Received(1).GetPagedSalesAsync(query.Page, query.PageSize, query.Filters, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<List<GetSaleResult>>(pagedSales.Items);
    }

    /// <summary>
    /// Tests empty result When getting sales Then returns empty paged result
    /// </summary>
    [Fact(DisplayName = "Given empty result When getting sales Then returns empty paged result")]
    public async Task Handle_EmptyResult_ReturnsEmptyPagedResult()
    {
        // Arrange
        var query = new SaleQueryParams { Page = 1, PageSize = 10 };
        var sales = new List<Sale>();
        var pagedSales = new PagedResult<Sale>(sales, query.Page, query.PageSize, 0);
        var mappedResults = new List<GetSaleResult>();

        _saleRepository.GetPagedSalesAsync(query.Page, query.PageSize, query.Filters, Arg.Any<CancellationToken>()).Returns(pagedSales);
        _mapper.Map<List<GetSaleResult>>(pagedSales.Items).Returns(mappedResults);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.Page.Should().Be(query.Page);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalPages.Should().Be(0);
        await _saleRepository.Received(1).GetPagedSalesAsync(query.Page, query.PageSize, query.Filters, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<List<GetSaleResult>>(pagedSales.Items);
    }

    /// <summary>
    /// Tests filters When getting sales Then applies filters correctly
    /// </summary>
    [Fact(DisplayName = "Given filters When getting sales Then applies filters correctly")]
    public async Task Handle_WithFilters_AppliesFiltersCorrectly()
    {
        // Arrange
        var query = new SaleQueryParams
        {
            Page = 1,
            PageSize = 10,
            Filters = new Dictionary<string, string>
            {
                { "SaleNumber", "Pedido do Thiago" },
                { "Date", "2025-02-14" }
            }
        };
        var sales = new List<Sale>
        {
            new Sale(Guid.NewGuid(), "Pedido do Thiago", DateTime.Parse("2025-02-14"), new Customer(Guid.NewGuid(), "Thiago"), new Branch(Guid.NewGuid(), "Filial", "Cidade"))
        };
        var pagedSales = new PagedResult<Sale>(sales, query.Page, query.PageSize, 1);
        var mappedResults = new List<GetSaleResult>
        {
            new GetSaleResult { Id = sales[0].Id, SaleNumber = sales[0].SaleNumber }
        };

        _saleRepository.GetPagedSalesAsync(query.Page, query.PageSize, query.Filters, Arg.Any<CancellationToken>()).Returns(pagedSales);
        _mapper.Map<List<GetSaleResult>>(pagedSales.Items).Returns(mappedResults);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items[0].SaleNumber.Should().Be("Pedido do Thiago");
        await _saleRepository.Received(1).GetPagedSalesAsync(query.Page, query.PageSize, query.Filters, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<List<GetSaleResult>>(pagedSales.Items);
    }
}