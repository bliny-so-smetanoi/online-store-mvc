const path = require('path');
const { merge } = require('webpack-merge');
const common = require('./webpack.common.js');

// Fast rebuilds: no type checking in the loader, no minification, persistent cache.
// Type errors are reported by `npm run typecheck` and by the production build.
module.exports = merge(common({ transpileOnly: true }), {
    mode: 'development',
    devtool: 'eval-cheap-module-source-map',
    cache: {
        type: 'filesystem',
        cacheDirectory: path.resolve(__dirname, 'node_modules/.cache/webpack'),
        buildDependencies: { config: [__filename, path.resolve(__dirname, 'webpack.common.js')] }
    },
    optimization: {
        minimize: false,
        removeAvailableModules: false,
        removeEmptyChunks: false
    },
    output: {
        pathinfo: false
    },
    watchOptions: {
        ignored: /node_modules/,
        aggregateTimeout: 200
    },
    stats: 'errors-warnings'
});
