import React from 'react';
import { Link } from 'react-router-dom';
import { usePosts } from '../../hooks/usePosts';
import { getImageUrl } from '../../utils/apiUtils';

function LatestBlog() {
  const { data: posts = [], isLoading } = usePosts();

  if (isLoading) return null;

  const topThreePosts = [...posts]
    .sort((a: any, b: any) => new Date(b.createdDate || b.publishedAt || 0).getTime() - new Date(a.createdDate || a.publishedAt || 0).getTime())
    .slice(0, 3);

  if (topThreePosts.length === 0) return null;

  return (
    <section className="mt-stack-lg px-margin-mobile pb-stack-lg">
      <h3 className="font-headline-md text-headline-md text-on-surface mb-stack-md text-center">
        Câu Chuyện & Cảm Hứng
      </h3>
      <div className="space-y-stack-md max-w-container-max mx-auto">
        {topThreePosts.map((post: any) => {
          const imageUrl = getImageUrl(post.imageUrl || post.thumbnailUrl);
          return (
            <Link
              key={post.id}
              to={`/blog/${post.id}`}
              className="block bg-surface-container-low rounded-xl overflow-hidden petal-shadow no-underline group"
            >
              <div className="h-48 overflow-hidden">
                <img
                  className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                  src={imageUrl}
                  alt={post.title}
                  loading="lazy"
                />
              </div>
              <div className="p-stack-sm">
                <span className="text-primary font-label-sm text-label-sm tracking-widest uppercase mb-1 block">
                  {post.category || 'Inspiration'}
                </span>
                <h4 className="font-headline-sm text-headline-sm text-on-surface mb-2 leading-tight">
                  {post.title}
                </h4>
                <p className="font-body-md text-body-md text-on-surface-variant line-clamp-2">
                  {post.shortDescription || post.excerpt || ''}
                </p>
              </div>
            </Link>
          );
        })}
      </div>
    </section>
  );
}

export default LatestBlog;
