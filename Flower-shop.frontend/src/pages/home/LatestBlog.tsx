import React from 'react';
import { usePosts } from '../../hooks/usePosts';
import PostCard from '../../components/PostCard';

function LatestBlog() {
    const { data: posts = [], isLoading } = usePosts();

    if (isLoading) return null;

    const topThreePosts = [...posts].sort((a: any, b: any) => b.id - a.id).slice(0, 3);

    return (
        <section className="py-stack-lg md:py-[100px] bg-surface-container-low border-y border-outline-variant/30">
            <div className="px-margin-mobile md:px-margin-desktop max-w-container-max mx-auto w-full">
                <div className="text-center mb-xl">
                    <h2 className="font-display-lg text-headline-md uppercase tracking-tight mb-sm">Câu Chuyện & Cảm Hứng Hoa</h2>
                    <p className="text-secondary font-body-md max-w-xl mx-auto">Khám phá những hiểu biết mới nhất của chúng tôi về thiết kế hoa, xu hướng theo mùa và nghệ thuật sống cùng hoa.</p>
                    <div className="w-12 h-0.5 bg-primary mx-auto mt-md"></div>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-3 gap-gutter">
                    {topThreePosts.map((item) => (
                        <PostCard key={item.id} post={item} />
                    ))}
                </div>
            </div>
        </section>
    );
}

export default LatestBlog;

