// frontend/src/api/transactionApi.js
//.資料夾裡有 index.js，就會自動讀取
import service from '.'

export const getTransactions = () => {
  return service.get('/api/Transactions')
}

export const createTransaction = (data) => {
  return service.post('/api/Transactions', data)
}

export const updateTransaction = (id, data) => {
  return service.put(`/api/Transactions/${id}`, data)
}

export const deleteTransaction = (id) => {
  return service.delete(`/api/Transactions/${id}`)
}
export const getCategories = () => {
  return service.get('/api/Categories')
}

export const createCategory = (data) => {
  return service.post('/api/Categories', data)
}

export const getAccounts = () => {
  return service.get('/api/Accounts')
}

export const createAccount = (data) => {
  return service.post('/api/Accounts', data)
}
