import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    proxy: {
      '/api': 'https://localhost:7224',
      '/hubs': {
        target: 'https://localhost:7224',
        ws: true,
      },
    },
  },
})
