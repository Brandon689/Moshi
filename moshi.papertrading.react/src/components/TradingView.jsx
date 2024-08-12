import React, { useEffect, useRef, useState } from 'react'
import { createChart, ColorType } from 'lightweight-charts'
import { Box, Heading, useTheme } from '@primer/react'

// Generate initial mock data
const generateInitialData = (numPoints = 100) => {
    const currentDate = new Date()
    currentDate.setDate(currentDate.getDate() - numPoints)
    let lastClose = 100

    return Array.from({ length: numPoints }, (_, i) => {
        currentDate.setDate(currentDate.getDate() + 1)
        lastClose += (Math.random() - 0.5) * 3
        return {
            time: currentDate.getTime() / 1000,
            value: parseFloat(lastClose.toFixed(2))
        }
    })
}

function TradingView({ onPriceUpdate }) {
    const chartContainerRef = useRef(null)
    const { colorMode } = useTheme()
    const [data, setData] = useState(generateInitialData())

    useEffect(() => {
        if (chartContainerRef.current) {
            const chartOptions = {
                layout: {
                    background: { type: ColorType.Solid, color: colorMode === 'dark' ? '#22272e' : '#ffffff' },
                    textColor: colorMode === 'dark' ? '#ffffff' : '#000000',
                },
                width: chartContainerRef.current.clientWidth,
                height: 300,
            }

            const chart = createChart(chartContainerRef.current, chartOptions)
            const lineSeries = chart.addLineSeries({
                color: colorMode === 'dark' ? '#2f81f7' : '#0969da',
            })

            lineSeries.setData(data)

            const updateInterval = setInterval(() => {
                const lastPoint = data[data.length - 1]
                const newPoint = {
                    time: new Date().getTime() / 1000,
                    value: parseFloat((lastPoint.value + (Math.random() - 0.5) * 2).toFixed(2))
                }
                setData(prevData => [...prevData, newPoint])
                lineSeries.update(newPoint)
                onPriceUpdate(newPoint.value)
            }, 1000)

            const handleResize = () => {
                chart.applyOptions({ width: chartContainerRef.current.clientWidth })
            }

            window.addEventListener('resize', handleResize)

            return () => {
                clearInterval(updateInterval)
                window.removeEventListener('resize', handleResize)
                chart.remove()
            }
        }
    }, [colorMode, onPriceUpdate])

    return (
        <Box p={4}>
            <Heading mb={2}>Live Stock Price Chart</Heading>
            <div ref={chartContainerRef} style={{ width: '100%' }} />
        </Box>
    )
}

export default TradingView
