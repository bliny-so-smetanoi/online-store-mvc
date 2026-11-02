const { merge } = require('webpack-merge');
const TerserPlugin = require('terser-webpack-plugin');
const CssMinimizerPlugin = require('css-minimizer-webpack-plugin');
const common = require('./webpack.common.js');

// Smallest output: full type checking, no source maps, aggressive JS/CSS minification.
module.exports = merge(common(), {
    mode: 'production',
    devtool: false,
    performance: {
        hints: false
    },
    optimization: {
        minimize: true,
        usedExports: true,
        sideEffects: true,
        concatenateModules: true,
        minimizer: [
            new TerserPlugin({
                parallel: true,
                extractComments: false, // no separate *.LICENSE.txt files
                terserOptions: {
                    ecma: 2017,
                    compress: {
                        passes: 3,
                        pure_getters: true,
                        drop_debugger: true,
                        pure_funcs: ['console.log', 'console.info', 'console.debug'] // keep console.warn/error
                    },
                    mangle: true,
                    format: {
                        comments: false
                    }
                }
            }),
            new CssMinimizerPlugin({
                parallel: true,
                minimizerOptions: {
                    preset: ['default', { discardComments: { removeAll: true } }]
                }
            })
        ]
    }
});
