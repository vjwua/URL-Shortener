import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { getInfo, updateInfo } from '../api/algorithmApi';
import { AlgorithmInfoDTO } from '../types';
import styles from './AboutPage.module.css';

export default function AboutPage() {
    const [info, setInfo] = useState<AlgorithmInfoDTO | null>(null);
    const [editText, setEditText] = useState('');
    const [isEditing, setIsEditing] = useState(false);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState('');
    const { isAdmin } = useAuth();

    useEffect(() => {
        fetchInfo();
    }, []);

    const fetchInfo = async () => {
        try {
            const data = await getInfo();
            setInfo(data);
        } catch {
            setError('Failed to load algorithm description.');
        } finally {
            setLoading(false);
        }
    };

    const handleEdit = () => {
        setEditText(info?.description ?? '');
        setIsEditing(true);
    };

    const handleCancel = () => {
        setIsEditing(false);
        setError('');
    };

    const handleSave = async () => {
        if (!editText.trim()) return;
        setSaving(true);
        setError('');
        try {
            await updateInfo({ description: editText.trim() });
            setInfo({ description: editText.trim() });
            setIsEditing(false);
        } catch {
            setError('Failed to save description.');
        } finally {
            setSaving(false);
        }
    };

    return (
        <div className={styles.container}>
            <div className={styles.card}>
                <div className={styles.header}>
                    <h1 className={styles.title}>About the algorithm</h1>
                    {isAdmin && !isEditing && (
                        <button className={styles.editButton} onClick={handleEdit}>
                            Edit
                        </button>
                    )}
                </div>

                {error && <div className={styles.error}>{error}</div>}

                {loading ? (
                    <div className={styles.loading}>Loading...</div>
                ) : isEditing ? (
                    <>
                        <textarea
                            className={styles.textarea}
                            value={editText}
                            onChange={e => setEditText(e.target.value)}
                        />
                        <div className={styles.editActions}>
                            <button className={styles.cancelButton} onClick={handleCancel}>
                                Cancel
                            </button>
                            <button
                                className={styles.saveButton}
                                onClick={handleSave}
                                disabled={saving}
                            >
                                {saving ? 'Saving...' : 'Save'}
                            </button>
                        </div>
                    </>
                ) : (
                    <p className={styles.description}>
                        {info?.description ?? 'No description available.'}
                    </p>
                )}
            </div>
        </div>
    );
}