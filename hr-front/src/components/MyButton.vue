<template>
    <button class="my-btn">
        <slot></slot>
    </button>
</template>

<!-- ⑫ 的试验品：验证全局注册后，页面不 import 也能直接用 -->
<script setup>
</script>

<style scoped>
.my-btn {
    padding: 6px 16px;
    background: #409eff;
    color: #fff;
    border: none;
    border-radius: 4px;
    cursor: pointer;
}
.my-btn:hover {
    opacity: .8;
}
</style>
