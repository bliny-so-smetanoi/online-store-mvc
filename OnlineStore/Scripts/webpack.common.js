const path = require('path');
const glob = require('glob');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

// Function to dynamically generate entries with folder structure
const entries = () => {
    const files = glob.sync('./src/**/*.ts'); // Find all .ts files in Scripts and subdirectories
    const entries = {};

    files.forEach(file => {
        // Remove the './Scripts/' prefix and the '.ts' suffix to create the entry name
        const entryName = file.replace('./src/', '').replace('.ts', '');
        entries[entryName] = './' + file;
    });

    return entries;
};

// Shared settings for webpack.dev.js and webpack.prod.js.
// ts-loader options are passed in by each environment config.
module.exports = (tsLoaderOptions = {}) => ({
    entry: entries(),
    output: {
        filename: '[name].bundle.js', // Output file name corresponds to entry key, preserving structure
        path: path.resolve(__dirname, '../wwwroot/js'),
        clean: true,
    },
    resolve: {
        extensions: [".ts", ".js"],
        extensionAlias: {'.js': ['.js', '.ts']}
    },
    module: {
        rules: [
            { test: /\.ts$/, use: { loader: 'ts-loader', options: tsLoaderOptions }, exclude: [/node_modules/] },

            // SASS/SCSS
            { test: /\.s[ac]ss$/i, use: [MiniCssExtractPlugin.loader, 'css-loader', 'sass-loader'] },

            // Plain CSS (DevExtreme themes)
            { test: /\.css$/i, use: [MiniCssExtractPlugin.loader, 'css-loader'] },

            // Assets (fonts for DevExtreme)
            { test: /\.(ttf|eot|woff2?)$/i, type: 'asset/resource', generator: { filename: 'fonts/[name][ext]' } },

            // Images
            { test: /\.(png|svg|jpg|jpeg|gif|webp)$/i, type: 'asset' }
        ]
    },
    optimization: {
        // One webpack runtime per page, so modules shared between bundles (jQuery, Bootstrap...) are instantiated once.
        // _Layout.cshtml must load runtime.bundle.js and vendors.bundle.js before any other bundle.
        runtimeChunk: { name: 'runtime' },
        splitChunks: {
            cacheGroups: {
                default: false,
                defaultVendors: false,
                // Third-party JS only; vendor CSS (bootstrap, icons) stays in src/index.css
                vendors: {
                    test: /[\\/]node_modules[\\/]/,
                    type: /^javascript\//,
                    name: 'vendors',
                    chunks: 'all',
                    enforce: true
                }
            }
        }
    },
    plugins: [
        new MiniCssExtractPlugin({
            filename: "[name].css"
        })
    ]
});
