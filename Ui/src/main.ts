import { createApp } from 'vue'
import PrimeVue from 'primevue/config'
import ToastService from 'primevue/toastservice'
import Tooltip from 'primevue/tooltip'
import './style.css'
import App from './App.vue'
import router from './router'
import 'primeicons/primeicons.css'
import 'primevue/resources/themes/lara-light-blue/theme.css'
import 'primevue/resources/primevue.min.css'

const app = createApp(App)

app.use(PrimeVue, {
  ripple: true
})

app.use(ToastService)
app.directive('tooltip', Tooltip)

app.use(router)

app.mount('#app')
