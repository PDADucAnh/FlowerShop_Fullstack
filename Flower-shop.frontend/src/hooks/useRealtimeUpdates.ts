import { useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';

const entityQueryMap: Record<string, string[]> = {
  CategoryProduct: ['categories', 'products'],
  Product: ['products'],
  Post: ['posts'],
};

const hubUrl = process.env.REACT_APP_API_URL
  ? `${process.env.REACT_APP_API_URL.replace('/api', '')}/hubs/notifications`
  : 'http://localhost:5000/hubs/notifications';

export function useRealtimeUpdates() {
  const queryClient = useQueryClient();

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
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
