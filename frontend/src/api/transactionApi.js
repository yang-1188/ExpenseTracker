// frontend/src/api/transactionApi.js
//.資料夾裡有 index.js，就會自動讀取
import axios from '.' // 引入我們設定好的 axios 實例

// 定義並匯出 API 呼叫函式
export const getTransactions = () => {
  return axios.get('/api/Transactions')
}

export const createTransaction = (data) => {
  return axios.post('/api/Transactions', data)
}
