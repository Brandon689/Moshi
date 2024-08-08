// src/components/Posts.js
import React, { useEffect, useState } from 'react';
import { getPosts, createPost, updatePost, deletePost } from '../services/api';

const Posts = () => {
    const [posts, setPosts] = useState([]);
    const [newPost, setNewPost] = useState({ title: '', content: '', authorId: 1, categoryId: 1 });

    useEffect(() => {
        fetchPosts();
    }, []);

    const fetchPosts = async () => {
        const response = await getPosts();
        setPosts(response.data);
    };

    const handleCreatePost = async () => {
        await createPost(newPost);
        fetchPosts();
    };

    const handleUpdatePost = async (id) => {
        await updatePost(id, newPost);
        fetchPosts();
    };

    const handleDeletePost = async (id) => {
        await deletePost(id);
        fetchPosts();
    };

    return (
        <div>
            <h2>Posts</h2>
            <div>
                <label>Title</label>
                <input
                    type="text"
                    placeholder="Title"
                    value={newPost.title}
                    onChange={(e) => setNewPost({ ...newPost, title: e.target.value })}
                />
                <label>Content</label>
                <textarea
                    placeholder="Content"
                    value={newPost.content}
                    onChange={(e) => setNewPost({ ...newPost, content: e.target.value })}
                />
                <button onClick={handleCreatePost}>Create Post</button>
            </div>

            <ul>
                {posts.map((post) => (
                    <li key={post.id}>
                        {post.title}
                        <button onClick={() => handleUpdatePost(post.id)}>Update</button>
                        <button onClick={() => handleDeletePost(post.id)}>Delete</button>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default Posts;
