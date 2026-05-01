import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { getById, remove } from '../api/shortUrlApi';
import { ShortUrlResponse } from '../types';
import styles from './ShortUrlInfoPage.module.css';

export default function ShortUrlInfoPage() {
    const { id } = useParams<{ id: string }>();
    const [url, setUrl] = useState<ShortUrlResponse | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const { isAdmin } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        if (!id) return;
        fetchUrl(id);
    }, [id]);

    const fetchUrl = async (urlId: string) => {
        try {
            const data = await getById(urlId);
            setUrl(data);
        } catch {
            setError('Short URL not found.');
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async () => {
        if (!url) return;
        try {
            await remove(url.id);
            navigate('/');
        } catch {
            setError('Failed to delete URL.');
        }
    };

    const formatDate = (iso: string) =>
        new Date(iso).toLocaleDateString('uk-UA', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });

    const shortLink = `${window.location.origin}/api/shorturl/reroute/${url?.shortCode}`;

    return (
        <div className={styles.container}>
            <button className={styles.backButton} onClick={() => navigate('/')}>
                ← Back
            </button>

            <div className={styles.card}>
                <h1 className={styles.title}>URL details</h1>

                {loading ? (
                    <div className={styles.loading}>Loading...</div>
                ) : error ? (
                    <div className={styles.error}>{error}</div>
                ) : url && (
                    <>
                        <div className={styles.fieldList}>
                            <div className={styles.field}>
                                <span className={styles.fieldLabel}>Original URL</span>
                            <a href={url.originalUrl} target="_blank" rel="noopener noreferrer" className={styles.fieldValueLink} >
                                {url.originalUrl}
                            </a>
                        </div>

                        <hr className={styles.divider} />

                        <div className={styles.field}>
                            <span className={styles.fieldLabel}>Short code</span>
                            <span className={styles.fieldValueMono}>{url.shortCode}</span>
                        </div>

                        <hr className={styles.divider} />

                        <div className={styles.field}>
                            <span className={styles.fieldLabel}>Short link</span>
                        <a href={shortLink} target="_blank" rel="noopener noreferrer" className={styles.fieldValueLink} >
                            {shortLink}
                        </a>
                    </div>

                <hr className={styles.divider} />

                <div className={styles.field}>
                    <span className={styles.fieldLabel}>Created at</span>
                    <span className={styles.fieldValue}>{formatDate(url.createdAt)}</span>
                </div>
            </div>

            {(isAdmin || true) && (
                <div className={styles.footer}>
                    <button className={styles.deleteButton} onClick={handleDelete}>
                        Delete
                    </button>
                </div>
            )}
        </>
    )
}
      </div >
    </div >
  );
}