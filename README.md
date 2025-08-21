# Minicommerce
🛒 Minicommerce API



/////////////////////////////////////////////////////////////////////////////////////////////////
                       ************************************
/////////////////////////////////////////////////////////////////////////////////////////////////
Run the API
When running the API, in Program.cs there is a script that triggers seeding, you may comment it out after your first Run
From the project root:

dotnet run --project .\src\Minicommerce.WebApi\Minicommerce.WebApi.csproj

BUT-----
Dont forget to migrate First 

🗄 Database Setup

Create a migration:

dotnet ef migrations add InitSchema `
  --project .\Minicommerce.Infrastructure\Minicommerce.Infrastructure.csproj `
  --startup-project .\Minicommerce.WebApi\Minicommerce.WebApi.csproj `
  --output-dir Data\Migrations `
  --context ApplicationDbContext


Apply migration:

dotnet ef database update `
  --project .\Minicommerce.Infrastructure\Minicommerce.Infrastructure.csproj `
  --startup-project .\Minicommerce.WebApi\Minicommerce.WebApi.csproj `
  --context ApplicationDbContext
/////////////////////////////////////////////////////////////////////////////////////////////////
                       ************************************
/////////////////////////////////////////////////////////////////////////////////////////////////

🔑 Authentication

Users must log in to obtain a JWT token.

Pass the token in the Authorization: Bearer <token> header with each request.
/////////////////////////////////////////////////////////////////////////////////////////////////
                       ************************************
/////////////////////////////////////////////////////////////////////////////////////////////////
📦 API Workflow

Here’s a typical user flow through the system:

Login
Authenticate as a user and receive a JWT token.

Browse Products
GET /api/products
Fetch a list of products and select the desired ProductId.

Manage Cart

Add product: POST /api/cart/items

Update quantity: PUT /api/cart/items/{id}

Remove product: DELETE /api/cart/items/{id}

Clear cart: DELETE /api/cart

View cart:

GET /api/cart (paginated)

GET /api/cart/{userId} (admin/specific user)

Checkout
POST /api/checkout
Prepare the cart for payment.

Choose Payment Method
POST /api/checkout/{checkoutId}/pay
Pick a mocked payment method.

Finalize Checkout
Checkout completes once payment is processed.

Create Order
POST /api/orders/{checkoutId}
Place the order from the completed checkout.

View Orders

GET /api/orders/my → get logged-in user’s orders

GET /api/orders → get all orders (admin only)

📂 Project Structure

Minicommerce.Domain → Core domain entities, aggregates, and value objects

Minicommerce.Application → CQRS commands/queries, DTOs, business logic

Minicommerce.Infrastructure → EF Core configurations, repositories, migrations

Minicommerce.WebApi → API controllers, authentication, DI setup

🧪 Example Endpoints
Products
GET /api/products

Cart
POST /api/cart/items
{
  "productId": "guid",
  "quantity": 2
}

Checkout
POST /api/checkout

Payment
POST /api/checkout/{checkoutId}/pay
{
  "paymentMethod": "CreditCard"
}

Orders
GET /api/orders/my

📖 Notes

Payment methods are mocked (no real payment gateway).

