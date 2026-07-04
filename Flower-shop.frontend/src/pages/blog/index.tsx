import React, { useState } from 'react';
import BlogSidebar from './BlogSidebar';
import PostCard from '../../components/PostCard';
import Pagination from '../../components/Pagination';
import { usePostsPaged } from '../../hooks/usePosts';
import type { Post } from '../../types/post';

const BlogPage: React.FC = () => {
  const [page, setPage] = useState(1);
  const pageSize = 6;
  const { data: paged, isLoading, error } = usePostsPaged(page, pageSize);
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);

  const posts = paged?.items ?? [];
  const filteredPosts = selectedCategoryId
    ? posts.filter((p: Post) => p.categoryId === selectedCategoryId)
    : posts;

  const handleCategoryChange = (id: number | null) => {
    setSelectedCategoryId(id);
    setPage(1);
  };

  return (
    <div className="bg-background text-on-background font-body-md antialiased pt-20">
      <main className="max-w-[1440px] mx-auto px-margin py-xl">
        <header className="mb-xl text-center space-y-md">
            <h3 className="text-label-sm uppercase tracking-[0.3em] text-secondary">Bài viết</h3>
            <h2 className="font-display-xl text-display-xl uppercase tracking-tighter text-primary">Bài viết thời trang</h2>
            <div className="w-12 h-0.5 bg-primary mx-auto"></div>
        </header>

        <div className="flex flex-col lg:flex-row gap-xl">
          <div className="flex-1 order-2 lg:order-1">
            {isLoading ? (
              <div className="text-center py-20">
                <div className="animate-pulse flex flex-col items-center">
                    <div className="size-12 bg-surface-container rounded-full mb-md"></div>
                    <p className="text-label-sm uppercase tracking-widest text-secondary">Đang tải...</p>
                </div>
              </div>
            ) : error ? (
              <div className="p-lg bg-error-container text-error text-label-sm uppercase tracking-widest font-bold text-center border border-error">Không thể tải bài viết vào lúc này.</div>
            ) : filteredPosts.length === 0 ? (
              <div className="text-center py-20 bg-surface-container-low border border-dashed border-outline-variant">
                <span className="material-symbols-outlined text-4xl text-outline mb-md">article</span>
                <p className="text-label-sm uppercase tracking-widest text-secondary">Không tìm thấy bài viết trong danh mục này.</p>
              </div>
            ) : (
              <>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-xl">
                  {filteredPosts.map((post: any) => (
                      <PostCard key={post.id} post={post} />
                  ))}
                </div>
                {!selectedCategoryId && paged && paged.totalPages > 1 && (
                  <Pagination
                    page={paged.page}
                    totalPages={paged.totalPages}
                    onPageChange={setPage}
                  />
                )}
              </>
            )}
          </div>

          <aside className="w-full lg:w-80 flex-shrink-0 order-1 lg:order-2">
            <BlogSidebar onCategoryChange={handleCategoryChange} activeId={selectedCategoryId} />
          </aside>
        </div>
      </main>
    </div>
  );
};

export default BlogPage;
