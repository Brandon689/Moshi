import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Typography, Card, CardContent, TextField, Button, Box } from '@mui/material';
import { useAuth } from '../AuthContext';
import api from '../api';

function Thread() {
    const { id } = useParams();
    const { user } = useAuth();
    const navigate = useNavigate();
    const [thread, setThread] = useState(null);
    const [posts, setPosts] = useState([]);
    const [newPostContent, setNewPostContent] = useState('');
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchThreadAndPosts = async () => {
            try {
                const threadResponse = await api.get(`/api/threads/${id}`);
                setThread(threadResponse.data);

                const postsResponse = await api.get(`/api/posts?threadId=${id}`);
                setPosts(postsResponse.data);
            } catch (error) {
                console.error('Error fetching thread and posts:', error);
                setError('Failed to load thread and posts');
            }
        };

        fetchThreadAndPosts();
    }, [id]);

    const handleSubmitPost = async (e) => {
        e.preventDefault();
        if (!user) {
            navigate('/login');
            return;
        }

        if (thread.isLocked) {
            setError('This thread is locked. You cannot post replies.');
            return;
        }
        console.log(user);
        console.log(user.id);
        try {
            const response = await api.post('/api/posts', {
                threadId: parseInt(id),
                userId: user.id,
                content: newPostContent
            });

            setPosts([...posts, response.data]);
            setNewPostContent('');
            setThread(prevThread => ({
                ...prevThread,
                replyCount: prevThread.replyCount + 1,
                lastPostId: response.data.id,
                lastPostAt: response.data.createdAt
            }));
        } catch (error) {
            console.error('Error creating post:', error);
            setError(error.response?.data?.message || 'Failed to create post');
        }
    };

    if (error) {
        return <Typography color="error">{error}</Typography>;
    }

    if (!thread) {
        return <Typography>Loading...</Typography>;
    }

    return (
        <Box sx={{ mt: 4 }}>
            <Typography variant="h4" gutterBottom>{thread.title}</Typography>
            <Box sx={{ mb: 2 }}>
                {thread.isLocked && <Chip label="Locked" color="error" sx={{ mr: 1 }} />}
                {thread.isPinned && <Chip label="Pinned" color="primary" sx={{ mr: 1 }} />}
                <Typography variant="caption">
                    Views: {thread.viewCount} | Replies: {thread.replyCount}
                </Typography>
            </Box>
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
            {user && !thread.isLocked ? (
                <Box component="form" onSubmit={handleSubmitPost} sx={{ mt: 4 }}>
                    <TextField
                        fullWidth
                        multiline
                        rows={4}
                        variant="outlined"
                        label="Your reply"
                        value={newPostContent}
                        onChange={(e) => setNewPostContent(e.target.value)}
                        required
                    />
                    <Button type="submit" variant="contained" sx={{ mt: 2 }}>
                        Submit Reply
                    </Button>
                </Box>
            ) : thread.isLocked ? (
                <Typography sx={{ mt: 4 }}>This thread is locked. New replies are not allowed.</Typography>
            ) : (
                <Typography sx={{ mt: 4 }}>Please <Button onClick={() => navigate('/login')}>log in</Button> to reply to this thread.</Typography>
            )}
        </Box>
    );
}

export default Thread;