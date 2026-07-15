import { useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';

const entityQueryMap: Record<string, string[]> = {
  CategoryProduct: ['categories', 'products'],
  Product: ['products'],
  Post: ['posts'],
  PromotionCampaign: ['promotions', 'products'],
  Coupon: ['coupons'],
  FlashSale: ['flashsales', 'products'],
  Advertisement: ['advertisements', 'banners'],
  SystemSettings: ['settings'],
};

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7224';
const hubUrl = `${apiUrl.replace('/api', '')}/hubs/notifications`;

export function useRealtimeUpdates() {
  const queryClient = useQueryClient();

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    connection.on('EntityChanged', (entityName: string) => {
      const queryKeys = entityQueryMap[entityName];
      if (queryKeys) {
        queryKeys.forEach(key => queryClient.invalidateQueries({ queryKey: [key] }));
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
