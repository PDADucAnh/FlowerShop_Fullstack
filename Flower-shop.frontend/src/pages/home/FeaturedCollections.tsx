import React from 'react';
import { Link } from 'react-router-dom';
import { useProductCategories } from '../../hooks/useCategories';
import { useProducts } from '../../hooks/useProducts';
import { getImageUrl } from '../../utils/apiUtils';

const FeaturedCollections: React.FC = () => {
  const { data: categories = [] } = useProductCategories();
  const { data: products = [] } = useProducts();

  const categoryList = Array.isArray(categories) ? categories : [];
  const productList = Array.isArray(products) ? products : [];

  const topProducts = productList.slice(0, 3);

  return (
    <section className="py-stack-lg md:py-[80px] px-margin-mobile md:px-margin-desktop max-w-container-max mx-auto w-full">
      <div className="flex flex-col md:flex-row justify-between items-end mb-stack-lg">
        <div>
          <h2 className="font-headline-md text-headline-md text-on-surface mb-stack-sm">Featured Collections</h2>
          <p className="font-body-md text-body-md text-on-surface-variant max-w-md">Discover our meticulously curated seasonal selections, designed to evoke emotion and elevate your space.</p>
        </div>
        <Link to="/shop" className="hidden md:inline-flex items-center text-primary font-label-md text-label-md hover:underline decoration-primary/30">
          View All <span className="material-symbols-outlined ml-1 text-[18px]">arrow_forward</span>
        </Link>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-gutter auto-rows-[300px] md:auto-rows-[400px]">
        {topProducts.length > 0 ? (
          <>
            <div className="md:col-span-2 group relative overflow-hidden rounded-xl petal-shadow-hover cursor-pointer bg-surface-container-lowest">
              <Link to={`/product/${topProducts[0].id}`} className="block w-full h-full">
                <img className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
                  src={getImageUrl(topProducts[0].imageUrl)}
                  alt={topProducts[0].name} />
                <div className="absolute inset-0 bg-gradient-to-t from-on-surface/60 to-transparent flex flex-col justify-end p-stack-md">
                  {topProducts[0].categoryProductName && (
                    <span className="inline-block bg-primary-fixed text-on-primary-fixed text-label-sm font-label-sm px-3 py-1 rounded-full w-max mb-2">{topProducts[0].categoryProductName}</span>
                  )}
                  <h3 className="font-headline-sm text-headline-sm text-surface-container-lowest">{topProducts[0].name}</h3>
                  <p className="font-body-md text-body-md text-surface-container-highest mt-1">
                    {topProducts[0].description ? topProducts[0].description.substring(0, 80) : 'Lush, romantic, and breathtakingly voluminous.'}
                  </p>
                </div>
              </Link>
            </div>
            {topProducts[1] && (
              <div className="group relative overflow-hidden rounded-xl petal-shadow-hover cursor-pointer bg-surface-container-lowest">
                <Link to={`/product/${topProducts[1].id}`} className="block w-full h-full">
                  <img className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
                    src={getImageUrl(topProducts[1].imageUrl)}
                    alt={topProducts[1].name} />
                  <div className="absolute inset-0 bg-gradient-to-t from-on-surface/60 to-transparent flex flex-col justify-end p-stack-md">
                    <h3 className="font-headline-sm text-headline-sm text-surface-container-lowest">{topProducts[1].name}</h3>
                    <p className="font-body-md text-body-md text-surface-container-highest mt-1">Modern sculptural elegance.</p>
                  </div>
                </Link>
              </div>
            )}
            {topProducts[2] && (
              <div className="group relative overflow-hidden rounded-xl petal-shadow-hover cursor-pointer bg-surface-container-lowest">
                <Link to={`/product/${topProducts[2].id}`} className="block w-full h-full">
                  <img className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
                    src={getImageUrl(topProducts[2].imageUrl)}
                    alt={topProducts[2].name} />
                  <div className="absolute inset-0 bg-gradient-to-t from-on-surface/60 to-transparent flex flex-col justify-end p-stack-md">
                    <h3 className="font-headline-sm text-headline-sm text-surface-container-lowest">{topProducts[2].name}</h3>
                  </div>
                </Link>
              </div>
            )}
          </>
        ) : (
          categoryList.map((cat: any, index: number) => (
            <div key={cat.id} className={`${index === 0 ? 'md:col-span-2' : ''} group relative overflow-hidden rounded-xl petal-shadow-hover cursor-pointer bg-${index === 3 ? 'secondary-fixed' : 'surface-container-lowest'}`}>
              <div className="absolute inset-0 p-stack-md flex flex-col justify-center items-start z-10">
                <span className="material-symbols-outlined text-primary mb-stack-sm text-[40px]">
                  {index === 0 ? 'local_florist' : index === 1 ? 'yard' : index === 2 ? 'flower' : 'cardiology'}
                </span>
                <h3 className="font-headline-md text-headline-md text-on-surface mb-2">{cat.name}</h3>
                <p className="font-body-md text-body-md text-on-surface-variant max-w-sm mb-stack-sm">{cat.description || 'Discover our curated collection.'}</p>
                <Link to={`/shop?category=${cat.id}`} className="text-primary font-label-md text-label-md border border-primary px-6 py-2 rounded-lg hover:bg-primary hover:text-on-primary transition-colors text-decoration-none">
                  Explore
                </Link>
              </div>
            </div>
          ))
        )}
      </div>

      <Link to="/shop" className="md:hidden inline-flex items-center text-primary font-label-md text-label-md hover:underline decoration-primary/30 mt-stack-md">
        View All Collections <span className="material-symbols-outlined ml-1 text-[18px]">arrow_forward</span>
      </Link>
    </section>
  );
};

export default FeaturedCollections;
