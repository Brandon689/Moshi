// src/components/Admin.js
import React from 'react';
import { Link, Route, Routes } from 'react-router-dom';
import Users from './Users';
import Posts from './Posts';
import Comments from './Comments';

const Admin = () => {
    return (
        <div className="container admin-section">
            <h1>Admin</h1>
            <nav className="admin-nav">
                <ul>
                    <li><Link to="users">Manage Users</Link></li>
                    <li><Link to="posts">Manage Posts</Link></li>
                    <li><Link to="comments">Manage Comments</Link></li>
                </ul>
            </nav>
            <Routes>
                <Route path="users" element={<Users />} />
                <Route path="posts" element={<Posts />} />
                <Route path="comments" element={<Comments />} />
            </Routes>
        </div>
    );
};

export default Admin;
