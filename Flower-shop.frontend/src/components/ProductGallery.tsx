import { useState, useRef, useCallback } from 'react'
import { Swiper as SwiperType } from 'swiper'
import { Swiper, SwiperSlide } from 'swiper/react'
import { Pagination, FreeMode } from 'swiper/modules'
import { getImageUrl } from '../utils/apiUtils'

import 'swiper/css'
import 'swiper/css/pagination'
import 'swiper/css/free-mode'

interface GalleryItem {
  type: 'image' | 'video'
  src: string
}

interface ProductGalleryProps {
  images: string[]
  productName: string
  stockQuantity: number
  isOutOfStock: boolean
  isLowStock: boolean
  videoUrl?: string
}

const fmt = (url?: string): string => getImageUrl(url) || ''

export default function ProductGallery({
  images,
  productName,
  stockQuantity,
  isOutOfStock,
  isLowStock,
  videoUrl,
}: ProductGalleryProps) {
  const swiperRef = useRef<SwiperType | null>(null)
  const [activeIdx, setActiveIdx] = useState(0)
  const [showLb, setShowLb] = useState(false)
  const [lbIdx, setLbIdx] = useState(0)

  const items: GalleryItem[] = [
    ...images.map((u) => ({ type: 'image' as const, src: u })),
    ...(videoUrl ? [{ type: 'video' as const, src: videoUrl }] : []),
  ]

  const openLb = useCallback((i: number) => {
    setLbIdx(i)
    setShowLb(true)
  }, [])

  const extra = items.length > 4 ? items.length - 4 : 0

  return (
    <>
      {/* Main Swiper */}
      <div className="relative w-full aspect-square rounded-xl overflow-hidden shadow-[0_4px_20px_rgba(171,44,93,0.02)] bg-surface-container-low group cursor-zoom-in">
        <Swiper
          onSwiper={(s) => { swiperRef.current = s }}
          onSlideChange={(s) => setActiveIdx(s.activeIndex)}
          modules={[Pagination, FreeMode]}
          pagination={{ dynamicBullets: true }}
          freeMode
          className="w-full h-full"
          onClick={() => openLb(activeIdx)}
        >
          {items.map((it, i) => (
            <SwiperSlide key={i}>
              {it.type === 'video' ? (
                <div className="w-full h-full flex items-center justify-center bg-black/5">
                  <span className="material-symbols-outlined text-6xl text-outline">play_circle</span>
                </div>
              ) : (
                <img
                  alt={`${productName} - ${i + 1}`}
                  className="w-full h-full object-cover transition-transform duration-700 group-hover:scale-105"
                  src={fmt(it.src)}
                  loading={i === 0 ? 'eager' : 'lazy'}
                />
              )}
            </SwiperSlide>
          ))}
        </Swiper>

        {(isOutOfStock || isLowStock) && (
          <div className={`absolute top-4 left-4 px-3 py-1 font-label-sm text-label-sm uppercase tracking-widest rounded-sm z-10 ${isOutOfStock ? 'bg-error text-on-error' : 'bg-warning text-on-warning'}`}>
            {isOutOfStock ? 'Hết hàng' : `Chỉ còn ${stockQuantity} sản phẩm`}
          </div>
        )}

        <div className="absolute bottom-3 right-3 bg-black/50 text-white text-xs px-2 py-0.5 rounded-full md:hidden z-10">
          {activeIdx + 1}/{items.length}
        </div>
      </div>

      {/* Thumbnails - desktop only */}
      <div className="hidden md:flex gap-2">
        {items.slice(0, 4).map((it, i) => (
          <div key={i} className="relative flex-shrink-0">
            <button
              onClick={() => { swiperRef.current?.slideTo(i); setActiveIdx(i) }}
              className={`w-16 h-16 sm:w-20 sm:h-20 rounded-lg overflow-hidden border-2 transition-colors p-0 bg-transparent cursor-pointer block ${
                activeIdx === i ? 'border-[#ab2c5d]' : 'border-transparent hover:border-outline-variant'
              }`}
            >
              {it.type === 'video' ? (
                <div className="w-full h-full bg-surface-container flex items-center justify-center">
                  <span className="material-symbols-outlined text-outline text-2xl">play_circle</span>
                </div>
              ) : (
                <img
                  alt={`${productName} - ${i + 1}`}
                  className="w-full h-full object-cover"
                  src={fmt(it.src)}
                  loading="lazy"
                />
              )}
            </button>
            {i === 3 && extra > 0 && (
              <button
                onClick={() => openLb(0)}
                className="absolute inset-0 bg-black/50 flex items-center justify-center rounded-lg cursor-pointer hover:bg-black/60 transition-colors border-0 w-full"
              >
                <span className="text-white font-bold text-lg">+{extra}</span>
              </button>
            )}
          </div>
        ))}
      </div>

      {/* Lightbox */}
      {showLb && (
        <div className="fixed inset-0 z-[100] bg-black/90 backdrop-blur-md flex flex-col justify-between py-6">
          <div className="flex justify-between items-center px-6">
            <span className="text-white font-label-md">{lbIdx + 1} / {items.length}</span>
            <button
              onClick={() => setShowLb(false)}
              className="text-white hover:text-primary p-2 bg-white/10 rounded-full transition-colors"
            >
              <span className="material-symbols-outlined">close</span>
            </button>
          </div>

          <div className="flex items-center justify-between px-4 max-h-[80vh]">
            <button
              onClick={() => setLbIdx((p) => (p === 0 ? items.length - 1 : p - 1))}
              className="text-white hover:text-primary p-3 bg-white/5 rounded-full transition-colors"
            >
              <span className="material-symbols-outlined">chevron_left</span>
            </button>

            {items[lbIdx]?.type === 'video' ? (
              <div className="w-full max-w-4xl aspect-video bg-black rounded-2xl overflow-hidden shadow-2xl">
                <iframe
                  className="w-full h-full"
                  src={`${items[lbIdx].src}?autoplay=1`}
                  title="Video hướng dẫn"
                  frameBorder="0"
                  allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                  allowFullScreen
                />
              </div>
            ) : (
              <img
                src={fmt(items[lbIdx]?.src)}
                alt="Xem ảnh phóng to"
                className="max-w-full max-h-[75vh] object-contain rounded-lg shadow-2xl"
                loading="lazy"
              />
            )}

            <button
              onClick={() => setLbIdx((p) => (p === items.length - 1 ? 0 : p + 1))}
              className="text-white hover:text-primary p-3 bg-white/5 rounded-full transition-colors"
            >
              <span className="material-symbols-outlined">chevron_right</span>
            </button>
          </div>

          <div className="flex justify-center gap-2 overflow-x-auto px-6">
            {items.map((it, i) => (
              <button
                key={i}
                onClick={() => setLbIdx(i)}
                className={`w-16 h-16 rounded-md overflow-hidden border-2 transition-all flex-shrink-0 p-0 bg-transparent cursor-pointer ${
                  lbIdx === i ? 'border-primary scale-105' : 'border-transparent opacity-60'
                }`}
              >
                {it.type === 'video' ? (
                  <div className="w-full h-full bg-surface-container/80 flex items-center justify-center">
                    <span className="material-symbols-outlined text-white text-2xl">play_circle</span>
                  </div>
                ) : (
                  <img src={fmt(it.src)} className="w-full h-full object-cover" alt="" loading="lazy" />
                )}
              </button>
            ))}
          </div>
        </div>
      )}
    </>
  )
}
