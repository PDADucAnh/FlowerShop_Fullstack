type Callback = (...args: any[]) => void;

class EventEmitter {
  private listeners: Record<string, Callback[]> = {};

  on(event: string, callback: Callback): () => void {
    if (!this.listeners[event]) this.listeners[event] = [];
    this.listeners[event].push(callback);
    return () => {
      this.listeners[event] = this.listeners[event].filter(cb => cb !== callback);
    };
  }

  emit(event: string, ...args: any[]): void {
    (this.listeners[event] || []).forEach(cb => cb(...args));
  }
}

export const authEvents = new EventEmitter();
