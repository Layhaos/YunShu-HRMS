// 校验工具箱：组件无关、只放校验函数（数据和弹窗不是它的活）
// validator 三参签名固定：(rule, value, callback)
//   合格 → callback() 空手放行；不合格 → callback(new Error("提示语"))
export const checkPhone = (rule, value, callback) => {
    if (!/^1[3-9]\d{9}$/.test(value)) {
        return callback(new Error("手机号格式不正确"))
    }
    callback()
}
