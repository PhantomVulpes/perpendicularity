import { createApp } from 'vue'
import PrimeVue from 'primevue/config'
import './style.css'
import App from './App.vue'
import 'primeicons/primeicons.css'
import 'primevue/resources/themes/lara-light-blue/theme.css'
import 'primevue/resources/primevue.min.css'

const app = createApp(App)

app.use(PrimeVue, {
  ripple: true
})

app.mount('#app')
