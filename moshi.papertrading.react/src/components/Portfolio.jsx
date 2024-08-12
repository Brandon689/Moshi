import React from 'react'
import { Box, Heading, Text, ThemeProvider, useTheme } from '@primer/react'

function Portfolio({ portfolio, currentPrice, transactions }) {
    const { theme } = useTheme()
    const portfolioValue = portfolio.cash + (portfolio.shares * currentPrice)
    const profitLoss = portfolioValue - 10000 // Assuming initial investment of $10000
    const profitLossPercentage = (profitLoss / 10000 * 100).toFixed(2)

    return (
        <Box p={4}>
            <Heading mb={3}>Your Portfolio</Heading>
            <Box mb={3} bg="canvas.subtle" borderColor="border.default" borderWidth={1} borderStyle="solid" borderRadius={2} p={3}>
                <Box display="grid" gridTemplateColumns="repeat(auto-fit, minmax(200px, 1fr))" gridGap={3}>
                    <PortfolioItem label="Cash" value={`$${portfolio.cash.toFixed(2)}`} />
                    <PortfolioItem label="Shares" value={portfolio.shares} />
                    <PortfolioItem label="Current Stock Price" value={`$${currentPrice.toFixed(2)}`} />
                    <PortfolioItem label="Portfolio Value" value={`$${portfolioValue.toFixed(2)}`} />
                    <PortfolioItem
                        label="Profit/Loss"
                        value={`$${profitLoss.toFixed(2)} (${profitLossPercentage}%)`}
                        valueColor={profitLoss >= 0 ? 'success.fg' : 'danger.fg'}
                    />
                </Box>
            </Box>

            <Heading as="h3" mb={2}>Transaction History</Heading>
            <Box overflowX="auto">
                <Box as="table" width="100%" style={{ borderCollapse: 'collapse' }}>
                    <Box as="thead">
                        <Box as="tr">
                            <TableHeader>Type</TableHeader>
                            <TableHeader>Quantity</TableHeader>
                            <TableHeader>Price</TableHeader>
                            <TableHeader>Date</TableHeader>
                        </Box>
                    </Box>
                    <Box as="tbody">
                        {transactions.map((transaction, index) => (
                            <Box as="tr" key={index}>
                                <TableCell>{transaction.type}</TableCell>
                                <TableCell>{transaction.quantity}</TableCell>
                                <TableCell>${transaction.price.toFixed(2)}</TableCell>
                                <TableCell>{transaction.date.toLocaleString()}</TableCell>
                            </Box>
                        ))}
                    </Box>
                </Box>
            </Box>
        </Box>
    )
}

function PortfolioItem({ label, value, valueColor = 'fg.default' }) {
    return (
        <Box display="flex" flexDirection="column">
            <Text fontSize={1} color="fg.muted" mb={1}>{label}</Text>
            <Text fontSize={3} fontWeight="bold" color={valueColor}>{value}</Text>
        </Box>
    )
}

function TableHeader({ children }) {
    return (
        <Box
            as="th"
            textAlign="left"
            p={2}
            borderBottom="1px solid"
            borderColor="border.default"
            color="fg.muted"
            fontSize={1}
        >
            {children}
        </Box>
    )
}

function TableCell({ children }) {
    return (
        <Box
            as="td"
            p={2}
            borderBottom="1px solid"
            borderColor="border.default"
            fontSize={2}
        >
            {children}
        </Box>
    )
}

export default Portfolio
