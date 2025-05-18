# Currency API

A modern, full-stack currency exchange rate tracking application built with .NET 7 and React. This project provides real-time currency rate monitoring, historical data tracking, and currency management capabilities.

## 🚀 Features

- Real-time currency exchange rate tracking
- Historical rate data visualization
- Currency management (add, update, delete)
- RESTful API endpoints
- Modern React frontend with real-time updates
- PostgreSQL database with Entity Framework Core
- Background workers for automatic rate updates
- In-memory caching for improved performance
- OpenExchangeRates API integration

## 🛠️ Tech Stack

### Backend
- .NET 7
- Entity Framework Core 9.0
- PostgreSQL
- ASP.NET Core Web API
- Background Services
- Memory Caching

### Frontend
- React
- TypeScript
- Vite
- Modern UI components

## 📋 Prerequisites

- .NET 7 SDK
- Node.js (v14 or later)
- PostgreSQL 14 or later
- Visual Studio 2022 or VS Code

## 🔧 Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/CurrencyAPI.git
cd CurrencyAPI
```

2. Set up PostgreSQL:
```bash
# Create a new PostgreSQL database
createdb mydb

# Or use your preferred PostgreSQL administration tool
```

3. Configure the database connection:
- Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=mydb;Username=your_username;Password=your_password"
  }
}
```

4. Set up the backend:
```bash
cd CurrencyAPI
dotnet restore
dotnet ef database update
```

5. Set up the frontend:
```bash
cd ../currency-frontend
npm install
```

## 🚀 Running the Application

### Backend
```bash
cd CurrencyAPI
dotnet run
```
The API will be available at `https://localhost:7072`

### Frontend
```bash
cd currency-frontend
npm run dev
```
The frontend will be available at `http://localhost:5173`

## 🔌 API Endpoints

- `GET /api/currency/{code}` - Get current rate for a specific currency
- `GET /api/currency/historical/{code}` - Get historical rates for a currency
- `POST /api/currency` - Add or update a currency rate
- `DELETE /api/currency/{code}` - Deactivate a currency

## 📦 Project Structure

```
CurrencyAPI/
├── Controllers/    # API endpoints
├── Services/       # Business logic
├── Data/          # Data access and repositories
├── Models/        # DTOs and view models
├── Entities/      # Database entities
├── Workers/       # Background services
├── Cache/         # Memory caching implementation
└── Configuration/ # App configuration

currency-frontend/
├── src/
│   ├── components/    # React components
│   ├── services/     # API integration
│   └── hooks/        # Custom React hooks
```

## 🗄️ Database Schema

The application uses two main tables:
- `Currencies`: Stores currency codes and their active status
- `CurrencyRates`: Stores historical exchange rates with timestamps

## 🔒 Environment Variables

Required environment variables:
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string
- `ApiSettings__CurrencyApiKey`: OpenExchangeRates API key

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.
