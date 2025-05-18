import React, { useState, ReactNode } from 'react';
import { Box, TextField, Button, Typography, Card, CardContent, CircularProgress, Alert } from '@mui/material';
import { useQuery } from 'react-query';
import { currencyApi } from '../api/currencyApi';

interface ConversionResult {
  from: string;
  to: string;
  amount: number;
  result: number;
}

interface TabPanelProps {
  children?: ReactNode;
  index: number;
  value: number;
}

const CURRENCY_REGEX = /^[A-Z]{3}$/;

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}
    >
      {value === index && <Box>{children}</Box>}
    </div>
  );
}

export const CurrencyConverter = () => {
  const [fromCurrency, setFromCurrency] = useState('USD');
  const [toCurrency, setToCurrency] = useState('EUR');
  const [amount, setAmount] = useState('1');
  const [errors, setErrors] = useState({
    fromCurrency: '',
    toCurrency: '',
    amount: ''
  });

  const validateInputs = () => {
    const newErrors = {
      fromCurrency: '',
      toCurrency: '',
      amount: ''
    };

    if (!CURRENCY_REGEX.test(fromCurrency)) {
      newErrors.fromCurrency = 'Currency code must be 3 uppercase letters';
    }
    if (!CURRENCY_REGEX.test(toCurrency)) {
      newErrors.toCurrency = 'Currency code must be 3 uppercase letters';
    }
    if (isNaN(Number(amount)) || Number(amount) <= 0) {
      newErrors.amount = 'Amount must be a positive number';
    }

    setErrors(newErrors);
    return !Object.values(newErrors).some(error => error !== '');
  };

  const { data: conversionResult, isLoading, error, refetch } = useQuery<ConversionResult>(
    ['conversion', fromCurrency, toCurrency, amount],
    () => currencyApi.calculateConversion(fromCurrency, toCurrency, parseFloat(amount)),
    {
      enabled: false,
      retry: false,
    }
  );

  const handleConvert = () => {
    if (validateInputs()) {
      refetch();
    }
  };

  const handleCurrencyChange = (field: 'fromCurrency' | 'toCurrency', value: string) => {
    const upperValue = value.toUpperCase();
    if (field === 'fromCurrency') {
      setFromCurrency(upperValue);
    } else {
      setToCurrency(upperValue);
    }
    setErrors(prev => ({ ...prev, [field]: '' }));
  };

  return (
    <Card sx={{ maxWidth: 600, mx: 'auto', mt: 4 }}>
      <CardContent>
        <Typography variant="h5" gutterBottom>
          Currency Converter
        </Typography>
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <TextField
            label="Amount"
            type="number"
            value={amount}
            onChange={(e) => {
              setAmount(e.target.value);
              setErrors(prev => ({ ...prev, amount: '' }));
            }}
            error={!!errors.amount}
            helperText={errors.amount}
            fullWidth
            inputProps={{ min: "0", step: "0.01" }}
          />
          <TextField
            label="From Currency"
            value={fromCurrency}
            onChange={(e) => handleCurrencyChange('fromCurrency', e.target.value)}
            placeholder="e.g. USD"
            error={!!errors.fromCurrency}
            helperText={errors.fromCurrency}
            inputProps={{ maxLength: 3 }}
            fullWidth
          />
          <TextField
            label="To Currency"
            value={toCurrency}
            onChange={(e) => handleCurrencyChange('toCurrency', e.target.value)}
            placeholder="e.g. EUR"
            error={!!errors.toCurrency}
            helperText={errors.toCurrency}
            inputProps={{ maxLength: 3 }}
            fullWidth
          />
          <Button
            variant="contained"
            onClick={handleConvert}
            disabled={isLoading || !fromCurrency || !toCurrency || !amount}
            fullWidth
          >
            {isLoading ? <CircularProgress size={24} /> : 'Convert'}
          </Button>

          {error && (
            <Alert severity="error">
              {(error as any)?.response?.data?.message || 
               (error as any)?.response?.data?.error || 
               (error as any)?.message ||
               'Failed to convert currency. Please try again.'}
            </Alert>
          )}

          {conversionResult && (
            <Alert severity="success">
              <Typography variant="body1">
                {conversionResult.amount} {conversionResult.from} = {conversionResult.result.toFixed(2)} {conversionResult.to}
              </Typography>
            </Alert>
          )}
        </Box>
      </CardContent>
    </Card>
  );
}; 