import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { getAll, create, remove, removeAll } from '../api/shortUrlApi';
import { ShortUrlResponse } from '../types';
import styles from './ShortUrlsTablePage.module.css';

export default function ShortUrlsTablePage() {
    const [urls, setUrls] = useState<ShortUrlResponse[]>([]);
    const [newUrl, setNewUrl] = useState('');
    const [loading, setLoading] = useState(true);
    const [creating, setCreating] = useState(false);
    const [error, setError] = useState('');
    const { isAuthenticated, isAdmin } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        fetchUrls();
    }, []);

    const fetchUrls = async () => {
        try {
            const data = await getAll();
            setUrls(data);
        } catch {
            setError('Failed to load URLs.');
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!newUrl.trim()) return;
        setCreating(true);
        setError('');
        try {
            const created = await create({ originalUrl: newUrl.trim() });
            setUrls(prev => [created, ...prev]);
            setNewUrl('');
        } catch {
            setError('Failed to create short URL. Make sure the URL is valid.');
        } finally {
            setCreating(false);
        }
    };

    const handleDelete = async (id: string) => {
        try {
            await remove(id);
            setUrls(prev => prev.filter(u => u.id !== id));
        } catch {
            setError('Failed to delete URL.');
        }
    };

    const handleDeleteAll = async () => {
        try {
            await removeAll();
            setUrls([]);
        } catch {
            setError('Failed to delete all URLs.');
        }
    };

    const formatDate = (iso: string) =>
        new Date(iso).toLocaleDateString('uk-UA', {
            day: '2-digit', month: '2-digit', year: 'numeric'
        });

    return (
        <div className={styles.container}>
            <div className={styles.header}>
                <div>
                    <h1 className={styles.title}>Short URLs</h1>
                    <p className={styles.subtitle}>
                        {urls.length} {urls.length === 1 ? 'entry' : 'entries'}
                    </p>
                </div>

                {isAdmin && urls.length > 0 && (
                    <button className={styles.deleteAllButton} onClick={handleDeleteAll}>
                        Delete all
                    </button>
                )}
            </div>

            {error && <div className={styles.error}>{error}</div>}

            {isAuthenticated && (
                <form className={styles.createForm} onSubmit={handleCreate}>
                    <input
                        type="url"
                        value={newUrl}
                        onChange={e => setNewUrl(e.target.value)}
                        placeholder="https://example.com/your-long-url"
                        required
                        className={styles.createInput}
                    />
                    <button type="submit" disabled={creating}>
                        {creating ? 'Creating...' : 'Shorten'}
                    </button>
                </form>
            )}

            <div className={styles.tableWrapper}>
                {loading ? (
                    <div className={styles.emptyState}>Loading...</div>
                ) : urls.length === 0 ? (
                    <div className={styles.emptyState}>No short URLs yet.</div>
                ) : (
                    <table className={styles.table}>
                        <colgroup>
                            <col style={{ width: '40%' }} />
                            <col style={{ width: '20%' }} />
                            <col style={{ width: '20%' }} />
                            {isAuthenticated && <col style={{ width: '20%' }} />}
                        </colgroup>
                        <thead>
                            <tr>
                                {['Original URL', 'Short code', 'Created'].map(h => (
                                    <th key={h} className={styles.th}>{h}</th>
                                ))}
                                {isAuthenticated && <th className={styles.th} />}
                            </tr>
                        </thead>
                        <tbody>
                            {urls.map(url => (
                                <tr
                                    key={url.id}
                                    className={styles.tr}
                                    onClick={() => navigate(`/urls/${url.id}`)}
                                >
                                    <td className={styles.tdUrl}>{url.originalUrl}</td>
                                    <td className={styles.tdCode}>{url.shortCode}</td>
                                    <td className={styles.tdDate}>{formatDate(url.createdAt)}</td>
                                    {isAuthenticated && (
                                        <td
                                            className={styles.tdAction}
                                            onClick={e => e.stopPropagation()}
                                        >
                                            <button
                                                className={styles.deleteButton}
                                                onClick={() => handleDelete(url.id)}
                                            >
                                                Delete
                                            </button>
                                        </td>
                                    )}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>
        </div>
    );
}