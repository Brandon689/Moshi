import React, { useState, useEffect } from 'react';
import api from '../api';
import { useParams } from 'react-router-dom';
import { Typography, Card, CardContent } from '@mui/material';

function Thread() {
    const [thread, setThread] = useState(null);
    const [posts, setPosts] = useState([]);
    const { id } = useParams();

    useEffect(() => {
        const fetchThreadAndPosts = async () => {
            try {
                const threadResponse = await api.get(`https://localhost:7018/api/threads/${id}`);
                setThread(threadResponse.data);

                const postsResponse = await api.get(`https://localhost:7018/api/posts?threadId=${id}`);
                setPosts(postsResponse.data);
            } catch (error) {
                console.error('Error fetching thread and posts:', error);
            }
        };

        fetchThreadAndPosts();
    }, [id]);

    if (!thread) return <Typography>Loading...</Typography>;

    return (
        <div>
            <Typography variant="h4" gutterBottom>{thread.title}</Typography>
            {posts.map(post => (
                <Card key={post.id} sx={{ mb: 2 }}>
                    <CardContent>
                        <Typography variant="body1">{post.content}</Typography>
                        <Typography variant="caption" color="text.secondary">
                            Posted by: {post.userId} on {new Date(post.createdAt).toLocaleString()}
                        </Typography>
                    </CardContent>
                </Card>
            ))}
        </div>
    );
}

export default Thread;
