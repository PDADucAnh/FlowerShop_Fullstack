import { useState, useEffect } from 'react';
import { Helmet } from 'react-helmet-async';
import pageService, { Page } from '../../services/pageService';

const About = () => {
  const [page, setPage] = useState<Page | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    pageService.getBySlug('gioi-thieu')
      .then(res => setPage(res))
      .catch(() => setPage(null))
      .finally(() => setLoading(false));
  }, []);

  return (
    <>
      <Helmet><title>Giới thiệu - FlowerShop</title></Helmet>
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
            <h1 className="text-4xl font-bold mb-4">Giới thiệu</h1>
            <p>Nội dung đang được cập nhật.</p>
          </div>
        )}
      </section>
    </>
  );
};

export default About;
