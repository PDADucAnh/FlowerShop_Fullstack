import { useState, useEffect, useCallback } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { useProduct, useProductsPaged } from '../../hooks/useProducts';
import { useCart } from '../../context/CartContext';
import { useWishlist } from '../../context/WishlistContext';
import { getImageUrl } from '../../utils/apiUtils';
import { formatCurrency } from '../../utils/currency';
import toast from 'react-hot-toast';

const formatImageUrl = (url?: string): string => {
  return getImageUrl(url) || '';
};

const ProductDetailPage = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const { addToCart } = useCart();
  const { isFavorite, toggleFavorite } = useWishlist();
  const { data: product, isLoading } = useProduct(id as string);
  
  const [quantity, setQuantity] = useState(1);
  const [selectedSize, setSelectedSize] = useState<'Classic' | 'Deluxe' | 'Grand'>('Classic');
  const [activeImage, setActiveImage] = useState<string>('');
  const [showVideoModal, setShowVideoModal] = useState(false);
  const [showLightbox, setShowLightbox] = useState(false);
  const [lightboxIndex, setLightboxIndex] = useState(0);

  const videoUrl = "https://www.youtube.com/embed/g20T21s0uVw"; // Aesthetic Flower Care Tutorial

  useEffect(() => {
    setQuantity(1);
    setSelectedSize('Classic');
    window.scrollTo(0, 0);
  }, [id]);

  useEffect(() => {
    if (product) {
      setActiveImage(product.imageUrl || '');
    }
  }, [product]);

  const isOutOfStock = product ? product.stockQuantity === 0 : false;
  const isLowStock = product ? product.stockQuantity > 0 && product.stockQuantity <= 5 : false;

  const sizePriceAdjustment = selectedSize === 'Deluxe' ? 300000 : selectedSize === 'Grand' ? 600000 : 0;
  const basePrice = product ? (product.discountPrice || product.price) : 0;
  const finalPrice = basePrice + sizePriceAdjustment;
  
  const originalPrice = product ? product.price : 0;
  const finalOriginalPrice = originalPrice + sizePriceAdjustment;

  const canAddToCart = product && quantity <= product.stockQuantity && product.stockQuantity > 0;

  const handleQuantityChange = useCallback((delta: number) => {
    if (!product) return;
    setQuantity((prev) => {
      const next = prev + delta;
      if (next < 1) return 1;
      if (next > product.stockQuantity) {
        toast.error(`Trong kho chỉ còn ${product.stockQuantity} sản phẩm`);
        return product.stockQuantity;
      }
      return next;
    });
  }, [product]);

  const handleAddToCart = useCallback(() => {
    if (!product) return;
    if (quantity > product.stockQuantity) {
      toast.error(`Chỉ còn ${product.stockQuantity} sản phẩm trong kho`);
      return;
    }
    const customizedProduct = {
      ...product,
      name: selectedSize !== 'Classic' ? `${product.name} (${selectedSize})` : product.name,
      price: finalPrice
    };
    addToCart(customizedProduct, quantity);
    toast.success(`Đã thêm "${customizedProduct.name}" (x${quantity}) vào giỏ hàng`);
  }, [product, quantity, selectedSize, finalPrice, addToCart]);

  const handleBuyNow = useCallback(() => {
    if (!product) return;
    if (quantity > product.stockQuantity) {
      toast.error(`Chỉ còn ${product.stockQuantity} sản phẩm trong kho`);
      return;
    }
    const customizedProduct = {
      ...product,
      name: selectedSize !== 'Classic' ? `${product.name} (${selectedSize})` : product.name,
      price: finalPrice
    };
    addToCart(customizedProduct, quantity);
    navigate('/checkout');
  }, [product, quantity, selectedSize, finalPrice, addToCart, navigate]);

  // Load related products
  const { data: relatedResult } = useProductsPaged(1, 5, null, null, product?.categoryProductId || null);
  const relatedProducts = relatedResult?.items?.filter((p: any) => p.id !== product?.id).slice(0, 4) || [];

  const galleryImages = product ? [
    product.imageUrl || '',
    "https://lh3.googleusercontent.com/aida-public/AB6AXuB9CddqN2JsuVI2rYrLjfhM9YJEFtmNp6f_UaBbD8XuKJ2qE5FLElGt1sSIizcpFIBWITclUw4cq9Zhpzs1vGtTEpSTzy6UOcn8Uf3146Ih0LPSnin2xvbSLqAc08l1_MwIKWQmPF5wXwQrMQBKupE_0bN9EZ4UW86h9zRflczjzRqvbIbsUFzIELmDwiL61nlxefYQguY7IW2PnRp72LshlZWLRnxebPiOJ0fpdgIhYGXjGLuVtDt4aPlSGy5hNcdAWNjn4O8NxTs",
    "https://lh3.googleusercontent.com/aida-public/AB6AXuARzZ2qpRYpsU19whOUnfBzCxrqw3zwzDMv9zTU5J2TAmeacj0BaWjrlo-IrlQjJBljoQbGoSoIzF21u9dh3bo3b2jHjQy2W8jd31qKC3K7kd0UPuDp3iUuEIUkfpGc1e_GJKTgrk9ZcdqGWOgIDg80Ulq6XjEjJwfpZxB9zidUfXHEwrxinQBnbjR5Cly7HOst4MdMJ_fdSnZ4arKWILEdaahJabulvl9C0Ro1R3yq3W49q7veNH8L0L6P1YrTeExOXmoSnGQXuus",
    "https://lh3.googleusercontent.com/aida-public/AB6AXuA8_xORpxwk86bL3I4VJlBsIa21JwThIYnfT_IvKL09XAhTuhrJHtHVTYz3lv02Uj2dU8HpKpZNGGsh4ULoq3Sf9ZZoFSqsoCcjVd8PdjIl8hPNpARjUw9RlqbsPb4b-tUKUfv8kb8ZMBB-QxF2PzIgAvpINuwnbQKOTvnXgZioxC-lZ1b13_z8DIbBBKuD7TGNLS6RPFGP5zE5X9SyhVB7zS_3FSH5utYQwqGx-gBglQ-m9DBJBpbcNJ_mvsxXQFVdTzgZjW--ZVk"
  ] : [];

  if (isLoading) {
    return (
      <div className="bg-background min-h-screen flex items-center justify-center">
        <div className="animate-pulse flex flex-col items-center">
          <div className="w-8 h-8 border-2 border-primary border-t-transparent rounded-full animate-spin mb-md" role="status">
            <span className="sr-only">Đang tải...</span>
          </div>
          <p className="font-label-sm text-label-sm uppercase tracking-widest text-secondary">Đang tải...</p>
        </div>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="text-center py-xl px-margin">
        <h2 className="font-display-xl text-headline-lg uppercase tracking-tight text-secondary">Không tìm thấy sản phẩm</h2>
        <Link to="/shop" className="text-primary font-label-sm uppercase tracking-widest mt-4 inline-block text-decoration-none btn-link-luxury">Quay lại cửa hàng</Link>
      </div>
    );
  }

  return (
    <div className="bg-background text-on-background font-body-md text-body-md antialiased overflow-x-hidden pt-20">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-stack-lg min-h-screen">
        {/* Breadcrumbs */}
        <nav aria-label="Breadcrumb" className="flex text-label-sm font-label-sm text-on-surface-variant mb-stack-md">
          <ol className="inline-flex items-center space-x-1 md:space-x-2 list-none p-0 m-0">
            <li className="inline-flex items-center">
              <Link className="hover:text-primary transition-colors text-decoration-none" to="/">Trang chủ</Link>
            </li>
            <li>
              <div className="flex items-center">
                <span className="material-symbols-outlined text-sm mx-1">chevron_right</span>
                <Link className="hover:text-primary transition-colors text-decoration-none" to="/shop">Cửa hàng</Link>
              </div>
            </li>
            {product.categoryProductName && (
              <li>
                <div className="flex items-center">
                  <span className="material-symbols-outlined text-sm mx-1">chevron_right</span>
                  <span className="text-secondary">{product.categoryProductName}</span>
                </div>
              </li>
            )}
            <li aria-current="page">
              <div className="flex items-center">
                <span className="material-symbols-outlined text-sm mx-1">chevron_right</span>
                <span className="text-on-surface truncate max-w-[200px] md:max-w-none">{product.name}</span>
              </div>
            </li>
          </ol>
        </nav>

        <div className="grid grid-cols-1 lg:grid-cols-12 gap-margin-desktop lg:gap-gutter items-start">
          {/* Left Column: Image Gallery */}
          <div className="lg:col-span-7 flex flex-col gap-stack-sm">
            <div 
              onClick={() => {
                const currentIdx = galleryImages.indexOf(activeImage);
                setLightboxIndex(currentIdx !== -1 ? currentIdx : 0);
                setShowLightbox(true);
              }}
              className="w-full aspect-[4/5] rounded-xl overflow-hidden shadow-[0_4px_20px_rgba(171,44,93,0.02)] bg-surface-container-low group cursor-zoom-in relative"
            >
              <img
                alt={product.name}
                className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
                src={formatImageUrl(activeImage || product.imageUrl)}
                loading="eager"
              />
              {(isOutOfStock || isLowStock) && (
                <div className={`absolute top-4 left-4 px-3 py-1 font-label-sm text-label-sm uppercase tracking-widest rounded-sm z-10 ${isOutOfStock ? 'bg-error text-on-error' : 'bg-warning text-on-warning'}`}>
                  {isOutOfStock ? 'Hết hàng' : `Chỉ còn ${product.stockQuantity} sản phẩm`}
                </div>
              )}
            </div>
            
            <div className="grid grid-cols-4 gap-stack-sm">
              {galleryImages.slice(0, 3).map((imgUrl, idx) => (
                <div
                  key={idx}
                  onClick={() => {
                    setActiveImage(imgUrl);
                    setLightboxIndex(idx);
                  }}
                  className={`aspect-square rounded-lg overflow-hidden cursor-pointer border-2 transition-colors ${
                    activeImage === imgUrl ? 'border-primary' : 'border-transparent hover:border-outline-variant'
                  }`}
                >
                  <img
                    alt={`${product.name} detail view ${idx + 1}`}
                    className="w-full h-full object-cover"
                    src={formatImageUrl(imgUrl)}
                    loading="lazy"
                  />
                </div>
              ))}
              
              {/* Video Thumbnail (4th Slot) - Clean grey background as mockup */}
              <div
                onClick={() => setShowVideoModal(true)}
                className="aspect-square rounded-lg overflow-hidden cursor-pointer border-2 border-transparent hover:border-outline-variant transition-colors bg-surface-container flex items-center justify-center relative group"
              >
                <span className="material-symbols-outlined text-outline text-3xl z-10 transition-transform group-hover:scale-110">play_circle</span>
              </div>
            </div>
          </div>

          {/* Right Column: Product Details */}
          <div className="lg:col-span-5 flex flex-col pt-2 md:pt-0 sticky top-stack-lg">
            {/* Header badge & Clean Title */}
            <div className="flex items-center gap-2 mb-2">
              <span className="bg-secondary-container text-on-secondary-container px-3 py-1 rounded-full font-label-sm text-label-sm">Bán chạy nhất</span>
              {product.categoryProductName && (
                <span className="bg-surface-container text-on-surface-variant px-3 py-1 rounded-full font-label-sm text-label-sm">{product.categoryProductName}</span>
              )}
            </div>
            
            <h1 className="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-surface mb-2">{product.name}</h1>
            
            <div className="flex items-baseline gap-4 mb-stack-md">
              <p className="font-headline-md text-headline-md text-primary">
                {formatCurrency(finalPrice)}
              </p>
              {product.discountPrice! > 0 && (
                <p className="font-body-md text-body-md text-on-surface-variant line-through opacity-60">
                  {formatCurrency(finalOriginalPrice)}
                </p>
              )}
            </div>

            <div className="flex items-center gap-2 mb-stack-md">
              <span className={`w-2.5 h-2.5 rounded-full ${isOutOfStock ? 'bg-error' : product.stockQuantity <= 10 ? 'bg-warning' : 'bg-success'}`} />
              <span className="font-label-sm text-label-sm text-secondary">
                {isOutOfStock ? 'Hết hàng' : `Còn ${product.stockQuantity} sản phẩm trong kho`}
              </span>
            </div>

            <p className="font-body-md text-body-md text-on-surface-variant mb-stack-lg leading-relaxed">
              {product.description || 'Chưa có mô tả chi tiết cho sản phẩm này.'}
            </p>

            {/* Configuration Options */}
            <div className="space-y-stack-md mb-stack-lg">
              <div>
                <label className="font-label-md text-label-md text-on-surface block mb-base">Chọn Kích thước</label>
                <div className="grid grid-cols-3 gap-2">
                  {(['Classic', 'Deluxe', 'Grand'] as const).map((size) => {
                    const isSelected = selectedSize === size;
                    const adjustmentText = size === 'Deluxe' ? ` (+${formatCurrency(300000)})` : size === 'Grand' ? ` (+${formatCurrency(600000)})` : '';
                    return (
                      <button
                        key={size}
                        onClick={() => setSelectedSize(size)}
                        className={`rounded-lg py-3 text-center transition-all duration-300 font-label-sm text-label-sm hover:scale-[1.02] active:scale-[0.98] ${
                          isSelected
                            ? 'border-2 border-primary bg-surface-container-lowest shadow-[0_4px_20px_rgba(171,44,93,0.08)] text-primary font-semibold'
                            : 'border border-outline-variant hover:border-primary hover:bg-surface-container-low text-on-surface-variant'
                        }`}
                      >
                        {size}{adjustmentText}
                      </button>
                    );
                  })}
                </div>
              </div>
            </div>

            {/* Add to Cart Area */}
            <div className="flex items-center gap-stack-sm mb-stack-lg">
              <div className="flex items-center border border-outline-variant rounded-lg bg-surface-container-lowest h-[52px]">
                <button
                  aria-label="Giảm số lượng"
                  className="px-4 py-2 text-on-surface-variant hover:text-primary transition-colors disabled:opacity-30 disabled:cursor-not-allowed"
                  disabled={quantity <= 1}
                  onClick={() => handleQuantityChange(-1)}
                >
                  <span className="material-symbols-outlined">remove</span>
                </button>
                <input
                  aria-label="Quantity"
                  className="w-12 text-center border-none focus:ring-0 font-body-md text-body-md bg-transparent text-on-surface focus:outline-none"
                  min="1"
                  max={product.stockQuantity}
                  type="number"
                  value={quantity}
                  onChange={(e) => {
                    const val = parseInt(e.target.value, 10);
                    if (isNaN(val) || val < 1) {
                      setQuantity(1);
                    } else if (val > product.stockQuantity) {
                      toast.error(`Chỉ còn ${product.stockQuantity} sản phẩm trong kho`);
                      setQuantity(product.stockQuantity);
                    } else {
                      setQuantity(val);
                    }
                  }}
                />
                <button
                  aria-label="Tăng số lượng"
                  className="px-4 py-2 text-on-surface-variant hover:text-primary transition-colors disabled:opacity-30 disabled:cursor-not-allowed"
                  disabled={quantity >= product.stockQuantity}
                  onClick={() => handleQuantityChange(1)}
                >
                  <span className="material-symbols-outlined">add</span>
                </button>
              </div>

              <button
                className="flex-1 bg-primary text-on-primary h-[52px] rounded-lg font-label-md text-label-md shadow-[0_4px_20px_rgba(171,44,93,0.2)] hover:shadow-[0_8px_30px_rgba(171,44,93,0.3)] hover:-translate-y-0.5 active:translate-y-0 active:scale-[0.98] transition-all flex items-center justify-center gap-2 disabled:opacity-40 disabled:cursor-not-allowed"
                disabled={!canAddToCart}
                onClick={handleAddToCart}
              >
                <span className="material-symbols-outlined">shopping_bag</span>
                {isOutOfStock ? 'Hết hàng' : 'Thêm vào giỏ'}
              </button>

              <button
                onClick={() => toggleFavorite(product)}
                className="h-[52px] w-[52px] border border-outline-variant rounded-lg flex items-center justify-center text-on-surface-variant hover:text-primary hover:border-primary transition-colors bg-surface-container-lowest"
              >
                <span className={`material-symbols-outlined ${isFavorite(product.id) ? 'text-error' : ''}`}>
                  {isFavorite(product.id) ? 'favorite' : 'favorite_border'}
                </span>
              </button>
            </div>

            <button
              disabled={!canAddToCart}
              onClick={handleBuyNow}
              className="w-full h-[52px] bg-transparent text-primary border border-primary hover:bg-primary-container/10 active:scale-[0.98] transition-all rounded-lg font-label-md text-label-md flex items-center justify-center gap-2 disabled:opacity-40 disabled:cursor-not-allowed mb-stack-lg"
            >
              <span className="material-symbols-outlined">flash_on</span>
              Mua ngay
            </button>

            {/* Accordions (Details) */}
            <div className="border-t border-surface-variant divide-y divide-surface-variant">
              <details className="group py-4" open>
                <summary className="flex justify-between items-center font-label-md text-label-md text-on-surface cursor-pointer list-none [&::-webkit-details-marker]:hidden">
                  Chăm sóc &amp; Bảo quản
                  <span className="material-symbols-outlined text-outline transition-transform group-open:rotate-180">expand_more</span>
                </summary>
                <div className="pt-4 font-body-md text-body-md text-on-surface-variant">
                  <ul className="space-y-2 list-none p-0 m-0">
                    <li className="flex items-start gap-2">
                      <span className="material-symbols-outlined text-sm text-primary mt-1">water_drop</span>
                      Cắt cuống hoa góc 45 độ khoảng 1-2 cm khi nhận hoa.
                    </li>
                    <li className="flex items-start gap-2">
                      <span className="material-symbols-outlined text-sm text-primary mt-1">sunny</span>
                      Đặt hoa nơi mát mẻ, tránh ánh nắng trực tiếp và gió lùa.
                    </li>
                    <li className="flex items-start gap-2">
                      <span className="material-symbols-outlined text-sm text-primary mt-1">local_drink</span>
                      Thay nước sạch và rửa bình cắm hoa mỗi 2 ngày một lần.
                    </li>
                  </ul>
                </div>
              </details>
              
              <details className="group py-4">
                <summary className="flex justify-between items-center font-label-md text-label-md text-on-surface cursor-pointer list-none [&::-webkit-details-marker]:hidden">
                  Thông tin Giao hàng
                  <span className="material-symbols-outlined text-outline transition-transform group-open:rotate-180">expand_more</span>
                </summary>
                <div className="pt-4 font-body-md text-body-md text-on-surface-variant">
                  Giao hàng hỏa tốc trong ngày đối với các đơn hàng đặt trước 14:00. Hoa được vận chuyển bằng phương tiện chuyên dụng kiểm soát nhiệt độ để đảm bảo độ tươi nguyên bản khi đến tay khách hàng.
                </div>
              </details>
            </div>
          </div>
        </div>

        {/* Related Products Section */}
        {relatedProducts.length > 0 && (
          <section className="mt-stack-lg pt-stack-lg border-t border-surface-variant">
            <div className="flex justify-between items-end mb-stack-md">
              <h2 className="font-headline-sm text-headline-sm text-on-surface">Sản phẩm tương tự</h2>
              <Link className="font-label-sm text-label-sm text-primary hover:underline hidden sm:block text-decoration-none" to="/shop">
                Xem tất cả
              </Link>
            </div>
            
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-gutter">
              {relatedProducts.map((p: any) => (
                <Link
                  key={p.id}
                  to={`/product/${p.id}`}
                  className="group cursor-pointer text-decoration-none block"
                >
                  <div className="w-full aspect-[4/5] rounded-xl overflow-hidden bg-surface-container-low mb-4 shadow-[0_4px_20px_rgba(171,44,93,0.02)] transition-shadow group-hover:shadow-[0_8px_30px_rgba(171,44,93,0.08)]">
                    <img
                      alt={p.name}
                      className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                      src={formatImageUrl(p.imageUrl)}
                      loading="lazy"
                    />
                  </div>
                  <div className="text-center">
                    <h3 className="font-label-md text-label-md text-on-surface mb-1 group-hover:text-primary transition-colors">{p.name}</h3>
                    <div className="flex justify-center gap-2 items-center">
                      <p className="font-body-md text-body-md text-primary mb-0 font-semibold">
                        {formatCurrency(p.discountPrice || p.price)}
                      </p>
                      {p.discountPrice! > 0 && (
                        <p className="font-body-sm text-body-sm text-secondary line-through mb-0 opacity-60">
                          {formatCurrency(p.price)}
                        </p>
                      )}
                    </div>
                  </div>
                </Link>
              ))}
            </div>
            
            <div className="mt-6 text-center sm:hidden">
              <Link className="font-label-md text-label-md text-primary border border-primary rounded-lg px-6 py-2 inline-block text-decoration-none" to="/shop">
                Xem tất cả
              </Link>
            </div>
          </section>
        )}
      </main>

      {/* Video Care Modal */}
      {showVideoModal && (
        <div className="fixed inset-0 z-[100] bg-black/80 backdrop-blur-md flex items-center justify-center p-4">
          <div className="relative w-full max-w-4xl aspect-video bg-black rounded-2xl overflow-hidden shadow-2xl">
            <button
              onClick={() => setShowVideoModal(false)}
              className="absolute top-4 right-4 text-white hover:text-primary z-10 p-2 bg-black/40 rounded-full hover:bg-black/60 transition-colors"
            >
              <span className="material-symbols-outlined">close</span>
            </button>
            <iframe
              className="w-full h-full"
              src={`${videoUrl}?autoplay=1`}
              title="Flower Care Tutorial"
              frameBorder="0"
              allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
              allowFullScreen
            />
          </div>
        </div>
      )}

      {/* Lightbox Modal */}
      {showLightbox && (
        <div className="fixed inset-0 z-[100] bg-black/90 backdrop-blur-md flex flex-col justify-between py-6">
          <div className="flex justify-between items-center px-6">
            <span className="text-white font-label-md">
              {lightboxIndex + 1} / {galleryImages.length}
            </span>
            <button
              onClick={() => setShowLightbox(false)}
              className="text-white hover:text-primary p-2 bg-white/10 rounded-full transition-colors"
            >
              <span className="material-symbols-outlined">close</span>
            </button>
          </div>

          <div className="flex items-center justify-between px-4 max-h-[80vh]">
            <button
              onClick={() => setLightboxIndex((prev) => (prev === 0 ? galleryImages.length - 1 : prev - 1))}
              className="text-white hover:text-primary p-3 bg-white/5 rounded-full transition-colors"
            >
              <span className="material-symbols-outlined">chevron_left</span>
            </button>
            <img
              src={formatImageUrl(galleryImages[lightboxIndex])}
              alt="Zoomed view"
              className="max-w-full max-h-[75vh] object-contain rounded-lg shadow-2xl"
              loading="lazy"
            />
            <button
              onClick={() => setLightboxIndex((prev) => (prev === galleryImages.length - 1 ? 0 : prev + 1))}
              className="text-white hover:text-primary p-3 bg-white/5 rounded-full transition-colors"
            >
              <span className="material-symbols-outlined">chevron_right</span>
            </button>
          </div>

          <div className="flex justify-center gap-2 overflow-x-auto px-6">
            {galleryImages.map((img, idx) => (
              <button
                key={idx}
                onClick={() => setLightboxIndex(idx)}
                className={`w-16 h-16 rounded-md overflow-hidden border-2 transition-all ${
                  lightboxIndex === idx ? 'border-primary scale-105' : 'border-transparent opacity-60'
                }`}
              >
                <img src={formatImageUrl(img)} className="w-full h-full object-cover" alt="" loading="lazy" />
              </button>
            ))}
          </div>
        </div>
      )}
    </div>
  );
};

export default ProductDetailPage;
