import React, { useState } from 'react';

const DataFetcher = () => {
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);

    const fetchData = async () => {
        setLoading(true);
        setError(null);
        try {
            const response = await fetch('https://localhost:7052/api/users');
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const result = await response.json();
            setData(result);
        } catch (error) {
            setError(error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
            <h1>Fetched Data</h1>
            <button onClick={fetchData}>Fetch Data</button>
            {loading && <div>Loading...</div>}
            {error && <div>Error: {error.message}</div>}
            <ul>
                {data.map((item, index) => (
                    <li key={index}>{item.username}</li>
                ))}
            </ul>
        </div>
    );
};

export default DataFetcher;
