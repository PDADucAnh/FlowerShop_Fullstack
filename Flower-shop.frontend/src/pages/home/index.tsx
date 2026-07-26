import React from 'react';
import HeroBanner from './HeroBanner';
import BestSellingProducts from './BestSellingProducts';
import ProductGrid from './ProductGrid';
import LatestBlog from './LatestBlog';
import SEO from '../../components/SEO';
import { useScrollReveal } from '../../hooks/useScrollReveal';

function ScrollSection({ children }: { children: React.ReactNode }) {
  const { ref, isVisible } = useScrollReveal<HTMLDivElement>();
  return (
    <div
      ref={ref}
      className={`transition-all duration-700 ${
        isVisible ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-4'
      }`}
    >
      {children}
    </div>
  );
}

function Home() {
  return (
    <div className="bg-surface text-on-surface font-body-md antialiased min-h-screen flex flex-col pt-20">
      <SEO title="Trang chủ" description="Cửa hàng hoa tươi PDA Flower - Hoa tươi mỗi ngày" />
      <main className="flex-grow flex flex-col">
        <HeroBanner />
        <ScrollSection><BestSellingProducts /></ScrollSection>
        <ScrollSection><ProductGrid categoryId={null} /></ScrollSection>
        <ScrollSection><LatestBlog /></ScrollSection>
      </main>
    </div>
  );
}

export default Home;
