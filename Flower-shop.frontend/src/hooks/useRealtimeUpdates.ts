import { useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';

const entityQueryMap: Record<string, string[]> = {
  CategoryProduct: ['categories', 'products'],
  Product: ['products'],
  Post: ['posts'],
  PromotionCampaign: ['promotions'],
  Coupon: ['coupons'],
  FlashSale: ['flashsales'],
  Advertisement: ['advertisements', 'banners'],
  SystemSettings: ['settings'],
};

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7224';
const hubUrl = `${apiUrl.replace('/api', '')}/hubs/notifications`;

const signalRLogger: signalR.ILogger = {
  log: (logLevel: signalR.LogLevel, message: string) => {
    if (logLevel < signalR.LogLevel.Warning) return;
    if (message.includes('Failed to start the connection') || message.includes('was stopped during negotiation')) return;
    console.warn(`[SignalR] ${message}`);
  },
};

export function useRealtimeUpdates() {
  const queryClient = useQueryClient();

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .configureLogging(signalRLogger)
      .build();

    connection.on('EntityChanged', (entityName: string) => {
      const queryKey = entityQueryMap[entityName];
      if (queryKey) {
        queryClient.invalidateQueries({ queryKey });
      }
    });

    connection.start().catch(() => {
      // SignalR connection failed — will retry automatically
    });

    return () => {
      connection.stop();
    };
  }, [queryClient]);
}
