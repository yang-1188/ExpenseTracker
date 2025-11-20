<script setup>
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'

const router = useRouter()

// --- 登出邏輯 ---
const handleLogout = () => {
  // 1. 清除 Token
  localStorage.removeItem('jwt_token')

  // 2. 顯示通知
  ElMessage.success('已登出')

  // 3. 跳回登入頁
  router.push('/login')
}
</script>

<template>
  <div class="dashboard-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <h2>歡迎回來！</h2>
        </div>
      </template>

      <div class="content">
        <p>你已經成功登入系統。</p>
        <p>這裡之後會顯示你的記帳列表。</p>

        <el-divider />

        <el-button type="danger" @click="handleLogout"> 登出系統 </el-button>
      </div>
    </el-card>
  </div>
</template>

<style scoped>
.dashboard-container {
  padding: 40px;
  display: flex;
  justify-content: center;
}
.box-card {
  width: 600px;
}
.content {
  text-align: center;
  font-size: 1.2rem;
  line-height: 2;
}
</style>
