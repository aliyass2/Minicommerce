using AutoMapper;
using MediatR;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Common.Models;
using Minicommerce.Domain.Cart;
using Minicommerce.Domain.Catalog;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.Application.Cart.AddItem;

public sealed class AddToCartCommandHandler
    : IRequestHandler<AddToCartCommand, Result<CartDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public AddToCartCommandHandler(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUser)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Result<CartDto>> Handle(AddToCartCommand request, CancellationToken ct)
    {
        try
        {
            // 0) Resolve current user
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                throw new CartException("User must be authenticated to modify a cart.");

            var cartRepo    = _uow.Repository<Minicommerce.Domain.Cart.Cart>();
            var productRepo = _uow.Repository<Product>();

            // 1) Load or create cart for this user
            var cart = await cartRepo.FirstOrDefaultAsync(c => c.UserId == userId, ct);
            if (cart is null)
            {
                cart = new Minicommerce.Domain.Cart.Cart(userId);
                await cartRepo.AddAsync(cart, ct);
            }

            // 2) Load product and validate
            var product = await productRepo.FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);
            if (product is null)
                throw new CatalogException("Product not found.");

            if (request.Quantity > product.StockQuantity)
                throw new CartException("Requested quantity exceeds available stock.");

            // 3) Add to cart
            cart.AddItem(product.Id, product.Name, product.Price.Amount, request.Quantity);

            // 4) Persist
            await _uow.SaveChangesAsync(ct);

            // 5) Return DTO wrapped in Result
            var dto = _mapper.Map<CartDto>(cart);
            return Result<CartDto>.Success(dto);
        }
        catch (CartException ex)
        {
            return Result<CartDto>.Failure(ex.Message);
        }
        catch (CatalogException ex)
        {
            return Result<CartDto>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result<CartDto>.Failure($"Failed to add item to cart: {ex.Message}");
        }
    }
}