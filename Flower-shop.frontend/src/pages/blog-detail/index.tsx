import React, { useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { usePost } from '../../hooks/usePosts';
import { useProducts } from '../../hooks/useProducts';
import DOMPurify from 'dompurify';
import { getImageUrl } from '../../utils/apiUtils';

const formatCurrency = (value: number): string => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

const BlogDetail: React.FC = () => {
    const { id } = useParams();
    const { data: post, isLoading } = usePost(id as string);
    const { data: allProducts = [] } = useProducts();

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [id]);

    const products = allProducts.slice(0, 4);

    if (isLoading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="animate-pulse flex flex-col items-center">
                    <div className="size-16 bg-surface-container rounded-full mb-md"></div>
                    <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">Retrieving Narrative...</p>
                </div>
            </div>
        );
    }

    if (!post) {
        return (
            <div className="text-center py-xl px-margin-mobile max-w-container-max mx-auto">
                <h2 className="font-headline-md text-headline-md text-secondary uppercase tracking-tighter">Narrative Not Found</h2>
                <Link to="/blog" className="text-primary font-label-sm uppercase tracking-widest mt-4 inline-block text-decoration-none btn-link-luxury">Back to Journal</Link>
            </div>
        );
    }

    const postImage = getImageUrl(post.imageUrl);

    return (
        <div className="bg-surface text-on-surface antialiased font-body-md">
            <main>
                <section className="w-full relative h-[614px] md:h-[819px] flex items-end pb-xl px-margin-mobile md:px-margin-desktop bg-surface-variant overflow-hidden">
                    <img alt={post.title} className="absolute inset-0 w-full h-full object-cover z-0" src={postImage} />
                    <div className="absolute inset-0 bg-gradient-to-t from-primary/80 to-transparent z-10"></div>
                    <div className="relative z-20 max-w-4xl mx-auto text-center w-full">
                        <div className="mb-sm">
                            <span className="font-label-sm text-label-sm text-surface-container-highest uppercase border border-surface-container-highest px-xs py-[2px] rounded">
                                {post.categoryName || 'Editorial'}
                            </span>
                        </div>
                        <h1 className="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-primary mb-md uppercase tracking-tighter drop-shadow-lg">{post.title}</h1>
                        {post.summary && <p className="font-body-lg text-body-lg text-white/80 max-w-2xl mx-auto">{post.summary}</p>}
                        <div className="mt-lg flex items-center justify-center gap-xs text-white/60 font-label-sm text-label-sm uppercase tracking-[0.2em] font-bold">
                            <span>By Editorial Team</span>
                            <span className="mx-2">/</span>
                            <span>{new Date(post.createdDate || Date.now()).toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' })}</span>
                        </div>
                    </div>
                </section>

                <div className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg grid grid-cols-1 md:grid-cols-12 gap-gutter relative">
                    <aside className="md:col-span-1 hidden md:flex flex-col items-center gap-md sticky top-32 h-fit">
                        <button className="p-xs text-secondary hover:text-primary transition-colors bg-transparent border-0 outline-none">
                            <span className="material-symbols-outlined">share</span>
                        </button>
                        <button className="p-xs text-secondary hover:text-primary transition-colors bg-transparent border-0 outline-none">
                            <span className="material-symbols-outlined">bookmark</span>
                        </button>
                        <div className="w-px h-12 bg-outline-variant my-xs"></div>
                        <button className="p-xs text-secondary hover:text-primary transition-colors bg-transparent border-0 outline-none">
                            <span className="material-symbols-outlined">print</span>
                        </button>
                    </aside>

                    <article className="md:col-span-7 lg:col-span-6 md:col-start-3 lg:col-start-4">
                        <div className="prose prose-lg max-w-none">
                            <div
                                className="blog-detail-content font-body-lg text-body-lg text-on-surface-variant mb-lg leading-relaxed first-letter:text-5xl first-letter:font-display-lg first-letter:float-left first-letter:mr-3 first-letter:text-primary"
                                dangerouslySetInnerHTML={{ __html: DOMPurify.sanitize(post.content) }}
                            ></div>
                        </div>

                        <div className="flex flex-wrap gap-sm mt-xl pt-lg border-t border-outline-variant">
                            <span className="font-label-sm text-label-sm text-primary border border-outline px-sm py-xs rounded uppercase tracking-widest font-bold">EDITORIAL</span>
                            <span className="font-label-sm text-label-sm text-primary border border-outline px-sm py-xs rounded uppercase tracking-widest font-bold">LATEST</span>
                            <span className="font-label-sm text-label-sm text-primary border border-outline px-sm py-xs rounded uppercase tracking-widest font-bold">STYLING</span>
                        </div>
                    </article>
                </div>

                <section className="bg-surface-container-low py-stack-lg border-y border-outline-variant">
                    <div className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop">
                        <div className="flex flex-col md:flex-row justify-between items-end mb-xl gap-md">
                            <div>
                                <h3 className="font-headline-md text-headline-md text-on-surface mb-xs uppercase tracking-tighter">Shop The Editorial</h3>
                                <p className="font-body-md text-body-md text-secondary italic">Curated pieces to recreate the layered aesthetic.</p>
                            </div>
                            <Link className="font-label-sm text-label-sm text-primary uppercase tracking-widest text-decoration-none font-bold btn-link-luxury" to="/shop">View All</Link>
                        </div>
                        <div className="grid grid-cols-1 md:grid-cols-4 gap-gutter">
                            {products.map((p: any) => (
                                <Link className="group block text-decoration-none" to={`/product/${p.id}`} key={p.id}>
                                    <div className="aspect-[4/5] bg-surface-container mb-sm overflow-hidden relative rounded-xl">
                                        <img alt={p.name} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700" src={getImageUrl(p.imageUrl)} />
                                        <div className="absolute inset-0 bg-black/5 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                                    </div>
                                    <h4 className="font-label-md text-label-md text-on-surface font-bold tracking-tight mb-1 truncate">{p.name}</h4>
                                    <p className="font-body-md text-body-md text-secondary">{formatCurrency(p.price)}</p>
                                </Link>
                            ))}
                        </div>
                    </div>
                </section>
            </main>
        </div>
    );
};

export default BlogDetail;
