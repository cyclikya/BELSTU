import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

export default defineConfig({
    plugins: [vue()],
    base: './',
    build: {
        rollupOptions: {
            input: 'TDW02-02.html'
        }
    }
});
