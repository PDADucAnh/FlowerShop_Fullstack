import React, { useEffect, useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import settingsService, { StoreInfo } from '../services/settingsService';
import layoutService, { FooterColumn, FooterLink } from '../services/layoutService';

const gridColsMap: Record<number, string> = {
  1: 'grid-cols-1 md:grid-cols-1',
  2: 'grid-cols-1 md:grid-cols-2',
  3: 'grid-cols-1 md:grid-cols-3',
  4: 'grid-cols-1 md:grid-cols-4',
  5: 'grid-cols-1 md:grid-cols-5',
  6: 'grid-cols-1 md:grid-cols-6',
};

const linkClasses =
  'font-body-md text-body-md text-on-surface-variant hover:text-primary transition-colors no-underline';

const Footer: React.FC = () => {
  const location = useLocation();
  const [storeInfo, setStoreInfo] = useState<StoreInfo | null>(null);
  const [footer, setFooter] = useState<FooterColumn[]>([]);

  useEffect(() => {
    Promise.all([
      settingsService.getStoreInfo(),
      layoutService.getLayout(),
    ])
      .then(([storeRes, layoutRes]) => {
        setStoreInfo(storeRes as unknown as StoreInfo);
        setFooter(layoutRes.footer);
      })
      .catch(() => {
        settingsService.getStoreInfo().then((res) => {
          setStoreInfo(res as unknown as StoreInfo);
        });
      });
  }, []);

  const s = storeInfo;
  const activeColumns = footer
    .filter((c) => c.isActive)
    .sort((a, b) => a.sortOrder - b.sortOrder);
  const columnCount = activeColumns.length || 3;
  const gridClass = gridColsMap[columnCount] || 'grid-cols-1 md:grid-cols-3';

  const renderLink = (link: FooterLink): React.ReactNode | null => {
    if (link.type === 'text_block') {
      return <span className={linkClasses}>{link.label}</span>;
    }
    if (link.type === 'page') {
      if (!link.pageId) return null;
      return (
        <Link to={`/page/${link.pageId}`} className={linkClasses}>
          {link.label}
        </Link>
      );
    }
    if (link.type === 'custom') {
      if (!link.url) return null;
      if (link.url.startsWith('http')) {
        return (
          <a
            href={link.url}
            target="_blank"
            rel="noopener noreferrer"
            className={linkClasses}
          >
            {link.label}
          </a>
        );
      }
      return (
        <Link to={link.url} className={linkClasses}>
          {link.label}
        </Link>
      );
    }
    return null;
  };

  if (location.pathname === '/order-confirmation') {
    return (
      <footer className="w-full mt-stack-lg border-t border-outline-variant bg-surface-container-low">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-gutter max-w-[1400px] mx-auto px-margin-desktop py-stack-lg">
          <div className="flex flex-col gap-4">
            <span className="font-display-lg text-display-lg-mobile text-primary">
              {s?.storeName ?? 'PDA FLOWER'}
            </span>
            <span className="font-body-md text-body-md text-secondary">
              &copy; 2026 {s?.storeName ?? 'PDA FLOWER'}. All rights reserved.
            </span>
          </div>
          <div className="flex flex-col gap-2">
            <Link
              className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline"
              to="/delivery-policy"
            >
              Delivery Policy
            </Link>
            <Link
              className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline"
              to="/return-policy"
            >
              Return Policy
            </Link>
          </div>
          <div className="flex flex-col gap-2">
            <Link
              className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline"
              to="/privacy-policy"
            >
              Privacy Policy
            </Link>
          </div>
          <div className="flex flex-col gap-2">
            <span className="font-body-md text-body-md text-on-surface-variant">
              {s?.address ?? 'Tay Ninh, Vietnam'}
            </span>
            <Link
              className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline"
              to="/contact"
            >
              Contact Us
            </Link>
            {s?.facebook && (
              <a
                href={s.facebook}
                target="_blank"
                rel="noopener noreferrer"
                className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline"
              >
                Facebook
              </a>
            )}
            {s?.zalo && (
              <a
                href={`https://zalo.me/${s.zalo}`}
                target="_blank"
                rel="noopener noreferrer"
                className="font-body-md text-body-md text-on-surface-variant hover:text-primary underline underline-offset-4 opacity-90 hover:opacity-100 transition-opacity no-underline"
              >
                Zalo
              </a>
            )}
          </div>
        </div>
      </footer>
    );
  }

  return (
    <footer className="bg-surface-container-lowest border-t border-outline-variant mt-auto w-full">
      <div className="max-w-[1400px] mx-auto px-margin-desktop py-stack-lg">
        <div className={`grid grid-cols-1 ${gridClass} gap-8`}>
          {activeColumns.map((col, colIdx) => {
            if (col.type === 'links') {
              return (
                <div key={colIdx}>
                  <h4 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">
                    {col.title}
                  </h4>
                  <ul className="space-y-2.5 list-none p-0 m-0">
                    {col.links.map((link) => {
                      const el = renderLink(link);
                      if (el === null) return null;
                      return <li key={link.id}>{el}</li>;
                    })}
                  </ul>
                </div>
              );
            }
            if (col.type === 'social_icons') {
              return (
                <div key={colIdx}>
                  <h4 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">
                    {col.title}
                  </h4>
                  <ul className="space-y-2.5 list-none p-0 m-0 font-body-md text-body-md text-on-surface-variant">
                    {s?.facebook && (
                      <li className="flex items-center gap-2">
                        <span className="material-symbols-outlined text-[20px] text-primary shrink-0">
                          public
                        </span>
                        <a
                          href={s.facebook}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="text-on-surface-variant hover:text-primary transition-colors no-underline"
                        >
                          Facebook
                        </a>
                      </li>
                    )}
                    {s?.zalo && (
                      <li className="flex items-center gap-2">
                        <span
                          className="text-[20px] text-primary shrink-0 font-bold"
                          style={{ fontFamily: 'sans-serif' }}
                        >
                          Z
                        </span>
                        <a
                          href={`https://zalo.me/${s.zalo}`}
                          target="_blank"
                          rel="noopener noreferrer"
                          className="text-on-surface-variant hover:text-primary transition-colors no-underline"
                        >
                          Zalo
                        </a>
                      </li>
                    )}
                  </ul>
                </div>
              );
            }
            if (col.type === 'text_block') {
              return (
                <div key={colIdx}>
                  <h4 className="font-label-sm text-label-sm uppercase tracking-widest text-primary mb-4">
                    {col.title}
                  </h4>
                  {col.links?.map((link) => (
                    <p
                      key={link.id}
                      className="font-body-md text-body-md text-on-surface-variant leading-relaxed"
                    >
                      {link.label}
                    </p>
                  ))}
                </div>
              );
            }
            return null;
          })}
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
