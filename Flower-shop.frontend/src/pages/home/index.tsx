import React from 'react';
import HeroBanner from './HeroBanner';
import BestSellingProducts from './BestSellingProducts';
import ProductGrid from './ProductGrid';
import LatestBlog from './LatestBlog';
import SEO from '../../components/SEO';

function Home() {

    return (
        <div className="bg-surface text-on-surface font-body-md antialiased min-h-screen flex flex-col pt-20">
            <SEO title="Trang chủ" description="Cửa hàng hoa tươi PDA Flower - Hoa tươi mỗi ngày" />
            <main className="flex-grow flex flex-col">
                <HeroBanner />
                <BestSellingProducts />
                <ProductGrid categoryId={null} />
                <LatestBlog />
            </main>
        </div>
    );
}

export default Home;

