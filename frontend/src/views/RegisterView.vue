<script setup>
import { ref } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'
import { useRouter } from 'vue-router'

const router = useRouter()
const loading = ref(false)
const form = ref({
  email: '',
  displayName: '',
  password: '',
})

const handleRegister = async () => {
  // 簡單的前端驗證
  if (!form.value.email || !form.value.password || !form.value.displayName) {
    ElMessage.warning('請填寫所有欄位')
    return
  }

  loading.value = true
  try {
    // --- 發送 API 請求 ---
    const response = await axios.post('https://localhost:7195/api/Auth/register', {
      email: form.value.email,
      displayName: form.value.displayName,
      password: form.value.password,
    })

    // --- 成功處理 ---
    ElMessage.success(response.data.message || '註冊成功！')

    // 跳轉到登入頁
    router.push('/login')
  } catch (error) {
    // --- 失敗處理 ---
    console.error(error)
    // 抓取後端回傳的錯誤訊息
    const msg = error.response?.data?.message || '註冊發生錯誤'
    const detail = error.response?.data?.detail || ''
    ElMessage.error(`${msg} ${detail}`)
  } finally {
    // 不管成功失敗，都要把 loading 關掉
    loading.value = false
  }
}

// --- Google 登入/註冊邏輯 (跟 LoginView 一模一樣) ---
const handleGoogleLogin = async (response) => {
  const idToken = response.credential
  loading.value = true
  try {
    // 呼叫同一個 API
    const apiResponse = await axios.post('https://localhost:7195/api/Auth/google-login', {
      idToken: idToken,
    })

    // 拿到 Token，直接存起來並登入
    const ourToken = apiResponse.data.token
    localStorage.setItem('jwt_token', ourToken)

    ElMessage.success('Google 註冊/登入成功！')
    router.push('/') // 直接進首頁，不用再登入一次
  } catch (error) {
    console.error(error)
    ElMessage.error('Google 驗證失敗')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="register-container">
    <el-card class="register-card">
      <template #header>
        <div class="card-header">
          <h2>註冊帳號</h2>
        </div>
      </template>

      <el-form :model="form" label-width="80px">
        <el-form-item label="Email">
          <el-input v-model="form.email" placeholder="請輸入 Email" />
        </el-form-item>

        <el-form-item label="暱稱">
          <el-input v-model="form.displayName" placeholder="請輸入顯示名稱" />
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
          <el-button type="primary" @click="handleRegister" :loading="loading" class="w-100">
            註冊
          </el-button>
          <el-button @click="$router.push('/login')"> 去登入 </el-button>
        </el-form-item>
      </el-form>
      <el-divider>或是</el-divider>
      <div class="google-btn-container">
        <GoogleLogin :callback="handleGoogleLogin" />
      </div>
    </el-card>
  </div>
</template>

<style scoped>
.register-container {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh; /* 讓登入框垂直置中 */
  background-color: #f0f2f5; /* 淺灰背景色 */
}
.register-card {
  width: 400px;
  max-width: 90%;
}
.card-header {
  text-align: center;
}
/* 讓按鈕有點間距 */
.el-button + .el-button {
  margin-left: 10px;
}
.google-btn-container {
  display: flex;
  justify-content: center;
  margin-top: 20px;
}
</style>
