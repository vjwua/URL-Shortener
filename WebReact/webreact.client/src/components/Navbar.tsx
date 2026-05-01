import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { logout } from '../api/accountApi';
import styles from './Navbar.module.css';

export default function Navbar() {
    const { isAuthenticated, isAdmin, logout: authLogout } = useAuth();
    const navigate = useNavigate();

    const handleLogout = async () => {
        try {
            const refreshToken = localStorage.getItem('refreshToken');
            if (refreshToken) await logout(refreshToken);
        } catch {
            //local cleanup
        } finally {
            authLogout();
            navigate('/login');
        }
    };

    return (
        <nav className={styles.navbar}>
            <div className={styles.left}>
                <NavLink to="/" className={styles.brand}>
                    URL Shortener
                </NavLink>

                <div className={styles.navLinks}>
                    <NavLink
                        to="/urls"
                        end
                        className={({ isActive }) =>
                            isActive ? `${styles.navLink} ${styles.navLinkActive}` : styles.navLink
                        }
                    >
                        URLs
                    </NavLink>

                    <NavLink
                        to="/about"
                        className={({ isActive }) =>
                            isActive ? `${styles.navLink} ${styles.navLinkActive}` : styles.navLink
                        }
                    >
                        About
                    </NavLink>
                </div>
            </div>

            <div className={styles.right}>
                {isAuthenticated ? (
                    <>
                        {isAdmin && (
                            <span className={styles.username}>Admin</span>
                        )}
                        <button className={styles.logoutButton} onClick={handleLogout}>
                            Sign out
                        </button>
                    </>
                ) : (
                    <NavLink to="/login" className={styles.loginLink}>
                        Sign in
                    </NavLink>
                )}
            </div>
        </nav>
    );
}