import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { getImageUrl } from '../utils/apiUtils';
import type { Post } from '../types/post';

interface PostCardProps {
  post: Post;
}

const PostCard: React.FC<PostCardProps> = ({ post }) => {
    const imageUrl = getImageUrl(post.imageUrl);
    const navigate = useNavigate();

    const handleCardClick = () => {
        navigate(`/blog/${post.id}`);
    };

    return (
        <div 
            onClick={handleCardClick}
            className="group cursor-pointer flex flex-col h-full"
        >
            <div className="aspect-[16/9] overflow-hidden rounded-xl petal-shadow mb-4 relative">
                <Link to={`/blog/${post.id}`} onClick={(e) => e.stopPropagation()}>
                    <img 
                        src={imageUrl}
                        className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105" 
                        alt={post.title}
                        loading="lazy"
                    />
                </Link>
            </div>
            
            <span className="font-label-sm text-label-sm text-primary uppercase tracking-widest mb-2 block">
                {post.categoryName || "Mẹo Thiết Kế"}
            </span>

            <h3 className="font-headline-sm text-headline-sm text-on-surface mb-2">
                <Link 
                    to={`/blog/${post.id}`} 
                    className="no-underline text-on-surface hover:text-primary transition-colors duration-300"
                    onClick={(e) => e.stopPropagation()}
                >
                    {post.title}
                </Link>
            </h3>

            <p className="font-body-md text-body-md text-on-surface-variant mb-4 line-clamp-3">
                {post.summary || 'Khám phá mẹo phong cách và xu hướng thời trang để nâng tầm phong cách của bạn...'}
            </p>

            <div className="mt-auto" onClick={(e) => e.stopPropagation()}>
                <Link 
                    className="text-primary font-label-md hover:underline decoration-primary/30 transition-all no-underline" 
                    to={`/blog/${post.id}`}
                >
                    Đọc Thêm
                </Link>
            </div>
        </div>
    );
}

export default PostCard;
