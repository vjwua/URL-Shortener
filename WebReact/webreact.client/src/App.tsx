import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import Navbar from './components/Navbar';
import ProtectedRoute from './components/ProtectedRoute';
import LoginPage from './pages/LoginPage';
import ShortUrlsTablePage from './pages/ShortUrlsTablePage';
import ShortUrlInfoPage from './pages/ShortUrlInfoPage';
import AboutPage from './pages/AboutPage';
import RegisterPage from './pages/RegisterPage';

export default function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <Navbar />
                <Routes>
                    <Route path="/login" element={<LoginPage />} />
                    <Route path="/register" element={<RegisterPage />} />
                    <Route path="/urls" element={<ShortUrlsTablePage />} />
                    <Route path="/urls/:id" element={
                        <ProtectedRoute>
                            <ShortUrlInfoPage />
                        </ProtectedRoute>
                    } />
                    <Route path="/about" element={<AboutPage />} />
                    <Route path="*" element={<Navigate to="/" />} />
                </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}