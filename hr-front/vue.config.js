const { defineConfig } = require('@vue/cli-service')
const AutoImport = require('unplugin-auto-import/webpack')
const Components = require('unplugin-vue-components/webpack')
const { ElementPlusResolver } = require('unplugin-vue-components/resolvers')

module.exports = defineConfig({
  transpileDependencies: true,
  // 端口/后端接口地址都放 .env.development，这里只负责读进来用
  devServer: {
    port: process.env.VUE_APP_PORT || 8081,
    // history 模式必配：直接访问/刷新 /login 这类地址时，把请求回落到 index.html 交给前端路由
    historyApiFallback: true,
  },
  // Element Plus 按需引入（unplugin 两插件）+ 页面标题（HtmlWebpackPlugin 的 title，
  // 标题在 public/index.html 里通过 <%= htmlWebpackPlugin.options.title %> 取值）
  chainWebpack: (config) => {
    config.plugin('auto-import').use(AutoImport({ resolvers: [ElementPlusResolver()] }))
    config.plugin('components').use(Components({ resolvers: [ElementPlusResolver()] }))
    config.plugin('html').tap((args) => {
      args[0].title = '人力资源管理平台'
      return args
    })
  },
})
