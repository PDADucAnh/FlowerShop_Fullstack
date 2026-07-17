import React, { useEffect, useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import settingsService, { StoreInfo } from '../services/settingsService';

const Footer: React.FC = () => {
  const location = useLocation();
  const [storeInfo, setStoreInfo] = useState<StoreInfo | null>(null);

  useEffect(() => {
    settingsService.getStoreInfo().then((res) => {
      setStoreInfo(res as unknown as StoreInfo);
    });
  }, []);

  const s = storeInfo;

  if (location.pathname === '/order-confirmation') {
    return (
      <footer className="w-full mt-stack-lg border-t border-outline-variant bg-surface-container-low">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-gutter max-w-[1400px] mx-auto px-margin-desktop py-stack-lg">
          <div className="flex flex-col gap-4">
            <span className="font-display-lg text-display-lg-mobile text-primary">{s?.storeName ?? 'PDA FLOWER'}</span>
            <span className="font-body-md text-body-md text-secondary">© 2026 {s?.storeName ?? 'PDA FLOWER'}. All rights reserved.</span>
          </div>
          <div className="flex flex-col gap-2">
            <Link className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline" to="/delivery-policy">Delivery Policy</Link>
            <Link className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline" to="/return-policy">Return Policy</Link>
          </div>
          <div className="flex flex-col gap-2">
            <Link className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline" to="/privacy-policy">Privacy Policy</Link>
          </div>
          <div className="flex flex-col gap-2">
            <span className="font-body-md text-body-md text-on-surface-variant">{s?.address ?? 'Tay Ninh, Vietnam'}</span>
            <Link className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline" to="/contact">Contact Us</Link>
            {s?.facebook && (
              <a href={s.facebook} target="_blank" rel="noopener noreferrer" className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline">Facebook</a>
            )}
            {s?.zalo && (
              <a href={`https://zalo.me/${s.zalo}`} target="_blank" rel="noopener noreferrer" className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline">Zalo</a>
            )}
          </div>
        </div>
      </footer>
    );
  }
  return (
    <footer className="bg-surface-container-lowest border-t border-outline-variant mt-auto w-full">
      <div className="max-w-[1400px] mx-auto px-margin-desktop py-stack-lg">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div className="space-y-4">
            <span className="font-headline-sm text-headline-sm text-primary block">{s?.storeName ?? 'PDA FLOWER'}</span>
            <p className="font-body-md text-body-md text-on-surface-variant leading-relaxed">
              Thương hiệu hoa tươi nghệ thuật hàng đầu, mang yêu thương gửi gắm qua từng đóa hoa tinh tế được chăm chút tỉ mỉ.
            </p>
          </div>

          <div>
            <h4 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Chính sách</h4>
            <ul className="space-y-2.5 list-none p-0 m-0">
              <li>
                <Link to="/delivery-policy" className="font-body-md text-body-md text-on-surface-variant hover:text-primary transition-colors no-underline">Giao hàng</Link>
              </li>
              <li>
                <Link to="/return-policy" className="font-body-md text-body-md text-on-surface-variant hover:text-primary transition-colors no-underline">Đổi trả</Link>
              </li>
              <li>
                <Link to="/privacy-policy" className="font-body-md text-body-md text-on-surface-variant hover:text-primary transition-colors no-underline">Bảo mật</Link>
              </li>
            </ul>
          </div>

          <div>
            <h4 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">Liên hệ</h4>
            <ul className="space-y-2.5 list-none p-0 m-0 font-body-md text-body-md text-on-surface-variant">
              <li className="flex items-start gap-2">
                <span className="material-symbols-outlined text-[20px] text-primary shrink-0">location_on</span>
                <span>{s?.address ?? '122 hẻm 3 Nguyễn Văn Trỗi, Châu Thành, Tây Ninh'}</span>
              </li>
              <li className="flex items-center gap-2">
                <span className="material-symbols-outlined text-[20px] text-primary shrink-0">call</span>
                <a href={`tel:${s?.hotline ?? '0965592852'}`} className="text-on-surface-variant hover:text-primary transition-colors no-underline">{s?.hotline ?? '0965 592 852'}</a>
              </li>
              <li className="flex items-center gap-2">
                <span className="material-symbols-outlined text-[20px] text-primary shrink-0">mail</span>
                <a href={`mailto:${s?.email ?? 'support@flowershop.retail'}`} className="text-on-surface-variant hover:text-primary transition-colors no-underline">{s?.email ?? 'support@flowershop.retail'}</a>
              </li>
              {s?.facebook && (
                <li className="flex items-center gap-2">
                  <span className="material-symbols-outlined text-[20px] text-primary shrink-0">public</span>
                  <a href={s.facebook} target="_blank" rel="noopener noreferrer" className="text-on-surface-variant hover:text-primary transition-colors no-underline">Facebook</a>
                </li>
              )}
              {s?.zalo && (
                <li className="flex items-center gap-2">
                  <span className="text-[20px] text-primary shrink-0 font-bold" style={{ fontFamily: 'sans-serif' }}>Z</span>
                  <a href={`https://zalo.me/${s.zalo}`} target="_blank" rel="noopener noreferrer" className="text-on-surface-variant hover:text-primary transition-colors no-underline">Zalo</a>
                </li>
              )}
            </ul>
          </div>
        </div>

        <div className="mt-10 pt-6 border-t border-outline-variant flex flex-col sm:flex-row justify-between items-center gap-4">
          <span className="font-body-md text-body-md text-on-surface-variant">
            &copy; 2026 {s?.storeName ?? 'PDA FLOWER'}.
          </span>
        </div>
      </div>
    </footer>
  );
};

export default Footer;