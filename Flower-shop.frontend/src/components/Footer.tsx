import React from 'react';
import { Link } from 'react-router-dom';

const Footer: React.FC = () => {
  return (
    <footer className="bg-surface-container-lowest border-t border-outline-variant w-full mt-auto">
      <div className="flex flex-col md:flex-row justify-between items-center px-margin-mobile md:px-margin-desktop py-stack-lg max-w-container-max mx-auto w-full gap-stack-md md:gap-0">
        <div className="font-headline-sm text-headline-sm text-primary">FlowerShop</div>
        <div className="flex flex-wrap justify-center gap-x-stack-md gap-y-stack-sm">
          <Link to="/" className="font-label-sm text-label-sm text-on-surface-variant hover:underline decoration-primary/30 transition-all rounded">Privacy Policy</Link>
          <Link to="/" className="font-label-sm text-label-sm text-on-surface-variant hover:underline decoration-primary/30 transition-all rounded">Shipping Info</Link>
          <Link to="/" className="font-label-sm text-label-sm text-on-surface-variant hover:underline decoration-primary/30 transition-all rounded">Terms of Service</Link>
          <Link to="/" className="font-label-sm text-label-sm text-on-surface-variant hover:underline decoration-primary/30 transition-all rounded">Floral Care Guide</Link>
        </div>
        <div className="font-body-md text-body-md text-on-surface-variant text-center md:text-right">
          &copy; {new Date().getFullYear()} FlowerShop. Crafted for Contemporary Romance.
        </div>
      </div>
    </footer>
  );
};

export default Footer;
