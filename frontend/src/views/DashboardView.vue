<script setup>
import { ref, onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getTransactions,
  createTransaction,
  getCategories,
  getAccounts,
  deleteTransaction,
} from '@/api/transactionApi'

const router = useRouter()

// --- 資料狀態 ---
const transactions = ref([]) //交易存放列表
const categoryOptions = ref([]) // 分類選單
const accountOptions = ref([]) // 帳戶選單

// --- UI 狀態 ---
const loading = ref(false) //控制 Element Plus Table 的 v-loading 動畫
const submitLoading = ref(false) // 送出按鈕轉圈圈
const dialogVisible = ref(false) // 控制新增視窗開關

// --- 新增交易表單 ---
const form = reactive({
  amount: 0,
  transactionDate: new Date(), // 預設今天
  notes: '',
  categoryId: '',
  accountId: '',
})

// --- 核心：初始化資料 (一次抓全部) ---
const fetchTransactions = async () => {
  loading.value = true
  try {
    // 同時發出三個請求，等待全部完成
    const [transRes, catRes, accRes] = await Promise.all([
      getTransactions(),
      getCategories(),
      getAccounts(),
    ])

    // 1. 填入交易列表
    transactions.value = transRes.data

    // 2. 填入分類選單 (轉換格式：id -> value, name -> label)
    categoryOptions.value = catRes.data.map((item) => ({
      label: item.name,
      value: item.id,
    }))

    // 3. 填入帳戶選單
    accountOptions.value = accRes.data.map((item) => ({
      label: item.name,
      value: item.id,
    }))
  } catch (error) {
    console.error(error)
    ElMessage.error('資料載入失敗，請檢查後端連線')
  } finally {
    loading.value = false
  }
}

// --- 動作：送出新增 ---
const handleCreate = async () => {
  // 簡單驗證
  if (form.amount <= 0 || !form.categoryId || !form.accountId) {
    ElMessage.warning('請填寫金額、分類與帳戶')
    return
  }

  submitLoading.value = true
  try {
    // 呼叫後端新增 API
    await createTransaction({
      amount: form.amount,
      transactionDate: form.transactionDate,
      notes: form.notes,
      categoryId: form.categoryId,
      accountId: form.accountId,
    })

    ElMessage.success('新增成功！')
    dialogVisible.value = false // 關閉視窗

    // 重新抓取交易列表 (更新畫面)
    const res = await getTransactions()
    transactions.value = res.data

    // 重置表單金額與備註 (保留分類與帳戶，方便連續記帳)
    form.amount = 0
    form.notes = ''
  } catch (error) {
    console.error(error)
    ElMessage.error('新增失敗')
  } finally {
    submitLoading.value = false
  }
}

// --- 刪除邏輯 ---
const handleDelete = (id) => {
  ElMessageBox.confirm('確定要刪除這筆記帳資料嗎？刪除後無法復原。', '警告', {
    confirmButtonText: '確定刪除',
    cancelButtonText: '取消',
    type: 'warning',
  })
    .then(async () => {
      // 使用者按了「確定」
      loading.value = true
      try {
        await deleteTransaction(id) // 呼叫 API
        ElMessage.success('刪除成功')

        // 重新抓取列表 (更新畫面)
        // 優化：其實也可以直接用 JS 從 transactions.value 陣列裡移除該筆資料，省一次流量
        // 但為了確保資料一致性，重新 fetch 是最穩的做法
        await fetchTransactions()
      } catch (error) {
        console.error(error)
        ElMessage.error('刪除失敗')
      } finally {
        loading.value = false
      }
    })
    .catch(() => {
      // 使用者按了「取消」，什麼都不做
    })
}
// --- 動作：登出 ---
const handleLogout = () => {
  // 1. 清除 Token
  localStorage.removeItem('jwt_token')
  // 2. 顯示通知
  ElMessage.success('已登出')
  // 3. 跳回登入頁
  router.push('/login')
}

// 頁面載入時執行
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
          <div>
            <el-button type="primary" @click="dialogVisible = true"> + 新增交易 </el-button>
            <el-button type="danger" size="small" @click="handleLogout">登出</el-button>
          </div>
        </div>
      </template>
      <el-table
        :data="transactions"
        v-loading="loading"
        style="width: 100%"
        empty-text="目前沒有記帳資料"
        ><el-table-column prop="transactionDate" label="日期" width="120">
          <template #default="scope">
            {{ new Date(scope.row.transactionDate).toLocaleDateString() }}
          </template>
        </el-table-column>

        <el-table-column prop="categoryName" label="分類" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.categoryType === 'Expense' ? 'danger' : 'success'">
              {{ scope.row.categoryName }}
            </el-tag>
          </template>
        </el-table-column>

        <el-table-column prop="accountName" label="帳戶" width="120" />

        <el-table-column prop="notes" label="備註" />

        <el-table-column prop="amount" label="金額" width="120" align="right">
          <template #default="scope">
            <span
              :style="{
                color: scope.row.categoryType === 'Expense' ? '#F56C6C' : '#67C23A',
                fontWeight: 'bold',
              }"
            >
              $ {{ scope.row.amount }}
            </span>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" align="center">
          <template #default="scope">
            <el-button link type="danger" size="small" @click="handleDelete(scope.row.id)">
              刪除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" title="新增一筆交易" width="500px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="日期">
          <el-date-picker
            v-model="form.transactionDate"
            type="date"
            placeholder="選擇日期"
            style="width: 100%"
          />
        </el-form-item>

        <el-form-item label="分類">
          <el-select v-model="form.categoryId" placeholder="請選擇分類" style="width: 100%">
            <el-option
              v-for="item in categoryOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>

        <el-form-item label="帳戶">
          <el-select v-model="form.accountId" placeholder="請選擇帳戶" style="width: 100%">
            <el-option
              v-for="item in accountOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>

        <el-form-item label="金額">
          <el-input-number v-model="form.amount" :min="1" style="width: 100%" />
        </el-form-item>

        <el-form-item label="備註">
          <el-input v-model="form.notes" placeholder="早餐、薪水..." />
        </el-form-item>
      </el-form>

      <template #footer>
        <span class="dialog-footer">
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleCreate" :loading="submitLoading">
            確認新增
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<style scoped>
.dashboard-container {
  padding: 20px;
  display: flex;
  justify-content: center;
}
.box-card {
  width: 900px;
  max-width: 100%;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
