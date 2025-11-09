import { ref, computed, watch, type Ref } from 'vue'

export function usePagination<T>(items: Ref<T[]>, itemsPerPage: number = 20) {
  const currentPage = ref(1)

  const totalPages = computed(() => {
    return Math.ceil(items.value.length / itemsPerPage)
  })

  const paginatedItems = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage
    const end = start + itemsPerPage
    return items.value.slice(start, end)
  })

  const showPagination = computed(() => {
    return items.value.length > itemsPerPage
  })

  // Reset to page 1 when items change (e.g., after CRUD operations)
  watch(() => items.value.length, () => {
    // If current page is beyond total pages, reset to last valid page
    if (currentPage.value > totalPages.value && totalPages.value > 0) {
      currentPage.value = totalPages.value
    } else if (totalPages.value === 0) {
      currentPage.value = 1
    }
  })

  const goToPage = (page: number) => {
    if (page >= 1 && page <= totalPages.value) {
      currentPage.value = page
    }
  }

  const nextPage = () => {
    if (currentPage.value < totalPages.value) {
      currentPage.value++
    }
  }

  const previousPage = () => {
    if (currentPage.value > 1) {
      currentPage.value--
    }
  }

  const resetPagination = () => {
    currentPage.value = 1
  }

  return {
    currentPage,
    totalPages,
    paginatedItems,
    showPagination,
    goToPage,
    nextPage,
    previousPage,
    resetPagination
  }
}
