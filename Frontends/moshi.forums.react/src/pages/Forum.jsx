import React, { useState, useEffect } from 'react';
import api from '../api';
import { useParams, Link } from 'react-router-dom';
import { List, ListItem, ListItemText, Typography } from '@mui/material';

function Forum() {
    const [threads, setThreads] = useState([]);
    const [forum, setForum] = useState(null);
    const { id } = useParams();

    useEffect(() => {
        const fetchForumAndThreads = async () => {
            try {
                const forumResponse = await api.get(`https://localhost:7018/api/forums/${id}`);
                setForum(forumResponse.data);

                const threadsResponse = await api.get(`https://localhost:7018/api/threads?forumId=${id}`);
                setThreads(threadsResponse.data);
            } catch (error) {
                console.error('Error fetching forum and threads:', error);
            }
        };

        fetchForumAndThreads();
    }, [id]);

    if (!forum) return <Typography>Loading...</Typography>;

    return (
        <div>
            <Typography variant="h4" gutterBottom>{forum.name}</Typography>
            <Typography variant="subtitle1" gutterBottom>{forum.description}</Typography>
            <List>
                {threads.map(thread => (
                    <ListItem key={thread.id} component={Link} to={`/thread/${thread.id}`}>
                        <ListItemText
                            primary={thread.title}
                            secondary={`Replies: ${thread.replyCount}, Views: ${thread.viewCount}`}
                        />
                    </ListItem>
                ))}
            </List>
        </div>
    );
}

export default Forum;
