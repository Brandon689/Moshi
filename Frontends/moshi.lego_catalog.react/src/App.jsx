import React, { useState } from 'react';
import { QueryClient, QueryClientProvider, useQuery } from '@tanstack/react-query';
import axios from 'axios';
import { BrowserRouter as Router, Route, Link, Routes } from 'react-router-dom';
import './App.css';

const queryClient = new QueryClient();

const BASE_URL = 'https://localhost:7175/api/catalog';
const fetchData = async (endpoint, page = 1, pageSize = 10) => {
  try {
    // Add a fake delay between 500ms and 1500ms
    const delay = Math.random() * 1000 + 500;
    await new Promise(resolve => setTimeout(resolve, delay));

    const { data } = await axios.get(`${BASE_URL}/${endpoint}`, {
      params: { page, pageSize }
    });
    return data;
  } catch (error) {
    console.error(`Error fetching ${endpoint}:`, error);
    throw error;
  }
};


function SkeletonItem() {
  return <li className="skeleton-item"></li>;
}

function LoadingList() {
  return (
    <ul>
      {[...Array(10)].map((_, index) => (
        <SkeletonItem key={index} />
      ))}
    </ul>
  );
}

function ListComponent({ title, endpoint, renderItem }) {
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const { isPending, error, data, isFetching } = useQuery({
    queryKey: [endpoint, page],
    queryFn: () => fetchData(endpoint, page, pageSize),
    keepPreviousData: true, // Keep the previous page's data while loading the next page
  });

  const handlePrevPage = () => setPage((prev) => Math.max(prev - 1, 1));
  const handleNextPage = () => setPage((prev) => prev + 1);

  return (
    <div className={`list-component ${isFetching ? 'is-fetching' : ''}`}>
      <h2>{title}</h2>
      {isPending && !data ? (
        <LoadingList />
      ) : error ? (
        <div className="error-message">An error occurred: {error.message || 'Unknown error'}</div>
      ) : (
        <>
          <ul>
            {data && data.map(renderItem)}
          </ul>
          <div className="pagination">
            <button onClick={handlePrevPage} disabled={page === 1}>
              &#8592; Previous
            </button>
            <span>Page {page}</span>
            <button onClick={handleNextPage} disabled={!data || data.length < pageSize}>
              Next &#8594;
            </button>
          </div>
          {isFetching && <div className="loading-indicator">Loading...</div>}
        </>
      )}
    </div>
  );
}


function Sets() {
  return (
    <ListComponent
      title="LEGO Sets"
      endpoint="sets"
      renderItem={(set) => (
        <li key={set.number}>
          {set.name} (Set #{set.number}) - Released in {set.year}, {set.numParts} parts
        </li>
      )}
    />
  );
}

function Parts() {
  return (
    <ListComponent
      title="LEGO Parts"
      endpoint="parts"
      renderItem={(part) => (
        <li key={part.number}>
          {part.name} (Part #{part.number}) - Category: {part.categoryId}
        </li>
      )}
    />
  );
}

function Minifigures() {
  return (
    <ListComponent
      title="LEGO Minifigures"
      endpoint="minifigures"
      renderItem={(minifig) => (
        <li key={minifig.number}>
          {minifig.name} (#{minifig.number}) - {minifig.numParts} parts
        </li>
      )}
    />
  );
}

function Categories() {
  return (
    <ListComponent
      title="LEGO Categories"
      endpoint="categories"
      renderItem={(category) => (
        <li key={category.categoryId}>{category.categoryName}</li>
      )}
    />
  );
}

function Colors() {
  return (
    <ListComponent
      title="LEGO Colors"
      endpoint="colors"
      renderItem={(color) => (
        <li key={color.colorId}>{color.colorName}</li>
      )}
    />
  );
}

function Navigation() {
  return (
    <nav className="navigation">
      <ul>
        <li><Link to="/sets">Sets</Link></li>
        <li><Link to="/parts">Parts</Link></li>
        <li><Link to="/minifigures">Minifigures</Link></li>
        <li><Link to="/categories">Categories</Link></li>
        <li><Link to="/colors">Colors</Link></li>
      </ul>
    </nav>
  );
}

function Layout({ children }) {
  return (
    <div className="layout">
      <h1>LEGO Catalog</h1>
      <Navigation />
      <main className="content">
        {children}
      </main>
    </div>
  );
}

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <Layout>
          <Routes>
            <Route path="/sets" element={<Sets />} />
            <Route path="/parts" element={<Parts />} />
            <Route path="/minifigures" element={<Minifigures />} />
            <Route path="/categories" element={<Categories />} />
            <Route path="/colors" element={<Colors />} />
          </Routes>
        </Layout>
      </Router>
    </QueryClientProvider>
  );
}

export default App;
