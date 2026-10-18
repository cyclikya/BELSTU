import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';

export default defineConfig({
    plugins: [vue()],
    // './' — пути к js и css в собранной странице будут относительными
    base: './',
    build: {
        // входная страница; в сборке получится dist/index.html
        rollupOptions: {
            input: 'index.html'
        }
    }
});
