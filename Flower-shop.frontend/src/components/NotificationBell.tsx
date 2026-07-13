import React, { useState, useRef, useEffect } from 'react';
import { useNotifications } from '../hooks/useNotifications';
import { Link, useNavigate } from 'react-router-dom';

export const NotificationBell: React.FC = () => {
    const { notifications, unreadCount, markAsRead, markAllAsRead } = useNotifications();
    const [isOpen, setIsOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    const handleNotificationClick = async (e: React.MouseEvent, n: any) => {
        e.preventDefault();
        setIsOpen(false);
        if (!n.isRead) {
            await markAsRead(n.id);
        }
        if (n.navigationUrl) {
            navigate(n.navigationUrl);
        }
    };

    const handleMarkAll = async (e: React.MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        await markAllAsRead();
    };

    return (
        <div className="relative" ref={dropdownRef}>
            <button 
                onClick={() => setIsOpen(!isOpen)}
                className="hover:text-primary/80 transition-colors text-primary bg-transparent border-0 cursor-pointer relative flex items-center justify-center p-2"
                aria-label="notifications"
            >
                <span className="material-symbols-outlined" data-icon="notifications">notifications</span>
                {unreadCount > 0 && (
                    <span className="absolute top-1 right-1 min-w-[16px] h-[16px] flex items-center justify-center bg-error text-on-error text-[10px] font-bold rounded-full leading-none px-1">
                        {unreadCount > 99 ? '99+' : unreadCount}
                    </span>
                )}
            </button>

            {isOpen && (
                <div className="absolute right-0 top-full mt-2 w-80 bg-surface shadow-lg rounded-xl border border-outline-variant/20 overflow-hidden z-50">
                    <div className="flex justify-between items-center p-3 border-b border-outline-variant/20 bg-surface-container-low">
                        <h3 className="text-sm font-bold m-0 text-on-surface">Thông báo</h3>
                        {unreadCount > 0 && (
                            <button 
                                onClick={handleMarkAll}
                                className="text-xs text-primary hover:underline bg-transparent border-0 cursor-pointer"
                            >
                                Đánh dấu đã đọc
                            </button>
                        )}
                    </div>
                    
                    <div className="max-h-96 overflow-y-auto">
                        {notifications.length === 0 ? (
                            <div className="p-4 text-center text-on-surface-variant text-sm">
                                Chưa có thông báo nào
                            </div>
                        ) : (
                            notifications.map((n: any) => (
                                <div 
                                    key={n.id} 
                                    onClick={(e) => handleNotificationClick(e, n)}
                                    className={`p-3 border-b border-outline-variant/10 cursor-pointer transition-colors hover:bg-surface-container-low ${
                                        !n.isRead ? 'bg-primary-container/10' : 'opacity-70'
                                    }`}
                                >
                                    <div className="flex justify-between items-start mb-1">
                                        <span className="text-xs font-bold text-primary">{n.type}</span>
                                        <span className="text-[10px] text-on-surface-variant">
                                            {new Date(n.createdAt).toLocaleDateString('vi-VN')}
                                        </span>
                                    </div>
                                    <div className="text-sm font-medium text-on-surface">{n.title}</div>
                                    <div className="text-xs text-on-surface-variant line-clamp-2 mt-1">{n.content}</div>
                                </div>
                            ))
                        )}
                    </div>
                </div>
            )}
        </div>
    );
};
