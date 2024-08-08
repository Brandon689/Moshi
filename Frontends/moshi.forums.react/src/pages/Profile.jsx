import React from 'react';
import { Typography, Container, Box } from '@mui/material';
import { useAuth } from '../AuthContext';

function Profile() {
    const { user } = useAuth();

    if (!user) {
        return <Typography>Please log in to view your profile.</Typography>;
    }

    return (
        <Container component="main" maxWidth="xs">
            <Box
                sx={{
                    marginTop: 8,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                }}
            >
                <Typography component="h1" variant="h5">
                    User Profile
                </Typography>
                <Typography>Username: {user.username}</Typography>
                <Typography>Email: {user.email}</Typography>
            </Box>
        </Container>
    );
}

export default Profile;
