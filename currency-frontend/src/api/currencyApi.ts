import axios from 'axios';

// Ensure we're using HTTP during development
const API_BASE_URL = 'http://localhost:5151/api/Currency';

// Add request interceptor for logging
axios.interceptors.request.use(request => {
  console.log('API Request:', {
    url: request.url,
    method: request.method,
    params: request.params,
    data: request.data
  });
  return request;
});

// Add response interceptor for logging
axios.interceptors.response.use(
  response => {
    console.log('API Response:', {
      status: response.status,
      data: response.data
    });
    return response;
  },
  error => {
    console.error('API Error:', {
      status: error.response?.status,
      data: error.response?.data,
      message: error.message
    });
    throw error;
  }
);

export interface CurrencyRate {
  currency: string;
  value: number;
}

export interface HistoricalRate {
  currency: string;
  value: number;
  lastUpdated: string;
}

export const currencyApi = {
  assignCurrency: async (currency: string, value: number) => {
    const response = await axios.post(`${API_BASE_URL}/assign`, { currency, value });
    return response.data;
  },

  getRate: async (currency: string) => {
    const response = await axios.get(`${API_BASE_URL}/rate/${currency}`);
    return response.data;
  },

  fetchAndSaveRates: async () => {
    const response = await axios.post(`${API_BASE_URL}/fetch-and-save`);
    return response.data;
  },

  calculateConversion: async (from: string, to: string, amount: number) => {
    const response = await axios.get(`${API_BASE_URL}/calculate`, {
      params: { from, to, amount }
    });
    return response.data;
  },

  getHistoricalRates: async (currency: string, fromDate?: Date, toDate?: Date) => {
    const response = await axios.get(`${API_BASE_URL}/historical`, {
      params: {
        currency,
        fromDate: fromDate?.toISOString(),
        toDate: toDate?.toISOString()
      }
    });
    return response.data;
  },

  deleteCurrency: async (currency: string) => {
    const response = await axios.delete(`${API_BASE_URL}/delete`, {
      params: { currency }
    });
    return response.data;
  }
}; 