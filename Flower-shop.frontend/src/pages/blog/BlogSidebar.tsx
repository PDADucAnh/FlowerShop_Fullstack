import React from 'react';
import { Link } from 'react-router-dom';
import { useBlogCategories } from '../../hooks/useCategories';
import { usePosts } from '../../hooks/usePosts';
import type { Category } from '../../types/category';

interface BlogSidebarProps {
  onCategoryChange: (id: number | null) => void;
  activeId: number | null;
}

const BlogSidebar = ({ onCategoryChange, activeId }: BlogSidebarProps) => {
  const { data: categories = [] } = useBlogCategories();
  const { data: allPosts = [] } = usePosts();
  const latestPosts = [...allPosts].sort((a: any, b: any) => b.id - a.id).slice(0, 3);

  return (
    <div className="space-y-xl">
      <div className="space-y-md">
        <h6 className="font-label-sm text-label-sm uppercase tracking-[0.3em] font-bold text-secondary border-b border-outline-variant pb-2">Editorial Pillars</h6>
        <ul className="space-y-sm list-none p-0">
          <li>
            <button
              className={`bg-transparent border-0 p-0 font-label-sm text-label-sm uppercase tracking-widest transition-all ${activeId === null ? 'text-primary font-bold' : 'text-secondary hover:text-primary'}`}
              onClick={() => onCategoryChange(null)}
            >
              The Full Narrative
            </button>
          </li>
          {(categories as Category[]).map((cat) => (
            <li key={cat.id}>
              <button
                className={`bg-transparent border-0 p-0 font-label-sm text-label-sm uppercase tracking-widest transition-all ${activeId === cat.id ? 'text-primary font-bold' : 'text-secondary hover:text-primary'}`}
                onClick={() => onCategoryChange(cat.id)}
              >
                {cat.name}
              </button>
            </li>
          ))}
        </ul>
      </div>

      <div className="space-y-md">
        <h6 className="font-label-sm text-label-sm uppercase tracking-[0.3em] font-bold text-secondary border-b border-outline-variant pb-2">Journal Highlights</h6>
        <div className="space-y-lg">
          {latestPosts.map((p: any) => (
            <div key={p.id} className="group cursor-pointer">
              <Link to={`/blog/${p.id}`} className="font-body-md font-bold uppercase tracking-tight text-on-surface group-hover:text-primary transition-colors text-decoration-none block mb-1 leading-tight">{p.title}</Link>
              <span className="font-label-sm text-label-sm text-outline uppercase tracking-widest">{p.createdDate ? new Date(p.createdDate).toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' }) : ''}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default BlogSidebar;
