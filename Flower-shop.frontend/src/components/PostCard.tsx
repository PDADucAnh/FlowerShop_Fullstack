import React from 'react';
import { Link } from 'react-router-dom';
import { getImageUrl } from '../utils/apiUtils';
import type { Post } from '../types/post';

interface PostCardProps {
  post: Post;
}

const PostCard: React.FC<PostCardProps> = ({ post }) => {
    const imageUrl = getImageUrl(post.imageUrl);

    return (
        <div className="group cursor-pointer flex flex-col h-full">
            <div className="aspect-[4/3] overflow-hidden mb-md bg-surface-container rounded-xl relative">
                <Link to={`/blog/${post.id}`}>
                    <img src={imageUrl} className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105" alt={post.title} />
                </Link>
            </div>
            <div className="flex items-center gap-2 text-outline text-[10px] mb-2 uppercase tracking-widest font-medium">
                <span className="material-symbols-outlined text-sm">calendar_today</span>
                <span>{post.createdDate ? new Date(post.createdDate).toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' }) : ''}</span>
            </div>
            <h3 className="font-headline-sm text-headline-sm text-on-surface mb-sm group-hover:text-primary transition-colors">
                <Link to={`/blog/${post.id}`} className="text-on-surface text-decoration-none hover:text-primary transition-colors">{post.title}</Link>
            </h3>
            <p className="font-body-md text-body-md text-on-surface-variant mb-4 leading-relaxed">
                {post.summary ? `${post.summary.substring(0, 100)}...` : 'Explore the latest in floral design and contemporary romance...'}
            </p>
            <div className="mt-auto">
                <Link to={`/blog/${post.id}`} className="font-label-sm text-label-sm text-primary font-semibold tracking-widest text-decoration-none btn-link-luxury">
                    Read Story
                </Link>
            </div>
        </div>
    );
}

export default PostCard;
