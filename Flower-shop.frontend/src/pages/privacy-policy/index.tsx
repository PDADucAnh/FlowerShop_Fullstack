import { useState, useEffect } from 'react';
import { Helmet } from 'react-helmet-async';
import pageService, { Page } from '../../services/pageService';

const PrivacyPolicy = () => {
  const [page, setPage] = useState<Page | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    pageService.getBySlug('chinh-sach-bao-mat')
      .then(res => setPage(res))
      .catch(() => setPage(null))
      .finally(() => setLoading(false));
  }, []);

  return (
    <>
      <Helmet><title>Chính sách bảo mật - FlowerShop</title></Helmet>
      <section className="py-16 px-4 max-w-4xl mx-auto">
        {loading ? (
          <div className="text-center text-gray-500 py-20">Đang tải...</div>
        ) : page ? (
          <>
            <h1 className="text-4xl font-bold mb-8">{page.title}</h1>
            <div className="prose prose-lg max-w-none" dangerouslySetInnerHTML={{ __html: page.content }} />
          </>
        ) : (
          <div className="text-center text-gray-500 py-20">
            <h1 className="text-4xl font-bold mb-4">Chính sách bảo mật</h1>
            <p>Nội dung đang được cập nhật.</p>
          </div>
        )}
      </section>
    </>
  );
};

export default PrivacyPolicy;
