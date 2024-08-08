// src/components/PostDetail.js
import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getPostById, getCommentsByPostId, createComment } from '../services/api';
import './PostDetail.css'; // Import the CSS file for styling

const PostDetail = () => {
    const { id } = useParams();
    const [post, setPost] = useState(null);
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState({ content: '', postId: id, userId: 1 });

    useEffect(() => {
        fetchPost();
        fetchComments();
    }, [id]);

    const fetchPost = async () => {
        const response = await getPostById(id);
        setPost(response.data);
    };

    const fetchComments = async () => {
        const response = await getCommentsByPostId(id);
        setComments(response.data);
    };

    const handleCreateComment = async () => {
        await createComment(newComment);
        setNewComment({ ...newComment, content: '' });
        fetchComments();
    };

    return (
        <div className="container post-detail">
            {post && (
                <>
                    <h2>{post.title}</h2>
                    <p>{post.content}</p>
                    <div className="comments-section">
                        <h3>Comments</h3>
                        <ul>
                            {comments.map((comment) => (
                                <li key={comment.id}>{comment.content}</li>
                            ))}
                        </ul>
                        <div className="comment-form">
                            <h4>Add a Comment</h4>
                            <textarea
                                placeholder="Write your comment here..."
                                value={newComment.content}
                                onChange={(e) => setNewComment({ ...newComment, content: e.target.value })}
                            />
                            <button onClick={handleCreateComment}>Submit Comment</button>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
};

export default PostDetail;
