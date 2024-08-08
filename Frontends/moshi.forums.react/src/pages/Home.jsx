import React, { useState, useEffect } from 'react';
import api from '../api';
import { Link } from 'react-router-dom';
import { List, ListItem, ListItemText, Typography } from '@mui/material';

function Home() {
    const [forums, setForums] = useState([]);

    useEffect(() => {
        const fetchForums = async () => {
            try {
                const response = await api.get('https://localhost:7018/api/forums');
                setForums(response.data);
            } catch (error) {
                console.error('Error fetching forums:', error);
            }
        };

        fetchForums();
    }, []);

    return (
        <div>
            <Typography variant="h4" gutterBottom>Forums</Typography>
            <List>
                {forums.map(forum => (
                    <ListItem key={forum.id} component={Link} to={`/forum/${forum.id}`}>
                        <ListItemText
                            primary={forum.name}
                            secondary={forum.description}
                        />
                    </ListItem>
                ))}
            </List>
        </div>
    );
}

export default Home;
