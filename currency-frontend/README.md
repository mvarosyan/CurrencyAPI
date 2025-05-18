# Currency Exchange Frontend

This is the React frontend for the Currency API project. It provides a modern, responsive interface for real-time currency rate tracking and historical data visualization.

## ✨ Features

- Real-time currency rate monitoring with automatic updates
- Historical rate visualization with interactive charts
- Modern, responsive UI built with TypeScript and Vite
- Real-time data synchronization with the backend API
- Memory-efficient data caching using React Query
- Comprehensive error handling and loading states
- Mobile-first responsive design

## 🛠️ Tech Stack

- React 18
- TypeScript
- Vite
- React Query for data fetching
- Modern UI components
- ESLint + Prettier for code quality

## 📋 Prerequisites

Before running this application, ensure you have:
- Node.js (v14 or later)
- npm (comes with Node.js)
- The Currency API backend running (default: http://localhost:7001)

## 🔧 Installation

1. Install dependencies:
```bash
npm install
```

2. Create a `.env` file in the frontend root:
```env
VITE_API_URL=http://localhost:7001
```

## 🚀 Development

```bash
# Start development server
npm run dev

# Run ESLint
npm run lint

# Run ESLint with auto-fix
npm run lint:fix

# Format code with Prettier
npm run format
```

The development server will be available at http://localhost:5173

## 📦 Production Build

Create a production build:
```bash
npm run build
```

Preview the production build locally:
```bash
npm run preview
```

The build output will be in the `dist` directory.

## 🔌 API Integration

The frontend communicates with the Currency API through the following endpoints:

- `GET /api/currency/{code}` - Fetch current rate
- `GET /api/currency/historical/{code}` - Fetch historical rates
- `POST /api/currency` - Update currency rate
- `DELETE /api/currency/{code}` - Deactivate currency

## 📁 Project Structure

```
src/
├── components/     # Reusable UI components
├── pages/         # Route components
├── hooks/         # Custom React hooks
├── services/      # API integration
├── utils/         # Helper functions
├── types/         # TypeScript definitions
└── styles/        # Global styles and themes
```

## 🧪 Testing

```bash
# Run unit tests
npm run test

# Run tests in watch mode
npm run test:watch

# Generate coverage report
npm run test:coverage
```

## 📚 Contributing

Please refer to the main README in the root directory for contribution guidelines.

## 📝 License

This project is licensed under the MIT License - see the main README for details. 