import React from 'react';
import { useBestSellingProducts } from '../../hooks/useProducts';
import ProductCard from '../../components/ProductCard';

function BestSellingProducts() {
    const { data: products = [], isLoading } = useBestSellingProducts(3);

    if (isLoading) {
        return (
            <section className="py-stack-lg md:py-[80px] px-margin-mobile md:px-margin-desktop max-w-container-max mx-auto w-full">
                <div className="text-center mb-xl">
                    <h2 className="font-display-lg text-headline-md uppercase tracking-tight mb-sm">Bán Chạy Nhất</h2>
                    <p className="text-secondary font-body-md max-w-xl mx-auto">Những thiết kế được yêu thích nhất, đích thân tuyển chọn cho bạn.</p>
                    <div className="w-12 h-0.5 bg-primary mx-auto mt-md animate-pulse"></div>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-gutter">
                    {[1, 2, 3].map((i) => (
                        <div key={i} className="animate-pulse bg-surface-container rounded-lg h-96" />
                    ))}
                </div>
            </section>
        );
    }

    return (
        <section className="py-stack-lg md:py-[80px] px-margin-mobile md:px-margin-desktop max-w-container-max mx-auto w-full">
            <div className="text-center mb-xl">
                <h2 className="font-display-lg text-headline-md uppercase tracking-tight mb-sm">Bán Chạy Nhất</h2>
                <p className="text-secondary font-body-md max-w-xl mx-auto">Những thiết kế được yêu thích nhất, đích thân tuyển chọn cho bạn.</p>
                <div className="w-12 h-0.5 bg-primary mx-auto mt-md"></div>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-gutter">
                {products.map((product: any) => (
                    <ProductCard key={product.id} item={product} variant="featured" />
                ))}
                {products.length === 0 && (
                    <div className="col-span-full text-center py-20 bg-surface-container-low border border-dashed border-outline-variant">
                        <span className="material-symbols-outlined text-4xl text-outline mb-md">whatshot</span>
                        <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">Chưa có dữ liệu bán chạy</p>
                    </div>
                )}
            </div>
        </section>
    );
}

export default BestSellingProducts;

