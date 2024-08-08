// src/components/Comments.js
import React, { useEffect, useState } from 'react';
import { getCommentsByPostId, createComment, updateComment, deleteComment } from '../services/api';

const Comments = ({ postId }) => {
    const [comments, setComments] = useState([]);
    const [newComment, setNewComment] = useState({ content: '', postId, userId: 1 });

    useEffect(() => {
        fetchComments();
    }, [postId]);

    const fetchComments = async () => {
        const response = await getCommentsByPostId(postId);
        setComments(response.data);
    };

    const handleCreateComment = async () => {
        await createComment(newComment);
        fetchComments();
    };

    const handleUpdateComment = async (id) => {
        await updateComment(id, newComment);
        fetchComments();
    };

    const handleDeleteComment = async (id) => {
        await deleteComment(id);
        fetchComments();
    };

    return (
        <div>
            <h2>Comments</h2>
            <div>
                <label>Content</label>
                <textarea
                    placeholder="Content"
                    value={newComment.content}
                    onChange={(e) => setNewComment({ ...newComment, content: e.target.value })}
                />
                <button onClick={handleCreateComment}>Create Comment</button>
            </div>

            <ul>
                {comments.map((comment) => (
                    <li key={comment.id}>
                        {comment.content}
                        <button onClick={() => handleUpdateComment(comment.id)}>Update</button>
                        <button onClick={() => handleDeleteComment(comment.id)}>Delete</button>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default Comments;
