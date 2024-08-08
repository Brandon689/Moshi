// src/App.js
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Blog from './components/Blog';
import Admin from './components/Admin';
import PostDetail from './components/PostDetail';
import './App.css'; // Import the CSS file

function App() {
    return (
        <Router>
            <div className="App">
                <Routes>
                    <Route path="/" element={<Blog />} />
                    <Route path="/admin/*" element={<Admin />} />
                    <Route path="/posts/:id" element={<PostDetail />} />
                </Routes>
            </div>
        </Router>
    );
}

export default App;
