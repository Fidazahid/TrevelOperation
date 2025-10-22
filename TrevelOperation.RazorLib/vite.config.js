import { defineConfig } from 'vite';
export default defineConfig({
    build: {
        emptyOutDir: false,
        outDir: '../TrevelOperation/wwwroot',
        rollupOptions: {
            input: {
                main: './Assets/Js/app.js',
                styles: './Assets/Css/app.css'
            },
            output: {
                entryFileNames: `assets/[name].js`,
                chunkFileNames: `assets/[name].js`,
                assetFileNames: `assets/[name].[ext]`
            }
        },
    }
});