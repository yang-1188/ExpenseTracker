<script setup>
import { ref } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'
import { useRouter } from 'vue-router'

const router = useRouter()
const loading = ref(false)

// 表單資料 (登入只需要 Email 和 Password)
const form = ref({
  email: '',
  password: '',
})

const handleLogin = async () => {
  if (!form.value.email || !form.value.password) {
    ElMessage.warning('請填寫所有欄位')
    return
  }

  loading.value = true
  try {
    // --- 發送 API 請求 (改為 /login) ---
    const response = await axios.post('https://localhost:7195/api/Auth/login', {
      email: form.value.email,
      password: form.value.password,
    })

    // --- 成功處理 ---
    // 1. 拿到 Token
    const token = response.data.token

    // 2. (重要) 把 Token 存到瀏覽器的 LocalStorage，這樣重新整理才不會登出
    localStorage.setItem('jwt_token', token)

    ElMessage.success('登入成功！')

    // 3. 跳轉到首頁 (或是之後的 Dashboard)
    router.push('/')
  } catch (error) {
    console.error(error)
    // 顯示錯誤 (這裡會顯示後端回傳的 "Invalid credentials" 或中文錯誤)
    const msg = error.response?.data?.message || '登入失敗'
    ElMessage.error(msg)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-container">
    <el-card class="login-card">
      <template #header>
        <div class="card-header">
          <h2>會員登入</h2>
        </div>
      </template>

      <el-form :model="form" label-width="80px">
        <el-form-item label="Email">
          <el-input v-model="form.email" placeholder="請輸入 Email" />
        </el-form-item>

        <el-form-item label="密碼">
          <el-input
            v-model="form.password"
            type="password"
            placeholder="請輸入密碼"
            show-password
          />
        </el-form-item>

        <el-form-item>
          <el-button type="primary" @click="handleLogin" :loading="loading" class="w-100">
            登入
          </el-button>
          <el-button @click="$router.push('/register')"> 去註冊 </el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f0f2f5;
}
.login-card {
  width: 400px;
  max-width: 90%;
}
.card-header {
  text-align: center;
}
.el-button + .el-button {
  margin-left: 10px;
}
</style>
