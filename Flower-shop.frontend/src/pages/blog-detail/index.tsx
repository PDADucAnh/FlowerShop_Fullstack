import React, { useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { usePost } from '../../hooks/usePosts';
import { useBestSellingProducts } from '../../hooks/useProducts';
import { useCart } from '../../context/CartContext';
import DOMPurify from 'dompurify';
import { getImageUrl, API_BASE_URL } from '../../utils/apiUtils';
import { formatCurrency } from '../../utils/currency';
import SEO from '../../components/SEO';
import type { Product } from '../../types/product';

const BlogDetail: React.FC = () => {
    const { id } = useParams();
    const { addToCart } = useCart();
    const { data: post, isLoading } = usePost(id as string);
    const { data: recommendedProducts = [] } = useBestSellingProducts(4);

    useEffect(() => {
        window.scrollTo(0, 0);
    }, [id]);

    const decodeHtmlEntities = (str: string): string => {
        const txt = document.createElement('textarea');
        txt.innerHTML = str;
        return txt.value;
    };

    const stripHtml = (html: string): string => {
        if (!html) return '';
        const tempDiv = document.createElement('div');
        tempDiv.innerHTML = html;
        const text = tempDiv.textContent || tempDiv.innerText || '';
        return text.trim();
    };

    const getProcessedContent = (content: string) => {
        if (!content) return '';
        const decoded = decodeHtmlEntities(content);
        const withAbsoluteUrls = decoded.replace(
            /src=["']\/uploads\/(.*?)["']/g,
            `src="${API_BASE_URL}/uploads/$1"`
        );
        return DOMPurify.sanitize(withAbsoluteUrls);
    };

    const products = Array.isArray(recommendedProducts) ? recommendedProducts : [];

    if (isLoading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="animate-pulse flex flex-col items-center">
                    <div className="size-16 bg-surface-container rounded-full mb-md"></div>
                    <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">Đang tải...</p>
                </div>
            </div>
        );
    }

    if (!post) {
        return (
            <div className="text-center py-xl px-margin">
                <h2 className="font-display-xl text-headline-lg uppercase tracking-tighter">Không tìm thấy bài viết</h2>
                <Link to="/blog" className="text-primary font-label-sm uppercase tracking-widest mt-4 inline-block text-decoration-none btn-link-luxury">Quay lại bài viết</Link>
            </div>
        );
    }

    const postImage = getImageUrl(post.imageUrl);

    return (
        <div className="bg-background text-on-background antialiased selection:bg-primary selection:text-on-primary font-body-md pt-20">
            <SEO title="Chi tiết bài viết" description="Chi tiết bài viết" />
            <main>
                <section className="w-full relative h-[614px] md:h-[819px] flex items-end pb-xl px-margin bg-surface-variant overflow-hidden">
                    <img
                        alt={post.title}
                        className="absolute inset-0 w-full h-full object-cover z-0"
                        src={postImage}
                        loading="lazy"
                    />
                    <div className="absolute inset-0 bg-gradient-to-t from-primary/80 to-transparent z-10"></div>
                    <div className="relative z-20 max-w-4xl mx-auto text-center w-full">
                        <div className="mb-sm">
                            <span className="font-label-sm text-label-sm text-surface-container-highest uppercase border border-surface-container-highest px-xs py-[2px] rounded-DEFAULT">
                                {post.categoryName || 'Hướng dẫn phong cách'}
                            </span>
                        </div>
                        <h1 className="font-display-xl text-display-xl-mobile md:text-display-xl text-on-primary mb-md uppercase tracking-tighter drop-shadow-lg">{post.title}</h1>
                        {post.summary && <p className="font-body-lg text-body-lg text-white/80 max-w-2xl mx-auto">{stripHtml(post.summary)}</p>}
                        <div className="mt-lg flex items-center justify-center gap-xs text-white/60 font-label-sm text-[10px] uppercase tracking-[0.2em] font-bold">
                            <span>Ban biên tập</span>
                            <span className="mx-2">/</span>
                            <span>{new Date(post.createdDate || Date.now()).toLocaleDateString('en-US', { day: 'numeric', month: 'short', year: 'numeric' })}</span>
                        </div>
                    </div>
                </section>

                <div className="max-w-[1440px] mx-auto px-margin py-xl md:py-[120px] grid grid-cols-1 md:grid-cols-12 gap-gutter relative">
                    <aside className="md:col-span-1 hidden md:flex flex-col items-center gap-md sticky top-32 h-fit">
                        <button className="p-xs text-secondary hover:text-primary transition-colors duration-300 bg-transparent border-0 outline-none">
                            <span className="material-symbols-outlined icon-fill">share</span>
                        </button>
                        <button className="p-xs text-secondary hover:text-primary transition-colors duration-300 bg-transparent border-0 outline-none">
                            <span className="material-symbols-outlined icon-fill">bookmark</span>
                        </button>
                        <div className="w-px h-12 bg-outline-variant my-xs"></div>
                        <button className="p-xs text-secondary hover:text-primary transition-colors duration-300 bg-transparent border-0 outline-none">
                            <span className="material-symbols-outlined icon-fill">print</span>
                        </button>
                    </aside>

                    <article className="md:col-span-7 lg:col-span-6 md:col-start-3 lg:col-start-4">
                        <div className="prose prose-lg max-w-none">
                            <div
                                className="blog-detail-content font-body-lg text-body-lg text-on-surface-variant mb-lg leading-relaxed first-letter:text-5xl first-letter:font-display-xl first-letter:float-left first-letter:mr-3 first-letter:text-primary"
                                dangerouslySetInnerHTML={{ __html: getProcessedContent(post.content) }}
                            ></div>
                        </div>

                        <div className="flex flex-wrap gap-sm mt-xl pt-lg border-t border-outline-variant">
                            <span className="font-label-sm text-label-sm text-primary border border-outline px-sm py-xs rounded-DEFAULT uppercase tracking-widest font-bold">EDITORIAL</span>
                            <span className="font-label-sm text-label-sm text-primary border border-outline px-sm py-xs rounded-DEFAULT uppercase tracking-widest font-bold">LATEST TRENDS</span>
                            <span className="font-label-sm text-label-sm text-primary border border-outline px-sm py-xs rounded-DEFAULT uppercase tracking-widest font-bold">STYLING</span>
                        </div>
                    </article>
                </div>

                <section className="bg-surface-container-low py-[120px] px-margin border-y border-outline-variant">
                    <div className="max-w-[1440px] mx-auto">
                        <div className="flex flex-col md:flex-row justify-between items-end mb-xl gap-md">
                            <div>
                                <h3 className="font-display-xl text-headline-lg text-primary mb-xs uppercase tracking-tighter">Mua sắm</h3>
                                <p className="font-body-md text-body-md text-secondary italic serif">Sản phẩm phối hợp để tạo phong cách layer.</p>
                            </div>
                            <Link className="font-label-sm text-label-sm text-primary uppercase tracking-widest text-decoration-none font-bold btn-link-luxury" to="/shop">Xem tất cả</Link>
                        </div>
                        <div className="grid grid-cols-1 md:grid-cols-4 gap-gutter">
                            {products.map((p: any) => (
                                <Link className="group block text-decoration-none" to={`/product/${p.id}`} key={p.id}>
                                    <div className="aspect-[4/5] bg-surface mb-sm overflow-hidden relative">
                                        <img
                                            alt={p.name}
                                            className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-700 ease-out"
                                            src={getImageUrl(p.imageUrl)}
                                            loading="lazy"
                                        />
                                        <div className="absolute inset-0 bg-black/5 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                                    </div>
                                    <h4 className="font-body-md text-sm text-primary font-bold uppercase tracking-tight mb-1">{p.name}</h4>
                                    <p className="font-body-md text-sm text-secondary">{formatCurrency(p.price)}</p>
                                </Link>
                            ))}
                        </div>
                    </div>
                </section>

                <div className="max-w-[1440px] mx-auto px-margin py-[120px] grid grid-cols-1 md:grid-cols-12 gap-gutter">
                    <div className="md:col-span-5 lg:col-span-4 bg-surface p-xl border border-outline-variant flex flex-col justify-center">
                        <h3 className="font-display-xl text-headline-sm text-primary mb-sm uppercase tracking-tighter">Bản tin</h3>
                        <p className="font-body-md text-body-md text-secondary mb-lg italic serif">Đăng ký để nhận cập nhật bài viết, truy cập sớm bộ sưu tập và mẹo phối đồ.</p>
                        <form className="flex flex-col gap-md">
                            <div className="relative">
                                <input className="block w-full px-0 py-sm text-body-md bg-transparent border-0 border-b border-outline appearance-none focus:outline-none focus:ring-0 focus:border-primary peer transition-colors" id="email" placeholder=" " type="email" />
                                <label className="absolute font-label-sm text-label-sm text-secondary duration-300 transform -translate-y-6 scale-75 top-3 -z-10 origin-[0] peer-focus:left-0 peer-focus:text-primary peer-placeholder-shown:scale-100 peer-placeholder-shown:translate-y-0 peer-focus:scale-75 peer-focus:-translate-y-6 uppercase tracking-widest" htmlFor="email">Địa chỉ Email</label>
                            </div>
                            <button className="w-full bg-primary text-on-primary font-label-sm text-label-sm uppercase py-sm border border-primary font-bold tracking-[0.2em] btn-luxury btn-primary-luxury" type="button">Đăng ký</button>
                        </form>
                    </div>

                    <div className="md:col-span-7 lg:col-span-7 md:col-start-6 lg:col-start-6 mt-xl md:mt-0">
                        <h3 className="font-headline-sm text-headline-sm text-primary mb-lg border-b border-outline-variant pb-sm uppercase tracking-widest">Bình luận</h3>
                        <div className="space-y-lg mb-xl">
                            <p className="text-secondary italic serif">Chưa có bình luận. Hãy chia sẻ ý kiến của bạn bên dưới.</p>
                        </div>
                        <div>
                            <h4 className="font-body-md text-sm font-bold text-primary mb-sm uppercase tracking-widest">Để lại bình luận</h4>
                            <form className="flex flex-col gap-md">
                                <div className="relative">
                                    <textarea className="block w-full px-0 py-sm text-body-md bg-transparent border-0 border-b border-outline appearance-none focus:outline-none focus:ring-0 focus:border-primary peer transition-colors resize-none" id="comment" placeholder=" " rows={4}></textarea>
                                    <label className="absolute font-label-sm text-label-sm text-secondary duration-300 transform -translate-y-6 scale-75 top-3 -z-10 origin-[0] peer-focus:left-0 peer-focus:text-primary peer-placeholder-shown:scale-100 peer-placeholder-shown:translate-y-0 peer-focus:scale-75 peer-focus:-translate-y-6 uppercase tracking-widest" htmlFor="comment">Nội dung</label>
                                </div>
                                <button className="self-start bg-transparent text-primary font-label-sm text-label-sm uppercase py-sm px-lg border border-primary font-bold tracking-[0.2em] btn-luxury btn-outline-luxury" type="button">Gửi bình luận</button>
                            </form>
                        </div>
                    </div>
                </div>
            </main>
        </div>
    );
};

export default BlogDetail;
