// fronten/src/api/index.js
import axios from 'axios'

// 1. 建立一個 Axios 實例 (Instance)
//    這樣我們就不會汙染到全域的 axios
const service = axios.create({
  // 設定後端 API 的基礎網址
  baseURL: 'https://localhost:7195',
  timeout: 5000, // 請求超過 5 秒就算失敗
})

// 2. 請求攔截器 (Request Interceptor)
//    在「發送請求之前」會執行這裡的程式碼
service.interceptors.request.use(
  (config) => {
    // 從 LocalStorage 拿出 Token
    const token = localStorage.getItem('jwt_token')

    // 如果有 Token，就把它加到 Header 裡
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

// 3. 回應攔截器 (Response Interceptor)
//    在「收到回應之後」會執行這裡的程式碼
service.interceptors.response.use(
  (response) => {
    // 如果成功，直接回傳結果
    return response
  },
  (error) => {
    // 如果失敗 (例如 Token 過期，後端回傳 401)
    if (error.response && error.response.status === 401) {
      // 自動清除無效 Token
      localStorage.removeItem('jwt_token')
      // 強制跳轉回登入頁 (這裡用原生 location 比較簡單，不用引入 router)
      window.location.href = '/login'
    }
    return Promise.reject(error)
  },
)

// 匯出這個設定好的 axios
export default service
