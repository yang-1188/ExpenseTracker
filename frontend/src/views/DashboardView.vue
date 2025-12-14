<script setup>
import { ref, onMounted, reactive } from 'vue'
import { useRouter } from 'vue-router'
import { Plus } from '@element-plus/icons-vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import {
  getTransactions,
  createTransaction,
  updateTransaction,
  deleteTransaction,
  getCategories,
  getAccounts,
  createCategory,
  createAccount,
} from '@/api/transactionApi'

const router = useRouter()

// --- 資料狀態 ---
const transactions = ref([]) //交易存放列表
const categoryOptions = ref([]) // 分類選單
const accountOptions = ref([]) // 帳戶選單

// --- UI 狀態 ---
const loading = ref(false) //控制 Element Plus Table 的 v-loading 動畫
const submitLoading = ref(false) // 送出按鈕轉圈圈
const subLoading = ref(false) // 控制子視窗(新增分類/帳戶)按鈕轉圈圈
const dialogVisible = ref(false) // 控制主視窗(新增交易)開關

// 子視窗控制 (避免跟主視窗衝突)
const subDialog = reactive({
  category: false,
  account: false,
})

// --- 編輯模式狀態 ---
const isEditMode = ref(false)
const editId = ref(null)

// --- 主表單資料 ---
const form = reactive({
  amount: 0,
  transactionDate: new Date(), // 預設今天
  notes: '',
  categoryId: '',
  accountId: '',
})

// 新增分類/帳戶的暫存表單
const newCategory = reactive({ name: '', type: 'Expense' })
const newAccount = reactive({ name: '', initialBalance: 0 })

// ==========================================
// 資料讀取層 (邏輯分離，方便重複呼叫)
// ==========================================

// 讀取並轉換分類選單
const fetchCategoriesData = async () => {
  const res = await getCategories()
  // 轉換格式: Element Plus Select 需要 label/value
  categoryOptions.value = res.data.map((item) => ({
    label: item.name,
    value: item.id,
    type: item.type, // 保留 type 做顏色判斷
  }))
}

// 讀取並轉換帳戶選單
const fetchAccountsData = async () => {
  const res = await getAccounts()
  accountOptions.value = res.data.map((item) => ({
    label: item.name,
    value: item.id,
  }))
}

// 讀取交易列表
const fetchTransactionsData = async () => {
  const res = await getTransactions()
  transactions.value = res.data
}

// 初始化：一次載入所有資料
const initData = async () => {
  loading.value = true
  try {
    // Promise.all 確保三個請求同時發出，效率最高
    await Promise.all([fetchTransactionsData(), fetchCategoriesData(), fetchAccountsData()])
  } catch (error) {
    console.error(error)
    ElMessage.error('資料載入失敗')
  } finally {
    loading.value = false
  }
}

//新增分類與帳戶
const handleAddCategory = async () => {
  if (!newCategory.name) return ElMessage.warning('請輸入分類名稱')

  subLoading.value = true
  try {
    // 1. 呼叫 API (POST)
    const res = await createCategory(newCategory)

    ElMessage.success('分類新增成功')

    // 2. 關閉視窗並清空
    subDialog.category = false
    newCategory.name = ''
    newCategory.type = 'Expense' // 重置回預設

    // 3. 只重新刷新分類選單 (不用重刷整個頁面)
    await fetchCategoriesData()

    // 4. 自動選取剛剛新增的項目 (提升體驗)
    form.categoryId = res.data.id
  } catch (error) {
    console.error(error)
    ElMessage.error('新增分類失敗')
  } finally {
    subLoading.value = false
  }
}

const handleAddAccount = async () => {
  if (!newAccount.name) return ElMessage.warning('請輸入帳戶名稱')

  subLoading.value = true
  try {
    const res = await createAccount(newAccount)

    ElMessage.success('帳戶新增成功')

    subDialog.account = false
    newAccount.name = ''

    // 只重新刷新帳戶選單
    await fetchAccountsData()

    // 自動選取
    form.accountId = res.data.id
  } catch (error) {
    console.error(error)
    ElMessage.error('新增帳戶失敗')
  } finally {
    subLoading.value = false
  }
}

// 主功能：交易 CRUD

// --- 打開「新增」視窗 ---
const openCreateDialog = () => {
  isEditMode.value = false // 設定為新增模式
  editId.value = null

  // 清空表單
  form.amount = 0
  form.transactionDate = new Date()
  form.notes = ''
  form.categoryId = ''
  form.accountId = ''

  dialogVisible.value = true
}

// --- 打開「編輯」視窗 (資料回填) ---
const handleEdit = (row) => {
  isEditMode.value = true // 設定為編輯模式
  editId.value = row.id // 記住這筆資料的 ID

  // 回填表單資料
  form.amount = row.amount
  form.notes = row.notes
  form.transactionDate = new Date(row.transactionDate) // 轉成 Date 物件
  form.categoryId = row.categoryId
  form.accountId = row.accountId

  dialogVisible.value = true
}

// --- 送出表單 (同時處理 新增 & 編輯) ---
const handleSubmit = async () => {
  if (form.amount <= 0 || !form.categoryId || !form.accountId) {
    ElMessage.warning('請填寫金額、分類與帳戶')
    return
  }

  submitLoading.value = true
  try {
    const payload = {
      amount: form.amount,
      transactionDate: form.transactionDate,
      notes: form.notes,
      categoryId: form.categoryId,
      accountId: form.accountId,
    }

    if (isEditMode.value) {
      await updateTransaction(editId.value, payload)
      ElMessage.success('更新成功！')
    } else {
      await createTransaction(payload)
      ElMessage.success('新增成功！')
    }

    dialogVisible.value = false // 關閉視窗
    await fetchTransactionsData() // 重抓列表
  } catch (error) {
    console.error(error)
    ElMessage.error(isEditMode.value ? '更新失敗' : '新增失敗')
  } finally {
    submitLoading.value = false
  }
}

// --- 刪除邏輯 ---
const handleDelete = (id) => {
  ElMessageBox.confirm('確定要刪除這筆記帳資料嗎？無法復原。', '警告', {
    confirmButtonText: '確定刪除',
    cancelButtonText: '取消',
    type: 'warning',
  }).then(async () => {
    loading.value = true
    try {
      await deleteTransaction(id)
      ElMessage.success('刪除成功')
      await fetchTransactionsData()
    } catch {
      ElMessage.error('刪除失敗')
    } finally {
      loading.value = false
    }
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
  initData()
})
</script>

<template>
  <div class="dashboard-container">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <h2>我的記賬本</h2>
          <div>
            <el-button type="primary" @click="openCreateDialog()"> + 新增交易 </el-button>
            <el-button type="danger" size="small" @click="handleLogout">登出</el-button>
          </div>
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

        <el-table-column prop="categoryName" label="分類" width="120">
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

        <el-table-column label="操作" width="150" align="center">
          <template #default="scope">
            <el-button type="primary" size="small" @click="handleEdit(scope.row)">編輯</el-button>
            <el-button type="danger" size="small" @click="handleDelete(scope.row.id)"
              >刪除</el-button
            >
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="isEditMode ? '編輯交易' : '新增交易'" width="500px">
      <el-form :model="form" label-width="80px">
        <el-form-item label="日期">
          <el-date-picker
            v-model="form.transactionDate"
            type="date"
            placeholder="選擇日期"
            style="width: 100%"
          />
        </el-form-item>

        <el-form-item label="分類" prop="categoryId">
          <div style="display: flex; width: 100%; gap: 10px">
            <el-select v-model="form.categoryId" placeholder="請選擇分類" style="flex: 1">
              <el-option
                v-for="item in categoryOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              >
                <span style="float: left">{{ item.label }}</span>
                <span style="float: right; color: #8492a6; font-size: 13px">
                  {{ item.type === 'Expense' ? '支出' : '收入' }}
                </span>
              </el-option>
            </el-select>
            <el-button type="primary" :icon="Plus" circle @click="subDialog.category = true" />
          </div>
        </el-form-item>

        <el-form-item label="帳戶" prop="accountId">
          <div style="display: flex; width: 100%; gap: 10px">
            <el-select v-model="form.accountId" placeholder="請選擇帳戶" style="flex: 1">
              <el-option
                v-for="item in accountOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
            <el-button type="success" :icon="Plus" circle @click="subDialog.account = true" />
          </div>
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
          <el-button type="primary" @click="handleSubmit" :loading="submitLoading">
            {{ isEditMode ? '確認修改' : '確認新增' }}
          </el-button>
        </span>
      </template>
    </el-dialog>

    <el-dialog v-model="subDialog.category" title="新增分類" width="400px" append-to-body>
      <el-form :model="newCategory" label-width="80px">
        <el-form-item label="名稱">
          <el-input v-model="newCategory.name" placeholder="例如：宵夜、獎金" />
        </el-form-item>
        <el-form-item label="類型">
          <el-radio-group v-model="newCategory.type">
            <el-radio label="Expense" value="Expense">支出</el-radio>
            <el-radio label="Income" value="Income">收入</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="subDialog.category = false">取消</el-button>
          <el-button type="primary" @click="handleAddCategory" :loading="subLoading"
            >確定新增</el-button
          >
        </span>
      </template>
    </el-dialog>

    <el-dialog v-model="subDialog.account" title="新增帳戶" width="400px" append-to-body>
      <el-form :model="newAccount" label-width="80px">
        <el-form-item label="名稱">
          <el-input v-model="newAccount.name" placeholder="例如：私房錢、股票" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="subDialog.account = false">取消</el-button>
          <el-button type="success" @click="handleAddAccount" :loading="subLoading"
            >確定新增</el-button
          >
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
