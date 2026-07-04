import React, { useState, useEffect, useCallback, useRef } from 'react';
import { Link } from 'react-router-dom';
import { Advertisement } from '../../types/advertisement';
import advertisementService from '../../services/advertisementService';
import { getImageUrl } from '../../utils/apiUtils';

const AUTO_PLAY_INTERVAL = 5000;

function HeroBanner() {
  const [slides, setSlides] = useState<Advertisement[]>([]);
  const [current, setCurrent] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  useEffect(() => {
    let cancelled = false;
    advertisementService.getActive()
      .then((data: Advertisement[]) => {
        if (!cancelled) {
          setSlides(Array.isArray(data) ? data : []);
          setLoading(false);
        }
      })
      .catch((err: Error) => {
        if (!cancelled) {
          setError(err.message);
          setLoading(false);
        }
      });
    return () => { cancelled = true; };
  }, []);

  const total = slides.length;

  const goTo = useCallback((index: number) => {
    setCurrent(((index % total) + total) % total);
  }, [total]);

  const next = useCallback(() => goTo(current + 1), [current, goTo]);
  const prev = useCallback(() => goTo(current - 1), [current, goTo]);

  useEffect(() => {
    if (total < 2) return;
    intervalRef.current = setInterval(next, AUTO_PLAY_INTERVAL);
    return () => {
      if (intervalRef.current) clearInterval(intervalRef.current);
    };
  }, [total, next]);

  const handleDotHover = (index: number) => {
    setCurrent(index);
    if (intervalRef.current) {
      clearInterval(intervalRef.current);
      intervalRef.current = setInterval(next, AUTO_PLAY_INTERVAL);
    }
  };

  if (loading) {
    return (
      <section className="relative h-[819px] md:h-[921px] w-full overflow-hidden flex items-center justify-center mb-xl bg-neutral-100">
        <div className="w-8 h-8 border-2 border-primary border-t-transparent rounded-full animate-spin" />
      </section>
    );
  }

  if (slides.length === 0) {
    return (
      <section className="relative h-[819px] md:h-[921px] w-full overflow-hidden mb-xl group/slider">
        <div className="absolute inset-0 w-full h-full">
          <img
            alt="Tuyệt Tác Hoa Tươi Nghệ Thuật"
            className="w-full h-full object-cover"
            src="https://images.unsplash.com/photo-1526047932273-341f2a7631f9?q=80&w=1600"
            loading="eager"
          />
          <div className="absolute inset-0 bg-gradient-to-t from-surface/80 via-surface/40 to-transparent" />
        </div>

        <div className="absolute inset-0 z-20 flex items-center justify-center">
          <div className="relative text-center px-margin max-w-3xl flex flex-col items-center">
            <span className="font-label-sm text-label-sm text-primary uppercase tracking-widest mb-stack-sm bg-surface-container-lowest/80 px-4 py-1 rounded-full backdrop-blur-sm petal-shadow inline-block">
              Lãng Mạn Đương Đại
            </span>
            <h1 className="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-surface mb-stack-md leading-tight">
              Tuyệt Tác Hoa Tươi Nghệ Thuật
            </h1>
            <p className="font-body-lg text-body-lg text-on-surface-variant mb-lg max-w-xl mx-auto">
              Khám phá những thiết kế hoa độc đáo, sang trọng mang phong cách châu Âu hiện đại.
            </p>
            <Link
              to="/shop"
              className="bg-primary text-on-primary px-8 py-4 font-label-sm text-label-sm uppercase tracking-widest border border-primary text-decoration-none btn-luxury btn-primary-luxury inline-block"
            >
              Mua ngay
            </Link>
          </div>
        </div>
      </section>
    );
  }

  return (
    <section className="relative h-[819px] md:h-[921px] w-full overflow-hidden mb-xl group/slider">
      {slides.map((slide, index) => (
        <div
          key={slide.id}
          className={`absolute inset-0 w-full h-full transition-opacity duration-700 ease-in-out ${
            index === current ? 'opacity-100 z-10' : 'opacity-0 z-0'
          }`}
        >
          <img
            alt={slide.title}
            className="w-full h-full object-cover"
            src={getImageUrl(slide.imageUrl)}
            loading="eager"
          />
          <div className="absolute inset-0 bg-gradient-to-t from-surface/80 via-surface/40 to-transparent" />
        </div>
      ))}

      <div className="absolute inset-0 z-20 flex items-center justify-center">
        <div className="relative text-center px-margin max-w-3xl flex flex-col items-center">
          {slides.map((slide, index) => (
            <div
              key={slide.id}
              className={`transition-all duration-700 ease-in-out ${
                index === current
                  ? 'opacity-100 translate-y-0'
                  : 'opacity-0 translate-y-4 absolute inset-0 pointer-events-none'
              }`}
            >
              {index === current && (
                <>
                  <span className="font-label-sm text-label-sm text-primary uppercase tracking-widest mb-stack-sm bg-surface-container-lowest/80 px-4 py-1 rounded-full backdrop-blur-sm petal-shadow inline-block">
                    Lãng Mạn Đương Đại
                  </span>
                  <h1 className="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-surface mb-stack-md leading-tight">
                    {slide.title}
                  </h1>
                  {slide.subtitle && (
                    <p className="font-body-lg text-body-lg text-on-surface-variant mb-lg max-w-xl mx-auto">
                      {slide.subtitle}
                    </p>
                  )}
                  {slide.linkUrl && (
                    <Link
                      to={slide.linkUrl}
                      className="bg-primary text-on-primary px-8 py-4 font-label-sm text-label-sm uppercase tracking-widest border border-primary text-decoration-none btn-luxury btn-primary-luxury inline-block"
                    >
                      Mua ngay
                    </Link>
                  )}
                </>
              )}
            </div>
          ))}
        </div>
      </div>

      {total > 1 && (
        <>
          <button
            onClick={prev}
            className="absolute left-4 top-1/2 -translate-y-1/2 z-30 w-12 h-12 flex items-center justify-center bg-white/10 backdrop-blur-sm rounded-full text-white opacity-0 group-hover/slider:opacity-100 transition-opacity duration-300 hover:bg-white/20"
            aria-label="Slide trước"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <button
            onClick={next}
            className="absolute right-4 top-1/2 -translate-y-1/2 z-30 w-12 h-12 flex items-center justify-center bg-white/10 backdrop-blur-sm rounded-full text-white opacity-0 group-hover/slider:opacity-100 transition-opacity duration-300 hover:bg-white/20"
            aria-label="Slide sau"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
          </button>

          <div className="absolute bottom-8 left-1/2 -translate-x-1/2 z-30 flex gap-3">
            {slides.map((slide, index) => (
              <button
                key={slide.id}
                onClick={() => handleDotHover(index)}
                className={`w-3 h-3 rounded-full transition-all duration-300 ${
                  index === current
                    ? 'bg-white w-8'
                    : 'bg-white/40 hover:bg-white/60'
                }`}
                aria-label={`Đến slide ${index + 1}`}
              />
            ))}
          </div>
        </>
      )}
    </section>
  );
}

export default HeroBanner;

