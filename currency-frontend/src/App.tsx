import { useState } from 'react';
import { Box, Container, AppBar, Toolbar, Typography, Tabs, Tab } from '@mui/material';
import { QueryClient, QueryClientProvider } from 'react-query';
import { CurrencyConverter } from './components/CurrencyConverter';
import { HistoricalRates } from './components/HistoricalRates';

const queryClient = new QueryClient();

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

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
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

function App() {
  const [currentTab, setCurrentTab] = useState(0);

  const handleTabChange = (event: React.SyntheticEvent, newValue: number) => {
    setCurrentTab(newValue);
  };

  return (
    <QueryClientProvider client={queryClient}>
      <Box sx={{ flexGrow: 1 }}>
        <AppBar position="static">
          <Toolbar>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
              Currency Exchange
            </Typography>
          </Toolbar>
        </AppBar>
        <Container>
          <Box sx={{ borderBottom: 1, borderColor: 'divider', mt: 2 }}>
            <Tabs value={currentTab} onChange={handleTabChange} aria-label="currency tabs">
              <Tab label="Currency Converter" />
              <Tab label="Historical Rates" />
            </Tabs>
          </Box>

          <TabPanel value={currentTab} index={0}>
            <CurrencyConverter />
          </TabPanel>
          <TabPanel value={currentTab} index={1}>
            <HistoricalRates />
          </TabPanel>
        </Container>
      </Box>
    </QueryClientProvider>
  );
}

export default App; 