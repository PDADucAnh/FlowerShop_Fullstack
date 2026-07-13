import { useState, useEffect, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import { useAuth } from '../context/AuthContext';

export const useNotifications = () => {
    const { token, isAuthenticated } = useAuth();
    const [notifications, setNotifications] = useState<any[]>([]);
    const [unreadCount, setUnreadCount] = useState(0);
    const [isConnected, setIsConnected] = useState(false);

    // API to fetch notifications
    const fetchNotifications = useCallback(async () => {
        if (!isAuthenticated) return;
        try {
            const res = await axios.get('http://localhost:5000/api/notifications', {
                headers: { Authorization: `Bearer ${token}` }
            });
            setNotifications(res.data.items);
            
            const countRes = await axios.get('http://localhost:5000/api/notifications/unread-count', {
                headers: { Authorization: `Bearer ${token}` }
            });
            setUnreadCount(countRes.data.count);
        } catch (error) {
            console.error('Failed to fetch notifications', error);
        }
    }, [isAuthenticated, token]);

    // Setup SignalR
    useEffect(() => {
        if (!isAuthenticated || !token) return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5000/hubs/notifications", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        connection.on("ReceiveNotification", (notification: any) => {
            setNotifications(prev => [notification, ...prev]);
            
            // Show browser notification
            if (Notification.permission === "granted") {
                new Notification(notification.title, { body: notification.content });
            }
        });

        connection.on("UnreadCountChanged", (count: number) => {
            setUnreadCount(count);
        });

        connection.start()
            .then(() => setIsConnected(true))
            .catch(err => console.error('SignalR Connection Error: ', err));

        fetchNotifications();

        return () => {
            connection.stop();
        };
    }, [isAuthenticated, token, fetchNotifications]);

    // Actions
    const markAsRead = async (id: number) => {
        try {
            await axios.put(`http://localhost:5000/api/notifications/${id}/read`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setNotifications(prev => 
                prev.map((n: any) => n.id === id ? { ...n, isRead: true } : n)
            );
        } catch (error) {
            console.error('Mark as read failed', error);
        }
    };

    const markAllAsRead = async () => {
        try {
            await axios.put('http://localhost:5000/api/notifications/read-all', {}, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setNotifications(prev => 
                prev.map((n: any) => ({ ...n, isRead: true }))
            );
            setUnreadCount(0);
        } catch (error) {
            console.error('Mark all as read failed', error);
        }
    };

    return {
        notifications,
        unreadCount,
        isConnected,
        markAsRead,
        markAllAsRead,
        fetchNotifications
    };
};
