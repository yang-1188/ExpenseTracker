// frontend/src/api/transactionApi.js
//.資料夾裡有 index.js，就會自動讀取
import service from '.'

export const getTransactions = () => {
  return service.get('/api/Transactions')
}

export const createTransaction = (data) => {
  return service.post('/api/Transactions', data)
}

// --- 👇 新增這兩個函式 ---
export const getCategories = () => {
  return service.get('/api/Categories')
}

export const getAccounts = () => {
  return service.get('/api/Accounts')
}
