import React from 'react';
import { Link } from 'react-router-dom';

const HeroBanner: React.FC = () => {
  return (
    <section className="relative w-full h-[819px] md:h-[921px] flex items-center justify-center overflow-hidden bg-surface-container-low">
      <div className="absolute inset-0 z-0">
        <img className="w-full h-full object-cover object-center opacity-90"
          alt="A sweeping, cinematic photograph of a luxurious floral arrangement taking up the entire frame. The arrangement features soft, oversized peonies, delicate ranunculus, and trailing greenery in shades of vibrant rose pink, blush, and cream. The background is a soft, airy white studio setting with bright, natural lighting that casts gentle, diffused shadows."
          src="https://lh3.googleusercontent.com/aida-public/AB6AXuAQgCPxHJv4TAtJSkLJK1lxv5pxk5Xx9tcMLuEZahnjxUAM5MHJ9YMowht7BmU0vjru6EyNZ9IcxJhjt3_vYE47f4yY40hHoIZtxR9dWPUybPpt7pleOWAhCtfQp4ObBn5mG4fC875XO9ndx76ByWQG9WUy4jU-VAii_rTtpjm519E7SU08X9_vgfzp78RU7SvxN6zskHy6mSJDD3fe-mfiZ_razbBvZeUvYOK_KuYX5gzf20U7bRAIROw9342RDX-owh6whD0thtw" />
        <div className="absolute inset-0 bg-gradient-to-t from-surface/80 via-surface/40 to-transparent"></div>
      </div>
      <div className="relative z-10 text-center px-margin-mobile md:px-margin-desktop max-w-3xl mx-auto flex flex-col items-center">
        <span className="font-label-sm text-label-sm text-primary uppercase tracking-widest mb-stack-sm bg-surface-container-lowest/80 px-4 py-1 rounded-full backdrop-blur-sm petal-shadow">Contemporary Romance</span>
        <h1 className="font-display-lg-mobile md:font-display-lg text-display-lg-mobile md:text-display-lg text-on-surface mb-stack-md leading-tight">
          Artistry in Every Bloom
        </h1>
        <p className="font-body-lg text-body-lg text-on-surface-variant mb-stack-lg max-w-xl mx-auto">
          Curated floral narratives for the discerning eye. We blend sophisticated editorial design with approachable warmth to create arrangements that speak volumes.
        </p>
        <Link to="/shop" className="bg-primary text-on-primary font-label-md text-label-md px-8 py-4 rounded-lg shadow-sm hover:shadow-md hover:-translate-y-0.5 transition-all duration-300 text-decoration-none">
          Explore Collections
        </Link>
      </div>
    </section>
  );
};

export default HeroBanner;
