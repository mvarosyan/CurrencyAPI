import React, { Component, ErrorInfo, ReactNode } from 'react';
import { Alert, Card, CardContent, Typography } from '@mui/material';

interface Props {
  children: ReactNode;
}

interface State {
  hasError: boolean;
  error: Error | null;
}

export class ErrorBoundary extends Component<Props, State> {
  public state: State = {
    hasError: false,
    error: null
  };

  public static getDerivedStateFromError(error: Error): State {
    return { hasError: true, error };
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('Uncaught error:', error, errorInfo);
  }

  public render() {
    if (this.state.hasError) {
      return (
        <Card sx={{ maxWidth: 800, mx: 'auto', mt: 4 }}>
          <CardContent>
            <Alert severity="error">
              <Typography variant="h6">Something went wrong</Typography>
              <Typography variant="body2">
                {this.state.error?.message || 'An unexpected error occurred'}
              </Typography>
            </Alert>
          </CardContent>
        </Card>
      );
    }

    return this.props.children;
  }
} 