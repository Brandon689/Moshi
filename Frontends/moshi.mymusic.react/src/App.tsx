import { useState } from 'react'
import './App.css'
import { User } from './types/types.ts'
import AlbumList from './AlbumList.js'
const exampleUser: User = {
  userId: 1,
  username: "music_lover_42",
  email: "johndoe@example.com",
  passwordHash: "$2a$10$dX5Tn9Yl6ZeI4Yz5Tz6QOuXO1.1.1XpY5vLLJ9Z5Z5Z5Z5Z5Z5",
  dateOfBirth: new Date("1990-05-15"),
  country: "United States",
  premiumStatus: true,
  createdAt: new Date("2023-01-01T12:00:00Z"),
  lastLogin: new Date("2024-08-20T08:30:00Z")
};

console.log(exampleUser);









function App() {

  return (
    <>
ded
<AlbumList/>
    </>
  )
}

export default App
