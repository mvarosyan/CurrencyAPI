import React, { useState, useEffect, useMemo } from 'react';
import { Box, TextField, Typography, Card, CardContent, Alert } from '@mui/material';
import { useQuery } from 'react-query';
import { currencyApi } from '../api/currencyApi';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, TooltipProps } from 'recharts';
import { ErrorBoundary } from './ErrorBoundary';

const CURRENCY_REGEX = /^[A-Z]{3}$/;

const formatDate = (date: string) => {
  try {
    const dateObj = new Date(date);
    if (isNaN(dateObj.getTime())) {
      return date;
    }
    return dateObj.toLocaleDateString('en-US', {
      month: '2-digit',
      day: '2-digit',
      year: 'numeric'
    });
  } catch (error) {
    console.error('Date formatting error:', error);
    return date;
  }
};

interface HistoricalDataPoint {
  date: string;
  value: number;
}

interface CustomTooltipProps extends TooltipProps<number, string> {
  active?: boolean;
  payload?: Array<{
    value: number;
    dataKey: string;
  }>;
  label?: string;
}

const CustomTooltip: React.FC<CustomTooltipProps> = ({ active, payload, label }) => {
  if (active && payload?.[0]) {
    try {
      const value = payload[0].value;
      const date = label;
      
      if (typeof value !== 'number' || !date) {
        console.log('Invalid tooltip data:', { value, date });
        return null;
      }

      return (
        <Card 
          elevation={3}
          sx={{ 
            p: 1.5,
            backgroundColor: '#fff',
            border: '1px solid rgba(0, 0, 0, 0.1)',
            minWidth: '150px'
          }}
        >
          <Typography variant="body2" sx={{ mb: 0.5, fontWeight: 'bold', color: '#666' }}>
            Date: {formatDate(date)}
          </Typography>
          <Typography variant="body2" sx={{ color: '#2196f3', fontWeight: 'bold' }}>
            Rate: {value.toFixed(4)}
          </Typography>
        </Card>
      );
    } catch (error) {
      console.error('Tooltip render error:', error);
      return null;
    }
  }
  return null;
};

const HistoricalRatesContent = () => {
  const [currency, setCurrency] = useState('USD');
  const [fromDate, setFromDate] = useState(() => {
    const date = new Date();
    date.setMonth(date.getMonth() - 1);
    return date.toISOString().split('T')[0];
  });
  const [toDate, setToDate] = useState(() => new Date().toISOString().split('T')[0]);
  const [error, setError] = useState('');

  const isValid = useMemo(() => {
    console.log('Validating inputs:', { currency, fromDate, toDate });
    
    if (!CURRENCY_REGEX.test(currency)) {
      return false;
    }
    
    try {
      const fromDateObj = new Date(fromDate);
      const toDateObj = new Date(toDate);
      
      if (fromDateObj > toDateObj) {
        return false;
      }
      
      if (isNaN(fromDateObj.getTime()) || isNaN(toDateObj.getTime())) {
        return false;
      }

      return true;
    } catch (error) {
      console.error('Date validation error:', error);
      return false;
    }
  }, [currency, fromDate, toDate]);

  const validateInputs = () => {
    if (!CURRENCY_REGEX.test(currency)) {
      setError('Currency code must be 3 uppercase letters');
      return false;
    }
    
    try {
      const fromDateObj = new Date(fromDate);
      const toDateObj = new Date(toDate);
      
      if (fromDateObj > toDateObj) {
        setError('From date must be before or equal to To date');
        return false;
      }
      
      if (isNaN(fromDateObj.getTime()) || isNaN(toDateObj.getTime())) {
        setError('Invalid date format');
        return false;
      }
    } catch (error) {
      console.error('Date validation error:', error);
      setError('Invalid date format');
      return false;
    }
    
    setError('');
    return true;
  };

  useEffect(() => {
    validateInputs();
  }, [currency, fromDate, toDate]);

  const { data: historicalData, isLoading, error: queryError } = useQuery<HistoricalDataPoint[]>(
    ['historical', currency, fromDate, toDate],
    async () => {
      console.log('Fetching historical data with params:', { currency, fromDate, toDate });
      const data = await currencyApi.getHistoricalRates(currency, new Date(fromDate), new Date(toDate));
      return data.map(item => ({
        date: item.lastUpdated,
        value: item.value
      }));
    },
    {
      enabled: isValid,
      retry: false,
      onError: (error: any) => {
        console.error('Historical rates query error:', error);
      }
    }
  );

  const handleCurrencyChange = (value: string) => {
    const upperValue = value.toUpperCase();
    setCurrency(upperValue);
  };

  return (
    <Card sx={{ maxWidth: 800, mx: 'auto', mt: 4 }}>
      <CardContent>
        <Typography variant="h5" gutterBottom>
          Historical Rates
        </Typography>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <TextField
            label="Currency"
            value={currency}
            onChange={(e) => handleCurrencyChange(e.target.value)}
            placeholder="e.g. USD"
            error={!!error && error.includes('Currency')}
            helperText={error.includes('Currency') ? error : ''}
            inputProps={{ maxLength: 3 }}
          />
          <Box sx={{ display: 'flex', gap: 2 }}>
            <TextField
              label="From Date"
              type="date"
              value={fromDate}
              onChange={(e) => {
                setFromDate(e.target.value);
                setError('');
              }}
              fullWidth
              error={!!error && error.includes('date')}
              helperText={error.includes('date') ? error : ''}
              InputLabelProps={{ shrink: true }}
            />
            <TextField
              label="To Date"
              type="date"
              value={toDate}
              onChange={(e) => {
                setToDate(e.target.value);
                setError('');
              }}
              fullWidth
              error={!!error && error.includes('date')}
              helperText={error.includes('date') ? error : ''}
              InputLabelProps={{ shrink: true }}
            />
          </Box>

          {queryError && (
            <Alert severity="error">
              {(queryError as any)?.response?.data?.error || 
               (queryError as any)?.message || 
               'Failed to fetch historical rates. Please try again.'}
            </Alert>
          )}

          {isLoading ? (
            <Alert severity="info">Loading historical rates...</Alert>
          ) : historicalData && historicalData.length > 0 ? (
            <Box component="div" sx={{ height: 400, mt: 2 }}>
              <ResponsiveContainer width="100%" height="100%">
                <LineChart 
                  data={historicalData}
                  margin={{ top: 10, right: 30, left: 60, bottom: 70 }}
                >
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis 
                    dataKey="date" 
                    tickFormatter={formatDate}
                    angle={-45}
                    textAnchor="end"
                    height={70}
                    interval="preserveStartEnd"
                    minTickGap={50}
                  />
                  <YAxis 
                    tickFormatter={(value) => value.toFixed(4)}
                    domain={['auto', 'auto']}
                    width={80}
                    tickMargin={5}
                    label={{ 
                      value: 'Rate',
                      angle: -90,
                      position: 'insideLeft',
                      offset: -40
                    }}
                  />
                  <Tooltip 
                    content={<CustomTooltip />}
                    cursor={{ strokeDasharray: '3 3' }}
                    wrapperStyle={{ 
                      zIndex: 1000,
                      backgroundColor: '#fff',
                      border: '1px solid #ccc',
                      borderRadius: '4px',
                      boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                      padding: '8px'
                    }}
                    isAnimationActive={false}
                  />
                  <Line 
                    type="monotone" 
                    dataKey="value" 
                    stroke="#2196f3"
                    strokeWidth={2}
                    dot={{ r: 3, fill: '#2196f3' }}
                    activeDot={{ 
                      r: 8,
                      stroke: '#2196f3',
                      strokeWidth: 2,
                      fill: '#fff'
                    }}
                    isAnimationActive={false}
                  />
                </LineChart>
              </ResponsiveContainer>
            </Box>
          ) : (
            <Alert severity="info">No historical data available for the selected period.</Alert>
          )}
        </Box>
      </CardContent>
    </Card>
  );
};

export const HistoricalRates = () => (
  <ErrorBoundary>
    <HistoricalRatesContent />
  </ErrorBoundary>
); 