// src/components/Users.js
import React, { useEffect, useState } from 'react';
import { getUsers, createUser, updateUser, deleteUser } from '../services/api';

const Users = () => {
    const [users, setUsers] = useState([]);
    const [newUser, setNewUser] = useState({ username: '', email: '', passwordHash: '', isAdmin: false });

    useEffect(() => {
        fetchUsers();
    }, []);

    const fetchUsers = async () => {
        const response = await getUsers();
        setUsers(response.data);
    };

    const handleCreateUser = async () => {
        await createUser(newUser);
        fetchUsers();
    };

    const handleUpdateUser = async (id) => {
        await updateUser(id, newUser);
        fetchUsers();
    };

    const handleDeleteUser = async (id) => {
        await deleteUser(id);
        fetchUsers();
    };

    return (
        <div>
            <h2>Users</h2>
            <div>
                <label>Username</label>
                <input
                    type="text"
                    placeholder="Username"
                    value={newUser.username}
                    onChange={(e) => setNewUser({ ...newUser, username: e.target.value })}
                />
                <label>Email</label>
                <input
                    type="email"
                    placeholder="Email"
                    value={newUser.email}
                    onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
                />
                <label>Password</label>
                <input
                    type="password"
                    placeholder="Password"
                    value={newUser.passwordHash}
                    onChange={(e) => setNewUser({ ...newUser, passwordHash: e.target.value })}
                />
                <label>
                    Admin:
                    <input
                        type="checkbox"
                        checked={newUser.isAdmin}
                        onChange={(e) => setNewUser({ ...newUser, isAdmin: e.target.checked })}
                    />
                </label>
                <button onClick={handleCreateUser}>Create User</button>
            </div>

            <ul>
                {users.map((user) => (
                    <li key={user.id}>
                        {user.username} ({user.email}) - Admin: {user.isAdmin ? 'Yes' : 'No'}
                        <button onClick={() => handleUpdateUser(user.id)}>Update</button>
                        <button onClick={() => handleDeleteUser(user.id)}>Delete</button>
                    </li>
                ))}
            </ul>
        </div>
    );
};

export default Users;
