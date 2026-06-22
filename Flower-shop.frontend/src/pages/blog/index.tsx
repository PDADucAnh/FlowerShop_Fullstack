import React, { useState } from 'react';
import BlogSidebar from './BlogSidebar';
import PostCard from '../../components/PostCard';
import { usePosts } from '../../hooks/usePosts';
import type { Post } from '../../types/post';

const BlogPage: React.FC = () => {
  const { data: posts = [], isLoading, error } = usePosts();
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(null);

  const filteredPosts = selectedCategoryId
    ? posts.filter((p: Post) => p.categoryId === selectedCategoryId)
    : posts;

  const handleCategoryChange = (id: number | null) => {
    setSelectedCategoryId(id);
  };

  return (
    <div className="bg-surface text-on-surface font-body-md antialiased">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg">
        <header className="mb-stack-lg text-center space-y-md">
            <h3 className="font-label-sm text-label-sm uppercase tracking-[0.3em] text-secondary">Editorial Narrative</h3>
            <h2 className="font-headline-md text-headline-md text-on-surface uppercase tracking-tighter">The Journal</h2>
            <div className="w-12 h-0.5 bg-primary mx-auto"></div>
        </header>

        <div className="flex flex-col lg:flex-row gap-xl">
          <div className="flex-1 order-2 lg:order-1">
            {isLoading ? (
              <div className="text-center py-20">
                <div className="animate-pulse flex flex-col items-center">
                    <div className="size-12 bg-surface-container rounded-full mb-md"></div>
                    <p className="font-label-sm uppercase tracking-widest text-secondary">Retrieving Narratives...</p>
                </div>
              </div>
            ) : error ? (
              <div className="p-lg bg-error-container text-error font-label-sm text-label-sm uppercase tracking-widest font-bold text-center border border-error">Unable to retrieve editorial stories at this time.</div>
            ) : filteredPosts.length === 0 ? (
              <div className="text-center py-20 bg-surface-container-low border border-dashed border-outline-variant">
                <span className="material-symbols-outlined text-4xl text-outline mb-md">article</span>
                <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">No editorial stories found in this pillar.</p>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-xl">
                {filteredPosts.map((post: any) => (
                    <PostCard key={post.id} post={post} />
                ))}
              </div>
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
