<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { getTransactions } from '@/api/transactionApi'

const router = useRouter()
const transactions = ref([]) //交易存放列表
const loading = ref(false) //控制 Element Plus Table 的 v-loading 動畫

// --- 取得交易列表 ---
const fetchTransactions = async () => {
  loading.value = true
  try {
    const response = await getTransactions()
    transactions.value = response.data
  } catch (error) {
    console.error(error)
    ElMessage.error('無法取得交易資料')
  } finally {
    loading.value = false
  }
}

// --- 登出邏輯 ---
const handleLogout = () => {
  // 1. 清除 Token
  localStorage.removeItem('jwt_token')
  // 2. 顯示通知
  ElMessage.success('已登出')
  // 3. 跳回登入頁
  router.push('/login')
}

onMounted(() => {
  fetchTransactions()
})
</script>

<template>
  <div class="dashboard-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <h2>我的記賬本</h2>
          <el-button type="danger" size="small" @click="handleLogout">登出</el-button>
        </div>
      </template>
      <el-table
        :data="transactions"
        v-loading="loading"
        style="width: 100%"
        empty-text="目前沒有記帳資料"
      >
        <el-table-column prop="transactionDate" label="日期" width="120">
          <template #default="scope">
            {{ new Date(scope.row.transactionDate).toLocaleDateString() }}
          </template>
        </el-table-column>

        <el-table-column prop="categoryName" label="分類" width="100">
          <template #default="scope">
            <el-tag>{{ scope.row.categoryName }}</el-tag>
          </template>
        </el-table-column>

        <el-table-column prop="notes" label="備註" />

        <el-table-column prop="amount" label="金額" width="120" align="right">
          <template #default="scope">
            <span style="font-weight: bold; color: #67c23a"> $ {{ scope.row.amount }} </span>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<style scoped>
.dashboard-container {
  padding: 20px;
  display: flex;
  justify-content: center;
}
.box-card {
  width: 800px;
  max-width: 100%;
}
.card-header {
  display: flex;
  justify-content: space-between; /* 標題和登出按鈕分開 */
  align-items: center;
}
</style>
