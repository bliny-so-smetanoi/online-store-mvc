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
module.exports = {
    entry: entries(),
    output: {
        filename: '[name].bundle.js', // Output file name corresponds to entry key, preserving structure
        path: path.resolve(__dirname, '../wwwroot/js'),
    },
    resolve: {
        extensions: [".ts", ".js"],
        extensionAlias: {'.js': ['.js', '.ts']}
    },
    module: {
        rules: [
            { test: /\.ts$/, use: 'ts-loader', exclude: [/node_modules/] },

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
    plugins: [
        new MiniCssExtractPlugin({
            filename: "[name].css"
        })
    ],
    watch: true
};
