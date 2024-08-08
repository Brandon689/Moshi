import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { CssBaseline, Container } from '@mui/material';
import { AuthProvider } from './AuthContext';
import Header from './components/Header';
import Home from './pages/Home';
import Forum from './pages/Forum';
import Thread from './pages/Thread';
import Login from './pages/Login';
import Register from './pages/Register';
import Profile from './pages/Profile';

function App() {
    return (
        <AuthProvider>
            <Router>
                <CssBaseline />
                <Header />
                <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
                    <Routes>
                        <Route path="/" element={<Home />} />
                        <Route path="/forum/:id" element={<Forum />} />
                        <Route path="/thread/:id" element={<Thread />} />
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        <Route path="/profile/:id" element={<Profile />} />
                    </Routes>
                </Container>
            </Router>
        </AuthProvider>
    );
}

export default App;
