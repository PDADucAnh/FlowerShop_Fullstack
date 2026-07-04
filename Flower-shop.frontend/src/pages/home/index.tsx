import React from 'react';
import HeroBanner from './HeroBanner';
import BestSellingProducts from './BestSellingProducts';
import ProductGrid from './ProductGrid';
import LatestBlog from './LatestBlog';
import { useRealtimeUpdates } from '../../hooks/useRealtimeUpdates';

function Home() {
    useRealtimeUpdates();

    return (
        <div className="bg-surface text-on-surface font-body-md antialiased min-h-screen flex flex-col pt-20">
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

