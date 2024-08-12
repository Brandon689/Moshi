// App.jsx
import React, { useState, useEffect } from 'react'
import { ThemeProvider, BaseStyles, Box, ButtonGroup, Button } from '@primer/react'
import TradingView from './components/TradingView'
import Portfolio from './components/Portfolio'
import OrderForm from './components/OrderForm'

function App() {
    const [theme, setTheme] = useState('light')
    const [currentPrice, setCurrentPrice] = useState(100)
    const [portfolio, setPortfolio] = useState({ cash: 10000, shares: 0 })
    const [transactions, setTransactions] = useState([])

    useEffect(() => {
        document.documentElement.setAttribute('data-color-mode', theme)
    }, [theme])

    const updatePrice = (newPrice) => {
        setCurrentPrice(newPrice)
    }

    const placeOrder = (orderType, quantity, price = currentPrice) => {
        const totalCost = quantity * price
        if (orderType === 'buy' && totalCost <= portfolio.cash) {
            setPortfolio(prev => ({
                cash: prev.cash - totalCost,
                shares: prev.shares + quantity
            }))
            setTransactions(prev => [...prev, { type: 'buy', quantity, price, date: new Date() }])
        } else if (orderType === 'sell' && quantity <= portfolio.shares) {
            setPortfolio(prev => ({
                cash: prev.cash + totalCost,
                shares: prev.shares - quantity
            }))
            setTransactions(prev => [...prev, { type: 'sell', quantity, price, date: new Date() }])
        }
    }

    return (
        <ThemeProvider colorMode={theme}>
            <BaseStyles>
                <Box bg="canvas.default" color="fg.default" minHeight="100vh" p={3}>
                    <ButtonGroup mb={3}>
                        <Button onClick={() => setTheme('light')}>Light</Button>
                        <Button onClick={() => setTheme('dark')}>Dark</Button>
                    </ButtonGroup>
                    <TradingView onPriceUpdate={updatePrice} />
                    <OrderForm currentPrice={currentPrice} placeOrder={placeOrder} />
                    <Portfolio portfolio={portfolio} currentPrice={currentPrice} transactions={transactions} />
                </Box>
            </BaseStyles>
        </ThemeProvider>
    )
}

export default App
