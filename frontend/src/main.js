import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'

//引入 Google Login 套件
import vue3GoogleLogin from 'vue3-google-login'

import App from './App.vue'
import router from './router'
import './assets/main.css'

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.use(ElementPlus)
//註冊 Google Login
app.use(vue3GoogleLogin, {
  clientId: '666265697749-jl109qu2lb5mv6qas1bff7issuoi67gi.apps.googleusercontent.com',
})
app.mount('#app')
