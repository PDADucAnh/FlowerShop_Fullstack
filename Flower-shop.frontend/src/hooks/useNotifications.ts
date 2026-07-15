import { useState, useEffect, useCallback } from 'react';
import * as signalR from '@microsoft/signalr';
import axios from 'axios';
import { useAuth } from '../context/AuthContext';
import { useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';

export const useNotifications = () => {
    const { token, isAuthenticated, logout } = useAuth();
    const queryClient = useQueryClient();
    const [notifications, setNotifications] = useState<any[]>([]);
    const [unreadCount, setUnreadCount] = useState(0);
    const [isConnected, setIsConnected] = useState(false);

    const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7224';

    // API to fetch notifications
    const fetchNotifications = useCallback(async () => {
        if (!isAuthenticated) return;
        try {
            const res = await axios.get(`${apiUrl}/api/notifications`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setNotifications(res.data.items);
            
            const countRes = await axios.get(`${apiUrl}/api/notifications/unread-count`, {
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

        const hubUrl = `${apiUrl.replace('/api', '')}/hubs/notifications`;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        connection.on("ReceiveNotification", (notification: any) => {
            setNotifications(prev => [notification, ...prev]);
            
            toast(notification.title, {
                icon: '🔔',
                style: {
                    borderRadius: '10px',
                    background: '#333',
                    color: '#fff',
                },
            });

            // Show browser notification
            if (Notification.permission === "granted") {
                new Notification(notification.title, { body: notification.content });
            }
        });

        connection.on("UnreadCountChanged", (count: number) => {
            setUnreadCount(count);
        });

        connection.on("OrderChanged", (data: any) => {
            console.log("SignalR OrderChanged", data);
            queryClient.invalidateQueries({ queryKey: ['orders'] });
            queryClient.invalidateQueries({ queryKey: ['order', data.orderId] });
            queryClient.invalidateQueries({ queryKey: ['orderDetail', data.orderId] });
        });

        connection.on("CustomerLocked", (data: any) => {
            console.log("SignalR CustomerLocked", data);
            alert("Tài khoản của bạn đã bị khóa bởi quản trị viên.");
            logout();
        });

        connection.on("AdminAnnouncement", (data: any) => {
            console.log("SignalR AdminAnnouncement", data);
            toast(data.title || "Thông báo từ quản trị viên", {
                icon: '📢',
                style: {
                    borderRadius: '10px',
                    background: '#1a73e8',
                    color: '#fff',
                },
            });
        });

        const startPromise = connection.start()
            .then(() => setIsConnected(true))
            .catch(err => console.error('SignalR Connection Error: ', err));

        fetchNotifications();

        return () => {
            startPromise.then(() => connection.stop());
        };
    }, [isAuthenticated, token, fetchNotifications]);

    // Actions
    const markAsRead = async (id: number) => {
        try {
            await axios.put(`${apiUrl}/api/notifications/${id}/read`, {}, {
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
            await axios.put(`${apiUrl}/api/notifications/read-all`, {}, {
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
