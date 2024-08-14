// OrderForm.jsx
import React, { useState } from 'react'
import { Box, Heading, Button } from '@primer/react'

function OrderForm({ currentPrice, placeOrder }) {
    const [orderType, setOrderType] = useState('market')
    const [quantity, setQuantity] = useState(1)
    const [limitPrice, setLimitPrice] = useState(currentPrice)

    const handleSubmit = (action) => {
        if (orderType === 'market') {
            placeOrder(action, quantity)
        } else {
            placeOrder(action, quantity, limitPrice)
        }
    }

    return (
        <Box p={4}>
            <Heading mb={2}>Place Order</Heading>
            <select
                value={orderType}
                onChange={(e) => setOrderType(e.target.value)}
                style={{ marginBottom: '8px', padding: '4px', width: '100%' }}
            >
                <option value="market">Market Order</option>
                <option value="limit">Limit Order</option>
            </select>
            <input
                type="number"
                value={quantity}
                onChange={(e) => setQuantity(parseInt(e.target.value))}
                style={{ marginBottom: '8px', padding: '4px', width: '100%' }}
                placeholder="Quantity"
            />
            {orderType === 'limit' && (
                <input
                    type="number"
                    value={limitPrice}
                    onChange={(e) => setLimitPrice(parseFloat(e.target.value))}
                    style={{ marginBottom: '8px', padding: '4px', width: '100%' }}
                    placeholder="Limit Price"
                />
            )}
            <div>
                <Button onClick={() => handleSubmit('buy')} mr={2}>Buy</Button>
                <Button onClick={() => handleSubmit('sell')}>Sell</Button>
            </div>
        </Box>
    )
}

export default OrderForm
