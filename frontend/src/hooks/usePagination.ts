import { useCallback, useState } from 'react';
import type { PagedResult } from '../types/api';

type FetchPage<T> = (page: number, pageSize: number) => Promise<PagedResult<T>>;

export function usePagination<T>(fetchPage: FetchPage<T>, pageSize = 20) {
  const [items, setItems] = useState<T[]>([]);
  const [page, setPage] = useState(0);
  const [totalCount, setTotalCount] = useState(0);
  const [hasNextPage, setHasNextPage] = useState(false);
  const [loading, setLoading] = useState(false);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const applyResult = useCallback((result: PagedResult<T>, append: boolean) => {
    setItems(prev => (append ? [...prev, ...result.items] : result.items));
    setPage(result.page);
    setTotalCount(result.totalCount);
    setHasNextPage(result.hasNextPage);
  }, []);

  const loadFirst = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await fetchPage(1, pageSize);
      applyResult(result, false);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao carregar dados');
    } finally {
      setLoading(false);
    }
  }, [applyResult, fetchPage, pageSize]);

  const loadMore = useCallback(async () => {
    if (loading || refreshing || !hasNextPage) return;
    setLoading(true);
    setError(null);
    try {
      const result = await fetchPage(page + 1, pageSize);
      applyResult(result, true);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao carregar mais');
    } finally {
      setLoading(false);
    }
  }, [applyResult, fetchPage, hasNextPage, loading, page, pageSize, refreshing]);

  const refresh = useCallback(async () => {
    setRefreshing(true);
    setError(null);
    try {
      const result = await fetchPage(1, pageSize);
      applyResult(result, false);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Erro ao atualizar');
    } finally {
      setRefreshing(false);
    }
  }, [applyResult, fetchPage, pageSize]);

  const totalPages = totalCount > 0 ? Math.ceil(totalCount / pageSize) : 0;

  return {
    items,
    page,
    totalCount,
    totalPages,
    hasNextPage,
    loading,
    refreshing,
    error,
    loadFirst,
    loadMore,
    refresh,
  };
}
